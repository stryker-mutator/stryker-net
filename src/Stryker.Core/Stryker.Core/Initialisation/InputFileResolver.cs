﻿using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Stryker.Core.Initialisation
{
    public interface IInputFileResolver
    {
        ProjectInfo ResolveInput(StrykerOptions options);
    }

    /// <summary>
    ///  - Reads .csproj to find project under test
    ///  - Scans project under test and store files to mutate
    ///  - Build composite for files
    /// </summary>
    public class InputFileResolver : IInputFileResolver
    {
        private readonly string[] _foldersToExclude = { "obj", "bin", "node_modules", "StrykerOutput" };
        private readonly IFileSystem _fileSystem;
        private readonly IProjectFileReader _projectFileReader;
        private readonly ILogger _logger;

        public InputFileResolver(IFileSystem fileSystem, IProjectFileReader projectFileReader)
        {
            _fileSystem = fileSystem;
            _projectFileReader = projectFileReader;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InputFileResolver>();
        }

        public InputFileResolver() : this(new FileSystem(), new ProjectFileReader()) { }

        /// <summary>
        /// Finds the referencedProjects and looks for all files that should be mutated in those projects
        /// </summary>
        public ProjectInfo ResolveInput(StrykerOptions options)
        {
            var result = new ProjectInfo();
            var testProjectFiles = new List<string>();
            string projectUnderTest = null;
            if (options.TestProjects != null && options.TestProjects.Any())
            {
                testProjectFiles = options.TestProjects.Select(FindProjectFile).ToList();
            }
            else
            {
                testProjectFiles.Add(FindProjectFile(options.BasePath));
            }

            var results = new List<ProjectAnalyzerResult>();
            foreach (var testProjectFile in testProjectFiles)
            {
                // Analyze the test project
                results.Add(_projectFileReader.AnalyzeProject(testProjectFile, options.SolutionPath));
            }
            result.TestProjectAnalyzerResults = results;

            // Determine project under test
            var reader = new ProjectFileReader();
            if (options.TestProjects != null && options.TestProjects.Any())
            {
                projectUnderTest = FindProjectFile(options.BasePath);
            }
            else
            {
                projectUnderTest = reader.DetermineProjectUnderTest(result.TestProjectAnalyzerResults.Single().ProjectReferences, options.ProjectUnderTestNameFilter);
            }

            _logger.LogInformation("The project {0} will be mutated", projectUnderTest);

            // Analyze project under test
            result.ProjectUnderTestAnalyzerResult = _projectFileReader.AnalyzeProject(projectUnderTest, options.SolutionPath);

            var inputFiles = new FolderComposite();
            var projectUnderTestDir = Path.GetDirectoryName(result.ProjectUnderTestAnalyzerResult.ProjectFilePath);
            foreach (var dir in ExtractProjectFolders(result.ProjectUnderTestAnalyzerResult))
            {
                var folder = _fileSystem.Path.Combine(Path.GetDirectoryName(projectUnderTestDir), dir);

                _logger.LogDebug($"Scanning {folder}");
                if (!_fileSystem.Directory.Exists(folder))
                {
                    throw new DirectoryNotFoundException($"Can't find {folder}");
                }
                inputFiles.Add(FindInputFiles(folder, projectUnderTestDir));
            }
            result.ProjectContents = inputFiles;

            ValidateResult(result, options);

            return result;
        }

        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// </summary>
        private FolderComposite FindInputFiles(string path, string projectUnderTestDir, string parentFolder = null)
        {
            var lastPathComponent = Path.GetFileName(path);

            var folderComposite = new FolderComposite
            {
                Name = lastPathComponent,
                FullPath = Path.GetFullPath(path),
                RelativePath = parentFolder is null ? lastPathComponent : Path.Combine(parentFolder, lastPathComponent),
                RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, Path.GetFullPath(path))
            };
            foreach (var folder in _fileSystem.Directory.EnumerateDirectories(folderComposite.FullPath).Where(x => !_foldersToExclude.Contains(Path.GetFileName(x))))
            {
                folderComposite.Add(FindInputFiles(folder, projectUnderTestDir, folderComposite.RelativePath));
            }
            foreach (var file in _fileSystem.Directory.GetFiles(folderComposite.FullPath, "*.cs", SearchOption.TopDirectoryOnly))
            {
                // Roslyn cannot compile xaml.cs files generated by xamarin. 
                // Since the files are generated they should not be mutated anyway, so skip these files.
                if (!file.EndsWith(".xaml.cs"))
                {
                    var fileName = Path.GetFileName(file);
                    folderComposite.Add(new FileLeaf()
                    {
                        SourceCode = _fileSystem.File.ReadAllText(file),
                        Name = _fileSystem.Path.GetFileName(file),
                        RelativePath = Path.Combine(folderComposite.RelativePath, fileName),
                        FullPath = file,
                        RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, file)
                    });
                }
            }

            return folderComposite;
        }

        public string FindProjectFile(string path)
        {
            string[] projectFiles = null;
            if (_fileSystem.File.Exists(path) && _fileSystem.Path.HasExtension(".csproj"))
            {
                return path;
            }

            try
            {
                projectFiles = _fileSystem.Directory.GetFileSystemEntries(path, "*.csproj");
            }
            catch (DirectoryNotFoundException)
            {
                throw new StrykerInputException($"No .csproj file found, please check your project directory at {path}");
            }

            _logger.LogTrace("Scanned the directory {0} for {1} files: found {2}", path, "*.csproj", projectFiles);

            if (projectFiles.Count() > 1)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Expected exactly one .csproj file, found more than one:");
                foreach (var file in projectFiles)
                {
                    sb.AppendLine(file);
                }
                sb.AppendLine();
                sb.AppendLine("Please specify a test project name filter that results in one project.");
                throw new StrykerInputException(sb.ToString());
            }
            else if (!projectFiles.Any())
            {
                throw new StrykerInputException($"No .csproj file found, please check your project directory at {path}");
            }
            _logger.LogDebug("Using {0} as project file", projectFiles.Single());

            return projectFiles.Single();
        }

        private IEnumerable<string> ExtractProjectFolders(ProjectAnalyzerResult projectAnalyzerResult)
        {
            var projectFilePath = projectAnalyzerResult.ProjectFilePath;
            var projectFile = _fileSystem.File.OpenText(projectFilePath);
            var xDocument = XDocument.Load(projectFile);
            var folders = new List<string>();
            var projectDirectory = _fileSystem.Path.GetDirectoryName(projectFilePath);
            folders.Add(projectDirectory);

            foreach (var sharedProject in new ProjectFileReader().FindSharedProjects(xDocument))
            {
                var sharedProjectName = ReplaceMsbuildProperties(sharedProject, projectAnalyzerResult);

                if (!_fileSystem.File.Exists(_fileSystem.Path.Combine(projectDirectory, sharedProjectName)))
                {
                    throw new FileNotFoundException($"Missing shared project {sharedProjectName}");
                }

                var directoryName = _fileSystem.Path.GetDirectoryName(sharedProjectName);
                folders.Add(_fileSystem.Path.Combine(projectDirectory, directoryName));
            }

            return folders;
        }

        private static string ReplaceMsbuildProperties(string projectReference, ProjectAnalyzerResult projectAnalyzerResult)
        {
            var propertyRegex = new Regex(@"\$\(([a-zA-Z_][a-zA-Z0-9_\-.]*)\)");
            var properties = projectAnalyzerResult.Properties;

            return propertyRegex.Replace(projectReference,
                m =>
                {
                    var property = m.Groups[1].Value;
                    if (properties.TryGetValue(property, out var propertyValue))
                    {
                        return propertyValue;
                    }

                    var message = $"Missing MSBuild property ({property}) in project reference ({projectReference}). Please check your project file ({projectAnalyzerResult.ProjectFilePath}) and try again.";
                    throw new StrykerInputException(message);
                });
        }

        private void ValidateResult(ProjectInfo projectInfo, StrykerOptions options)
        {
            // if references contains Microsoft.VisualStudio.QualityTools.UnitTestFramework 
            // we have detected usage of mstest V1 and should exit
            if (projectInfo.TestProjectAnalyzerResults.Any(testProject => testProject.References
                .Any(r => r.Contains("Microsoft.VisualStudio.QualityTools.UnitTestFramework"))))
            {
                throw new StrykerInputException("Please upgrade to MsTest V2. Stryker.NET uses VSTest which does not support MsTest V1.",
                    @"See https://devblogs.microsoft.com/devops/upgrade-to-mstest-v2/ for upgrade instructions.");
            }

            // if IsTestProject true property not found and project is full framework, force vstest runner
            if (projectInfo.TestProjectAnalyzerResults.Any(testProject => testProject.TargetFramework == Framework.NetClassic &&
                options.TestRunner != TestRunner.VsTest &&
                (!testProject.Properties.ContainsKey("IsTestProject") ||
                (testProject.Properties.ContainsKey("IsTestProject") &&
                !bool.Parse(testProject.Properties["IsTestProject"])))))
            {
                _logger.LogWarning($"Testrunner set from {options.TestRunner} to {TestRunner.VsTest} because IsTestProject property not set to true. This is only supported for vstest.");
                options.TestRunner = TestRunner.VsTest;
            }
        }
    }
}
