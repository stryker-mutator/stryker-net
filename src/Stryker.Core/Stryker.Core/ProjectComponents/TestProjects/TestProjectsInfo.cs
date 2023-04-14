using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public class TestProjectsInfo
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger<MutationTestProcess> _logger;

        public IEnumerable<TestProject> TestProjects { get; set; }
        public IEnumerable<TestFile> TestFiles => TestProjects.SelectMany(testProject => testProject.TestFiles);
        public IEnumerable<IAnalyzerResult> AnalyzerResults => TestProjects.Select(testProject => testProject.AnalyzerResult);

        public TestProjectsInfo(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        public static TestProjectsInfo operator +(TestProjectsInfo a, TestProjectsInfo b)
        {
            a.TestProjects = a.TestProjects.Union(b.TestProjects);
            return a;
        }

        public void RestoreOriginalAssembly(IAnalyzerResult sourceProject)
        {
            foreach (var testProject in AnalyzerResults)
            {
                var injectionPath = GetInjectionFilePath(testProject, sourceProject);
                var backupFilePath = GetBackupName(injectionPath);
                if (_fileSystem.File.Exists(backupFilePath))
                {
                    _fileSystem.File.Copy(backupFilePath, injectionPath, true);
                }
            }
        }

        public void BackupOriginalAssembly(IAnalyzerResult sourceProject)
        {
            foreach (var testProject in AnalyzerResults)
            {
                var injectionPath = GetInjectionFilePath(testProject, sourceProject);
                var backupFilePath = GetBackupName(injectionPath);
                if (!_fileSystem.Directory.Exists(sourceProject.GetAssemblyDirectoryPath()))
                {
                    _fileSystem.Directory.CreateDirectory(sourceProject.GetAssemblyDirectoryPath());
                }
                if (_fileSystem.File.Exists(injectionPath))
                {
                    // Only create backup if there isn't already a backup
                    if (!_fileSystem.File.Exists(backupFilePath))
                    {
                        _fileSystem.File.Move(injectionPath, backupFilePath, false);
                    }
                }
                else
                {
                    _logger.LogWarning("Could not locate source assembly {injectionPath}", injectionPath);
                } 
            }
        }

        public static string GetInjectionFilePath(IAnalyzerResult testProject, IAnalyzerResult sourceProject) => Path.Combine(testProject.GetAssemblyDirectoryPath(), sourceProject.GetAssemblyFileName());

        private static string GetBackupName(string injectionPath) => injectionPath + ".stryker-unchanged";
    }
}
