using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Configuration.Options;
using Stryker.Core.MutationTest;
using Stryker.Utilities.Buildalyzer;
using Stryker.Utilities.EmbeddedResources;
using Stryker.Utilities.Logging;

namespace Stryker.Core.Compiling;

public interface ICSharpCompilingProcess
{
    CompilingProcessResult Compile(IEnumerable<SyntaxTree> syntaxTrees, Stream ilStream, Stream symbolStream);
    IEnumerable<SemanticModel> GetSemanticModels(IEnumerable<SyntaxTree> syntaxTrees);
}

/// <summary>
/// This process is in control of compiling the assembly and rolling back mutations that cannot compile
/// Compiles the given input onto the memory stream
/// </summary>
public class CsharpCompilingProcess : ICSharpCompilingProcess
{
    private const int MaxAttempt = 50;
    private readonly MutationTestInput _input;
    private readonly IStrykerOptions _options;
    private readonly ICSharpRollbackProcess _rollbackProcess;
    private readonly ILogger _logger;
    private GeneratorDriver _generatorDriver;
    private CSharpCompilation _compilation;

    public CsharpCompilingProcess(MutationTestInput input,
        ICSharpRollbackProcess rollbackProcess = null,
        IStrykerOptions options = null)
    {
        _input = input;
        _options = options ?? new StrykerOptions();
        _rollbackProcess = rollbackProcess ?? new CSharpRollbackProcess();
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<CsharpCompilingProcess>();
    }

    private string AssemblyName =>
        _input.SourceProjectInfo.AnalyzerResult.GetAssemblyName();

    /// <summary>
    /// Compiles the given input onto the memory stream
    /// The compiling process is closely related to the rollback process. When the initial compilation fails, the rollback process will be executed.
    /// <param name="syntaxTrees">The syntax trees to compile</param>
    /// <param name="ilStream">The memory stream to store the compilation result onto</param>
    /// <param name="symbolStream">The memory stream to store the debug symbol</param>
    /// </summary>
    public CompilingProcessResult Compile(IEnumerable<SyntaxTree> syntaxTrees, Stream ilStream, Stream symbolStream)
    {
        var compilation = GetCSharpCompilation(syntaxTrees, [.._input.SourceProjectInfo.AnalyzerResult.GetAdditionalTexts()]);

        compilation = RunSourceGenerators(compilation);
        // first try compiling
        var retryCount = 1;
        (var rollbackProcessResult, var emitResult, retryCount) = TryCompilation(ilStream, symbolStream, ref compilation, null, ICSharpRollbackProcess.Mode.Normal, retryCount);

        // If compiling failed and the error has no location, log and throw exception.
        if (!emitResult.Success && emitResult.Diagnostics.Any(diagnostic => diagnostic.Location == Location.None && diagnostic.Severity == DiagnosticSeverity.Error))
        {
            _logger.LogError("Failed to build the mutated assembly due to unrecoverable error: {Error}",
                emitResult.Diagnostics.First(diagnostic => diagnostic.Location == Location.None && diagnostic.Severity == DiagnosticSeverity.Error));
            DumpErrorDetails(emitResult.Diagnostics);
            throw new CompilationException("General Build Failure detected.");
        }

        var mode = ICSharpRollbackProcess.Mode.Normal;
        for (var count = 1; !emitResult.Success && count < MaxAttempt; count++)
        {
            mode = count switch
            {
                MaxAttempt - 1 => ICSharpRollbackProcess.Mode.LastChance,
                >= MaxAttempt - 3 => ICSharpRollbackProcess.Mode.Aggressive,
                _ => mode
            };
            // compilation did not succeed. let's compile a couple of times more for good measure
            (rollbackProcessResult, emitResult, retryCount) = TryCompilation(ilStream, symbolStream, ref compilation,
                emitResult, mode, retryCount);
        }

        if (emitResult.Success)
        {
            return new CompilingProcessResult(
                true,
                rollbackProcessResult?.RollbackedIds ?? []);
        }
        // compiling failed
        _logger.LogError("Failed to restore the project to a buildable state. Please report the issue. Stryker can not proceed further");
        DumpErrorDetails(emitResult.Diagnostics);
        throw new CompilationException("Failed to restore build able state.");
    }

