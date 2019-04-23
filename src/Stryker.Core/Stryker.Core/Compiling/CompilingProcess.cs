using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Compiling
{
    public interface ICompilingProcess
    {
        /// <summary>
        /// Compiles the given input onto the memorystream
        /// </summary>
        /// <param name="syntaxTrees"></param>
        /// <param name="ms">The memorystream to function as output</param>
        /// <param name="devMode">set to true to activate devmode (provides more information in case of internal failure)</param>
        CompilingProcessResult Compile(IEnumerable<SyntaxTree> syntaxTrees, MemoryStream ms, bool devMode);
    }

    /// <summary>
    /// This process is in control of compiling the assembly and rollbacking mutations that cannot compile
    /// </summary>
    public class CompilingProcess : ICompilingProcess
    {
        private MutationTestInput _input { get; set; }
        private IRollbackProcess _rollbackProcess { get; set; }
        private ILogger _logger { get; set; }

        public CompilingProcess(MutationTestInput input,
            IRollbackProcess rollbackProcess)
        {
            _input = input;
            _rollbackProcess = rollbackProcess;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<CompilingProcess>();
        }

        /// <summary>
        /// The compiling process is closely related to the rollback process. When the initial compilation fails, the rollback process will be executed.
        /// </summary>
        /// <param name="syntaxTrees">The syntaxtrees to compile</param>
        /// <param name="ms">The memory stream to store the compilation result onto</param>
        /// <param name="devMode"></param>
        public CompilingProcessResult Compile(IEnumerable<SyntaxTree> syntaxTrees, MemoryStream ms, bool devMode)
        {
            var analyzerResult = _input.ProjectInfo.ProjectUnderTestAnalyzerResult;

            var compiler = CSharpCompilation.Create(analyzerResult.Properties.GetValueOrDefault("TargetName"),
                syntaxTrees: syntaxTrees,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                                                      allowUnsafe: true,
                                                      cryptoKeyFile: analyzerResult.SignAssembly ? analyzerResult.AssemblyOriginatorKeyFile : null,
                                                      strongNameProvider: analyzerResult.SignAssembly ? new DesktopStrongNameProvider() : null),

                references: _input.AssemblyReferences);
            RollbackProcessResult rollbackProcessResult = null;

            // first try compiling
            var emitResult = compiler.Emit(ms, manifestResources: analyzerResult.Resources);

            var retryCount = 0;
            // compilation did not succeed, rollbacking failed mutations
            if (!emitResult.Success)
            {
                retryCount = 1;
                (rollbackProcessResult, emitResult, retryCount) = RetryCompilation(ms, compiler, emitResult, devMode, retryCount);
            }

            // compilation still did not succeed. let's compile a couple times more for good measure
            for (var count = 0; !emitResult.Success && count < 100; count++)
            {
                (rollbackProcessResult, emitResult, retryCount) = RetryCompilation(ms, rollbackProcessResult.Compilation, emitResult, devMode, retryCount);
            }

            LogEmitResult(emitResult);
            if (!emitResult.Success)
            {
                // compiling failed
                _logger.LogError("Failed to restore the project to a buildable state. Please report the issue. Stryker can not proceed further");
                foreach (var emitResultDiagnostic in emitResult.Diagnostics)
                {
                    _logger.LogWarning($"{emitResultDiagnostic}");
                }
                throw new ApplicationException("Failed to restore build able state.");
            }
            return new CompilingProcessResult()
            {
                Success = emitResult.Success,
                RollbackResult = rollbackProcessResult
            };
        }

        private (RollbackProcessResult, EmitResult, int) RetryCompilation(MemoryStream ms,
            CSharpCompilation compilation,
            EmitResult previousEmitResult,
            bool devMode,
            int retryCount)
        {
            LogEmitResult(previousEmitResult);
            // remove broken mutations
            var rollbackProcessResult = _rollbackProcess.Start(compilation, previousEmitResult.Diagnostics, devMode);

            // reset the memoryStream for the second compilation
            ms.SetLength(0);

            _logger.LogDebug($"Retrying compilation for {retryCount}{FirstSecondThird(retryCount)} time.");

            var emitResult = rollbackProcessResult.Compilation.Emit(ms, manifestResources: _input.ProjectInfo.ProjectUnderTestAnalyzerResult.Resources);
            return (rollbackProcessResult, emitResult, ++retryCount);
        }

        private void LogEmitResult(EmitResult result)
        {
            if (!result.Success)
            {
                _logger.LogDebug("Compilation failed");

                foreach (var err in result.Diagnostics.Where(x => x.Severity is DiagnosticSeverity.Error))
                {
                    _logger.LogDebug("{0}, {1}", err?.GetMessage() ?? "No message", err?.Location?.SourceTree?.FilePath ?? "Unknown filepath");
                }
            }
            else
            {
                _logger.LogDebug("Compilation successful");
            }
        }

        private string FirstSecondThird(int number)
        {
            return
                number > 3 ? "th" :
                number == 3 ? "rd" :
                number == 2 ? "nd" :
                "st";
        }
    }
}
