using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.TestRunners;


namespace Stryker.Core.Initialisation
{
    public interface IInputFileResolver
    {
        ProjectInfo ResolveInput(IStrykerOptions options);
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

        public InputFileResolver(IFileSystem fileSystem, IProjectFileReader projectFileReader)
        {
            _fileSystem = fileSystem;
            _projectFileReader = projectFileReader ?? new ProjectFileReader();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InputFileResolver>();
        }

        public InputFileResolver() : this(new FileSystem(), new ProjectFileReader()) { }

        /// <summary>
        /// Finds the referencedProjects and looks for all files that should be mutated in those projects
        /// </summary>
        public ProjectInfo ResolveInput(IStrykerOptions options)
        {
            var projectInfo = new ProjectInfo();
            // Determine test projects
            var testProjectFiles = new List<string>();
            string projectUnderTest = null;
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
                testProjectAnalyzerResults.Add(_projectFileReader.AnalyzeProject(testProjectFile, options.SolutionPath));
            }
            projectInfo.TestProjectAnalyzerResults = testProjectAnalyzerResults;

            // Determine project under test
            if (options.TestProjects != null && options.TestProjects.Any())
            {
                projectUnderTest = FindProjectFile(options.BasePath);
            }
            else
            {
                projectUnderTest = FindProjectUnderTest(projectInfo.TestProjectAnalyzerResults, options.ProjectUnderTestNameFilter);
            }

            _logger.LogInformation("The project {0} will be mutated.", projectUnderTest);

            // Analyze project under test
            projectInfo.ProjectUnderTestAnalyzerResult = _projectFileReader.AnalyzeProject(projectUnderTest, options.SolutionPath);

            IProjectComponent inputFiles = new CsharpProjectComponentsBuilder(projectInfo, options, _foldersToExclude, _logger, _fileSystem).Build();
            projectInfo.ProjectContents = inputFiles;

            ValidateTestProjectsCanBeExecuted(projectInfo, options);
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
            if (_fileSystem.File.Exists(path) && _fileSystem.Path.HasExtension(".csproj"))
            {
                return path;
            }

            string[] projectFiles;
            try
            {
                projectFiles = _fileSystem.Directory.GetFiles(path, "*.*").Where(file => file.ToLower().EndsWith("csproj")).ToArray();
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

        private void ValidateTestProjectsCanBeExecuted(ProjectInfo projectInfo, IStrykerOptions options)
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
            if (projectInfo.TestProjectAnalyzerResults.Any(testProject => testProject.GetTargetFramework() == Framework.DotNetClassic &&
                options.TestRunner != TestRunner.VsTest &&
                (!testProject.Properties.ContainsKey("IsTestProject") ||
                (testProject.Properties.ContainsKey("IsTestProject") &&
                !bool.Parse(testProject.Properties["IsTestProject"])))))
            {
                _logger.LogWarning($"Testrunner set from {options.TestRunner} to {TestRunner.VsTest} because IsTestProject property not set to true. This is only supported for vstest.");
                options.TestRunner = TestRunner.VsTest;
            }
        }

        private string DetermineProjectUnderTestWithNameFilter(string projectUnderTestNameFilter, IEnumerable<string> projectReferences)
        {
            var stringBuilder = new StringBuilder();
            var referenceChoice = BuildReferenceChoice(projectReferences);

            var projectReferencesMatchingNameFilter = projectReferences.Where(x => x.ToLower().Contains(projectUnderTestNameFilter.ToLower()));
            if (!projectReferencesMatchingNameFilter.Any())
            {
                stringBuilder.Append("No project reference matched your --project-file=");
                stringBuilder.AppendLine(projectUnderTestNameFilter);
                stringBuilder.Append(referenceChoice);
                AppendExampleIfPossible(stringBuilder, projectReferences, projectUnderTestNameFilter);

                throw new StrykerInputException(ErrorMessage, stringBuilder.ToString());
            }
            else if (projectReferencesMatchingNameFilter.Count() > 1)
            {
                stringBuilder.Append("More than one project reference matched your --project-file=");
                stringBuilder.Append(projectUnderTestNameFilter);
                stringBuilder.AppendLine(" argument to specify the project to mutate, please specify the name more detailed.");
                stringBuilder.Append(referenceChoice);
                AppendExampleIfPossible(stringBuilder, projectReferences, projectUnderTestNameFilter);

                throw new StrykerInputException(ErrorMessage, stringBuilder.ToString());
            }

            return FilePathUtils.NormalizePathSeparators(projectReferencesMatchingNameFilter.Single());
        }

        private string DetermineProjectUnderTestWithoutNameFilter(IEnumerable<string> projectReferences)
        {
            var stringBuilder = new StringBuilder();
            var referenceChoice = BuildReferenceChoice(projectReferences);

            if (projectReferences.Count() > 1) // Too many references found
            {
                stringBuilder.AppendLine("Test project contains more than one project references. Please add the --project-file=[projectname] argument to specify which project to mutate.");
                stringBuilder.Append(referenceChoice);
                AppendExampleIfPossible(stringBuilder, projectReferences);

                throw new StrykerInputException(ErrorMessage, stringBuilder.ToString());
            }

            if (!projectReferences.Any()) // No references found
            {
                stringBuilder.AppendLine("No project references found. Please add a project reference to your test project and retry.");

                throw new StrykerInputException(ErrorMessage, stringBuilder.ToString());
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

        #region string helper methods

        private void AppendExampleIfPossible(StringBuilder builder, IEnumerable<string> projectReferences, string filter = null)
        {
            var otherProjectReference = projectReferences.FirstOrDefault(
                o => !string.Equals(o, filter, StringComparison.OrdinalIgnoreCase));
            if (otherProjectReference is null)
            {
                //not possible to find somethig different.
                return;
            }

            builder.AppendLine("");
            builder.AppendLine($"Example: --project-file={otherProjectReference}");
        }

        private StringBuilder BuildReferenceChoice(IEnumerable<string> projectReferences)
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

        #endregion
    }
}
