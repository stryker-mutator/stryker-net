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
        private IFileSystem _fileSystem { get; }
        private IProjectFileReader _projectFileReader { get; }
        private ILogger _logger { get; set; }

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
            var projectAnalyzerResult = new ProjectInfo();
            var projectFile = ScanProjectFile(options.BasePath);

            // Analyze the test project
            projectAnalyzerResult.TestProjectAnalyzerResult = _projectFileReader.AnalyzeProject(projectFile, options.SolutionPath);

            // Determine project under test
            var reader = new ProjectFileReader();
            var projectUnderTest = reader.DetermineProjectUnderTest(projectAnalyzerResult.TestProjectAnalyzerResult.ProjectReferences, options.ProjectUnderTestNameFilter);

            // Analyze project under test
            projectAnalyzerResult.ProjectUnderTestAnalyzerResult = _projectFileReader.AnalyzeProject(projectUnderTest, options.SolutionPath);

            var inputFiles = new FolderComposite();
            projectAnalyzerResult.FullFramework = !projectAnalyzerResult.TestProjectAnalyzerResult.TargetFramework.Contains("netcoreapp", StringComparison.InvariantCultureIgnoreCase);
            var projectUnderTestDir = Path.GetDirectoryName(projectAnalyzerResult.ProjectUnderTestAnalyzerResult.ProjectFilePath);

            IDictionary<string, string> compileIncludeLinkedFiles = FindCompileLinkedFiles(projectAnalyzerResult.ProjectUnderTestAnalyzerResult.ProjectFilePath);
            var sharedProjectFilePaths = FindSharedProjects(projectAnalyzerResult.ProjectUnderTestAnalyzerResult.ProjectFilePath, projectAnalyzerResult.TestProjectAnalyzerResult);

            var projectFolders = ExtractProjectFolders(projectAnalyzerResult.ProjectUnderTestAnalyzerResult, sharedProjectFilePaths);

            foreach (var dir in projectFolders)
            {
                var folder = _fileSystem.Path.Combine(Path.GetDirectoryName(projectUnderTestDir), dir);

                _logger.LogDebug($"Scanning {folder}");
                if (!_fileSystem.Directory.Exists(folder))
                {
                    throw new DirectoryNotFoundException($"Can't find {folder}");
                }
                inputFiles.Add(FindInputFiles(folder, options.FilesToExclude.ToList()));
            }

            foreach (var sharedProjectFile in sharedProjectFilePaths)
            {
                compileIncludeLinkedFiles.ToList().AddRange(FindCompileLinkedFiles(sharedProjectFile));
            }

            foreach (var compileIncludeLinkedFile in compileIncludeLinkedFiles.OrderBy(a => a.Value))
            {
                var fullActualFilePath = Path.GetFullPath(compileIncludeLinkedFile.Key);
                var fullLinkFilePath = Path.GetFullPath(Path.Combine(
                    Path.GetDirectoryName(projectAnalyzerResult.ProjectUnderTestAnalyzerResult.ProjectFilePath), compileIncludeLinkedFile.Value));
                if (inputFiles.Children.First().FullPath == Path.GetDirectoryName(fullLinkFilePath))
                {
                    inputFiles.Children.Add(new FileLeaf()
                    {
                        SourceCode = _fileSystem.File.ReadAllText(fullActualFilePath),
                        Name = _fileSystem.Path.GetFileName(fullActualFilePath),
                        RelativePath = Path.Combine(inputFiles.Children.First().RelativePath, Path.GetFileName(fullLinkFilePath)),
                        FullPath = fullLinkFilePath
                    });
                }
            }

            MarkInputFilesAsExcluded(inputFiles, options.FilesToExclude.ToList(), projectUnderTestDir);
            projectAnalyzerResult.ProjectContents = inputFiles;

            ValidateResult(projectAnalyzerResult, options);

            return projectAnalyzerResult;
        }

        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private FolderComposite FindInputFiles(string path, List<string> filesToExclude, string parentFolder = null)
        {
            var lastPathComponent = Path.GetFileName(path);

            var folderComposite = new FolderComposite
            {
                Name = lastPathComponent,
                FullPath = Path.GetFullPath(path),
                RelativePath = parentFolder is null ? lastPathComponent : Path.Combine(parentFolder, lastPathComponent)
            };
            foreach (var folder in _fileSystem.Directory.EnumerateDirectories(folderComposite.FullPath).Where(x => !_foldersToExclude.Contains(Path.GetFileName(x))))
            {
                folderComposite.Add(FindInputFiles(folder, filesToExclude, folderComposite.RelativePath));
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
                        FullPath = file
                    });
                }
            }

            return folderComposite;
        }

        private void MarkInputFilesAsExcluded(FolderComposite root, List<string> pathsToExclude, string projectUnderTestPath)
        {
            var allFiles = root.GetAllFiles().ToList();

            foreach (var path in pathsToExclude)
            {
                var fullPath = path.StartsWith(projectUnderTestPath) ? path : Path.GetFullPath(projectUnderTestPath + path);
                if (!Path.HasExtension(path))
                {
                    _logger.LogInformation("Scanning dir {0} for files to exclude.", fullPath);
                }
                var filesToExclude = allFiles.Where(x => x.FullPath.StartsWith(fullPath)).ToList();
                if (filesToExclude.Count() == 0)
                {
                    _logger.LogWarning("No file to exclude was found for path {0}. Did you mean to exclude another file?", fullPath);
                }
                foreach (var file in filesToExclude)
                {
                    _logger.LogInformation("File {0} will be excluded from mutation.", file.FullPath);
                    file.IsExcluded = true;
                }
            }
        }

        public string ScanProjectFile(string currentDirectory)
        {
            var projectFiles = _fileSystem.Directory.GetFiles(currentDirectory, "*.csproj");
            _logger.LogTrace("Scanned the current directory for *.csproj files: found {0}", projectFiles);
            if (projectFiles.Count() > 1)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Expected exactly one .csproj file, found more than one:");
                foreach (var file in projectFiles)
                {
                    sb.AppendLine(file);
                }
                sb.AppendLine();
                sb.AppendLine("Please fix your project contents");
                throw new StrykerInputException(sb.ToString());
            }
            else if (!projectFiles.Any())
            {
                throw new StrykerInputException($"No .csproj file found, please check your project directory at {Directory.GetCurrentDirectory()}");
            }
            _logger.LogInformation("Using {0} as project file", projectFiles.Single());
            return projectFiles.Single();
        }

        private IEnumerable<string> ExtractProjectFolders(ProjectAnalyzerResult projectAnalyzerResult, IEnumerable<string> sharedProjects)
        {
            var projectFilePath = projectAnalyzerResult.ProjectFilePath;
            var projectDirectory = _fileSystem.Path.GetDirectoryName(projectFilePath);

            var folders = new List<string>
            {
                projectDirectory
            };

            foreach (var sharedProjectFile in sharedProjects)
            {
                var sharedProjectDirectoryName = _fileSystem.Path.GetDirectoryName(sharedProjectFile);
                folders.Add(_fileSystem.Path.Combine(projectDirectory, sharedProjectDirectoryName));
            }

            return folders;
        }

        private IEnumerable<string> FindSharedProjects(string projectFilePath, ProjectAnalyzerResult projectAnalyzerResult)
        {
            var sharedProjects = new List<string>();
            var projectFile = _fileSystem.File.OpenText(projectFilePath);
            var projectDirectory = _fileSystem.Path.GetDirectoryName(projectFilePath);

            var xDocument = XDocument.Load(projectFile);
            foreach (var sharedProject in _projectFileReader.FindSharedProjects(xDocument))
            {
                var sharedProjectName = ReplaceMsbuildProperties(sharedProject, projectAnalyzerResult);

                if (!_fileSystem.File.Exists(_fileSystem.Path.Combine(projectDirectory, sharedProjectName)))
                {
                    throw new FileNotFoundException($"Missing shared project {sharedProjectName}");
                }

                sharedProjects.Add(_fileSystem.Path.Combine(projectDirectory, sharedProject));
            }

            return sharedProjects;
        }

        private IDictionary<string, string> FindCompileLinkedFiles(string projectFilePath)
        {
            var files = new Dictionary<string, string>();
            var projectFile = _fileSystem.File.OpenText(projectFilePath);
            var projectDirectory = _fileSystem.Path.GetDirectoryName(projectFilePath);

            var xDocument = XDocument.Load(projectFile);

            foreach (var compileLinkedFile in _projectFileReader.FindLinkedFiles(xDocument))
            {
                if (!_fileSystem.File.Exists(_fileSystem.Path.Combine(projectDirectory, compileLinkedFile.Key)))
                {
                    throw new FileNotFoundException($"Missing compile linked file {compileLinkedFile}");
                }

                files.Add(_fileSystem.Path.Combine(projectDirectory, compileLinkedFile.Key), compileLinkedFile.Value);
            }

            return files;
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
            if (projectInfo.FullFramework &&
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
