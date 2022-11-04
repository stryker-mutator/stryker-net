using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;

namespace Stryker.Core.Initialisation.Buildalyzer
{
    public static class IAnalyzerResultExtensions
    {
        public static string GetAssemblyPath(this IAnalyzerResult analyzerResult) =>
            FilePathUtils.NormalizePathSeparators(Path.Combine(
                FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetDir"]),
                FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetFileName"])));

        public static string GetAssemblyName(this IAnalyzerResult analyzerResult) => FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["AssemblyName"]);

        public static IEnumerable<ResourceDescription> GetResources(this IAnalyzerResult analyzerResult, ILogger logger) => EmbeddedResourcesGenerator.GetManifestResources(GetAssemblyPath(analyzerResult), logger);

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

        public static CSharpParseOptions GetParseOptions(this IAnalyzerResult analyzerResult, StrykerOptions options) => new CSharpParseOptions(options.LanguageVersion, DocumentationMode.None, preprocessorSymbols: analyzerResult.PreprocessorSymbols);

        public static string AssemblyAttributeFileName(this IAnalyzerResult analyzerResult) =>
            analyzerResult.GetPropertyOrDefault("GeneratedAssemblyInfoFile",
                (Path.GetFileNameWithoutExtension(analyzerResult.ProjectFilePath) + ".AssemblyInfo.cs")
                .ToLowerInvariant());

        public static string GetSymbolFileName(this IAnalyzerResult analyzerResult) => Path.ChangeExtension(analyzerResult.GetAssemblyName(), ".pdb");

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
            public static readonly IAnalyzerAssemblyLoader Instance = new AnalyzerAssemblyLoader();

            private AnalyzerAssemblyLoader() { }

            public void AddDependencyLocation(string fullPath) { }

            public Assembly LoadFromPath(string fullPath) => Assembly.LoadFrom(fullPath);
        }

        internal static NuGetFramework GetNuGetFramework(this IAnalyzerResult analyzerResult)
        {
            var framework = NuGetFramework.Parse(analyzerResult.TargetFramework ?? "");
            if (framework == NuGetFramework.UnsupportedFramework)
            {
                var atPath = string.IsNullOrEmpty(analyzerResult.ProjectFilePath) ? "" : $" at '{analyzerResult.ProjectFilePath}'";
                var message = $"The target framework '{analyzerResult.TargetFramework}' is not supported. Please fix the target framework in the csproj{atPath}.";
                throw new InputException(message);
            }
            return framework;
        }

        internal static bool TargetsFullFramework(this IAnalyzerResult analyzerResult) => GetNuGetFramework(analyzerResult).IsDesktop();

        public static Language GetLanguage(this IAnalyzerResult analyzerResult) =>
            analyzerResult.GetPropertyOrDefault("Language") switch
            {
                "F#" => Language.Fsharp,
                "C#" => Language.Csharp,
                _ => Language.Undefined,
            };

        public static bool IsTestProject(this IAnalyzerResult analyzerResult)
        {
            var isTestProject = analyzerResult.GetPropertyOrDefault("IsTestProject", false);
            var hasTestProjectTypeGuid = analyzerResult
                .GetPropertyOrDefault("ProjectTypeGuids", "")
                .Contains("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}");

            return isTestProject || hasTestProjectTypeGuid;
        }

        private static OutputKind GetOutputKind(this IAnalyzerResult analyzerResult) =>
            analyzerResult.GetPropertyOrDefault("OutputType") switch
            {
                "Exe" => OutputKind.ConsoleApplication,
                "WinExe" => OutputKind.WindowsApplication,
                "Module" => OutputKind.NetModule,
                "AppContainerExe" => OutputKind.WindowsRuntimeApplication,
                "WinMdObj" => OutputKind.WindowsRuntimeMetadata,
                _ => OutputKind.DynamicallyLinkedLibrary
            };

        private static NullableContextOptions GetNullableContextOptions(this IAnalyzerResult analyzerResult)
        {
            if (!Enum.TryParse(analyzerResult.GetPropertyOrDefault("Nullable", "enable"), true, out NullableContextOptions nullableOptions))
            {
                nullableOptions = NullableContextOptions.Enable;
            }

            return nullableOptions;
        }

        private static bool IsSignedAssembly(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("SignAssembly", false);

        private static string GetAssemblyOriginatorKeyFile(this IAnalyzerResult analyzerResult) =>
            Path.Combine(
                Path.GetDirectoryName(analyzerResult.ProjectFilePath),
                analyzerResult.GetPropertyOrDefault("AssemblyOriginatorKeyFile"));

        private static bool GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, bool defaultBoolean) => bool.Parse(GetPropertyOrDefault(analyzerResult, name, defaultBoolean.ToString()));

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
