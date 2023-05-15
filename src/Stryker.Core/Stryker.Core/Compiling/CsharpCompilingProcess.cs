using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;

namespace Stryker.Core.Compiling
{
    public interface ICompilingProcess
    {
        CompilingProcessResult Compile(IEnumerable<SyntaxTree> syntaxTrees, Stream ilStream, Stream symbolStream);
    }

    /// <summary>
    /// This process is in control of compiling the assembly and rolling back mutations that cannot compile
    /// Compiles the given input onto the memory stream
    /// </summary>
    public class CsharpCompilingProcess : ICompilingProcess
    {
        private const int MaxAttempt = 50;
        private readonly MutationTestInput _input;
        private readonly StrykerOptions _options;
        private readonly IRollbackProcess _rollbackProcess;
        private readonly ILogger _logger;

        public CsharpCompilingProcess(MutationTestInput input,
            IRollbackProcess rollbackProcess = null,
            StrykerOptions options = null)
        {
            _input = input;
            _options = options ?? new StrykerOptions();
            _rollbackProcess = rollbackProcess ?? new RollbackProcess();
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
            var analyzerResult = _input.SourceProjectInfo.AnalyzerResult;
            var trees = syntaxTrees.ToList();
            var compilationOptions = analyzerResult.GetCompilationOptions();

            var compilation = CSharpCompilation.Create(AssemblyName,
                syntaxTrees: trees,
                options: compilationOptions,
                references: _input.SourceProjectInfo.AnalyzerResult.References.Select(r => MetadataReference.CreateFromFile(r)));

            // C# source generators must be executed before compilation
            compilation = RunSourceGenerators(analyzerResult, compilation);

            // first try compiling
            var retryCount = 1;
            (var rollbackProcessResult, var emitResult, retryCount) = TryCompilation(ilStream, symbolStream, compilation, null, false, retryCount);

            // If compiling failed and the error has no location, log and throw exception.
            if (!emitResult.Success && emitResult.Diagnostics.Any(diagnostic => diagnostic.Location == Location.None && diagnostic.Severity == DiagnosticSeverity.Error))
            {
                _logger.LogError("Failed to build the mutated assembly due to unrecoverable error: {0}",
                    emitResult.Diagnostics.First(diagnostic => diagnostic.Location == Location.None && diagnostic.Severity == DiagnosticSeverity.Error));
                throw new CompilationException("General Build Failure detected.");
            }

            for (var count = 1; !emitResult.Success && count < MaxAttempt; count++)
            {
                // compilation did not succeed. let's compile a couple times more for good measure
                (rollbackProcessResult, emitResult, retryCount) = TryCompilation(ilStream, symbolStream, rollbackProcessResult?.Compilation ?? compilation, emitResult, retryCount == MaxAttempt - 1, retryCount);
            }

            if (emitResult.Success)
            {
                return new CompilingProcessResult()
                {
                    Success = emitResult.Success,
                    RollbackResult = rollbackProcessResult
                };
            }
            // compiling failed
            _logger.LogError("Failed to restore the project to a buildable state. Please report the issue. Stryker can not proceed further");
            foreach (var emitResultDiagnostic in emitResult.Diagnostics)
            {
                _logger.LogWarning($"{emitResultDiagnostic}");
            }
            throw new CompilationException("Failed to restore build able state.");
        }

        private CSharpCompilation RunSourceGenerators(IAnalyzerResult analyzerResult, Compilation compilation)
        {
            var generators = analyzerResult.GetSourceGenerators(_logger);
            _ = CSharpGeneratorDriver
                .Create(generators, parseOptions: analyzerResult.GetParseOptions(_options))
                .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            var errors = diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error && diagnostic.Location == Location.None).ToList();
            if (errors.Count > 0)
            {
                foreach (var diagnostic in errors)
                {
                    _logger.LogError("Failed to generate source code for mutated assembly: {0}", diagnostic);
                }
                throw new CompilationException("Source Generator Failure");
            }
            return outputCompilation as CSharpCompilation;
        }

        private (RollbackProcessResult, EmitResult, int) TryCompilation(
            Stream ms,
            Stream symbolStream,
            CSharpCompilation compilation,
            EmitResult previousEmitResult,
            bool lastAttempt,
            int retryCount)
        {
            RollbackProcessResult rollbackProcessResult = null;

            if (previousEmitResult != null)
            {
                // remove broken mutations
                rollbackProcessResult = _rollbackProcess.Start(compilation, previousEmitResult.Diagnostics, lastAttempt, _options.DevMode);
                compilation = rollbackProcessResult.Compilation;
            }

            // reset the memoryStream
            ms.SetLength(0);
            symbolStream?.SetLength(0);

            _logger.LogDebug("Trying compilation for the {retryCount} time.", ReadableNumber(retryCount));

            var emitOptions = symbolStream == null ? null : new EmitOptions(false, DebugInformationFormat.PortablePdb,
                _input.SourceProjectInfo.AnalyzerResult.GetSymbolFileName());
            var emitResult = compilation.Emit(
                ms,
                symbolStream,
                manifestResources: _input.SourceProjectInfo.AnalyzerResult.GetResources(_logger),
                win32Resources: compilation.CreateDefaultWin32Resources(
                    true, // Important!
                    false,
                    null,
                    null),
                options: emitOptions);

            LogEmitResult(emitResult);

            return (rollbackProcessResult, emitResult, retryCount+1);
        }

        private void LogEmitResult(EmitResult result)
        {
            if (!result.Success)
            {
                _logger.LogDebug("Compilation failed");

                foreach (var err in result.Diagnostics.Where(x => x.Severity is DiagnosticSeverity.Error))
                {
                    _logger.LogDebug("{0}, {1}", err?.GetMessage() ?? "No message", err?.Location.SourceTree?.FilePath ?? "Unknown filepath");
                }
            }
            else
            {
                _logger.LogDebug("Compilation successful");
            }
        }

        private static string ReadableNumber(int number) => number switch
        {
            1 => "first",
            2 => "second",
            3 => "third",
            _ => number + "th"
        };
    }
}
