﻿using Buildalyzer;
using Microsoft.CodeAnalysis;
using Stryker.Core.Initialisation.ProjectComponent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

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
            var outputPath = "";
            if (FullFramework)
            {
                outputPath = Path.Combine(TestProjectAnalyzerResult.Properties.GetValueOrDefault("MSBuildProjectDirectory"),
                    TestProjectAnalyzerResult.Properties.GetValueOrDefault("OutputPath"));
            }
            else
            {
                outputPath = TestProjectAnalyzerResult.Properties.GetValueOrDefault("OutputPath");
            }

            var targetFileName = ProjectUnderTestAnalyzerResult.Properties.GetValueOrDefault("TargetFileName");
            string injectionPath = FilePathUtils.ConvertPathSeparators(Path.Combine(outputPath, targetFileName));
            return injectionPath;
        }

        public string GetTestBinariesPath()
        {
            var outputPath = "";
            if (FullFramework)
            {
                outputPath = Path.Combine(TestProjectAnalyzerResult.Properties.GetValueOrDefault("MSBuildProjectDirectory"),
                    TestProjectAnalyzerResult.Properties.GetValueOrDefault("OutputPath"));
            }
            else
            {
                outputPath = TestProjectAnalyzerResult.Properties.GetValueOrDefault("OutputPath");
            }

            var targetFileName = TestProjectAnalyzerResult.Properties.GetValueOrDefault("TargetFileName");
            string binariesPath = FilePathUtils.ConvertPathSeparators(Path.Combine(outputPath, targetFileName));
            return binariesPath;
        }
    }

    public class ProjectAnalyzerResult
    {
        private AnalyzerResult _analyzerResult { get; set; }

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
                return _resources ?? CreateResourceDescriptions(_analyzerResult.Items["EmbeddedResource"]);
            }
            set => _resources = value;
        }

        public ProjectAnalyzerResult(AnalyzerResult analyzerResult)
        {
            _analyzerResult = analyzerResult;
        }

        private IEnumerable<ResourceDescription> CreateResourceDescriptions(ProjectItem[] embeddedResources)
        {
            foreach (var resource in embeddedResources)
            {
                yield return new ResourceDescription(
                    /*_analyzerResult.Items[] + */Regex.Replace(Regex.Replace(resource.ItemSpec, @"/", "."), @"\\", "."),
                    () => File.OpenRead(Path.Combine(Path.GetDirectoryName(_projectFilePath), resource.ItemSpec)),
                    true);
            }
        }
    }
}
