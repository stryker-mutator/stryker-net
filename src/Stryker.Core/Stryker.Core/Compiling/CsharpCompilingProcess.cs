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
using Stryker.Core.ProjectComponents;
using Stryker.Utilities.Buildalyzer;
using Stryker.Utilities.EmbeddedResources;
using Stryker.Utilities.Logging;

namespace Stryker.Core.Compiling;

public interface ICSharpCompilingProcess
{
    CompilingProcessResult Compile(Stream ilStream, Stream symbolStream);

    SemanticModel GetSemanticModel(SyntaxTree syntaxTree);
}

/// <summary>
/// This process is in control of compiling the assembly and rolling back mutations that cannot compile
/// Compiles the given input onto the memory stream
/// </summary>
public class CsharpCompilingProcess : ICSharpCompilingProcess, ICompilationContent
{
    private const int MaxAttempt = 50;
    private readonly MutationTestInput _input;
    private readonly IStrykerOptions _options;
    private readonly ICSharpRollbackProcess _rollbackProcess;
    private readonly ILogger _logger;
    private GeneratorDriver _generatorDriver;
    private Compilation _compilation;
    private readonly IEnumerable<SyntaxTree> _originalSyntaxTrees;
    private bool _needToRunGenerators;

    public CsharpCompilingProcess(MutationTestInput input,
        ICSharpRollbackProcess rollbackProcess = null,
        IStrykerOptions options = null,
        IEnumerable<SyntaxTree> syntaxTrees = null)
    {
        _input = input;
        _options = options ?? new StrykerOptions();
        _rollbackProcess = rollbackProcess ?? new CSharpRollbackProcess();
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<CsharpCompilingProcess>();
        _originalSyntaxTrees = syntaxTrees ?? ((ProjectComponent) _input.SourceProjectInfo.ProjectContents).CompilationSyntaxTrees.ToList();
    }

    private string AssemblyName =>
        _input.SourceProjectInfo.AnalyzerResult.GetAssemblyName();

    /// <summary>
    /// Compiles the given input onto the memory stream
    /// The compiling process is closely related to the rollback process. When the initial compilation fails, the rollback process will be executed.
    /// <param name="ilStream">The memory stream to store the compilation result onto</param>
    /// <param name="symbolStream">The memory stream to store the debug symbol</param>
    /// </summary>
    public CompilingProcessResult Compile(Stream ilStream, Stream symbolStream)
    {
        InitCSharpCompilation();
        RunSourceGenerators();
        // first try compiling
        var retryCount = 1;
        var (rollbackProcessResult, emitResult) = TryCompilation(ilStream, symbolStream, null, ICSharpRollbackProcess.Mode.Normal, retryCount++);
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
            (rollbackProcessResult, emitResult) = TryCompilation(ilStream, symbolStream,
                emitResult, mode, retryCount++);
        }

