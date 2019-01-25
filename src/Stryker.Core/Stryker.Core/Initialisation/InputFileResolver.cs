using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;

namespace Stryker.Core.Initialisation
{
    public interface IInputFileResolver
    {
        ProjectInfo ResolveInput(string currentDirectory, 
            string projectUnderTestNameFilter, 
            List<string> filesToExclude);
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
        public ProjectInfo ResolveInput(string currentDirectory, 
            string projectUnderTestNameFilter, 
            List<string> filesToExclude)
        {
            var result = new ProjectInfo();
            var projectFile = ScanProjectFile(currentDirectory);

            // Analyze the test project
            result.TestProjectAnalyzerResult = _projectFileReader.AnalyzeProject(projectFile);

            // Determine project under test
            var reader = new ProjectFileReader();
            var projectUnderTest = reader.DetermineProjectUnderTest(result.TestProjectAnalyzerResult.ProjectReferences, projectUnderTestNameFilter);
            
            // Analyze project under test
            result.ProjectUnderTestAnalyzerResult = _projectFileReader.AnalyzeProject(projectUnderTest);

            var inputFiles = new FolderComposite();
            result.FullFramework = !result.TestProjectAnalyzerResult.TargetFramework.Contains("netcoreapp", StringComparison.InvariantCultureIgnoreCase);
            var projectUnderTestDir = Path.GetDirectoryName(result.ProjectUnderTestAnalyzerResult.ProjectFilePath);
            foreach (var dir in ExtractProjectFolders(result.ProjectUnderTestAnalyzerResult.ProjectFilePath, result.FullFramework))
            {
                var folder = _fileSystem.Path.Combine(Path.GetDirectoryName(projectUnderTestDir), dir);

                _logger.LogDebug($"Scanning {folder}");
                if (!_fileSystem.Directory.Exists(folder))
                {
                    throw new DirectoryNotFoundException($"Can't find {folder}");
                }
                inputFiles.Add(FindInputFiles(folder, filesToExclude));
            }
            MarkInputFilesAsExcluded(inputFiles, filesToExclude, projectUnderTestDir);
            result.ProjectContents = inputFiles;

            return result;
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
                var fileName = Path.GetFileName(file);
                folderComposite.Add(new FileLeaf()
                {
                    SourceCode = _fileSystem.File.ReadAllText(file),
                    Name = _fileSystem.Path.GetFileName(file),
                    RelativePath = Path.Combine(folderComposite.RelativePath, fileName),
                    FullPath = file
                });
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
            var projectFiles = _fileSystem.Directory.GetFiles(currentDirectory, "*.csproj", SearchOption.AllDirectories);
            _logger.LogTrace("Scanned the current directory for *.csproj files: found {0}", projectFiles);
            if (projectFiles.Count() > 1)
            {
                throw new StrykerInputException("Expected exactly one .csproj file, found more than one. Please fix your project contents");
            }
            else if (!projectFiles.Any())
            {
                throw new StrykerInputException($"No .csproj file found, please check your project directory at {Directory.GetCurrentDirectory()}");
            }
            _logger.LogInformation("Using {0} as project file", projectFiles.First());
            return projectFiles.First();
        }

        private IEnumerable<string> ExtractProjectFolders(string projectFilePath, bool fullFramework)
        {
            var projectFile = _fileSystem.File.OpenText(projectFilePath);
            var xDocument = XDocument.Load(projectFile);
            var folders = new List<string>();
            var projectDirectory = _fileSystem.Path.GetDirectoryName(projectFilePath);
            folders.Add(projectDirectory);
            if (!fullFramework)
            {
                foreach (var sharedProject in new ProjectFileReader().FindSharedProjects(xDocument))
                {

                    if (!_fileSystem.File.Exists(_fileSystem.Path.Combine(projectDirectory, sharedProject)))
                    {
                        throw new FileNotFoundException($"Missing shared project {sharedProject}");
                    }

                    var directoryName = _fileSystem.Path.GetDirectoryName(sharedProject);
                    folders.Add(_fileSystem.Path.Combine(projectDirectory, directoryName));
                }
            }

            return folders;
        }
    }
}
