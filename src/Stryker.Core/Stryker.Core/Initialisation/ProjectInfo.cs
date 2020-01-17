using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.ProjectComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Initialisation
{
    public class ProjectInfo
    {
        public ProjectAnalyzerResult TestProjectAnalyzerResult { get; set; }
        public ProjectAnalyzerResult ProjectUnderTestAnalyzerResult { get; set; }

        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public FolderComposite ProjectContents { get; set; }

        public string GetInjectionPath()
        {
            return Path.Combine(Path.GetDirectoryName(FilePathUtils.NormalizePathSeparators(TestProjectAnalyzerResult.AssemblyPath)), Path.GetFileName(ProjectUnderTestAnalyzerResult.AssemblyPath));
        }

        public string GetTestBinariesPath()
        {
            return TestProjectAnalyzerResult.AssemblyPath;
        }
    }

    public enum Framework
    {
        NetClassic,
        NetCore,
        NetStandard,
        Unknown
    };

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
            get => _assemblyPath ?? FilePathUtils.NormalizePathSeparators(Path.Combine(
                FilePathUtils.NormalizePathSeparators(_analyzerResult.Properties["TargetDir"]),
                FilePathUtils.NormalizePathSeparators(_analyzerResult.Properties["TargetFileName"])));
            set => _assemblyPath = FilePathUtils.NormalizePathSeparators(value);
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

        private string _targetFrameworkVersionString;

        /// <summary>
        /// Reads the TargetFramework MSBuild property which includes the target framework and the target framework version
        /// </summary>
        /// <value>
        /// The TargetFramework MSBuild property including the target framework. Example: netcoreapp3.0
        /// </value>
        public string TargetFrameworkVersionString
        {
            get => _targetFrameworkVersionString ?? _analyzerResult?.TargetFramework;
            set => _targetFrameworkVersionString = value;
        }

        private Framework _targetFramework = Framework.Unknown;

        /// <summary>
        /// Extracts a target <c>Framework</c> from the MSBuild property TargetFramework
        /// </summary>
        /// <value>
        /// The target <c>Framework</c> of the project
        /// </value>
        /// <example>
        /// Framework.NetClassic
        /// </example>
        public Framework TargetFramework
        {
            get => _targetFramework == Framework.Unknown ? TargetFrameworkAndVersion.framework : _targetFramework;
            set => _targetFramework = value;
        }

        private Version _targetFrameworkVersion;

        /// <summary>
        /// Extracts a target <c>Version</c> from the MSBuild property TargetFramework
        /// </summary>
        /// <value>
        /// The <c>Version</c> of the target framework
        /// </value>
        /// <example>
        /// 3.0
        /// </example>
        public Version TargetFrameworkVersion
        {
            get => _targetFrameworkVersion ?? TargetFrameworkAndVersion.version;
            set => _targetFrameworkVersion = value;
        }

        private IList<string> _defineConstants;
        public IList<string> DefineConstants
        {
            get => _defineConstants ?? BuildDefineConstants();
            set => _defineConstants = value;
        }

        private IList<string> BuildDefineConstants()
        {
            var constants = _analyzerResult?.GetProperty("DefineConstants")?.Split(";")?.ToList() ?? new List<string>();

            var (frameworkDoesNotSupportAppDomain, frameworkDoesNotSupportPipes) = CompatibilityModes;

            if (frameworkDoesNotSupportAppDomain)
            {
                constants.Add("STRYKER_NO_DOMAIN");
            }
            if (frameworkDoesNotSupportPipes)
            {
                constants.Add("STRYKER_NO_PIPE");
            }

            return constants;
        }

        public (bool compat_noAppDomain, bool compat_noPipe) CompatibilityModes
        {
            get
            {
                var (framework, version) = TargetFrameworkAndVersion;

                bool compat_noAppDomain = false;
                bool compat_noPipe = false;

                switch (framework)
                {
                    case Framework.NetCore when version.Major < 2:
                        compat_noAppDomain = true;
                        break;
                    case Framework.NetStandard when version.Major < 2:
                        compat_noAppDomain = true;
                        compat_noPipe = true;
                        break;
                    case Framework.Unknown:
                    case Framework.NetClassic:
                        compat_noPipe = version < new Version(3, 5);
                        break;
                }

                return (compat_noAppDomain, compat_noPipe);
            }
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

        private bool? _signAssembly;
        public bool SignAssembly
        {
            get => _signAssembly ?? bool.Parse(_analyzerResult?.Properties?.GetValueOrDefault("SignAssembly") ?? "false");
            set => _signAssembly = value;
        }

        private string _assemblyOriginatorKeyFile;
        public string AssemblyOriginatorKeyFile
        {
            get => _assemblyOriginatorKeyFile ?? Path.Combine(
                Path.GetDirectoryName(ProjectFilePath),
                _analyzerResult?.Properties?.GetValueOrDefault("AssemblyOriginatorKeyFile"));
            set => _assemblyOriginatorKeyFile = value;
        }

        private IEnumerable<ResourceDescription> _resources;
        public IEnumerable<ResourceDescription> Resources
        {
            get => _resources ?? EmbeddedResourcesGenerator.GetManifestResources(AssemblyPath, _logger);
            set => _resources = value;
        }

        /// <summary>
        /// Extracts a target <c>Framework</c> and <c>Version</c> from the MSBuild property TargetFramework
        /// </summary>
        /// <returns>
        /// A tuple of <c>Framework</c> and <c>Version</c> which together form the target framework and framework version of the project.
        /// </returns>
        /// <example>
        /// <c>(Framework.NetCore, 3.0)</c>
        /// </example>
        public (Framework framework, Version version) TargetFrameworkAndVersion
        {
            get
            {
                var label = new Dictionary<string, Framework>
                {
                    ["netcoreapp"] = Framework.NetCore,
                    ["netstandard"] = Framework.NetStandard,
                    ["net"] = Framework.NetClassic
                };
                try
                {
                    var analysis = Regex.Match(TargetFrameworkVersionString ?? string.Empty, "(?<kind>\\D+)(?<version>[\\d\\.]+)");
                    if (analysis.Success && label.ContainsKey(analysis.Groups["kind"].Value))
                    {
                        var version = analysis.Groups["version"].Value;
                        if (!version.Contains('.'))
                        {
                            if (version.Length == 2)
                            // we have a aggregated version id
                            {
                                version = $"{version[0]}.{version.Substring(1)}";
                            }
                            else if (version.Length == 3)
                            {
                                version = $"{version[0]}.{version[1]}.{version[2]}";
                            }
                        }
                        return (label[analysis.Groups["kind"].Value], new Version(version));
                    }

                    return (Framework.Unknown, new Version());
                }
                catch (ArgumentException)
                {
                    throw new StrykerInputException($"Unable to parse framework version string {TargetFrameworkVersionString}. Please fix the framework version in the csproj.");
                }
            }
        }
    }
}
