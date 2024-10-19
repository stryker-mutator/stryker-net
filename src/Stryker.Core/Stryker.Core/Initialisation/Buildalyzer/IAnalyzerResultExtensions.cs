using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using Stryker.Abstractions.Exceptions;
using Stryker.Utilities;

namespace Stryker.Core.Initialisation.Buildalyzer;

public static class IAnalyzerResultExtensions
{
    public static string GetAssemblyFileName(this IAnalyzerResult analyzerResult) =>
        FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetFileName"]);

    public static bool BuildsAnAssembly(this IAnalyzerResult analyzerResult) => analyzerResult.Properties.ContainsKey("TargetFileName");

    public static string GetReferenceAssemblyPath(this IAnalyzerResult analyzerResult) => analyzerResult.Properties.TryGetValue("TargetRefPath", out var property) ? FilePathUtils.NormalizePathSeparators(property) : analyzerResult.GetAssemblyPath();

    public static string GetAssemblyDirectoryPath(this IAnalyzerResult analyzerResult) =>
        FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetDir"]);

    public static string GetAssemblyPath(this IAnalyzerResult analyzerResult) =>
        FilePathUtils.NormalizePathSeparators(Path.Combine(analyzerResult.GetAssemblyDirectoryPath(),
            analyzerResult.GetAssemblyFileName()));

