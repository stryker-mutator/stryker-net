using FSharp.Compiler.SourceCodeServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static FSharp.Compiler.SyntaxTree;
using ParsedInput = FSharp.Compiler.SyntaxTree.ParsedInput;

namespace Stryker.Core.Compiling
{
    class CompilingProcessFsharp
    {
        private readonly MutationTestInput _input;
        private readonly IRollbackProcess _rollbackProcess;
        private readonly ILogger _logger;

        public CompilingProcessFsharp(MutationTestInput input,
            IRollbackProcess rollbackProcess)
        {
            _input = input;
            _rollbackProcess = rollbackProcess;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<CompilingProcessFsharp>();
        }

        private string AssemblyName =>
            _input.ProjectInfo.ProjectUnderTestAnalyzerResult.AssemblyName;

        public CompilingProcessResult Compile(IEnumerable<ParsedInput> syntaxTrees, Stream ilStream, Stream memoryStream, bool devMode)
        {
            var analyzerResult = _input.ProjectInfo.ProjectUnderTestAnalyzerResult;
            FSharpList<ParsedInput> trees = ListModule.OfSeq(syntaxTrees);
            var compilationOptions = analyzerResult.GetCompilationOptions();

            var checker = FSharpChecker.Create(null, null, null, null, null, null, null, null);

            FSharpList<string> dependencies = ListModule.OfSeq(analyzerResult.References);
            var memstream = (TextWriter)new StreamWriter(memoryStream ?? new MemoryStream());
            var ilstream = (TextWriter)new StreamWriter(ilStream);
            var streams = new FSharpOption<Tuple<TextWriter, TextWriter>>(Tuple.Create(ilstream, memstream));

            var compilation = checker.CompileToDynamicAssembly(trees, AssemblyName, dependencies, streams, devMode, null, null );
                /*CSharpCompilation.Create(AssemblyName,
                syntaxTrees: trees,
                options: compilationOptions,
                references: _input.AssemblyReferences);*/
            RollbackProcessResult rollbackProcessResult;
            return null;

            // first try compiling
            //EmitResult emitResult;
            //var retryCount = 1;
            //(rollbackProcessResult, emitResult, retryCount) = TryCompilation(ilStream, symbolStream, compilation, null, false, devMode, retryCount);

            //// If compiling failed and the error has no location, log and throw exception.
            //if (!emitResult.Success && emitResult.Diagnostics.Any(diagnostic => diagnostic.Location == Location.None && diagnostic.Severity == DiagnosticSeverity.Error))
            //{
            //    _logger.LogError("Failed to build the mutated assembly due to unrecoverable error: {0}",
            //        emitResult.Diagnostics.First(diagnostic => diagnostic.Location == Location.None && diagnostic.Severity == DiagnosticSeverity.Error));
            //    throw new StrykerCompilationException("General Build Failure detected.");
            //}

            //const int maxAttempt = 50;
            //for (var count = 1; !emitResult.Success && count < maxAttempt; count++)
            //{
            //    // compilation did not succeed. let's compile a couple times more for good measure
            //    (rollbackProcessResult, emitResult, retryCount) = TryCompilation(ilStream, symbolStream, rollbackProcessResult?.Compilation ?? compilation, emitResult, retryCount == maxAttempt - 1, devMode, retryCount);
            //}

            //if (emitResult.Success)
            //{
            //    return new CompilingProcessResult()
            //    {
            //        Success = emitResult.Success,
            //        RollbackResult = rollbackProcessResult
            //    };
            //}
            //// compiling failed
            //_logger.LogError("Failed to restore the project to a buildable state. Please report the issue. Stryker can not proceed further");
            //foreach (var emitResultDiagnostic in emitResult.Diagnostics)
            //{
            //    _logger.LogWarning($"{emitResultDiagnostic}");
            //}
            //throw new StrykerCompilationException("Failed to restore build able state.");
        }
    }
}
