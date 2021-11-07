using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.ToolHelpers;

namespace Stryker.Core.Initialisation.Buildalyzer
{
    public static class IAnalyzerResultExtensions
    {
        public static string GetAssemblyPath(this IAnalyzerResult analyzerResult)
        {
            return FilePathUtils.NormalizePathSeparators(Path.Combine(
                FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetDir"]),
                FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetFileName"])));
        }

        public static string GetAssemblyName(this IAnalyzerResult analyzerResult)
        {
            return FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["AssemblyName"]);
        }

        public static IEnumerable<ResourceDescription> GetResources(this IAnalyzerResult analyzerResult, IEnumerable<MetadataReference> references, ILogger logger)
        {
            return EmbeddedResourcesGenerator.GetManifestResources(GetAssemblyPath(analyzerResult), references, logger);
        }

        public static CSharpCompilationOptions GetCompilationOptions(this IAnalyzerResult analyzerResult)
        {
            var compilationOptions = new CSharpCompilationOptions(analyzerResult.GetOutputKind())
                .WithNullableContextOptions(analyzerResult.GetNullableContextOptions())
                .WithAllowUnsafe(analyzerResult.GetPropertyOrDefault("AllowUnsafeBlocks", true))
                .WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default)
                .WithConcurrentBuild(true)
                .WithModuleName(analyzerResult.GetAssemblyName())
                .WithOverflowChecks(analyzerResult.GetPropertyOrDefault("CheckForOverflowUnderflow", false));

            if (analyzerResult.IsSignedAssembly())
            {
                compilationOptions = compilationOptions.WithCryptoKeyFile(analyzerResult.GetAssemblyOriginatorKeyFile())
                    .WithStrongNameProvider(new DesktopStrongNameProvider());
            }
            return compilationOptions;
        }

        public static IList<string> GetDefineConstants(this IAnalyzerResult analyzerResult)
        {
            return analyzerResult?.GetPropertyOrDefault("DefineConstants", "").Split(";").Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>();
        }

        public static string AssemblyAttributeFileName(this IAnalyzerResult analyzerResult)
        {
            return analyzerResult.GetPropertyOrDefault("GeneratedAssemblyInfoFile",
                (Path.GetFileNameWithoutExtension(analyzerResult.ProjectFilePath) + ".AssemblyInfo.cs")
                .ToLowerInvariant());
        }

        public static (bool frameworkSupportsAppDomain, bool frameworkSupportsPipes) CompatibilityModes(this IAnalyzerResult analyzerResult)
        {
            var (framework, version) = analyzerResult.GetTargetFrameworkAndVersion();

            var frameworkSupportsAppDomain = true;
            var frameworkSupportsPipes = true;

            switch (framework)
            {
                case Framework.DotNet when version.Major < 2:
                    frameworkSupportsAppDomain = false;
                    break;
                case Framework.DotNetStandard when version.Major < 2:
                    frameworkSupportsAppDomain = false;
                    frameworkSupportsPipes = false;
                    break;
                case Framework.Unknown:
                case Framework.DotNetClassic:
                    frameworkSupportsPipes = version < new Version(3, 5);
                    break;
            }

            return (frameworkSupportsAppDomain, frameworkSupportsPipes);
        }

        public static string GetSymbolFileName(this IAnalyzerResult analyzerResult)
        {
            return Path.ChangeExtension(analyzerResult.GetAssemblyName(), ".pdb");
        }

        public static IEnumerable<ISourceGenerator> GetSourceGenerators(this IAnalyzerResult analyzerResult, ILogger logger = null)
        {
            var generators = new List<ISourceGenerator>();
            foreach (var analyzer in analyzerResult.AnalyzerReferences)
            {
                try
                {
                    var analyzerFileReference = new AnalyzerFileReference(analyzer, AnalyzerAssemblyLoader.Instance);
                    analyzerFileReference.AnalyzerLoadFailed += (sender, e) => throw e.Exception ?? new InvalidOperationException(e.Message);
                    foreach (var generator in analyzerFileReference.GetGenerators(LanguageNames.CSharp))
                    {
                        generators.Add(generator);
                    }
                }
                catch (Exception e)
                {
                    logger?.LogWarning(e,
                    $"Analyzer/Generator assembly {analyzer} could not be loaded. {Environment.NewLine}" +
                    "Generated source code may be missing.");
                }
            }

            return generators;
        }

        private sealed class AnalyzerAssemblyLoader : IAnalyzerAssemblyLoader
        {
            public static IAnalyzerAssemblyLoader Instance = new AnalyzerAssemblyLoader();

            private AnalyzerAssemblyLoader() { }

            public void AddDependencyLocation(string fullPath) { }

