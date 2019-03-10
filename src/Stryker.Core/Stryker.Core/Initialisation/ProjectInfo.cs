using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.IO;

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
            return FilePathUtils.ConvertPathSeparators(Path.Combine(
                FilePathUtils.ConvertPathSeparators(Path.GetDirectoryName(TestProjectAnalyzerResult.AssemblyPath)),
                FilePathUtils.ConvertPathSeparators(Path.GetFileName(ProjectUnderTestAnalyzerResult.AssemblyPath))));
        }

        public string GetTestBinariesPath()
        {
            return TestProjectAnalyzerResult.AssemblyPath;
        }
    }

    public class ProjectAnalyzerResult
    {
        private readonly ILogger _logger;
        private readonly AnalyzerResult _analyzerResult;

        public ProjectAnalyzerResult(ILogger logger, AnalyzerResult analyzerResult)
        {
            _logger = logger;
            _analyzerResult = analyzerResult;
        }

        private string _assemblyPath;
        public string AssemblyPath
        {
            get => _assemblyPath ?? FilePathUtils.ConvertPathSeparators(Path.Combine(
                FilePathUtils.ConvertPathSeparators(_analyzerResult.Properties["TargetDir"]),
                FilePathUtils.ConvertPathSeparators(_analyzerResult.Properties["TargetFileName"])));
            set => _assemblyPath = value;
        }

        private IEnumerable<string> _projectReferences;
        public IEnumerable<string> ProjectReferences
        {
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
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> PackageReferences
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

        private IEnumerable<ResourceDescription> _resources;
        public IEnumerable<ResourceDescription> Resources
        {
            get
            {
                return _resources ?? EmbeddedResourcesGenerator.GetManifestResources(AssemblyPath, _logger);
            }
            set => _resources = value;
        }
    }
}