    public static string GetAssemblyName(this IAnalyzerResult analyzerResult) =>
        FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["AssemblyName"]);

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

    public static string AssemblyAttributeFileName(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("GeneratedAssemblyInfoFile",
            (Path.GetFileNameWithoutExtension(analyzerResult.ProjectFilePath) + ".AssemblyInfo.cs")
            .ToLowerInvariant());

    public static string GetSymbolFileName(this IAnalyzerResult analyzerResult) =>
        Path.ChangeExtension(analyzerResult.GetAssemblyName(), ".pdb");

    public static string TargetPlatform(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("TargetPlatform", "AnyCPU");

    public static string MsBuildPath(this IAnalyzerResult analyzerResult) => analyzerResult.Analyzer?.EnvironmentFactory.GetBuildEnvironment()?.MsBuildExePath;

    public static IEnumerable<ISourceGenerator> GetSourceGenerators(this IAnalyzerResult analyzerResult, ILogger logger)
    {
        var generators = new List<ISourceGenerator>();
        foreach (var analyzer in analyzerResult.AnalyzerReferences)
        {
            try
            {
                var analyzerFileReference = new AnalyzerFileReference(analyzer, AnalyzerAssemblyLoader.Instance);
                analyzerFileReference.AnalyzerLoadFailed += (sender, e) => LogAnalyzerLoadError(logger, sender, e);
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

    [ExcludeFromCodeCoverage(Justification = "Impossible to unit test")]
    private static void LogAnalyzerLoadError(ILogger logger, object sender, AnalyzerLoadFailureEventArgs e)
    {
        var source = ((AnalyzerReference)sender)?.Display ?? "unknown";
        logger?.LogWarning(
            "Failed to load analyzer '{Source}': {Message} (error : {Error}, analyzer: {Analyzer}).",
            source, e.Message, Enum.GetName(e.ErrorCode.GetType(), e.ErrorCode) ?? e.ErrorCode.ToString(),
            e.TypeName ?? "All");
        if (e.ErrorCode == AnalyzerLoadFailureEventArgs.FailureErrorCode.ReferencesNewerCompiler)
        {
            logger?.LogWarning(
                "The analyzer '{Source}' references a newer version ({ReferencedCompilerVersion}) of the compiler than the one used by Stryker.NET.",
                source, e.ReferencedCompilerVersion);
        }

        if (e.Exception != null)
        {
            logger?.LogWarning("Failed to load analyzer '{Source}': Exception {Exception}.", source, e.Exception);
        }
    }

    public static IEnumerable<MetadataReference> LoadReferences(this IAnalyzerResult analyzerResult)
    {
        foreach (var reference in analyzerResult.References)
        {
            var referenceFileNameWithoutExtension = Path.GetFileNameWithoutExtension(reference);
            string packageWithAlias = null;

            if (analyzerResult.PackageReferences is not null)
            {
                // Check for any matching package reference with an alias using LINQ
                packageWithAlias = analyzerResult.PackageReferences
                    .Where(pr => pr.Key == referenceFileNameWithoutExtension && pr.Value.ContainsKey("Aliases"))
                    .Select(pr => pr.Value["Aliases"])
                    .FirstOrDefault();
            }
                
            if (packageWithAlias is not null)
            {
                // Return the reference with the alias
                yield return MetadataReference.CreateFromFile(reference).WithAliases(new[] { packageWithAlias });
            }
            else
            {
                // If no alias is found, return the reference without aliases
                yield return MetadataReference.CreateFromFile(reference);
            }
        }
    }

    private sealed class AnalyzerAssemblyLoader : IAnalyzerAssemblyLoader
    {
        public static readonly IAnalyzerAssemblyLoader Instance = new AnalyzerAssemblyLoader();

        private AnalyzerAssemblyLoader() { }

        public void AddDependencyLocation(string fullPath)
        {
        }

        public Assembly LoadFromPath(string fullPath) =>
            Assembly.LoadFrom(fullPath); //NOSONAR we actually need to load a specified file, not a specific assembly
    }

    internal static NuGetFramework GetNuGetFramework(this IAnalyzerResult analyzerResult)
    {
        var framework = NuGetFramework.Parse(analyzerResult.TargetFramework);
        if (framework != NuGetFramework.UnsupportedFramework)
        {
            return framework;
        }

        var atPath = string.IsNullOrEmpty(analyzerResult.ProjectFilePath)
            ? ""
            : $" at '{analyzerResult.ProjectFilePath}'";
        var message =
            $"The target framework '{analyzerResult.TargetFramework}' is not supported. Please fix the target framework in the csproj{atPath}.";
        throw new InputException(message);
    }

    internal static bool TargetsFullFramework(this IAnalyzerResult analyzerResult) => analyzerResult.GetNuGetFramework().IsDesktop();

    public static Language GetLanguage(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("Language") switch
        {
            "F#" => Language.Fsharp,
            "C#" => Language.Csharp,
            _ => Language.Undefined,
        };

    private static readonly string[] knownTestPackages = ["MSTest.TestFramework", "xunit", "NUnit"];

    public static bool IsTestProject(this IEnumerable<IAnalyzerResult> analyzerResults) => analyzerResults.Any(x => x.IsTestProject());

    public static bool IsTestProject(this IAnalyzerResult analyzerResult)
    {
        if (!bool.TryParse(analyzerResult.GetPropertyOrDefault("IsTestProject"), out var isTestProject))
        {
            isTestProject = Array.Exists(knownTestPackages, n => analyzerResult.PackageReferences.ContainsKey(n));
        }

        var hasTestProjectTypeGuid = analyzerResult
            .GetPropertyOrDefault("ProjectTypeGuids", "")
            .Contains("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}");

        return isTestProject || hasTestProjectTypeGuid;
    }

    internal static OutputKind GetOutputKind(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("OutputType") switch
        {
            "Exe" => OutputKind.ConsoleApplication,
            "WinExe" => OutputKind.WindowsApplication,
            "Module" => OutputKind.NetModule,
            "AppContainerExe" => OutputKind.WindowsRuntimeApplication,
            "WinMdObj" => OutputKind.WindowsRuntimeMetadata,
            _ => OutputKind.DynamicallyLinkedLibrary
        };

    internal static bool IsSignedAssembly(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("SignAssembly", false);

    internal static string GetAssemblyOriginatorKeyFile(this IAnalyzerResult analyzerResult)
    {
        var assemblyKeyFileProp = analyzerResult.GetPropertyOrDefault("AssemblyOriginatorKeyFile");
        return assemblyKeyFileProp is null ? null : Path.Combine(Path.GetDirectoryName(analyzerResult.ProjectFilePath) ?? ".", assemblyKeyFileProp);
    }

    internal static ImmutableDictionary<string, ReportDiagnostic> GetDiagnosticOptions(
        this IAnalyzerResult analyzerResult)
    {
        var noWarnString = analyzerResult.GetPropertyOrDefault("NoWarn");
        var noWarn = ParseDiagnostics(noWarnString).ToDictionary(x => x, _ => ReportDiagnostic.Suppress);

        var warningsAsErrorsString = analyzerResult.GetPropertyOrDefault("WarningsAsErrors");
        var warningsAsErrors = ParseDiagnostics(warningsAsErrorsString).ToDictionary(x => x, _ => ReportDiagnostic.Error);

        var warningsNotAsErrorsString = analyzerResult.GetPropertyOrDefault("WarningsNotAsErrors");
        var warningsNotAsErrors = ParseDiagnostics(warningsNotAsErrorsString).ToDictionary(x => x, _ => ReportDiagnostic.Warn);

        // merge settings,
        var diagnosticOptions = new Dictionary<string, ReportDiagnostic>(warningsAsErrors);
        foreach (var item in warningsNotAsErrors)
        {
            diagnosticOptions[item.Key] = item.Value;
        }

        foreach (var item in noWarn)
        {
            diagnosticOptions[item.Key] = item.Value;
        }

        return diagnosticOptions.ToImmutableDictionary();
    }

    private static IEnumerable<string> ParseDiagnostics(string diagnostics)
    {
        if(string.IsNullOrWhiteSpace(diagnostics))
        {
            return [];
        }

        return diagnostics
            .Split(";")
            .Distinct()
            .Where(x => !string.IsNullOrWhiteSpace(x));
    } 

    internal static int GetWarningLevel(this IAnalyzerResult analyzerResult) =>
        int.Parse(analyzerResult.GetPropertyOrDefault("WarningLevel", "4"));

    private static string GetRootNamespace(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("RootNamespace") ?? analyzerResult.GetAssemblyName();

    internal static bool GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, bool defaultBoolean) =>
        bool.Parse(analyzerResult.GetPropertyOrDefault(name, defaultBoolean.ToString()));

    internal static string GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name,
        string defaultValue = default) =>
        analyzerResult.Properties.GetValueOrDefault(name, defaultValue);

    private static IEnumerable<IProjectItem> GetItem(this IAnalyzerResult analyzerResult, string name) => !analyzerResult.Items.TryGetValue(name, out var item) ? [] : item;
}
