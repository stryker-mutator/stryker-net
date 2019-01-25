using Buildalyzer;
using Stryker.Core.Initialisation.ProjectComponent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Initialisation
{
    public class ProjectInfo
    {
        public ProjectAnalyzerResult TestProjectAnalyzerResult { get; set; }
        public ProjectAnalyzerResult ProjectUnderTestAnalyzerResult { get; set; }

        public bool FullFramework { get; set; }

        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public FolderComposite ProjectContents { get; set; }

        public string GetInjectionPath()
        {
            var outputPath = ProjectUnderTestAnalyzerResult.Properties.GetValueOrDefault("OutputPath");
            var assemblyName = ProjectUnderTestAnalyzerResult.Properties.GetValueOrDefault("AssemblyName");
            string injectionPath = Path.Combine(outputPath, assemblyName + ".dll");
            return injectionPath;
        }
    }

    public class ProjectAnalyzerResult
    {
        private AnalyzerResult _analyzerResult { get; set; }

        private IEnumerable<string> _projectReferences;
        public IEnumerable<string> ProjectReferences {
            get => _projectReferences ?? _analyzerResult.ProjectReferences;
            set => _projectReferences = value;
        }

        private IReadOnlyDictionary<string, string> _properties;
        public IReadOnlyDictionary<string, string> Properties
        {
            get => _properties ?? _analyzerResult.Properties;
            set => _properties = value;
        }

        private string _targetFramework;
        public string TargetFramework
        {
            get => _targetFramework ?? _analyzerResult.TargetFramework;
            set => _targetFramework = value;
        }

        private IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _packageReferences;
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>  PackageReferences
        {
            get => _packageReferences ?? _analyzerResult.PackageReferences;
            set => _packageReferences = value;
        }

        private string _projectFilePath;
        public string ProjectFilePath
        {
            get => _projectFilePath ?? _analyzerResult.ProjectFilePath;
            set => _projectFilePath = value;
        }

        private string[] _references;
        public string[] References
        {
            get => _references ?? _analyzerResult.References;
            set => _references = value;
        }

        public ProjectAnalyzerResult(AnalyzerResult analyzerResult)
        {
            _analyzerResult = analyzerResult;
        }
    }
}
