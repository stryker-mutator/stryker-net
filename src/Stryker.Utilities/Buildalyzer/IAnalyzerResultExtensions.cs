using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Buildalyzer;
using Microsoft.Build.Logging.StructuredLogger;
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

    private static readonly string[] KnownTestPackages = ["MSTest.TestFramework", "xunit", "NUnit", "nunit"];

    /// <summary>
    /// Checks if a project analysis is valid for all given target frameworks. If no target frameworks are given, it checks if the overall analysis was successful.
    /// </summary>
    /// <param name="br">Analysis results.</param>
    /// <param name="targetFrameworks">list of frameworks to check for</param>
    /// <returns>true if analysis was successful</returns>
    public static bool IsValidFor(this IAnalyzerResults br, string[] targetFrameworks) => br.OverallSuccess
        || (targetFrameworks.Length>0
            && Array.TrueForAll(targetFrameworks, fmw => br.Results.Any( r=> r.IsValidFor(fmw))));

    public static bool IsTestProject(this IEnumerable<IAnalyzerResult> analyzerResults) => analyzerResults.Any(x => x.IsTestProject());

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

    extension(IAnalyzerResult analyzerResult)
    {
       public string GetAssemblyFileName() =>
            FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetFileName"]);

        public  bool BuildsAnAssembly() => analyzerResult.Properties.ContainsKey("TargetFileName");

        public string GetReferenceAssemblyPath() =>
            analyzerResult.Properties.TryGetValue("TargetRefPath", out var property) ?
                FilePathUtils.NormalizePathSeparators(property) : analyzerResult.GetAssemblyPath();

        public string GetAssemblyDirectoryPath() =>
            FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["TargetDir"]);

        public string GetAssemblyPath() =>
            FilePathUtils.NormalizePathSeparators(Path.Combine(analyzerResult.GetAssemblyDirectoryPath(),
                analyzerResult.GetAssemblyFileName()));

        public string GetAssemblyName() =>
            FilePathUtils.NormalizePathSeparators(analyzerResult.Properties["AssemblyName"]);

        public IEnumerable<ResourceDescription> GetResources()
        {
            var rootNamespace = analyzerResult.GetRootNamespace();
            var embeddedResources = analyzerResult.GetItem("EmbeddedResource").Select(x => x.ItemSpec);
            return EmbeddedResourcesGenerator.GetManifestResources(
                analyzerResult.GetAssemblyPath(),
                analyzerResult.ProjectFilePath,
                rootNamespace,
                embeddedResources);
        }

        public string AssemblyAttributeFileName() =>
            analyzerResult.GetPropertyOrDefault("GeneratedAssemblyInfoFile",
                (Path.GetFileNameWithoutExtension(analyzerResult.ProjectFilePath) + ".AssemblyInfo.cs")
                .ToLowerInvariant());

        public string GetSymbolFileName() => analyzerResult.GetAssemblyName() + ".pdb";

        public string TargetPlatform() => analyzerResult.GetPropertyOrDefault("TargetPlatform", "AnyCPU");

        public string? MsBuildPath() => analyzerResult.Analyzer?.EnvironmentFactory.GetBuildEnvironment()?.MsBuildExePath;

        public IEnumerable<ISourceGenerator> GetSourceGenerators(ILogger logger)
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

        public IEnumerable<AdditionalText> GetAdditionalTexts() =>
            analyzerResult .AdditionalFiles?.Select(additionalFile => new AdditionalTextFromFile(additionalFile)) ?? [];

        public IEnumerable<MetadataReference> LoadReferences()
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

        public NuGetFramework? GetNuGetFramework()
        {
            var frameworkText = analyzerResult.TargetFramework;
            if (string.IsNullOrEmpty(frameworkText))
            {
                return null;
            }
            var framework = NuGetFramework.Parse(frameworkText);
            if (framework != NuGetFramework.UnsupportedFramework)
            {
                return framework;
            }

            var atPath = string.IsNullOrEmpty(analyzerResult.ProjectFilePath)
                ? ""
                : $" at '{analyzerResult.ProjectFilePath}'";
            var message =
                $"The target framework '{frameworkText}' is not supported. Please fix the target framework in the csproj{atPath}.";
            throw new InputException(message);
        }

        public bool TargetsDesktop() => analyzerResult.GetNuGetFramework()?.IsDesktop() == true;

        public Language GetLanguage() =>
            analyzerResult.GetPropertyOrDefault("Language") switch
            {
                "F#" => Language.Fsharp,
                "C#" => Language.Csharp,
                _ => Language.Undefined,
            };


        /// <summary>
        /// checks if an analyzer result is valid
        /// </summary>
        /// <param name="br">analyzer result used for determination</param>
        /// <returns>true if result is complete enough</returns>
        public bool IsValid() => analyzerResult.Succeeded || (analyzerResult.SourceFiles.Length > 0 && analyzerResult.References.Length > 0)
                                                          || (analyzerResult.IsTestProject()
                                                              && analyzerResult.Properties.ContainsKey("TargetDir")
                                                              && analyzerResult.ProjectReferences.Any());

        /// <summary>
        /// checks if an analyzer result is valid for a specific framework
        /// </summary>
        /// <param name="br">analyzer result used for determination</param>
        /// <param name="framework">framework to test for</param>
        /// <returns>true if result is complete enough</returns>
        private bool IsValidFor(string framework) => analyzerResult.IsValid() && analyzerResult.TargetFramework == framework;

        private bool IsTestProject()
        {
            // if 'IsTestingPlatformApplication' is defined and true, this is a test project
            if (analyzerResult.TryGetProperty("IsTestingPlatformApplication", out var value)
                && bool.TryParse(value, out var isMtp)
                && isMtp)
            {
                return true;
            }

            // if 'IsTestProject' is defined, we use its value to check if it's a test project (or not)
            if (analyzerResult.TryGetProperty("IsTestProject", out value))
            {
                return bool.TryParse(value, out var isTestProject) && isTestProject;
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

        public OutputKind GetOutputKind() =>
            analyzerResult.GetPropertyOrDefault("OutputType") switch
            {
                "Exe" => OutputKind.ConsoleApplication,
                "WinExe" => OutputKind.WindowsApplication,
                "Module" => OutputKind.NetModule,
                "AppContainerExe" => OutputKind.WindowsRuntimeApplication,
                "WinMdObj" => OutputKind.WindowsRuntimeMetadata,
                _ => OutputKind.DynamicallyLinkedLibrary
            };

        public string GetCompilerApiVersion() =>
            analyzerResult.GetPropertyOrDefault("CompilerAPIVersion", "Unknown");

        public bool IsSignedAssembly() =>
            analyzerResult.GetPropertyOrDefault("SignAssembly", false);

        public bool IsDelayedSignedAssembly() =>
                analyzerResult.GetPropertyOrDefault("DelaySign", false);

        public string? GetAssemblyOriginatorKeyFile()
        {
            var assemblyKeyFileProp = analyzerResult.GetPropertyOrDefault("AssemblyOriginatorKeyFile");
            return string.IsNullOrEmpty(assemblyKeyFileProp) ? null : Path.Combine(Path.GetDirectoryName(analyzerResult.ProjectFilePath) ?? ".", assemblyKeyFileProp);
        }

        public ImmutableDictionary<string, ReportDiagnostic> GetDiagnosticOptions()
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

        public AnalyzerConfigOptionsProvider GetAnalyzerConfigOptionsProvider(IFileSystem fileSystem)
        {
            var analyzerConfigFiles = analyzerResult.GetAnalyzerConfigFiles(fileSystem).ToList();
            if (analyzerConfigFiles.Count == 0)
            {
                return new AnalyzerConfigOptionsProviderFromProperties(analyzerResult.Properties);
            }

            var analyzerConfigs = analyzerConfigFiles
                .Select(path => AnalyzerConfig.Parse(SourceText.From(fileSystem.File.ReadAllText(path)), path));
            var set = AnalyzerConfigSet.Create(analyzerConfigs.ToImmutableArray());
            return new AnalyzerConfigOptionsProviderFromSet(set);
        }

        public int GetWarningLevel() =>
            int.Parse(analyzerResult.GetPropertyOrDefault("WarningLevel", "4"));

        private string GetRootNamespace() =>
            analyzerResult.Properties.TryGetValue("RootNamespace", out var rootNamespace) &&
            !string.IsNullOrEmpty(rootNamespace)
                ? rootNamespace
                : analyzerResult.GetAssemblyName();

        public bool GetPropertyOrDefault(string name, bool defaultBoolean)
        {
            if (analyzerResult.Properties.TryGetValue(name, out var value) && !string.IsNullOrEmpty(value))
            {
                return bool.Parse(value);
            }
            return defaultBoolean;
        }

        public string GetPropertyOrDefault(string name,
            string defaultValue = default) =>
            analyzerResult.Properties.GetValueOrDefault(name, defaultValue);

        private bool TryGetProperty(string name, [NotNullWhen(true)] out string? value) =>
            analyzerResult.Properties.TryGetValue(name, out value) && !string.IsNullOrEmpty(value);

        private IProjectItem[] GetItem(string name) => !analyzerResult.Items.TryGetValue(name, out var item) ? [] : item;

        private IEnumerable<string> GetAnalyzerConfigFiles(IFileSystem fileSystem)
        {
            const string ArgName = "analyzerconfig:";
            var projectDirectory = fileSystem.Path.GetDirectoryName(analyzerResult.ProjectFilePath);
            // Analyzer config paths in the compiler command line are often RELATIVE to the project
            // directory (e.g. obj/.../<Project>.GeneratedMSBuildEditorConfig.editorconfig, which carries
            // the CompilerVisibleProperty / CompilerVisibleItemMetadata that generators such as CsWin32
            // read). They must be resolved against the project directory, not the current working
            // directory, or File.Exists silently drops them and the generator options are lost.
            return analyzerResult.CompilerArguments.Where(ValidateArg)
                .Select(arg =>
                    ResolveAnalyzerConfigPath(arg[(ArgName.Length + 1)..].TrimQuotes(), projectDirectory, fileSystem))
                .Distinct();

            bool ValidateArg(string arg)
            {
                return arg[0] is '/' or '-' && arg.Length > ArgName.Length+2 && arg[1..(ArgName.Length+1)] == ArgName;
            }
        }
    }

    private static string ResolveAnalyzerConfigPath(string path, string? projectDirectory, IFileSystem fileSystem) =>
        string.IsNullOrEmpty(projectDirectory) || fileSystem.Path.IsPathRooted(path)
            ? path
            : fileSystem.Path.Combine(projectDirectory, path);

    // analyzer option provider using additional files
    private sealed class AnalyzerConfigOptionsProviderFromSet(AnalyzerConfigSet configSet) : AnalyzerConfigOptionsProvider
    {
        private readonly DictionaryAnalyzerConfigOptions _emptyAnalyzerConfigOptions =
            new(ImmutableDictionary<string, string>.Empty.WithComparers(AnalyzerConfigOptions.KeyComparer));

        public override AnalyzerConfigOptions GlobalOptions =>
            new DictionaryAnalyzerConfigOptions(configSet.GlobalConfigOptions.AnalyzerOptions);

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) =>
            GetOptionsForPath(tree?.FilePath);

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) =>
            GetOptionsForPath(textFile?.Path);

        private DictionaryAnalyzerConfigOptions GetOptionsForPath(string? path) =>
            string.IsNullOrEmpty(path)
                ? _emptyAnalyzerConfigOptions
                : new DictionaryAnalyzerConfigOptions(configSet.GetOptionsForSourcePath(NormalizePath(path)).AnalyzerOptions);

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
                keyComparer: KeyComparer))
    {
    }

    private sealed class EmptyAnalyzerConfigOptions()
        : DictionaryAnalyzerConfigOptions(
            ImmutableDictionary<string, string>.Empty.WithComparers(KeyComparer));

    private class DictionaryAnalyzerConfigOptions(ImmutableDictionary<string, string> options) : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value) => options.TryGetValue(key, out value!);

        public override IEnumerable<string> Keys => options.Keys;
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
