using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Compiling
{
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
        public CompilingProcessResult Compile(IEnumerable<SyntaxTree> syntaxTrees, MemoryStream ms)
        {
            var compiler = CSharpCompilation.Create(_input.ProjectInfo.ProjectUnderTestAssemblyName,
                syntaxTrees: syntaxTrees,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: _input.AssemblyReferences);
            RollbackProcessResult rollbackProcessResult = null;

            // first try compiling
            var emitResult = compiler.Emit(ms);

            if (!emitResult.Success)
            {
                LogEmitResult(emitResult);
                // remove broken mutations
                rollbackProcessResult = _rollbackProcess.Start(compiler, emitResult.Diagnostics);

                // reset the memoryStream for the second compilation
                ms.SetLength(0);

                // second try compiling
                emitResult = rollbackProcessResult.Compilation.Emit(ms);
            }

            LogEmitResult(emitResult);
            return new CompilingProcessResult()
            {
                Success = emitResult.Success,
                RollbackResult = rollbackProcessResult
            };
        }

        private void LogEmitResult(EmitResult result)
        {
            if(!result.Success)
            {
                _logger.LogDebug("Compilation failed");

                foreach (var err in result.Diagnostics.Where(x => x.Severity is DiagnosticSeverity.Error))
                {
                    _logger.LogDebug("{0}, {1}", err.GetMessage(), err.Location.SourceTree.FilePath);
                }
            } else
            {
                _logger.LogDebug("Compilation successful");
            }
        }
    }
}
