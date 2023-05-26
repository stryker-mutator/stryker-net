using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static string GetAssemblyFileName(this IAnalyzerResult analyzerResult) => FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetFileName"]);
        public static string GetAssemblyDirectoryPath(this IAnalyzerResult analyzerResult) => FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetDir"]);
        public static string GetAssemblyPath(this IAnalyzerResult analyzerResult) => FilePathUtils.NormalizePathSeparators(Path.Combine(analyzerResult.GetAssemblyDirectoryPath(), analyzerResult.GetAssemblyFileName()));

        public static string GetAssemblyName(this IAnalyzerResult analyzerResult) => FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["AssemblyName"]);

        public static IEnumerable<ResourceDescription> GetResources(this IAnalyzerResult analyzerResult, ILogger logger)
        {
            var rootNamespace = analyzerResult.GetRootNamespace();
            var embeddedResources = analyzerResult.GetItem("EmbeddedResource").Select(x => x.ItemSpec);
            return EmbeddedResourcesGenerator.GetManifestResources(
                analyzerResult.GetAssemblyPath(),
                analyzerResult.ProjectFilePath,
                rootNamespace,
                embeddedResources);
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

            if (analyzerResult.IsSignedAssembly() && analyzerResult.GetAssemblyOriginatorKeyFile() is var keyFile && keyFile is not null)
            {
                compilationOptions = compilationOptions.WithCryptoKeyFile(keyFile)
                    .WithStrongNameProvider(new DesktopStrongNameProvider());
            }
            return compilationOptions;
        }

        public static CSharpParseOptions GetParseOptions(this IAnalyzerResult analyzerResult, StrykerOptions options) => new CSharpParseOptions(options.LanguageVersion, DocumentationMode.None, preprocessorSymbols: analyzerResult.PreprocessorSymbols);

        public static string AssemblyAttributeFileName(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("GeneratedAssemblyInfoFile",
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

            public void AddDependencyLocation(string fullPath)
            {
                // discard any specific folder
            }

            public Assembly LoadFromPath(string fullPath) => Assembly.LoadFrom(fullPath); //NOSONAR we actually need to load a specified file, not a specific assembly
        }

        internal static NuGetFramework GetNuGetFramework(this IAnalyzerResult analyzerResult)
        {
            var framework = NuGetFramework.Parse(analyzerResult.TargetFramework ?? "");
            if (framework != NuGetFramework.UnsupportedFramework)
            {
                return framework;
            }
            var atPath = string.IsNullOrEmpty(analyzerResult.ProjectFilePath) ? "" : $" at '{analyzerResult.ProjectFilePath}'";
            var message = $"The target framework '{analyzerResult.TargetFramework}' is not supported. Please fix the target framework in the csproj{atPath}.";
            throw new InputException(message);
        }

        internal static bool TargetsFullFramework(this IAnalyzerResult analyzerResult) => GetNuGetFramework(analyzerResult).IsDesktop();

        public static Language GetLanguage(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("Language") switch
        {
            "F#" => Language.Fsharp,
            "C#" => Language.Csharp,
            _ => Language.Undefined,
        };

        private static readonly string[] knownTestPackages = { "MSTest.TestFramework", "xunit", "NUnit" };

        public static bool IsTestProject(this IAnalyzerResult analyzerResult)
        {
            if (!bool.TryParse(analyzerResult.GetPropertyOrDefault("IsTestProject"), out var isTestProject))
            {
                isTestProject = knownTestPackages.Any(n => analyzerResult.PackageReferences.ContainsKey(n));
            }
            var hasTestProjectTypeGuid = analyzerResult
                .GetPropertyOrDefault("ProjectTypeGuids", "")
                .Contains("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}");

            return isTestProject || hasTestProjectTypeGuid;
        }

        private static OutputKind GetOutputKind(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("OutputType") switch
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

        private static string GetAssemblyOriginatorKeyFile(this IAnalyzerResult analyzerResult)
        {
            var assemblyKeyFileProp = analyzerResult.GetPropertyOrDefault("AssemblyOriginatorKeyFile");
            if (assemblyKeyFileProp is null)
            {
                return assemblyKeyFileProp;
            }

            return Path.Combine(Path.GetDirectoryName(analyzerResult.ProjectFilePath), assemblyKeyFileProp);
        }

        private static string GetRootNamespace(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("RootNamespace") ?? analyzerResult.GetAssemblyName();

        private static bool GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, bool defaultBoolean) => bool.Parse(GetPropertyOrDefault(analyzerResult, name, defaultBoolean.ToString()));

        private static string GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, string defaultValue = default)
        {
            if (analyzerResult.Properties is null || !analyzerResult.Properties.TryGetValue(name, out var property))
            {
                return defaultValue;
            }

            return property;
        }

        private static IEnumerable<IProjectItem> GetItem(this IAnalyzerResult analyzerResult, string name)
        {
            if (analyzerResult.Items is null || !analyzerResult.Items.TryGetValue(name, out var item))
            {
                return Enumerable.Empty<IProjectItem>();
            }

            return item;
        }
    }
}
