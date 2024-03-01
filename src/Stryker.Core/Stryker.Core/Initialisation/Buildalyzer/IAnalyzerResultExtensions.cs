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
using Stryker.Core.Exceptions;

namespace Stryker.Core.Initialisation.Buildalyzer;

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

    public static string AssemblyAttributeFileName(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("GeneratedAssemblyInfoFile",
            (Path.GetFileNameWithoutExtension(analyzerResult.ProjectFilePath) + ".AssemblyInfo.cs")
            .ToLowerInvariant());

    public static string GetSymbolFileName(this IAnalyzerResult analyzerResult) => Path.ChangeExtension(analyzerResult.GetAssemblyName(), ".pdb");

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
            $"Failed to load analyzer '{source}': {e.Message} (error : {Enum.GetName(e.ErrorCode.GetType(), e.ErrorCode) ?? e.ErrorCode.ToString()}, analyzer: {e.TypeName ?? "All"}).");
        if (e.ErrorCode == AnalyzerLoadFailureEventArgs.FailureErrorCode.ReferencesNewerCompiler)
        {
            logger?.LogWarning(
                $"The analyzer '{source}' references a newer version ({e.ReferencedCompilerVersion}) of the compiler than the one used by Stryker.NET.");
        }

        if (e.Exception != null)
        {
            logger?.LogWarning($"Failed to load analyzer '{source}': Exception {e.Exception}.");
        }
    }

    public static IEnumerable<MetadataReference> LoadReferences(this IAnalyzerResult analyzerResult)
    {
        foreach (var reference in analyzerResult.References)
        {
            if (reference.Contains('='))
            {
                // we have an alias
                var split = reference.Split('=');
                var aliases = split[0].Split(',');
                yield return MetadataReference.CreateFromFile(split[1]).WithAliases(aliases);
            }
            else
            {
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

    internal static bool TargetsFullFramework(this IAnalyzerResult analyzerResult)
    {
        var nuGetFramework = GetNuGetFramework(analyzerResult);

        return nuGetFramework.IsDesktop() && (nuGetFramework.Version.Major<4 || nuGetFramework.Version is { Major: 4, Minor: < 8 });
    }

    public static Language GetLanguage(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("Language") switch
    {
        "F#" => Language.Fsharp,
        "C#" => Language.Csharp,
        _ => Language.Undefined,
    };

    private static readonly string[] KnownTestPackages = { "MSTest.TestFramework", "xunit", "NUnit" };

    public static bool IsTestProject(this IAnalyzerResult analyzerResult)
    {
        if (!bool.TryParse(analyzerResult.GetPropertyOrDefault("IsTestProject"), out var isTestProject))
        {
            isTestProject = Array.Exists(KnownTestPackages, n => analyzerResult.PackageReferences.ContainsKey(n));
        }
        var hasTestProjectTypeGuid = analyzerResult
            .GetPropertyOrDefault("ProjectTypeGuids", "")
            .Contains("{3AC096D0-A1C2-E12C-1390-A8335801FDAB}");

        return isTestProject || hasTestProjectTypeGuid;
    }

    internal static OutputKind GetOutputKind(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("OutputType") switch
    {
        "Exe" => OutputKind.ConsoleApplication,
        "WinExe" => OutputKind.WindowsApplication,
        "Module" => OutputKind.NetModule,
        "AppContainerExe" => OutputKind.WindowsRuntimeApplication,
        "WinMdObj" => OutputKind.WindowsRuntimeMetadata,
        _ => OutputKind.DynamicallyLinkedLibrary
    };

    internal static bool IsSignedAssembly(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("SignAssembly", false);

    internal static string GetAssemblyOriginatorKeyFile(this IAnalyzerResult analyzerResult)
    {
        var assemblyKeyFileProp = analyzerResult.GetPropertyOrDefault("AssemblyOriginatorKeyFile");
        if (assemblyKeyFileProp is null)
        {
            return assemblyKeyFileProp;
        }

        return Path.Combine(Path.GetDirectoryName(analyzerResult.ProjectFilePath), assemblyKeyFileProp);
    }

    internal static ImmutableDictionary<string, ReportDiagnostic> GetDiagnosticOptions(this IAnalyzerResult analyzerResult)
    {
        var noWarnString = analyzerResult.GetPropertyOrDefault("NoWarn");
        var noWarn = noWarnString is not null ? noWarnString.Split(";").Where(x => !string.IsNullOrWhiteSpace(x)).ToDictionary(x => x, x => ReportDiagnostic.Suppress) : new Dictionary<string, ReportDiagnostic>();

        var warningsAsErrorsString = analyzerResult.GetPropertyOrDefault("WarningsAsErrors");
        var warningsAsErrors = warningsAsErrorsString is not null ? warningsAsErrorsString.Split(";").Where(x => !string.IsNullOrWhiteSpace(x)).ToDictionary(x => x, x => ReportDiagnostic.Error) : new Dictionary<string, ReportDiagnostic>();

        var warningsNotAsErrorsString = analyzerResult.GetPropertyOrDefault("WarningsNotAsErrors");
        var warningsNotAsErrors = warningsNotAsErrorsString is not null ? warningsNotAsErrorsString.Split(";").Where(x => !string.IsNullOrWhiteSpace(x)).ToDictionary(x => x, x => ReportDiagnostic.Warn) : new Dictionary<string, ReportDiagnostic>();

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

    internal static int GetWarningLevel(this IAnalyzerResult analyzerResult) => int.Parse(analyzerResult.GetPropertyOrDefault("WarningLevel", "4"));

    private static string GetRootNamespace(this IAnalyzerResult analyzerResult) => analyzerResult.GetPropertyOrDefault("RootNamespace") ?? analyzerResult.GetAssemblyName();

    internal static bool GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, bool defaultBoolean) => bool.Parse(GetPropertyOrDefault(analyzerResult, name, defaultBoolean.ToString()));

    internal static string GetPropertyOrDefault(this IAnalyzerResult analyzerResult, string name, string defaultValue = default)
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