    /// <summary>
    /// Analyzes the syntax trees and returns the semantic models
    /// </summary>
    /// <param name="syntaxTrees">The syntax trees to analyze</param>
    /// <returns>Semantic models</returns>
    public IEnumerable<SemanticModel> GetSemanticModels(IEnumerable<SyntaxTree> syntaxTrees)
    {
        var compilation = RunSourceGenerators(GetCSharpCompilation(syntaxTrees, [.._input.SourceProjectInfo.AnalyzerResult.GetAdditionalTexts()]));
        // extract semantic models from compilation
        return compilation.SyntaxTrees.Select(tree => compilation.GetSemanticModel(tree)).ToList();
    }

    private static readonly string[] IgnoredErrors = ["RZ3600"];

    // Can't test or mock code generators, so we exclude them from coverage
    [ExcludeFromCodeCoverage]
    private Compilation RunSourceGenerators(Compilation compilation)
    {
        _generatorDriver = _generatorDriver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var errors = diagnostics.Where(diagnostic => IgnoredErrors.Contains(diagnostic.Id) || (diagnostic.Severity == DiagnosticSeverity.Error && diagnostic.Location == Location.None)).ToList();
        if (errors.Count == 0)
        {
            return outputCompilation;
        }
        var fail = false;
        foreach (var diagnostic in errors)
        {
            if (IgnoredErrors.Contains(diagnostic.Id))
            {
                _logger.LogWarning("Stryker encountered a known error from a coe generator but it will keep on. Compilation may still fail later on: {0}", diagnostic);
            }
            else
            {
                _logger.LogError("Failed to generate source code for mutated assembly: {Diagnostics}", diagnostic);
                fail = true;
            }
        }
        if (fail)
        {
            throw new CompilationException("Source Generator Failure");
        }
        return outputCompilation;
    }

    private Compilation GetCSharpCompilation(IEnumerable<SyntaxTree> syntaxTrees,
        ImmutableArray<AdditionalText> additionalTexts)
    {
        var analyzerResult = _input.SourceProjectInfo.AnalyzerResult;

        if (_compilation == null)
        {
            _compilation= CSharpCompilation.Create(AssemblyName,
                syntaxTrees.ToList(),
                _input.SourceProjectInfo.AnalyzerResult.LoadReferences(),
                analyzerResult.GetCompilationOptions());
            _generatorDriver = CSharpGeneratorDriver
                .Create(analyzerResult.GetSourceGenerators(_logger), parseOptions: analyzerResult.GetParseOptions(_options),
                    optionsProvider: new SimpleAnalyserConfigOptionsProvider(analyzerResult)).AddAdditionalTexts(additionalTexts);
        }

        return _compilation;
    }

    private (CSharpRollbackProcessResult, EmitResult, int) TryCompilation(
        Stream ms,
        Stream symbolStream,
        ref Compilation compilation,
        EmitResult previousEmitResult,
        ICSharpRollbackProcess.Mode mode,
        int retryCount)
    {
        CSharpRollbackProcessResult rollbackProcessResult = null;

        _logger.LogDebug("Trying compilation for the {retryCount} time.", ReadableNumber(retryCount));
        var emitOptions = symbolStream == null ? null : new EmitOptions(false, DebugInformationFormat.PortablePdb,
            _input.SourceProjectInfo.AnalyzerResult.GetSymbolFileName());
        EmitResult emitResult = null;
        var resourceDescriptions = _input.SourceProjectInfo.AnalyzerResult.GetResources(_logger);
        while (emitResult == null)
        {
            if (previousEmitResult != null)
            {
                // remove broken mutations
                rollbackProcessResult = _rollbackProcess.Start(compilation, previousEmitResult.Diagnostics, mode, _options.DiagMode);
                compilation =  RunSourceGenerators(rollbackProcessResult.Compilation);
            }

            // reset the memoryStreams
            ms.SetLength(0);
            symbolStream?.SetLength(0);
            try
            {
                emitResult = compilation.Emit(
                    ms,
                    symbolStream,
                    manifestResources: resourceDescriptions,
                    win32Resources: compilation.CreateDefaultWin32Resources(
                        true, // Important!
                        false,
                        null,
                        null),
                    options: emitOptions);
            }
#pragma warning disable S1696 // this catches an exception raised by the C# compiler
            catch (NullReferenceException e)
            {
                _logger.LogError("Roslyn C# compiler raised an NullReferenceException. This is a known Roslyn's issue that may be triggered by invalid usage of conditional access expression.");
                _logger.LogInformation(e, "Exception");
                _logger.LogError("Stryker will attempt to skip problematic files.");
                compilation = ScanForCauseOfException(compilation);
                EmbeddedResourcesGenerator.ResetCache();
            }
        }

        LogEmitResult(emitResult);
        return (rollbackProcessResult, emitResult, retryCount + 1);
    }

