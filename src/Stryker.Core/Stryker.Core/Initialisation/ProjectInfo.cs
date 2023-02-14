using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Buildalyzer;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Initialisation
{
    public class ProjectInfo : IProjectAndTest
    {
        private readonly IFileSystem _fileSystem;

        public ProjectInfo() : this(null) { }

        public ProjectInfo(IFileSystem fileSystem) => _fileSystem = fileSystem ?? new FileSystem();

        public IEnumerable<IAnalyzerResult> TestProjectAnalyzerResults { get; set; }

        public IAnalyzerResult ProjectUnderTestAnalyzerResult { get; set; }

        /// <summary>
        /// Stores warning messages aggregated during the testing process
        /// </summary>
        public IList<string> ProjectWarnings {get;} = new List<string>();

        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public IProjectComponent ProjectContents { get; set; }

        public CodeInjection CodeInjector { get;} = new();
        
        public string GetInjectionFilePath(IAnalyzerResult analyzerResult) => Path.Combine(
                Path.GetDirectoryName(analyzerResult.GetAssemblyPath()),
                Path.GetFileName(ProjectUnderTestAnalyzerResult.GetAssemblyPath()));

        public virtual void RestoreOriginalAssembly()
        {
            foreach (var testProject in TestProjectAnalyzerResults)
            {
                var injectionPath = GetInjectionFilePath(testProject);
                _fileSystem.File.Copy(GetBackupName(injectionPath), injectionPath, true);
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
        public bool IsFullFramework => ProjectUnderTestAnalyzerResult.TargetsFullFramework();
        public string HelperNamespace => CodeInjector.HelperNamespace;
        public IReadOnlyList<string> TestAssemblies => TestProjectAnalyzerResults.Select( a => a.GetAssemblyPath()).ToList();
    }
}
