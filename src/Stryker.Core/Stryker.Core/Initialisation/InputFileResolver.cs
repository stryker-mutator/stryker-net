using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ToolHelpers;

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
        private const string ErrorMessage = "Project reference issue.";
        private readonly string[] _foldersToExclude = { "obj", "bin", "node_modules", "StrykerOutput" };
        private readonly IFileSystem _fileSystem;
        private readonly IProjectFileReader _projectFileReader;
        private readonly ILogger _logger;
        private readonly INugetHelper _nugetHelper;

        public InputFileResolver(IFileSystem fileSystem, IProjectFileReader projectFileReader, INugetHelper nugetHelper, ILogger<InputFileResolver> logger = null)
        {
            _fileSystem = fileSystem;
            _projectFileReader = projectFileReader ?? new ProjectFileReader();
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<InputFileResolver>();
            _nugetHelper = nugetHelper;
        }

        public InputFileResolver() : this(new FileSystem(), new ProjectFileReader(), new NugetHelper(), null) { }

        /// <summary>
        /// Finds the referencedProjects and looks for all files that should be mutated in those projects
        /// </summary>
        public ProjectInfo ResolveInput(StrykerOptions options)
        {
            var projectInfo = new ProjectInfo(_fileSystem);
            // Determine test projects
            var testProjectFiles = new List<string>();
            if (options.TestProjects != null && options.TestProjects.Any())
            {
                testProjectFiles = options.TestProjects.Select(FindTestProject).ToList();
            }
            else
            {
                testProjectFiles.Add(FindTestProject(options.BasePath));
            }

            var testProjectAnalyzerResults = new List<IAnalyzerResult>();
            foreach (var testProjectFile in testProjectFiles)
            {
                // Analyze the test project
                testProjectAnalyzerResults.Add(_projectFileReader.AnalyzeProject(testProjectFile, options.SolutionPath, options.TargetFramework));
            }
            projectInfo.TestProjectAnalyzerResults = testProjectAnalyzerResults;

            // Determine project under test
            var projectUnderTest = FindProjectUnderTest(projectInfo.TestProjectAnalyzerResults, options.ProjectUnderTestName);

            _logger.LogInformation("The project {0} will be mutated.", projectUnderTest);

            // Analyze project under test
            projectInfo.ProjectUnderTestAnalyzerResult = _projectFileReader.AnalyzeProject(projectUnderTest, options.SolutionPath, options.TargetFramework);

            //to test Fsharp support you would need to create a FsharpProjectComponentsBuilder
            IProjectComponent inputFiles = new CsharpProjectComponentsBuilder(projectInfo, options, _foldersToExclude, _logger, _fileSystem, _nugetHelper).Build();
            projectInfo.ProjectContents = inputFiles;

            ValidateTestProjectsCanBeExecuted(projectInfo);
            _logger.LogInformation("Analysis complete.");

            return projectInfo;
        }

        public string FindTestProject(string path)
        {
            var projectFile = FindProjectFile(path);
            _logger.LogDebug("Using {0} as test project", projectFile);

            return projectFile;
        }

        private string FindProjectFile(string path)
        {
            if (_fileSystem.File.Exists(path) && (_fileSystem.Path.HasExtension(".csproj") || _fileSystem.Path.HasExtension(".fsproj")))
            {
                return path;
            }

            string[] projectFiles;
            try
            {
                projectFiles = _fileSystem.Directory.GetFiles(path, "*.*").Where(file => file.EndsWith("csproj", StringComparison.OrdinalIgnoreCase) || file.EndsWith("fsproj", StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            catch (DirectoryNotFoundException)
            {
                throw new InputException($"No .csproj file found, please check your project directory at {path}");
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
                throw new InputException(sb.ToString());
            }
            else if (!projectFiles.Any())
            {
                throw new InputException($"No .csproj file found, please check your project directory at {path}");
            }
            _logger.LogTrace("Found project file {file} in path {path}", projectFiles.Single(), path);

            return projectFiles.Single();
        }

        public string FindProjectUnderTest(IEnumerable<IAnalyzerResult> testProjects, string projectUnderTestNameFilter)
        {
            IEnumerable<string> projectReferences = FindProjectsReferencedByAllTestProjects(testProjects);

            string projectUnderTestPath;

            if (string.IsNullOrEmpty(projectUnderTestNameFilter))
            {
                projectUnderTestPath = DetermineProjectUnderTestWithoutNameFilter(projectReferences);
            }
            else
            {
                projectUnderTestPath = DetermineProjectUnderTestWithNameFilter(projectUnderTestNameFilter, projectReferences);
            }

            _logger.LogDebug("Using {0} as project under test", projectUnderTestPath);

            return projectUnderTestPath;
        }

        private void ValidateTestProjectsCanBeExecuted(ProjectInfo projectInfo)
        {
            // if references contains Microsoft.VisualStudio.QualityTools.UnitTestFramework 
            // we have detected usage of mstest V1 and should exit
            if (projectInfo.TestProjectAnalyzerResults.Any(testProject => testProject.References
                .Any(r => r.Contains("Microsoft.VisualStudio.QualityTools.UnitTestFramework"))))
            {
                throw new InputException("Please upgrade to MsTest V2. Stryker.NET uses VSTest which does not support MsTest V1.",
                    @"See https://devblogs.microsoft.com/devops/upgrade-to-mstest-v2/ for upgrade instructions.");
            }
        }

        private string DetermineProjectUnderTestWithNameFilter(string projectUnderTestNameFilter, IEnumerable<string> projectReferences)
        {
            var stringBuilder = new StringBuilder();
            var referenceChoice = BuildReferenceChoice(projectReferences);

            var projectReferencesMatchingNameFilter = projectReferences.Where(x => x.Contains(projectUnderTestNameFilter, StringComparison.OrdinalIgnoreCase));
            if (!projectReferencesMatchingNameFilter.Any())
            {
                stringBuilder.Append("No project reference matched the given project filter");
                stringBuilder.AppendLine(projectUnderTestNameFilter);
                stringBuilder.Append(referenceChoice);

                throw new InputException(ErrorMessage, stringBuilder.ToString());
            }
            else if (projectReferencesMatchingNameFilter.Count() > 1)
            {
                stringBuilder.Append("More than one project reference matched the given project filter ");
                stringBuilder.Append($"'{projectUnderTestNameFilter}'");
                stringBuilder.AppendLine(", please specify the full name of the project reference.");
                stringBuilder.Append(referenceChoice);

                throw new InputException(ErrorMessage, stringBuilder.ToString());
            }

            return FilePathUtils.NormalizePathSeparators(projectReferencesMatchingNameFilter.Single());
        }

        private string DetermineProjectUnderTestWithoutNameFilter(IEnumerable<string> projectReferences)
        {
            var stringBuilder = new StringBuilder();
            var referenceChoice = BuildReferenceChoice(projectReferences);

            if (projectReferences.Count() > 1) // Too many references found
            {
                stringBuilder.AppendLine("Test project contains more than one project reference. Please set the project option (https://stryker-mutator.io/docs/stryker-net/configuration#project-file-name) to specify which project to mutate.");
                stringBuilder.Append(referenceChoice);

                throw new InputException(ErrorMessage, stringBuilder.ToString());
            }

            if (!projectReferences.Any()) // No references found
            {
                stringBuilder.AppendLine("No project references found. Please add a project reference to your test project and retry.");

                throw new InputException(ErrorMessage, stringBuilder.ToString());
            }

            return projectReferences.Single();
        }

        private static IEnumerable<string> FindProjectsReferencedByAllTestProjects(IEnumerable<IAnalyzerResult> testProjects)
        {
            var amountOfTestProjects = testProjects.Count();
            var allProjectReferences = testProjects.SelectMany(t => t.ProjectReferences);
            var projectReferences = allProjectReferences.GroupBy(x => x).Where(g => g.Count() == amountOfTestProjects).Select(g => g.Key);
            return projectReferences;
        }

        private static StringBuilder BuildReferenceChoice(IEnumerable<string> projectReferences)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Choose one of the following references:");
            builder.AppendLine("");

            foreach (string projectReference in projectReferences)
            {
                builder.Append("  ");
                builder.AppendLine(projectReference);
            }
            return builder;
        }
    }
}
