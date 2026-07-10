using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
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
        analyzerResult.GetAssemblyName() + ".pdb";

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
                logger.LogWarning(e,
                    """
                    Analyzer/Generator assembly {0} could not be loaded.
                    Generated source code may be missing.
                    """, analyzer);
            }
        }

        return generators;
    }

    public static IEnumerable<AdditionalText> GetAdditionalTexts(this IAnalyzerResult result) =>
        result.AdditionalFiles?.Select(additionalFile => new AdditionalTextFromFile(additionalFile)) ?? [];

    public static IEnumerable<string> GetAnalyzerConfigFiles(this IAnalyzerResult result)
    {
        // Analyzer config paths in the compiler command line are often RELATIVE to the project
        // directory (e.g. obj/.../<Project>.GeneratedMSBuildEditorConfig.editorconfig, which carries
        // the CompilerVisibleProperty / CompilerVisibleItemMetadata that generators such as CsWin32
        // read). They must be resolved against the project directory, not the current working
        // directory, or File.Exists silently drops them and the generator options are lost.
        var projectDirectory = Path.GetDirectoryName(result.ProjectFilePath);
        return (result.CompilerArguments ?? [])
            .Select(GetAnalyzerConfigPathFromArgument)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path => ResolveAnalyzerConfigPath(path!, projectDirectory))
            .Where(File.Exists)
            .Distinct();
    }

    private static string ResolveAnalyzerConfigPath(string path, string? projectDirectory) =>
        Path.IsPathRooted(path) || string.IsNullOrEmpty(projectDirectory)
            ? path
            : Path.Combine(projectDirectory, path);

    public static AnalyzerConfigOptionsProvider GetAnalyzerConfigOptionsProvider(this IAnalyzerResult result)
    {
        var analyzerConfigFiles = result.GetAnalyzerConfigFiles().ToList();
        if (analyzerConfigFiles.Any())
        {
            var analyzerConfigs = analyzerConfigFiles
                .Select(path => AnalyzerConfig.Parse(SourceText.From(File.ReadAllText(path)), path))
                .ToImmutableArray();
            var set = AnalyzerConfigSet.Create(analyzerConfigs);
            return new AnalyzerConfigOptionsProviderFromSet(set);
        }

        return new AnalyzerConfigOptionsProviderFromProperties(result.Properties);
    }

    private static string? GetAnalyzerConfigPathFromArgument(string arg)
    {
        const string slashPrefix = "/analyzerconfig:";
        const string dashPrefix = "-analyzerconfig:";
        if (arg.StartsWith(slashPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return arg[slashPrefix.Length..].Trim('"');
        }
        if (arg.StartsWith(dashPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return arg[dashPrefix.Length..].Trim('"');
        }
        return null;
    }

    // Roslyn does not appear to expose usable implementations of these types (required for additional files support)
    private sealed class AdditionalTextFromFile(string path) : AdditionalText
    {
        private readonly Lazy<string> _source = new(() => File.ReadAllText(path));

        public override SourceText? GetText(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return SourceText.From(_source.Value, Encoding.UTF8);
        }

        public override string Path => path;
    }

    private sealed class AnalyzerConfigOptionsProviderFromSet(AnalyzerConfigSet configSet) : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigSet _configSet = configSet;
        private readonly DictionaryAnalyzerConfigOptions _emptyAnalyzerConfigOptions =
            new(ImmutableDictionary<string, string>.Empty.WithComparers(AnalyzerConfigOptions.KeyComparer));

        public override AnalyzerConfigOptions GlobalOptions => new DictionaryAnalyzerConfigOptions(_configSet.GlobalConfigOptions.AnalyzerOptions);

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) =>
            string.IsNullOrEmpty(tree?.FilePath)
                ? _emptyAnalyzerConfigOptions
                : new DictionaryAnalyzerConfigOptions(_configSet.GetOptionsForSourcePath(NormalizePath(tree.FilePath)).AnalyzerOptions);

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) =>
            string.IsNullOrEmpty(textFile?.Path)
                ? _emptyAnalyzerConfigOptions
                : new DictionaryAnalyzerConfigOptions(_configSet.GetOptionsForSourcePath(NormalizePath(textFile.Path)).AnalyzerOptions);

        // Roslyn's AnalyzerConfigSet matches section headers using forward slashes, so a
        // backslash Windows path must be normalized or per-file build_metadata never resolves.
        private static string NormalizePath(string path) => path.Replace('\\', '/');
    }

    private sealed class AnalyzerConfigOptionsProviderFromProperties(IReadOnlyDictionary<string, string> properties) : AnalyzerConfigOptionsProvider
    {
        public override AnalyzerConfigOptions GlobalOptions => new AnalyzerConfigOptionsFromProperties(properties);
        private static readonly AnalyzerConfigOptions NullAnalyzerConfigOptions = new EmptyAnalyzerConfigOptions();

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => NullAnalyzerConfigOptions;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => NullAnalyzerConfigOptions;
    }

    private sealed class AnalyzerConfigOptionsFromProperties(IReadOnlyDictionary<string, string> properties) : DictionaryAnalyzerConfigOptions(
        properties
            .ToImmutableDictionary(
                keyValuePair => $"build_property.{keyValuePair.Key}",
                keyValuePair => keyValuePair.Value,
                AnalyzerConfigOptions.KeyComparer))
    {
    }

    private sealed class EmptyAnalyzerConfigOptions : DictionaryAnalyzerConfigOptions
    {
        public EmptyAnalyzerConfigOptions() : base(ImmutableDictionary<string, string>.Empty.WithComparers(AnalyzerConfigOptions.KeyComparer)) { }
    }

    private class DictionaryAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly ImmutableDictionary<string, string> _options;

        public DictionaryAnalyzerConfigOptions(IDictionary<string, string> options) =>
            _options = options.ToImmutableDictionary(AnalyzerConfigOptions.KeyComparer);

        public DictionaryAnalyzerConfigOptions(ImmutableDictionary<string, string> options)
        {
            _options = options;
        }

        public override bool TryGetValue(string key, out string value) => _options.TryGetValue(key, out value!);

        public override IEnumerable<string> Keys => _options.Keys;
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

    public static bool TargetsDesktop(this IAnalyzerResult analyzerResult) => analyzerResult.GetNuGetFramework()?.IsDesktop() == true;

    public static Language GetLanguage(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("Language") switch
        {
            "F#" => Language.Fsharp,
            "C#" => Language.Csharp,
            _ => Language.Undefined,
        };

    private static readonly string[] KnownTestPackages = ["MSTest.TestFramework", "xunit", "NUnit", "nunit"];

    /// <summary>
    /// checks if an analyzer result is valid
    /// </summary>
    /// <param name="br">analyzer result used for determination</param>
    /// <returns>true if result is complete enough</returns>
    public static bool IsValid(this IAnalyzerResult br) => br.Succeeded || (br.SourceFiles.Length > 0 && br.References.Length > 0)
    || (br.IsTestProject() && br.Properties.ContainsKey("TargetDir") && br.ProjectReferences.Any());

    /// <summary>
    /// checks if an analyzer result is valid for a specific framework
    /// </summary>
    /// <param name="br">analyzer result used for determination</param>
    /// <param name="framework">framework to test for</param>
    /// <returns>true if result is complete enough</returns>
    public static bool IsValidFor(this IAnalyzerResult br, string framework) => br.IsValid() && br.TargetFramework == framework;

    public static bool IsTestProject(this IEnumerable<IAnalyzerResult> analyzerResults) => analyzerResults.Any(x => x.IsTestProject());

    private static bool IsTestProject(this IAnalyzerResult analyzerResult)
    {
        if (bool.TryParse(analyzerResult.GetPropertyOrDefault("IsTestingPlatformApplication"), out var isTestingPlatformApplication) && isTestingPlatformApplication)
        {
            return true;
        }

        if (bool.TryParse(analyzerResult.GetPropertyOrDefault("IsTestProject"), out var isTestProject) && isTestProject)
        {
            return true;
        }

        if (Array.Exists(KnownTestPackages, n => analyzerResult.PackageReferences.ContainsKey(n)))
        {
            return true;
        }

        const string TestProjectTypeGuid = "{3AC096D0-A1C2-E12C-1390-A8335801FDAB}";
        return analyzerResult
            .GetPropertyOrDefault("ProjectTypeGuids", "")
            .Contains(TestProjectTypeGuid);
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

    public static string GetCompilerApiVersion(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("CompilerAPIVersion", "Unknown");

    public static bool IsSignedAssembly(this IAnalyzerResult analyzerResult) =>
        analyzerResult.GetPropertyOrDefault("SignAssembly", false);

    public static bool IsDelayedSignedAssembly(this IAnalyzerResult analyzerResult) =>
            analyzerResult.GetPropertyOrDefault("DelaySign", false);

    public static string? GetAssemblyOriginatorKeyFile(this IAnalyzerResult analyzerResult)
    {
        var assemblyKeyFileProp = analyzerResult.GetPropertyOrDefault("AssemblyOriginatorKeyFile");
        return string.IsNullOrEmpty(assemblyKeyFileProp) ? null : Path.Combine(Path.GetDirectoryName(analyzerResult.ProjectFilePath) ?? ".", assemblyKeyFileProp);
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
                _cache[fullPath] = SafeLoadFrom(fullPath);
            }
        }

        public Assembly LoadFromPath(string fullPath)
        {
            if (!_cache.TryGetValue(fullPath, out var assembly))
            {
                _cache[fullPath] = assembly = SafeLoadFrom(fullPath);
            }
            return assembly;
        }

        [ExcludeFromCodeCoverage(Justification = "Impossible to unit test")]
        private static Assembly SafeLoadFrom(string fullPath)
        {
            try
            {
                return Assembly.LoadFrom(fullPath); //NOSONAR we actually need to load a specified file, not a specific assembly
            }
            catch (FileLoadException)
            {
                // This can happen if the assembly has already been loaded: CLR refuses to load the same
                // assembly from two different paths. In that case, we try to find the already loaded assembly.
                // if we fail, we simply rethrow the original exception
                var assemblyName = AssemblyName.GetAssemblyName(fullPath);
                // find already loaded assembly
                var loadedAssembly = AppDomain.CurrentDomain.GetAssemblies()
                    .FirstOrDefault(a => AssemblyName.ReferenceMatchesDefinition(a.GetName(), assemblyName));
                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }

                throw;
            }
        }
    }
}
