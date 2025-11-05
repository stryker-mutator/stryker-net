using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Utilities.EmbeddedResources;

namespace Stryker.Utilities.Buildalyzer;

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

    public static string? MsBuildPath(this IAnalyzerResult analyzerResult) => analyzerResult.Analyzer?.EnvironmentFactory.GetBuildEnvironment()?.MsBuildExePath;

    public static IEnumerable<ISourceGenerator> GetSourceGenerators(this IAnalyzerResult analyzerResult, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        var generators = new List<ISourceGenerator>();
        foreach (var analyzer in analyzerResult.AnalyzerReferences)
        {
            try
            {
                var analyzerFileReference = new AnalyzerFileReference(analyzer, AnalyzerAssemblyLoader.Instance);
                analyzerFileReference.AnalyzerLoadFailed += (sender, e) => LogAnalyzerLoadError(logger, sender, e);
                generators.AddRange(analyzerFileReference.GetGenerators(LanguageNames.CSharp));
            }
            catch (Exception e)
            {
                logger?.LogWarning(e,
                    """
                    Analyzer/Generator assembly {0} could not be loaded.
                    Generated source code may be missing.
                    """, analyzer);
            }
        }

        return generators;
    }

    [ExcludeFromCodeCoverage(Justification = "Impossible to unit test")]
    private static void LogAnalyzerLoadError(ILogger? logger, object? sender, AnalyzerLoadFailureEventArgs e)
    {
        var source = (sender as AnalyzerReference)?.Display ?? "unknown";
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

            if (!analyzerResult.ReferenceAliases.TryGetValue(reference, out var aliases))
            {
                aliases = [];
            }

            // If no alias is found, return the reference without aliases
            yield return MetadataReference.CreateFromFile(reference).WithAliases(aliases);
        }
    }

    public static NuGetFramework? GetNuGetFramework(this IAnalyzerResult analyzerResult)
    {
        if (string.IsNullOrEmpty(analyzerResult.TargetFramework))
        {
            return null;
        }
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

    public static bool TargetsFullFramework(this IAnalyzerResult analyzerResult) => analyzerResult.GetNuGetFramework()?.IsDesktop() == true;

    public static Language GetLanguage(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("Language") switch
        {
            "F#" => Language.Fsharp,
            "C#" => Language.Csharp,
            _ => Language.Undefined,
        };

    private static readonly string[] knownTestPackages = ["MSTest.TestFramework", "xunit", "NUnit"];

    // checks if an analyzer result is valid
    public static bool IsValid(this IAnalyzerResult br) => br.Succeeded || (br.SourceFiles.Length > 0 && br.References.Length > 0);

    public static bool IsValidFor(this IAnalyzerResult br, string framework) => br.IsValid() && br.TargetFramework == framework;

    public static bool IsTestProject(this IEnumerable<IAnalyzerResult> analyzerResults) => analyzerResults.Any(x => x.IsTestProject());

    private static bool IsTestProject(this IAnalyzerResult analyzerResult)
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

    public static OutputKind GetOutputKind(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("OutputType") switch
        {
            "Exe" => OutputKind.ConsoleApplication,
            "WinExe" => OutputKind.WindowsApplication,
            "Module" => OutputKind.NetModule,
            "AppContainerExe" => OutputKind.WindowsRuntimeApplication,
            "WinMdObj" => OutputKind.WindowsRuntimeMetadata,
            _ => OutputKind.DynamicallyLinkedLibrary
        };

    public static bool IsSignedAssembly(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("SignAssembly", false);

    public static bool IsDelayedSignedAssembly(this IAnalyzerResult analyzerResult) =>
            analyzerResult.GetPropertyOrDefault("DelaySign", false);

    public static string? GetAssemblyOriginatorKeyFile(this IAnalyzerResult analyzerResult)
    {
        var assemblyKeyFileProp = analyzerResult.GetPropertyOrDefault("AssemblyOriginatorKeyFile");
        if (assemblyKeyFileProp is not null)
        {
            return Path.Combine(Path.GetDirectoryName(analyzerResult.ProjectFilePath) ?? ".", assemblyKeyFileProp);
        }
        else
        {
            return null;
        }
    }

    public static ImmutableDictionary<string, ReportDiagnostic> GetDiagnosticOptions(
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
            .Select(x => x.Trim('\r', '\n', ' '))
            .Distinct()
            .Where(x => !string.IsNullOrWhiteSpace(x));
    }

    public static int GetWarningLevel(this IAnalyzerResult analyzerResult) =>
        int.Parse(analyzerResult.GetPropertyOrDefault("WarningLevel", "4"));

    private static string GetRootNamespace(this IAnalyzerResult analyzerResult) =>
        analyzerResult.Properties.TryGetValue("RootNamespace", out var rootNamespace) &&
        !string.IsNullOrEmpty(rootNamespace)
            ? rootNamespace
            : analyzerResult.GetAssemblyName();

    public static bool GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, bool defaultBoolean)
    {
        if (analyzerResult.Properties.TryGetValue(name, out var value) && !string.IsNullOrEmpty(value))
        {
            return bool.Parse(value);
        }
        return defaultBoolean;
    }

    public static string GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name,
        string defaultValue = default) =>
        analyzerResult.Properties.GetValueOrDefault(name, defaultValue);

    private static IProjectItem[] GetItem(this IAnalyzerResult analyzerResult, string name) => !analyzerResult.Items.TryGetValue(name, out var item) ? [] : item;

    private sealed class AnalyzerAssemblyLoader : IAnalyzerAssemblyLoader
    {
        public static readonly IAnalyzerAssemblyLoader Instance = new AnalyzerAssemblyLoader();

        private readonly Dictionary<string, Assembly> _cache = [];

        private AnalyzerAssemblyLoader() { }

        public void AddDependencyLocation(string fullPath)
        {
            if (!_cache.ContainsKey(fullPath))
            {
                _cache[fullPath] = Assembly.LoadFrom(fullPath); //NOSONAR we actually need to load a specified file, not a specific assembly
            }
        }

        public Assembly LoadFromPath(string fullPath)
        {
            if (!_cache.TryGetValue(fullPath, out var assembly))
            {
                _cache[fullPath] = assembly = Assembly.LoadFrom(fullPath); //NOSONAR we actually need to load a specified file, not a specific assembly
            }
            return assembly;
        }
    }
}
