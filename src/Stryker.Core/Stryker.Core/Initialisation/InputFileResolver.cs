using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters.Extensions;
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
        private const string ErrorMessage = "Project reference issue.";
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

            _logger.LogInformation("Identifying project to mutate.");
            var testProjectAnalyzerResults = new List<ProjectAnalyzerResult>();
            foreach (var testProjectFile in testProjectFiles)
            {
                // Analyze the test project
                testProjectAnalyzerResults.Add(_projectFileReader.AnalyzeProject(testProjectFile, options.SolutionPath));
            }
            projectInfo.TestProjectAnalyzerResults = testProjectAnalyzerResults;

            // Determine project under test
            projectUnderTest = FindProjectUnderTest(projectInfo.TestProjectAnalyzerResults, options.ProjectUnderTestNameFilter);

            _logger.LogInformation("The project {0} will be mutated.", projectUnderTest);

            // Analyze project under test
            projectInfo.ProjectUnderTestAnalyzerResult = _projectFileReader.AnalyzeProject(projectUnderTest, options.SolutionPath);

            FolderComposite inputFiles;
            if (projectInfo.ProjectUnderTestAnalyzerResult.SourceFiles != null && projectInfo.ProjectUnderTestAnalyzerResult.SourceFiles.Any())
            {
                inputFiles = FindProjectFilesUsingBuildalyzer(projectInfo.ProjectUnderTestAnalyzerResult, options);
            }
            else
            {
                inputFiles = FindProjectFilesScanningProjectFolders(projectInfo.ProjectUnderTestAnalyzerResult, options);
            }
            projectInfo.ProjectContents = inputFiles;

            ValidateTestProjectsCanBeExecuted(projectInfo, options);
            _logger.LogInformation("Analysis complete.");

            return projectInfo;
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

        private FolderComposite FindProjectFilesUsingBuildalyzer(ProjectAnalyzerResult analyzerResult, StrykerOptions options)
        {
            var inputFiles = new FolderComposite();
            var projectUnderTestDir = Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            var projectRoot = Path.GetDirectoryName(projectUnderTestDir);
            var generatedAssemblyInfo = (_fileSystem.Path.GetFileNameWithoutExtension(analyzerResult.ProjectFilePath) + ".AssemblyInfo.cs").ToLowerInvariant();
            var rootFolderComposite = new FolderComposite()
            {
                Name = string.Empty,
                FullPath = projectRoot,
                RelativePath = string.Empty,
                RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, projectUnderTestDir)
            };
            var cache = new Dictionary<string, FolderComposite> { [string.Empty] = rootFolderComposite };
            inputFiles.Add(rootFolderComposite);

            CSharpParseOptions cSharpParseOptions = BuildCsharpParseOptions(analyzerResult, options);
            InjectMutantHelpers(rootFolderComposite, cSharpParseOptions);

            foreach (var sourceFile in analyzerResult.SourceFiles)
            {
                // Skip xamarin UI generated files
                if (sourceFile.EndsWith(".xaml.cs"))
                {
                    continue;
                }

                // Skip assembly info
                if (_fileSystem.Path.GetFileName(sourceFile).ToLowerInvariant() == generatedAssemblyInfo)
                {
                    continue;
                }

                var relativePath = Path.GetRelativePath(projectUnderTestDir, sourceFile);
                var folderComposite = GetOrBuildFolderComposite(cache, Path.GetDirectoryName(relativePath), projectUnderTestDir, projectRoot, inputFiles);
                var fileName = Path.GetFileName(sourceFile);

                var file = new FileLeaf()
                {
                    SourceCode = _fileSystem.File.ReadAllText(sourceFile),
                    Name = _fileSystem.Path.GetFileName(sourceFile),
                    RelativePath = _fileSystem.Path.Combine(folderComposite.RelativePath, fileName),
                    FullPath = sourceFile,
                    RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, sourceFile)
                };

                // Get the syntax tree for the source file
                var syntaxTree = CSharpSyntaxTree.ParseText(file.SourceCode,
                    path: file.FullPath,
                    options: cSharpParseOptions);

                // don't mutate auto generated code
                if (syntaxTree.IsGenerated())
                {
                    _logger.LogDebug("Skipping auto-generated code file: {fileName}", file.Name);
                    folderComposite.AddCompilationSyntaxTree(syntaxTree); // Add the syntaxTree to the list of compilationSyntaxTrees
                    continue; // Don't add the file to the folderComposite as we're not reporting on the file
                }


                file.SyntaxTree = syntaxTree;
                folderComposite.Add(file);
            }

            return inputFiles;
        }

        private void InjectMutantHelpers(FolderComposite rootFolderComposite, CSharpParseOptions cSharpParseOptions)
        {
            foreach (var (name, code) in CodeInjection.MutantHelpers)
            {
                rootFolderComposite.AddCompilationSyntaxTree(CSharpSyntaxTree.ParseText(code, path: name, options: cSharpParseOptions));
            }
        }

        private CSharpParseOptions BuildCsharpParseOptions(ProjectAnalyzerResult analyzerResult, StrykerOptions options)
        {
            var preprocessorSymbols = analyzerResult.DefineConstants;

            var cSharpParseOptions = new CSharpParseOptions(options.LanguageVersion, DocumentationMode.None, preprocessorSymbols: preprocessorSymbols);
            return cSharpParseOptions;
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
                        RelativePathToProjectFile = Path.GetRelativePath(projectUnderTestDir, fullPath)
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

        public string FindTestProject(string path)
        {
            var projectFile = FindProjectFile(path);
            _logger.LogDebug("Using {0} as test project", projectFile);

            return projectFile;
        }

        public string FindProjectUnderTest(IEnumerable<ProjectAnalyzerResult> testProjects, string projectUnderTestNameFilter)
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

        public string FindProjectFile(string path)
        {
            if (_fileSystem.File.Exists(path) && _fileSystem.Path.HasExtension(".csproj"))
            {
                return path;
            }

            string[] projectFiles;
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
            _logger.LogTrace("Found project file {file} in path {path}", projectFiles.Single(), path);

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

        private void ValidateTestProjectsCanBeExecuted(ProjectInfo projectInfo, StrykerOptions options)
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

        private static IEnumerable<string> FindProjectsReferencedByAllTestProjects(IEnumerable<ProjectAnalyzerResult> testProjects)
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
            StringBuilder builder = new StringBuilder();
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
