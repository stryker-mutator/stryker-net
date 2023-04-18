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
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Initialisation
{
    public interface IInputFileResolver
    {
        TestProjectsInfo ResolveTestProjectsInfo(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects);
        SourceProjectInfo ResolveSourceProjectInfo(StrykerOptions options, IEnumerable<IAnalyzerResult> testProjectsInfo, IEnumerable<IAnalyzerResult> solutionProjects);
        IFileSystem FileSystem { get; }
    }

    /// <summary>
    ///  - Reads .csproj to find project under test
    ///  - Scans project under test and store files to mutate
    ///  - Build composite for files
    /// </summary>
    public class InputFileResolver : IInputFileResolver
    {
        private readonly string[] _foldersToExclude = { "obj", "bin", "node_modules", "StrykerOutput" };
        private readonly IProjectFileReader _projectFileReader;
        private readonly ILogger _logger;

        public InputFileResolver(IFileSystem fileSystem, IProjectFileReader projectFileReader, ILogger<InputFileResolver> logger = null)
        {
            FileSystem = fileSystem;
            _projectFileReader = projectFileReader ?? new ProjectFileReader();
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<InputFileResolver>();
        }

        public InputFileResolver() : this(new FileSystem(), new ProjectFileReader(), null) { }

        public IFileSystem FileSystem { get; }

        public TestProjectsInfo ResolveTestProjectsInfo(StrykerOptions options, IEnumerable<IAnalyzerResult> solutionProjects)

        {
            List<string> testProjectFiles;
            if (options.TestProjects != null && options.TestProjects.Any())
            {
                testProjectFiles = options.TestProjects.Select(FindTestProject).ToList();
            }
            else
            {
                testProjectFiles = new List<string> {FindTestProject(options.ProjectPath) };
            }

            var testProjects = new List<TestProject>();
            foreach (var testProjectFile in testProjectFiles)
            {
                // Analyze the test project
                var testProjectAnalyzerResult = _projectFileReader.AnalyzeProject(testProjectFile, options.SolutionPath, options.TargetFramework, solutionProjects);

                testProjects.Add(new TestProject(FileSystem, testProjectAnalyzerResult));
            }

            return new TestProjectsInfo(FileSystem)
            {
                TestProjects = testProjects
            };
        }

        public SourceProjectInfo ResolveSourceProjectInfo(StrykerOptions options, IEnumerable<IAnalyzerResult> testProjects, IEnumerable<IAnalyzerResult> solutionProjects)
        {
            var targetProjectInfo = new SourceProjectInfo();

            // Analyze source project
            var targetProject = FindSourceProject(testProjects, options.SourceProjectName, solutionProjects);

            targetProjectInfo.AnalyzerResult = _projectFileReader.AnalyzeProject(targetProject, options.SolutionPath, options.TargetFramework, solutionProjects);

            var language = targetProjectInfo.AnalyzerResult.GetLanguage();
            if (language == Language.Fsharp)
            {
                _logger.LogError(targetProjectInfo.LogError("Mutation testing of F# projects is not ready yet. No mutants will be generated."));
            }

            var builder = GetProjectComponentBuilder(language, options, targetProjectInfo);
            var inputFiles = builder.Build();
            targetProjectInfo.ProjectContents = inputFiles;

            _logger.LogInformation("Found project {0} to mutate.", targetProject);
            return targetProjectInfo;
        }

        public string FindTestProject(string path)
        {
            var projectFile = FindProjectFile(path);
            _logger.LogDebug("Using {0} as test project", projectFile);
            return projectFile;
        }

        private string FindProjectFile(string path)
        {
            if (FileSystem.File.Exists(path) && (FileSystem.Path.HasExtension(".csproj") || FileSystem.Path.HasExtension(".fsproj")))
            {
                return path;
            }

            string[] projectFiles;
            try
            {
                projectFiles = FileSystem.Directory.GetFiles(path, "*.*").Where(file => file.EndsWith("csproj", StringComparison.OrdinalIgnoreCase) || file.EndsWith("fsproj", StringComparison.OrdinalIgnoreCase)).ToArray();
            }
            catch (DirectoryNotFoundException)
            {
                throw new InputException($"No .csproj or .sln file found, please check your project directory at {path}");
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
                throw new InputException($"No .csproj or .sln file found, please check your project or solution directory at {path}");
            }
            _logger.LogTrace("Found project file {file} in path {path}", projectFiles.Single(), path);

            return projectFiles.Single();
        }

        public string FindSourceProject(IEnumerable<IAnalyzerResult> testProjects, string projectUnderTestNameFilter, IEnumerable<IAnalyzerResult> solutionProjects)
        {
            var projectReferences = FindProjectsReferencedByAllTestProjects(testProjects);

            string sourceProjectPath;

            if (string.IsNullOrEmpty(projectUnderTestNameFilter))
            {
                sourceProjectPath = DetermineTargetProjectWithoutNameFilter(projectReferences);
            }
            else
            {
                sourceProjectPath = DetermineSourceProjectWithNameFilter(projectUnderTestNameFilter, projectReferences);
            }

            _logger.LogDebug("Using {0} as project under test", sourceProjectPath);

            return sourceProjectPath;
        }

        internal string DetermineSourceProjectWithNameFilter(string sourceProjectFilter, IEnumerable<string> projectReferences)
        {
            var stringBuilder = new StringBuilder();

            var referenceChoice = BuildReferenceChoice(projectReferences);

            var normalizedProjectUnderTestNameFilter = sourceProjectFilter.Replace("\\", "/");
            var projectReferencesMatchingNameFilter = projectReferences
                .Where(x => x.Replace("\\", "/").Contains(normalizedProjectUnderTestNameFilter, StringComparison.OrdinalIgnoreCase));

            if (!projectReferencesMatchingNameFilter.Any())
            {
                stringBuilder.Append("No project reference matched the given project filter ");
                stringBuilder.Append($"'{sourceProjectFilter}'");
                stringBuilder.Append(referenceChoice);

                throw new InputException(stringBuilder.ToString());
            }
            else if (projectReferencesMatchingNameFilter.Count() > 1)
            {
                stringBuilder.Append("More than one project reference matched the given project filter ");
                stringBuilder.Append($"'{sourceProjectFilter}'");
                stringBuilder.AppendLine(", please specify the full name of the project reference.");
                stringBuilder.Append(referenceChoice);

                throw new InputException(stringBuilder.ToString());
            }

            return projectReferencesMatchingNameFilter.Single();
        }

        private string DetermineTargetProjectWithoutNameFilter(IEnumerable<string> projectReferences)
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
            builder.AppendLine("Choose one of the following references:").AppendLine("");

            foreach (var projectReference in projectReferences)
            {
                builder.Append("  ").AppendLine(projectReference.Replace("\\", "/"));
            }
            return builder;
        }

        private ProjectComponentsBuilder GetProjectComponentBuilder(
            Language language,
            StrykerOptions options,
            SourceProjectInfo projectInfo) => language switch
            {
                Language.Csharp => new CsharpProjectComponentsBuilder(
                    projectInfo,
                    options,
                    _foldersToExclude,
                    _logger,
                    FileSystem),

                Language.Fsharp => new FsharpProjectComponentsBuilder(
                    projectInfo,
                    _foldersToExclude,
                    _logger,
                    FileSystem),

                _ => throw new NotSupportedException()
            };
    }
}
