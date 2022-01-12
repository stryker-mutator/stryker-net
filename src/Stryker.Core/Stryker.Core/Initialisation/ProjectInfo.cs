using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Buildalyzer;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Initialisation
{
    public class ProjectInfo
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;

        public ProjectInfo() : this(null) { }

        public ProjectInfo(IFileSystem fileSystem)
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectInfo>();
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public IEnumerable<IAnalyzerResult> TestProjectAnalyzerResults { get; set; }
        public IAnalyzerResult ProjectUnderTestAnalyzerResult { get; set; }

        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public IProjectComponent ProjectContents { get; set; }

        public string GetInjectionFilePath(IAnalyzerResult analyzerResult) =>
            Path.Combine(
                Path.GetDirectoryName(analyzerResult.GetAssemblyPath()),
                Path.GetFileName(ProjectUnderTestAnalyzerResult.GetAssemblyPath()));

        public virtual void RestoreOriginalAssembly()
        {
            foreach (var testProject in TestProjectAnalyzerResults)
            {
                var injectionPath = GetInjectionFilePath(testProject);
                try
                {
                     _fileSystem.File.Move(GetBackupName(injectionPath), injectionPath, true);
                }
                catch (UnauthorizedAccessException e)
                {
                    _logger.LogWarning("Failed to restore the original assembly. {0}", e);
                }
            }
        }

        public virtual void BackupOriginalAssembly()
        {
            foreach (var testProject in TestProjectAnalyzerResults)
            {
                var injectionPath = GetInjectionFilePath(testProject);
                var destFileName = GetBackupName(injectionPath);
                if (!_fileSystem.Directory.Exists(Path.GetDirectoryName(injectionPath)))
                {
                    _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(injectionPath));
                }
                if (_fileSystem.File.Exists(injectionPath) && !_fileSystem.File.Exists(destFileName))
                {
                    // if the backup is here, it means the source is already mutated, so no backup
                    _fileSystem.File.Move(injectionPath, destFileName, false);
                }
            }
        }

        private static string GetBackupName(string injectionPath) => injectionPath + ".stryker-unchanged";
    }
}
