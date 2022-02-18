using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Initialisation.ProjectAnalyzer
{
    public static class IAnalyzerResultExtensions
    {
        /// <summary>
        /// Method needed for ProjectFileReader, which uses Buildalyzer directly.
        /// </summary>
        /// <param name="buildAlyzerResult"></param>
        /// <returns></returns>
        public static IAnalysisResult ToAnalyzerResult(this Buildalyzer.IAnalyzerResult buildAlyzerResult)
        {
            return new AnalyzerResult(buildAlyzerResult.ProjectFilePath, buildAlyzerResult.References, buildAlyzerResult.ProjectReferences,
                                        buildAlyzerResult.AnalyzerReferences, buildAlyzerResult.PreprocessorSymbols, buildAlyzerResult.Properties,
                                        buildAlyzerResult.SourceFiles, buildAlyzerResult.Succeeded, buildAlyzerResult.TargetFramework);
        }

        public static string GetAssemblyPath(this IAnalysisResult analyzerResult)
        {
            return FilePathUtils.NormalizePathSeparators(Path.Combine(
                FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetDir"]),
                FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetFileName"])));
        }

        public static string GetAssemblyName(this IAnalysisResult analyzerResult)
        {
            return FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["AssemblyName"]);
        }

        public static IEnumerable<ResourceDescription> GetResources(this IAnalysisResult analyzerResult, ILogger logger)
        {
            return EmbeddedResourcesGenerator.GetManifestResources(GetAssemblyPath(analyzerResult), logger);
        }

        public static CSharpCompilationOptions GetCompilationOptions(this IAnalysisResult analyzerResult)
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

        public static string AssemblyAttributeFileName(this IAnalysisResult analyzerResult)
        {
            return analyzerResult.GetPropertyOrDefault("GeneratedAssemblyInfoFile",
                (Path.GetFileNameWithoutExtension(analyzerResult.ProjectFilePath) + ".AssemblyInfo.cs")
                .ToLowerInvariant());
        }

        public static string GetSymbolFileName(this IAnalysisResult analyzerResult)
        {
            return Path.ChangeExtension(analyzerResult.GetAssemblyName(), ".pdb");
        }

        public static IEnumerable<ISourceGenerator> GetSourceGenerators(this IAnalysisResult analyzerResult, ILogger logger = null)
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

        internal static NuGetFramework GetNuGetFramework(this IAnalysisResult analyzerResult)
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

        internal static bool TargetsFullFramework(this IAnalysisResult analyzerResult)
        {
            return GetNuGetFramework(analyzerResult).IsDesktop();
        }

        internal static bool TargetsFullFramework(this Buildalyzer.IAnalyzerResult analyzerResult)
        {
            return TargetsFullFramework(analyzerResult.ToAnalyzerResult());
        }


        public static Language GetLanguage(this IAnalysisResult analyzerResult)
        {
            return analyzerResult.GetPropertyOrDefault("Language") switch
            {
                "F#" => Language.Fsharp,
                "C#" => Language.Csharp,
                _ => Language.Undefined,
            };
        }

        public static bool IsTestProject(this IAnalysisResult analyzerResult)
        {
            var isTestProject = analyzerResult.GetPropertyOrDefault("IsTestProject", false);
            var hasTestProjectTypeGuid = analyzerResult
                .GetPropertyOrDefault("ProjectTypeGuids", "")
                .Contains("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}");

            return isTestProject || hasTestProjectTypeGuid;
        }

        private static OutputKind GetOutputKind(this IAnalysisResult analyzerResult)
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

        private static NullableContextOptions GetNullableContextOptions(this IAnalysisResult analyzerResult)
        {
            if (!Enum.TryParse(analyzerResult.GetPropertyOrDefault("Nullable", "enable"), true, out NullableContextOptions nullableOptions))
            {
                nullableOptions = NullableContextOptions.Enable;
            }

            return nullableOptions;
        }

        private static bool IsSignedAssembly(this IAnalysisResult analyzerResult)
        {
            return analyzerResult.GetPropertyOrDefault("SignAssembly", false);
        }

        private static string GetAssemblyOriginatorKeyFile(this IAnalysisResult analyzerResult)
        {
            return Path.Combine(
                Path.GetDirectoryName(analyzerResult.ProjectFilePath),
                analyzerResult.GetPropertyOrDefault("AssemblyOriginatorKeyFile"));
        }

        private static bool GetPropertyOrDefault(this IAnalysisResult analyzerResult, string name, bool defaultBoolean)
        {
            var property = GetPropertyOrDefault(analyzerResult, name, defaultBoolean.ToString());

            return bool.Parse(property);
        }

        private static string GetPropertyOrDefault(this IAnalysisResult analyzerResult, string name, string defaultValue = null)
        {
            if (analyzerResult.Properties is null || !analyzerResult.Properties.TryGetValue(name, out var property))
            {
                return defaultValue;
            }

            return property;
        }
    }
}
