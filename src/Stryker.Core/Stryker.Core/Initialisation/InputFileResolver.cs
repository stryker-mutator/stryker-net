using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Logging;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;

namespace Stryker.Core.Initialisation
{
    public interface IInputFileResolver
    {
        ProjectInfo ResolveInput(string currentDirectory, string projectUnderTestNameFilter);
    }

    /// <summary>
    ///  - Reads .csproj to find project under test
    ///  - Scans project under test and store files to mutate
    ///  - Build composite for files
    /// </summary>
    public class InputFileResolver : IInputFileResolver
    {
        private IEnumerable<string> _foldersToIgnore = new string[] { "obj", "bin", "node_modules" };
        private IFileSystem _fileSystem { get; }
        private ILogger _logger { get; set; }

        public InputFileResolver(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InputFileResolver>();
        }

        public InputFileResolver() : this(new FileSystem()) { }

        /// <summary>
        /// Finds the referencedProjects and looks for all files that should be mutated in those projects
        /// </summary>
        public ProjectInfo ResolveInput(string currentDirectory, string projectName)
        {
            var projectFile = ScanProjectFile(currentDirectory);
            var currentProjectInfo = ReadProjectFile(projectFile, projectName);
            var projectReferencePath = FilePathUtils.ConvertPathSeparators(currentProjectInfo.ProjectReference);
            
            var projectUnderTestPath = Path.GetDirectoryName(Path.GetFullPath(Path.Combine(currentDirectory, projectReferencePath)));
            var projectReference = Path.Combine(projectUnderTestPath, Path.GetFileName(projectReferencePath));
            var projectFilePath = Path.GetFullPath(projectReference);
            var projectUnderTestInfo = FindProjectUnderTestAssemblyName(projectFilePath);
            var inputFiles = new FolderComposite();
            
            foreach (var dir in ExtractProjectFolders(projectFilePath))
            {
                var folder = _fileSystem.Path.Combine(Path.GetDirectoryName(projectFilePath), dir);

                _logger.LogDebug($"Scanning {folder}");
                if (!_fileSystem.Directory.Exists(folder))
                {
                     throw new DirectoryNotFoundException($"Can't find {folder}");
                }
                inputFiles.Add(FindInputFiles(folder));
            }
            
            return new ProjectInfo()
            {
                TestProjectPath = currentDirectory,
                TestProjectFileName = Path.GetFileName(projectFile),
                TargetFramework = currentProjectInfo.TargetFramework,
                ProjectContents = inputFiles,
                ProjectUnderTestPath = projectUnderTestPath,
                ProjectUnderTestAssemblyName = projectUnderTestInfo ?? Path.GetFileNameWithoutExtension(projectReferencePath),
                ProjectUnderTestProjectName = Path.GetFileNameWithoutExtension(projectReferencePath),
                AppendTargetFrameworkToOutputPath = currentProjectInfo.AppendTargetFrameworkToOutputPath
            };
        }

        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private FolderComposite FindInputFiles(string path)
        {
            var folderComposite = new FolderComposite()
            {
                Name = Path.GetFullPath(path)
            };
            foreach (var folder in _fileSystem.Directory.EnumerateDirectories(path).Where(x => !_foldersToIgnore.Contains(Path.GetFileName(x))))
            {
                folderComposite.Add(FindInputFiles(folder));
            }
            foreach (var file in _fileSystem.Directory.GetFiles(_fileSystem.Path.GetFullPath(path), "*.cs", SearchOption.TopDirectoryOnly))
            {
               
                folderComposite.Add(new FileLeaf()
                {
                    SourceCode = _fileSystem.File.ReadAllText(file),
                    Name = _fileSystem.Path.GetFileName(file),
                    FullPath = file
                });
            }
            return folderComposite;
        }

        public string ScanProjectFile(string currentDirectory)
        {
            var projectFiles = _fileSystem.Directory.GetFiles(currentDirectory, "*.csproj", SearchOption.AllDirectories);
            _logger.LogTrace("Scanned the current directory for *.csproj files: found {0}", projectFiles);
            if (projectFiles.Count() > 1)
            {
                throw new StrykerInputException("Expected exactly one .csproj file, found more than one. Please fix your project contents");
            } else if (!projectFiles.Any())
            {
                throw new StrykerInputException($"No .csproj file found, please check your project directory at {Directory.GetCurrentDirectory()}");
            }
            _logger.LogInformation("Using {0} as project file", projectFiles.First());
            return projectFiles.First();
        }

        private IEnumerable<string> ExtractProjectFolders(string projectFilePath)
        {
            var projectFile = _fileSystem.File.OpenText(projectFilePath);
            var xDocument = XDocument.Load(projectFile);
            var folders = new List<string>();
            var projectDirectory = _fileSystem.Path.GetDirectoryName(projectFilePath);
            folders.Add(projectDirectory);
            foreach (var sharedProject in new ProjectFileReader().FindSharedProjects(xDocument))
            {
                
                if (!_fileSystem.File.Exists(_fileSystem.Path.Combine(projectDirectory, sharedProject)))
                {
                    throw new FileNotFoundException($"Missing shared project {sharedProject}");
                }

                var directoryName = _fileSystem.Path.GetDirectoryName(sharedProject);
                folders.Add(_fileSystem.Path.Combine(projectDirectory, directoryName));
            }
            return folders;
        }

        public ProjectFile ReadProjectFile(string projectFilePath, string projectName)
        {
            var projectFile = _fileSystem.File.OpenText(projectFilePath);
            var xDocument = XDocument.Load(projectFile);
            var projectInfo = new ProjectFileReader().ReadProjectFile(xDocument, projectName);

            _logger.LogDebug("Values found in project file {@0}", projectInfo);

            return projectInfo;
        }

        public string FindProjectUnderTestAssemblyName(string projectFilePath)
        {
            var projectFile = _fileSystem.File.OpenText(projectFilePath);
            var xDocument = XDocument.Load(projectFile);
            return new ProjectFileReader().FindAssemblyName(xDocument);
        }
    }
}
