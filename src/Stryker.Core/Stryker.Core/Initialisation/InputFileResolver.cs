using Microsoft.Extensions.Logging;
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
        private readonly string[] _foldersToExclude = { "obj", "bin", "node_modules" };
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
            var testProjectFile = FindProjectFile(options.BasePath, options.TestProjectNameFilter);

            // Analyze the test project
            result.TestProjectAnalyzerResult = _projectFileReader.AnalyzeProject(testProjectFile, options.SolutionPath);

            // Determine project under test
            var reader = new ProjectFileReader();
            var projectUnderTest = reader.DetermineProjectUnderTest(result.TestProjectAnalyzerResult.ProjectReferences, options.ProjectUnderTestNameFilter);
            _logger.LogInformation("The project {0} will be mutated", projectUnderTest);

            // Analyze project under test
            result.ProjectUnderTestAnalyzerResult = _projectFileReader.AnalyzeProject(projectUnderTest, options.SolutionPath);

            FolderComposite inputFiles;
            if (result.ProjectUnderTestAnalyzerResult.SourceFiles!=null && result.ProjectUnderTestAnalyzerResult.SourceFiles.Any())
            {
                inputFiles = FindProjectFilesUsingBuildAlyzer(result.ProjectUnderTestAnalyzerResult);
            }
            else
            {
                inputFiles = FindProjectFilesScanningProjectFolders(result.ProjectUnderTestAnalyzerResult);
            }
            result.ProjectContents = inputFiles;

            ValidateResult(result, options);

            return result;
        }

        private FolderComposite FindProjectFilesScanningProjectFolders(ProjectAnalyzerResult analyzerResult)
        {
            var inputFiles = new FolderComposite();
            var projectUnderTestDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            foreach (var dir in ExtractProjectFolders(analyzerResult))
            {
                var folder = _fileSystem.Path.Combine(Path.GetDirectoryName(projectUnderTestDir), dir);

                _logger.LogDebug($"Scanning {folder}");
                if (!_fileSystem.Directory.Exists(folder))
                {
                    throw new DirectoryNotFoundException($"Can't find {folder}");
                }

                inputFiles.Add(FindInputFiles(folder, projectUnderTestDir));
            }

            return inputFiles;
        }

        private FolderComposite FindProjectFilesUsingBuildAlyzer(ProjectAnalyzerResult analyzerResult)
        {
            var inputFiles = new FolderComposite();
            var projectUnderTestDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            var projectRoot = Path.GetDirectoryName(projectUnderTestDir);
            var generatedAssemblyInfo =
                (_fileSystem.Path.GetFileNameWithoutExtension(analyzerResult.ProjectFilePath) + ".AssemblyInfo.cs").ToLowerInvariant();
            var rootFolderComposite = new FolderComposite()
            {
                Name = string.Empty,
                FullPath = projectRoot,
                RelativePath = string.Empty,
                RelativePathToProjectFile =
                    Path.GetRelativePath(projectUnderTestDir, projectUnderTestDir)
            };
            var cache = new Dictionary<string, FolderComposite> {[string.Empty] = rootFolderComposite};
            inputFiles.Add(rootFolderComposite);
            foreach (var sourceFile in analyzerResult.SourceFiles)
            {
                if (sourceFile.EndsWith(".xaml.cs"))
                {
                    continue;
                }

                if (_fileSystem.Path.GetFileName(sourceFile).ToLowerInvariant() == generatedAssemblyInfo)
                {
                    continue;
                }

                var relativePath = Path.GetRelativePath(projectUnderTestDir, sourceFile);
                var folderComposite = GetOrBuildFolderComposite(cache, Path.GetDirectoryName(relativePath), projectUnderTestDir,
                    projectRoot, inputFiles);
                var fileName = Path.GetFileName(sourceFile);
                folderComposite.Add(new FileLeaf()
                {
                    SourceCode = _fileSystem.File.ReadAllText(sourceFile),
                    Name = _fileSystem.Path.GetFileName(sourceFile),
                    RelativePath = _fileSystem.Path.Combine(folderComposite.RelativePath, fileName),
                    FullPath = sourceFile,
                    RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, sourceFile)
                });
            }

            return inputFiles;
        }

        // get the FolderComposite object representing the the project's folder 'targetFolder'. Build the needed FolderComposite(s) for a complete path
        private FolderComposite GetOrBuildFolderComposite(IDictionary<string, FolderComposite> cache, string targetFolder, string projectUnderTestDir,
            string projectRoot, ProjectComponent inputFiles)
        {
            if (cache.ContainsKey(targetFolder))
            {
                return cache[targetFolder];
            }

            var folder = targetFolder;
            FolderComposite subDir = null;
            while (!string.IsNullOrEmpty(folder))
            {
                if (!cache.ContainsKey(folder))
                {
                    // we have not scanned this folder yet
                    var sub = Path.GetFileName(folder);
                    var fullPath = _fileSystem.Path.Combine(projectUnderTestDir, sub);
                    var newComposite = new FolderComposite
                    {
                        Name = sub,
                        FullPath = fullPath,
                        RelativePath = Path.GetRelativePath(projectRoot, fullPath),
                        RelativePathToProjectFile =
                            Path.GetRelativePath(projectUnderTestDir, fullPath)
                    };
                    if (subDir != null)
                    {
                        newComposite.Add(subDir);
                    }

                    cache.Add(folder, newComposite);
                    subDir = newComposite;
                    folder = Path.GetDirectoryName(folder);
                    if (string.IsNullOrEmpty(folder))
                    {
                        // we are at root
                        inputFiles.Add(subDir);
                    }
                }
                else
                {
                    cache[folder].Add(subDir);
                    break;
                }
            }

            return cache[targetFolder];
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

        public string FindProjectFile(string basePath, string testProjectNameFilter)
        {
            string filter = BuildTestProjectFilter(basePath, testProjectNameFilter);

            var projectFiles = _fileSystem.Directory.GetFileSystemEntries(basePath, filter);

            _logger.LogTrace("Scanned the directory {0} for {1} files: found {2}", basePath, filter, projectFiles);

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
                throw new StrykerInputException($"No .csproj file found, please check your project directory at {basePath}");
            }

            _logger.LogDebug("Using {0} as project file", projectFiles.Single());

            return projectFiles.Single();
        }

        private string BuildTestProjectFilter(string basePath, string testProjectNameFilter)
        {
            // Make sure the filter is relative to the base path otherwise we cannot find it
            var filter = FilePathUtils.NormalizePathSeparators(testProjectNameFilter.Replace(basePath, "", StringComparison.InvariantCultureIgnoreCase));

            // If the filter starts with directory separator char, remove it
            if (filter.Replace("*", "").StartsWith(Path.DirectorySeparatorChar))
            {
                filter = filter.Remove(filter.IndexOf(Path.DirectorySeparatorChar), $"{Path.DirectorySeparatorChar}".Length);
            }

            // Make sure filter contains wildcard
            filter = $"*{filter}";

            return filter;
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
            if (projectInfo.TestProjectAnalyzerResult.References
                .Any(r => r.Contains("Microsoft.VisualStudio.QualityTools.UnitTestFramework")))
            {
                throw new StrykerInputException("Please upgrade to MsTest V2. Stryker.NET uses VSTest which does not support MsTest V1.",
                    @"See https://devblogs.microsoft.com/devops/upgrade-to-mstest-v2/ for upgrade instructions.");
            }

            // if IsTestProject true property not found and project is full framework, force vstest runner
            if (projectInfo.TestProjectAnalyzerResult.TargetFramework == Framework.NetClassic &&
                options.TestRunner != TestRunner.VsTest &&
                (!projectInfo.TestProjectAnalyzerResult.Properties.ContainsKey("IsTestProject") ||
                (projectInfo.TestProjectAnalyzerResult.Properties.ContainsKey("IsTestProject") &&
                !bool.Parse(projectInfo.TestProjectAnalyzerResult.Properties["IsTestProject"]))))
            {
                _logger.LogWarning($"Testrunner set from {options.TestRunner} to {TestRunner.VsTest} because IsTestProject property not set to true. This is only supported for vstest.");
                options.TestRunner = TestRunner.VsTest;
            }
        }
    }
}
