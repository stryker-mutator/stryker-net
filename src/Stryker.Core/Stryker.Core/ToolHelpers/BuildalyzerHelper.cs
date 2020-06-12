using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Core.ToolHelpers
{
    public static class BuildalyzerHelper
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

        public static IEnumerable<ResourceDescription> GetResources(this IAnalyzerResult analyzerResult, ILogger logger)
        {
            return EmbeddedResourcesGenerator.GetManifestResources(GetAssemblyPath(analyzerResult), logger);
        }

        public static string GetAssemblyOriginatorKeyFile(this IAnalyzerResult analyzerResult)
        {
            return Path.Combine(
                Path.GetDirectoryName(analyzerResult.ProjectFilePath),
                analyzerResult?.Properties?.GetValueOrDefault("AssemblyOriginatorKeyFile"));
        }

        public static string GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, string defaultValue = null)
        {
            if (analyzerResult.Properties == null || !analyzerResult.Properties.TryGetValue(name, out var result))
            {
                result = defaultValue;
            }

            return result;
        }

        public static bool GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, bool defaultBoolean)
        {
            if (analyzerResult.Properties != null && analyzerResult.Properties.TryGetValue(name, out var result))
            {
                return bool.Parse(result);
            }

            return defaultBoolean;
        }

        public static CSharpCompilationOptions GetCompilationOptions(this IAnalyzerResult analyzerResult)
        {
            var kind = analyzerResult.GetPropertyOrDefault("OutputType") switch
            {
                "Exe" => OutputKind.ConsoleApplication,
                "WinExe" => OutputKind.WindowsApplication,
                "Module" => OutputKind.NetModule,
                "AppContainerExe" => OutputKind.WindowsRuntimeApplication,
                "WinMdObj" => OutputKind.WindowsRuntimeMetadata,
                _ => OutputKind.DynamicallyLinkedLibrary
            };

            if (!Enum.TryParse(typeof(NullableContextOptions), analyzerResult.GetPropertyOrDefault("Nullable", "enable"), true, out var nullableOptions))
            {
                nullableOptions = NullableContextOptions.Enable;
            }

            var result = new CSharpCompilationOptions(kind)
                .WithNullableContextOptions((NullableContextOptions)nullableOptions)
                .WithAllowUnsafe(analyzerResult.GetPropertyOrDefault("AllowUnsafeBlocks", true))
                .WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default)
                .WithConcurrentBuild(true)
                .WithModuleName(analyzerResult.GetAssemblyName())
                .WithOverflowChecks(analyzerResult.GetPropertyOrDefault("CheckForOverflowUnderflow", false));

            if (analyzerResult.IsSignedAssembly())
            {
                result = result.WithCryptoKeyFile(Path.Combine(
                        Path.GetDirectoryName(analyzerResult.ProjectFilePath),
                        analyzerResult.GetPropertyOrDefault("AssemblyOriginatorKeyFile")))
                    .WithStrongNameProvider(new DesktopStrongNameProvider());
            }
            return result;
        }

        public static bool IsSignedAssembly(this IAnalyzerResult analyzerResult)
        {
            return bool.Parse(analyzerResult?.Properties?.GetValueOrDefault("SignAssembly") ?? "false");
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
        public static (Framework Framework, Version Version) TargetFrameworkAndVersion(this IAnalyzerResult analyzerResult)
        {
            var label = new Dictionary<string, Framework>
            {
                ["netcoreapp"] = Framework.NetCore,
                ["netstandard"] = Framework.NetStandard,
                ["net"] = Framework.NetClassic
            };
            try
            {
                var analysis = Regex.Match(analyzerResult.TargetFramework ?? string.Empty, "(?<kind>\\D+)(?<version>[\\d\\.]+)");
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
                throw new StrykerInputException($"Unable to parse framework version string {analyzerResult.TargetFramework}. Please fix the framework version in the csproj.");
            }
        }

        public static IList<string> GetDefineConstants(this IAnalyzerResult analyzerResult)
        {
            var constants = analyzerResult?.GetPropertyOrDefault("DefineConstants", "").Split(";").Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>();

            var (frameworkSupportsAppDomain, frameworkSupportsPipes) = CompatibilityModes(analyzerResult);

            if (!frameworkSupportsAppDomain)
            {
                constants.Add("STRYKER_NO_DOMAIN");
            }
            if (!frameworkSupportsPipes)
            {
                constants.Add("STRYKER_NO_PIPE");
            }

            return constants;
        }

        public static string AssemblyAttributeFileName(this IAnalyzerResult analyzerResult)
        {
            return analyzerResult.GetPropertyOrDefault("GeneratedAssemblyInfoFile",
                (Path.GetFileNameWithoutExtension(analyzerResult.ProjectFilePath) + ".AssemblyInfo.cs")
                .ToLowerInvariant());
        }

        public static (bool frameworkSupportsAppDomain, bool frameworkSupportsPipes) CompatibilityModes(this IAnalyzerResult analyzerResult)
        {
            var (framework, version) = analyzerResult.TargetFrameworkAndVersion();

            bool frameworkSupportsAppDomain = true;
            bool frameworkSupportsPipes = true;

            switch (framework)
            {
                case Framework.NetCore when version.Major < 2:
                    frameworkSupportsAppDomain = false;
                    break;
                case Framework.NetStandard when version.Major < 2:
                    frameworkSupportsAppDomain = false;
                    frameworkSupportsPipes = false;
                    break;
                case Framework.Unknown:
                case Framework.NetClassic:
                    frameworkSupportsPipes = version < new Version(3, 5);
                    break;
            }

            return (frameworkSupportsAppDomain, frameworkSupportsPipes);
        }

        public static string GetSymbolFileName(this IAnalyzerResult analyzerResult)
        {
            return Path.ChangeExtension(analyzerResult.GetAssemblyName(), ".pdb");
        }
    }
}
