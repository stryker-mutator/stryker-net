using Microsoft.Extensions.Logging;
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
        ProjectInfo ResolveInput(string currentDirectory, string projectUnderTestNameFilter, List<string> filesToExclude);
    }

    /// <summary>
    ///  - Reads .csproj to find project under test
    ///  - Scans project under test and store files to mutate
    ///  - Build composite for files
    /// </summary>
    public class InputFileResolver : IInputFileResolver
    {
        private string[] _foldersToExclude = { "obj", "bin", "node_modules" };
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
        public ProjectInfo ResolveInput(string currentDirectory, string projectName, List<string> filesToExclude)
        {
            string projectFile = ScanProjectFile(currentDirectory);
            var currentProjectInfo = ReadProjectFile(projectFile, projectName);
            var projectReferencePath = FilePathUtils.ConvertPathSeparators(currentProjectInfo.ProjectReference);
            var projectUnderTestPath = Path.GetDirectoryName(Path.GetFullPath(Path.Combine(currentDirectory, projectReferencePath)));
            var projectUnderTestInfo = FindProjectUnderTestAssemblyName(Path.GetFullPath(Path.Combine(projectUnderTestPath, Path.GetFileName(projectReferencePath))));
            var inputFiles = FindInputFiles(projectUnderTestPath, filesToExclude);

            return new ProjectInfo()
            {
                TestProjectPath = currentDirectory,
                TestProjectFileName = Path.GetFileName(projectFile),
                TargetFramework = currentProjectInfo.TargetFramework,
                ProjectContents = inputFiles,
                ProjectUnderTestPath = projectUnderTestPath,
                ProjectUnderTestAssemblyName = projectUnderTestInfo ?? Path.GetFileNameWithoutExtension(projectReferencePath),
                ProjectUnderTestProjectName = Path.GetFileNameWithoutExtension(projectReferencePath),
            };
        }

        /// <summary>
        /// Recursively scans the given directory for files to mutate
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private FolderComposite FindInputFiles(string path, List<string> filesToExclude)
        {
            var folderComposite = new FolderComposite
            {
                Name = Path.GetFullPath(path)
            };

            foreach (var folder in _fileSystem.Directory.EnumerateDirectories(path).Where(x => !_foldersToExclude.Contains(Path.GetFileName(x))))
            {
                folderComposite.Add(FindInputFiles(folder, filesToExclude));
            }

            foreach (var file in _fileSystem.Directory.GetFiles(path, "*.cs", SearchOption.TopDirectoryOnly))
            {
                if (IncludeAsFileLeaf(file, filesToExclude))
                {
                    folderComposite.Add(
                        new FileLeaf
                        {
                            SourceCode = _fileSystem.File.ReadAllText(file),
                            Name = Path.GetFileName(file),
                            FullPath = file
                        });
                }
            }

            return folderComposite;
        }

        private bool IncludeAsFileLeaf(string filePath, List<string> filesToExclude)
        {
            return filesToExclude.All(file => Path.GetFullPath(file) != Path.GetFullPath(filePath));
        }

        public string ScanProjectFile(string currentDirectory)
        {
            var projectFiles = _fileSystem.Directory.GetFiles(currentDirectory, "*.csproj", SearchOption.AllDirectories);
            _logger.LogTrace("Scanned the current directory for *.csproj files: found {0}", projectFiles);
            if (projectFiles.Count() > 1)
            {
                throw new FileNotFoundException("Expected exactly one .csproj file, found more than one. Please fix your project contents");
            } else if (!projectFiles.Any())
            {
                throw new FileNotFoundException($"No .csproj file found, please check your project directory at {Directory.GetCurrentDirectory()}");
            }
            _logger.LogInformation("Using {0} as project file", projectFiles.First());
            return projectFiles.First();
        }

        public ProjectFile ReadProjectFile(string projectFilePath, string projectName)
        {
            var projectFile = _fileSystem.File.OpenText(projectFilePath);
            XDocument xDocument = XDocument.Load(projectFile);
            var projectInfo = new ProjectFileReader().ReadProjectFile(xDocument, projectName);

            _logger.LogDebug("Values found in project file {@0}", projectInfo);

            return projectInfo;
        }

        public string FindProjectUnderTestAssemblyName(string projectFilePath)
        {
            var projectFile = _fileSystem.File.OpenText(projectFilePath);
            XDocument xDocument = XDocument.Load(projectFile);
            return new ProjectFileReader().FindAssemblyName(xDocument);
        }
    }
}