    [ExcludeFromCodeCoverage]
    // unable to simulate a CS compiler fault
    private Compilation ScanForCauseOfException(Compilation compilation)
    {
        var syntaxTrees = compilation.SyntaxTrees.ToList();
        var cleanedSyntaxTrees = new HashSet<SyntaxTree>();
        // compile each file separately to identify the culprit(s)
        foreach (var st in syntaxTrees)
        {
            var local = compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(st);
            try
            {
                using var ms = new MemoryStream();
                local.Emit(
                    ms,
                    manifestResources: _input.SourceProjectInfo.AnalyzerResult.GetResources(_logger),
                    options: null);
                cleanedSyntaxTrees.Add(st);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to compile {FilePath} (compiler crash)", st.FilePath);
                _logger.LogTrace("source code:\n {Source}", st.GetText());
                var cleanUpFile = _rollbackProcess.CleanUpFile(st);
                cleanedSyntaxTrees.Add(cleanUpFile);
            }
        }
        _logger.LogError("Please report an issue and provide the source code of the file that caused the exception for analysis.");
        return compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(cleanedSyntaxTrees);
    }

    private void LogEmitResult(EmitResult result)
    {
        if (!result.Success)
        {
            _logger.LogDebug("Compilation failed");

            foreach (var err in result.Diagnostics.Where(x => x.Severity is DiagnosticSeverity.Error))
            {
                _logger.LogDebug("{ErrorMessage}, {ErrorLocation}", err?.GetMessage() ?? "No message", err?.Location.ToString() ?? "Unknown filepath");
            }
        }
        else
        {
            _logger.LogDebug("Compilation successful");
        }
    }

    private void DumpErrorDetails(IEnumerable<Diagnostic> diagnostics)
    {
        var messageBuilder = new StringBuilder();
        var materializedDiagnostics = diagnostics.ToArray();

        foreach (var diagnostic in materializedDiagnostics)
        {
            messageBuilder
                .Append(Environment.NewLine)
                .Append(diagnostic.Id).Append(": ").AppendLine(diagnostic.ToString());
        }

        _logger.LogTrace("Compilation errors: {Diagnostics}", messageBuilder.ToString());
    }

    private static string ReadableNumber(int number) => number switch
    {
        1 => "first",
        2 => "second",
        3 => "third",
        _ => number + "th"
    };

    // This class is used to provide the options to the source generators
    [ExcludeFromCodeCoverage]
    private sealed class SimpleAnalyserConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly NullAnalyzerConfigOptions _nullProvider = new();

        internal SimpleAnalyserConfigOptionsProvider(IAnalyzerResult result) => GlobalOptions = new SimpleAnalyzerConfigOptions(result);

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => _nullProvider;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _nullProvider;

        public override AnalyzerConfigOptions GlobalOptions { get; }

        private sealed class SimpleAnalyzerConfigOptions(IAnalyzerResult result) : AnalyzerConfigOptions
        {
            private const string Prefix = "build_property.";
            private readonly IReadOnlyDictionary<string, string> _options = result.Properties;

            public override bool TryGetValue(string key, out string value)
            {
                if (key.StartsWith(Prefix))
                {
                    return _options.TryGetValue(key[Prefix.Length..], out value);
                }

                value = null;
                return false;
            }

            public override IEnumerable<string> Keys => _options.Keys.Select(key => Prefix + key);
        }

        private sealed class NullAnalyzerConfigOptions : AnalyzerConfigOptions
        {
            public override bool TryGetValue(string key, out string value)
            {
                value = null;
                return false;
            }

            public override IEnumerable<string> Keys => [];
        }

    }
}

