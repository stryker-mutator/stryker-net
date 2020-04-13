using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.Initialisation
{
    public class ProjectInfo
    {
        public IEnumerable<ProjectAnalyzerResult> TestProjectAnalyzerResults { get; set; }
        public ProjectAnalyzerResult ProjectUnderTestAnalyzerResult { get; set; }

        /// <summary>
        /// The Folder/File structure found in the project under test.
        /// </summary>
        public FolderComposite ProjectContents { get; set; }

        public string GetInjectionPath(ProjectAnalyzerResult testProject)
        {
            return Path.Combine(Path.GetDirectoryName(FilePathUtils.NormalizePathSeparators(testProject.AssemblyPath)), 
                Path.GetFileName(ProjectUnderTestAnalyzerResult.AssemblyPath));
        }

        public string GetTestBinariesPath(ProjectAnalyzerResult projectAnalyzerResult)
        {
            return projectAnalyzerResult.AssemblyPath;
        }
    }

    public enum Framework
    {
        NetClassic,
        NetCore,
        NetStandard,
        Unknown
    }

    public class ProjectAnalyzerResult
    {
        private readonly ILogger _logger;
        private readonly AnalyzerResult _analyzerResult;
        private string _assemblyPath;
        private IEnumerable<string> _projectReferences;
        private IEnumerable<string> _sourceFiles;
        private IEnumerable<ResourceDescription> _resources;
        private IReadOnlyDictionary<string, string> _properties;
        private string _targetFrameworkVersionString;
        private Framework _targetFramework = Framework.Unknown;
        private string _projectFilePath;
        private string[] _references;

        public ProjectAnalyzerResult(ILogger logger, AnalyzerResult analyzerResult)
        {
            _logger = logger;
            _analyzerResult = analyzerResult;
        }

        public string TargetFileName => GetPropertyOrDefault("TargetFileName", AssemblyName+".dll");

        public string AssemblyName =>  GetPropertyOrDefault("AssemblyName");

        public string SymbolFileName => $"{AssemblyName}.pdb";

        public string AssemblyPath
        {
            get => _assemblyPath ?? Path.Combine(TargetDirectory, TargetFileName);
            set => _assemblyPath = FilePathUtils.NormalizePathSeparators(value);
        }

        public string TargetDirectory => _assemblyPath != null ? Path.GetDirectoryName(_assemblyPath) : FilePathUtils.NormalizePathSeparators(GetPropertyOrDefault("TargetDir"));

        public IEnumerable<string> ProjectReferences
        {
            get => _projectReferences ?? _analyzerResult.ProjectReferences;
            set => _projectReferences = value;
        }

        public IEnumerable<string> SourceFiles
        {
            get => _sourceFiles ?? _analyzerResult?.SourceFiles;
            set => _sourceFiles = value;
        }

        public IReadOnlyDictionary<string, string> Properties
        {
            get => _properties ?? _analyzerResult?.Properties;
            set => _properties = value;
        }

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

        /// <summary>
        /// Extracts a target <c>Version</c> from the MSBuild property TargetFramework
        /// </summary>
        /// <value>
        /// The <c>Version</c> of the target framework
        /// </value>
        /// <example>
        /// 3.0
        /// </example>
        public Version TargetFrameworkVersion => TargetFrameworkAndVersion.version;

        public IList<string> DefineConstants => BuildDefineConstants();

        private IList<string> BuildDefineConstants()
        {
            var constants = GetPropertyOrDefault("DefineConstants", "").Split(";").ToList();

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

                var compat_noAppDomain = false;
                var compat_noPipe = false;

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

        public string ProjectFilePath
        {
            get => _projectFilePath ?? _analyzerResult?.ProjectFilePath;
            set => _projectFilePath = value;
        }

        public string[] References
        {
            get => _references ?? _analyzerResult.References;
            set => _references = value;
        }

        public IEnumerable<ResourceDescription> Resources
        {
            get => _resources ?? EmbeddedResourcesGenerator.GetManifestResources(AssemblyPath, _logger);
            set => _resources = value;
        }

        public string AssemblyAttributeFileName =>
            GetPropertyOrDefault("GeneratedAssemblyInfoFile",
                (Path.GetFileNameWithoutExtension(ProjectFilePath) + ".AssemblyInfo.cs")
                .ToLowerInvariant());

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

        public string GetPropertyOrDefault(string name, string defaultValue = null)
        {
            if (Properties == null || !Properties.TryGetValue(name, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        public bool GetPropertyOrDefault(string name, bool defaultBoolean)
        {
            if (Properties != null && Properties.TryGetValue(name, out var result))
            {
                return bool.Parse(result);
            }

            return defaultBoolean;
        }

        public CSharpCompilationOptions GetCompilationOptions()
        {
            var kind = GetPropertyOrDefault("OutputType") switch
            {
                "Exe" => OutputKind.ConsoleApplication,
                "WinExe" => OutputKind.WindowsApplication,
                "Module" => OutputKind.NetModule,
                "AppContainerExe" => OutputKind.WindowsRuntimeApplication,
                "WinMdObj" => OutputKind.WindowsRuntimeMetadata,
                _ => OutputKind.DynamicallyLinkedLibrary
            };

            if (!Enum.TryParse(typeof(NullableContextOptions), GetPropertyOrDefault("Nullable", "enable"), true, out var nullableOptions))
            {
                nullableOptions = NullableContextOptions.Enable;
            }
            
            var result = new CSharpCompilationOptions(kind)
                .WithNullableContextOptions((NullableContextOptions) nullableOptions)
                .WithAllowUnsafe(GetPropertyOrDefault("AllowUnsafeBlocks", true))
                .WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default)
                .WithConcurrentBuild(true)
                .WithModuleName(TargetFileName)
                .WithOverflowChecks(GetPropertyOrDefault("CheckForOverflowUnderflow", false));

            if (GetPropertyOrDefault("SignAssembly", false))
            {
                result = result.WithCryptoKeyFile(Path.Combine(
                        Path.GetDirectoryName(ProjectFilePath),
                        GetPropertyOrDefault("AssemblyOriginatorKeyFile")))
                    .WithStrongNameProvider(new DesktopStrongNameProvider());
            }
            return result;
        }
    }
}