        if (emitResult.Success)
        {
            return new CompilingProcessResult(
                true,
                rollbackProcessResult ?? []);
        }
        // compiling failed
        _logger.LogError("Failed to restore the project to a buildable state. Please report the issue. Stryker can not proceed further");
        DumpErrorDetails(emitResult.Diagnostics);
        throw new CompilationException("Failed to restore build able state.");
    }

    /// <summary>
    /// Analyzes the syntax trees and returns the semantic models
    /// </summary>
    /// <param name="syntaxTree">The syntax tree to analyze</param>
    /// <returns>Semantic models</returns>
    public SemanticModel GetSemanticModel(SyntaxTree syntaxTree)
    {
        InitCSharpCompilation();
        // extract semantic models from compilation
        return _compilation.GetSemanticModel(syntaxTree);
    }

    // Can't test or mock code generators, so we exclude them from coverage
    [ExcludeFromCodeCoverage]
    private void RunSourceGenerators()
    {
        if (!_needToRunGenerators)
        {
            return;
        }

        _generatorDriver = _generatorDriver.RunGeneratorsAndUpdateCompilation(_compilation.RemoveSyntaxTrees(GeneratedTrees)
            , out _compilation, out var diagnostics);
        _needToRunGenerators = false;
        var errors = diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error && diagnostic.Location == Location.None).ToList();
        if (errors.Count == 0)
        {
            return;
        }
        _logger.LogError("Failed to generate source code for mutated assembly, errors are: {Diagnostics}",
            string.Join(Environment.NewLine, errors.Select(e => e.ToString())));
        throw new CompilationException("Source Generator Failure");
    }

    private void InitCSharpCompilation()
    {
        if (_compilation != null)
        {
            return;
        }

        var analyzerResult = _input.SourceProjectInfo.AnalyzerResult;
        // create the compilation context
        _compilation= CSharpCompilation.Create(AssemblyName,
            _originalSyntaxTrees,
            _input.SourceProjectInfo.AnalyzerResult.LoadReferences(),
            analyzerResult.GetCompilationOptions());
        // create the driver for source generators
        _generatorDriver = CSharpGeneratorDriver
            .Create(analyzerResult.GetSourceGenerators(_logger), parseOptions: analyzerResult.GetParseOptions(_options),
                additionalTexts:[.._input.SourceProjectInfo.AnalyzerResult.GetAdditionalTexts()],
                optionsProvider: new SimpleAnalyserConfigOptionsProvider(analyzerResult));
        // run the generators
        _needToRunGenerators = true;
        RunSourceGenerators();
    }

    private (IEnumerable<int>, EmitResult) TryCompilation(
        Stream ms,
        Stream symbolStream,
        EmitResult previousEmitResult,
        ICSharpRollbackProcess.Mode mode,
        int retryCount)
    {
        IEnumerable<int> rollbackProcessResult = null;

        _logger.LogDebug("Trying compilation for the {retryCount} time.", ReadableNumber(retryCount));
        var emitOptions = symbolStream == null ? null : new EmitOptions(false, DebugInformationFormat.PortablePdb,
            _input.SourceProjectInfo.AnalyzerResult.GetSymbolFileName());
        var resourceDescriptions = _input.SourceProjectInfo.AnalyzerResult.GetResources(_logger);
        if (previousEmitResult != null)
        {
            // remove broken mutations
            rollbackProcessResult = _rollbackProcess.RollbackMutationsInError(this, previousEmitResult.Diagnostics, mode, _options.DiagMode);
            RunSourceGenerators();
        }

        // reset the memoryStreams
        ms.SetLength(0);
        symbolStream?.SetLength(0);
        var emitResult = ActualCompilation(ms, symbolStream, resourceDescriptions, emitOptions);

        LogEmitResult(emitResult);
        return (rollbackProcessResult, emitResult);
    }

    [ExcludeFromCodeCoverage]     // unable to simulate a CS compiler fault
    private EmitResult ActualCompilation(Stream ms, Stream symbolStream, IEnumerable<ResourceDescription> resourceDescriptions,
        EmitOptions emitOptions)
    {
        EmitResult emitResult = null;

        try
        {
            emitResult = _compilation.Emit(
                ms,
                symbolStream,
                manifestResources: resourceDescriptions,
                win32Resources: _compilation.CreateDefaultWin32Resources(
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
            // cleanup the files triggering an exception
            ScanForCauseOfException();
            EmbeddedResourcesGenerator.ResetCache();
            ms.SetLength(0);
            symbolStream?.SetLength(0);
            // try again
            emitResult = _compilation.Emit(
                ms,
                symbolStream,
                manifestResources: resourceDescriptions,
                win32Resources: _compilation.CreateDefaultWin32Resources(
                    true, // Important!
                    false,
                    null,
                    null),
                options: emitOptions);
        }

        return emitResult;
    }

    [ExcludeFromCodeCoverage]     // unable to simulate a CS compiler fault
    // This method tries to identify which source file triggers a compiler exception
    private void ScanForCauseOfException()
    {
        var syntaxTrees = _compilation.SyntaxTrees.ToList();
        var cleanedSyntaxTrees = new HashSet<SyntaxTree>();
        // compile each file separately to identify the culprit(s)
        // we disregard generated files. If those trigger an exception, we can't fix it
        foreach (var st in syntaxTrees.Where(st => !GeneratedTrees.Contains(st)))
        {
            var local = _compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(st);
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
                var cleanUpFile = _originalSyntaxTrees.FirstOrDefault(x => x.FilePath == st.FilePath);
                if (cleanUpFile == null)
                {
                    _logger.LogError("Failed to find the original syntax tree for {FilePath}. Assuming it is a generated file.", st.FilePath);
                    continue;
                }
                cleanedSyntaxTrees.Add(cleanUpFile);
            }
        }
        _logger.LogError("Please report an issue and provide the source code of the file that caused the exception for analysis.");
        _compilation = _compilation.RemoveAllSyntaxTrees().AddSyntaxTrees(cleanedSyntaxTrees);
        _needToRunGenerators = true;
        RunSourceGenerators();
    }

    private ImmutableArray<SyntaxTree> GeneratedTrees => _generatorDriver.GetRunResult().GeneratedTrees;

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

    public IEnumerable<SyntaxTree> SyntaxTrees => _compilation.SyntaxTrees;

    public void ReplaceSyntaxTree(SyntaxTree original, SyntaxTree updated)
    {
        _compilation = _compilation.ReplaceSyntaxTree(original, updated);
        _needToRunGenerators = true;
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

