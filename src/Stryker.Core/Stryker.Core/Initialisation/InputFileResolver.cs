using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.TargetProjects;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Initialisation
{
    public interface IInputFileResolver
    {
        TestProjectsInfo ResolveTestProjectsInfo(StrykerOptions options);
        TargetProjectInfo ResolveSourceProjectInfo(StrykerOptions options, TestProjectsInfo testProjectsInfo);
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

        public InputFileResolver(IFileSystem fileSystem, IProjectFileReader projectFileReader, ILogger<InputFileResolver> logger = null)
        {
            _fileSystem = fileSystem;
            _projectFileReader = projectFileReader ?? new ProjectFileReader();
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<InputFileResolver>();
        }

        public InputFileResolver() : this(new FileSystem(), new ProjectFileReader(), null) { }

        public TestProjectsInfo ResolveTestProjectsInfo(StrykerOptions options)
        {
            var testProjectFiles = new List<string>();
            if (options.TestProjects != null && options.TestProjects.Any())
            {
                testProjectFiles = options.TestProjects.Select(FindTestProject).ToList();
            }
            else
            {
                testProjectFiles.Add(FindTestProject(options.ProjectPath));
            }

            var testProjects = new List<TestProject>();
            foreach (var testProjectFile in testProjectFiles)
            {
                var testProjectAnalyzerResult = _projectFileReader.AnalyzeProject(testProjectFile, options.SolutionPath, options.TargetFramework);

                testProjects.Add(new TestProject(_fileSystem, testProjectAnalyzerResult));
            }

            return new TestProjectsInfo
            {
                TestProjects = testProjects
            };
        }

        public TargetProjectInfo ResolveSourceProjectInfo(StrykerOptions options, TestProjectsInfo testProjectsInfo)
        {
            var sourceProjectInfo = new TargetProjectInfo(_fileSystem)
            {
                TestProjectAnalyzerResults = testProjectsInfo.TestProjects.Select(t => t.TestProjectAnalyzerResult)
            };

            // Determine project under test
            var projectUnderTest = FindProjectUnderTest(testProjectsInfo, options.ProjectUnderTestName);

            sourceProjectInfo.ProjectUnderTestAnalyzerResult = _projectFileReader.AnalyzeProject(projectUnderTest, options.SolutionPath, options.TargetFramework);

            var language = TargetProjectInfo.ProjectUnderTestAnalyzerResult.GetLanguage();
            if (language == Language.Fsharp)
            {
                _logger.LogError("Mutation testing of F# projects is not ready yet. No mutants will be generated.");
            }

            var builder = GetProjectComponentBuilder(language, options, SourceProjectInfo);
            var inputFiles = builder.Build();
            projectInfo.ProjectContents = inputFiles;

            _logger.LogInformation("Found project {0} to mutate.", projectUnderTest);

            return sourceProjectInfo;
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

        public string FindProjectUnderTest(TestProjectsInfo testProjectsInfo, string projectUnderTestNameFilter)
        {
            var projectReferences = FindProjectsReferencedByAllTestProjects(testProjectsInfo.TestProjects);

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

        internal string DetermineProjectUnderTestWithNameFilter(string projectUnderTestNameFilter, IEnumerable<string> projectReferences)
        {
            var stringBuilder = new StringBuilder();

            var referenceChoice = BuildReferenceChoice(projectReferences);

            var normalizedProjectUnderTestNameFilter = projectUnderTestNameFilter.Replace("/", "\\");
            var projectReferencesMatchingNameFilter = projectReferences
                .Where(x => x.Contains(normalizedProjectUnderTestNameFilter, StringComparison.OrdinalIgnoreCase));
            if (!projectReferencesMatchingNameFilter.Any())
            {
                stringBuilder.Append("No project reference matched the given project filter ");
                stringBuilder.Append($"'{projectUnderTestNameFilter}'");
                stringBuilder.Append(referenceChoice);

                throw new InputException(stringBuilder.ToString());
            }
            else if (projectReferencesMatchingNameFilter.Count() > 1)
            {
                stringBuilder.Append("More than one project reference matched the given project filter ");
                stringBuilder.Append($"'{projectUnderTestNameFilter}'");
                stringBuilder.AppendLine(", please specify the full name of the project reference.");
                stringBuilder.Append(referenceChoice);

                throw new InputException(stringBuilder.ToString());
            }

            return projectReferencesMatchingNameFilter.Single();
        }

        private string DetermineProjectUnderTestWithoutNameFilter(IEnumerable<string> projectReferences)
        {
            var stringBuilder = new StringBuilder();
            var referenceChoice = BuildReferenceChoice(projectReferences);

            if (projectReferences.Count() > 1) // Too many references found
            {
                stringBuilder.AppendLine("Test project contains more than one project reference. Please set the project option (https://stryker-mutator.io/docs/stryker-net/configuration#project-file-name) to specify which project to mutate.");
                stringBuilder.Append(referenceChoice);

                throw new InputException(stringBuilder.ToString());
            }

            if (!projectReferences.Any()) // No references found
            {
                stringBuilder.AppendLine("No project references found. Please add a project reference to your test project and retry.");

                throw new InputException(stringBuilder.ToString());
            }

            return projectReferences.Single();
        }

        private static IEnumerable<string> FindProjectsReferencedByAllTestProjects(IEnumerable<TestProject> testProjects)
        {
            var amountOfTestProjects = testProjects.Count();
            var allProjectReferences = testProjects.SelectMany(t => t.TestProjectAnalyzerResult.ProjectReferences);
            var projectReferences = allProjectReferences.GroupBy(x => x).Where(g => g.Count() == amountOfTestProjects).Select(g => g.Key);
            return projectReferences;
        }

        private static StringBuilder BuildReferenceChoice(IEnumerable<string> projectReferences)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Choose one of the following references:");
            builder.AppendLine("");

            foreach (var projectReference in projectReferences)
            {
                builder.Append("  ");
                builder.AppendLine(projectReference);
            }
            return builder;
        }

        private ProjectComponentsBuilder GetProjectComponentBuilder(
            Language language,
            StrykerOptions options,
            ProjectInfo projectInfo) => language switch
            {
                Language.Csharp => new CsharpProjectComponentsBuilder(
                    projectInfo,
                    options,
                    _foldersToExclude,
                    _logger,
                    _fileSystem),

                Language.Fsharp => new FsharpProjectComponentsBuilder(
                    projectInfo,
                    _foldersToExclude,
                    _logger,
                    _fileSystem),

                _ => throw new NotSupportedException()
            };
    }
}