            public Assembly LoadFromPath(string fullPath) => Assembly.LoadFrom(fullPath);
        }

        public static Framework GetTargetFramework(this IAnalyzerResult analyzerResult)
        {
            try
            {
                return ParseTargetFramework(analyzerResult.TargetFramework);
            }
            catch (ArgumentException)
            {
                throw new InputException($"Unable to parse framework version string {analyzerResult.TargetFramework}. Please fix the framework version in the csproj.");
            }
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
        public static (Framework Framework, Version Version) GetTargetFrameworkAndVersion(this IAnalyzerResult analyzerResult)
        {
            try
            {
                return (ParseTargetFramework(analyzerResult.TargetFramework), ParseTargetFrameworkVersion(analyzerResult.TargetFramework));
            }
            catch (ArgumentException)
            {
                throw new InputException($"Unable to parse framework version string {analyzerResult.TargetFramework}. Please fix the framework version in the csproj.");
            }
        }

        public static Language GetLanguage(this IAnalyzerResult analyzerResult)
        {
            return analyzerResult.GetPropertyOrDefault("Language") switch
            {
                "F#" => Language.Fsharp,
                "C#" => Language.Csharp,
                _ => Language.Undefined,
            };
        }

        public static bool IsTestProject(this IAnalyzerResult analyzerResult)
        {
            var isTestProject = analyzerResult.GetPropertyOrDefault("IsTestProject", false);
            var hasTestProjectTypeGuid = analyzerResult
                .GetPropertyOrDefault("ProjectTypeGuids", "")
                .Contains("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}");

            return isTestProject || hasTestProjectTypeGuid;
        }

        private static Framework ParseTargetFramework(string targetFrameworkVersionString)
        {
            return targetFrameworkVersionString switch
            {
                string framework when framework.StartsWith("netcoreapp") => Framework.DotNet,
                string framework when framework.StartsWith("netstandard") => Framework.DotNetStandard,
                string framework when framework.StartsWith("net") && char.GetNumericValue(framework[3]) >= 5 => Framework.DotNet,
                string framework when framework.StartsWith("net") && char.GetNumericValue(framework[3]) <= 4 => Framework.DotNetClassic,
                _ => Framework.Unknown
            };
        }

        private static Version ParseTargetFrameworkVersion(string targetFrameworkVersionString)
        {
            var analysis = Regex.Match(targetFrameworkVersionString ?? string.Empty, "(?<version>[\\d\\.]+)");
            if (analysis.Success)
            {
                var version = analysis.Groups["version"].Value;
                if (!version.Contains('.'))
                {
                    version = version.Length switch
                    {
                        1 => $"{version}.0",
                        2 => $"{version[0]}.{version.Substring(1)}",
                        3 => $"{version[0]}.{version[1]}.{version[2]}",
                        _ => throw new ArgumentException("invalid version")
                    };
                }
                return new Version(version);
            }
            return new Version();
        }

        private static OutputKind GetOutputKind(this IAnalyzerResult analyzerResult)
        {
            return analyzerResult.GetPropertyOrDefault("OutputType") switch
            {
                "Exe" => OutputKind.ConsoleApplication,
                "WinExe" => OutputKind.WindowsApplication,
                "Module" => OutputKind.NetModule,
                "AppContainerExe" => OutputKind.WindowsRuntimeApplication,
                "WinMdObj" => OutputKind.WindowsRuntimeMetadata,
                _ => OutputKind.DynamicallyLinkedLibrary
            };
        }

        private static NullableContextOptions GetNullableContextOptions(this IAnalyzerResult analyzerResult)
        {
            if (!Enum.TryParse(analyzerResult.GetPropertyOrDefault("Nullable", "enable"), true, out NullableContextOptions nullableOptions))
            {
                nullableOptions = NullableContextOptions.Enable;
            }

            return nullableOptions;
        }

        private static bool IsSignedAssembly(this IAnalyzerResult analyzerResult)
        {
            return analyzerResult.GetPropertyOrDefault("SignAssembly", false);
        }

        private static string GetAssemblyOriginatorKeyFile(this IAnalyzerResult analyzerResult)
        {
            return Path.Combine(
                Path.GetDirectoryName(analyzerResult.ProjectFilePath),
                analyzerResult.GetPropertyOrDefault("AssemblyOriginatorKeyFile"));
        }

        private static bool GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, bool defaultBoolean)
        {
            var property = GetPropertyOrDefault(analyzerResult, name, defaultBoolean.ToString());

            return bool.Parse(property);
        }

        private static string GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, string defaultValue = null)
        {
            if (analyzerResult.Properties is null || !analyzerResult.Properties.TryGetValue(name, out var property))
            {
                return defaultValue;
            }

            return property;
        }
    }
}
