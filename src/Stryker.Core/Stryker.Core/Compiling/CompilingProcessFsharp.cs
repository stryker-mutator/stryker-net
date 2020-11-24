using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using FSharp.Compiler.SourceCodeServices;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Control;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using ParsedInput = FSharp.Compiler.SyntaxTree.ParsedInput;

namespace Stryker.Core.Compiling
{
    public class CompilingProcessFsharp
    {
        private readonly MutationTestInput _input;
        private readonly IRollbackProcess _rollbackProcess;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public CompilingProcessFsharp(MutationTestInput input,
            IRollbackProcess rollbackProcess, IFileSystem fileSystem)
        {
            _input = input;
            _rollbackProcess = rollbackProcess;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<CompilingProcessFsharp>();
            _fileSystem = fileSystem;
        }

        private string AssemblyName =>
            //@"C:\Program Files(x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\CommonExtensions\Microsoft\FSharp\FSharp.Build.dll";
            _input.ProjectInfo.ProjectUnderTestAnalyzerResult.AssemblyName;

        public CompilingProcessResult Compile(IEnumerable<ParsedInput> syntaxTrees, bool devMode)
        {
            var analyzerResult = _input.ProjectInfo.ProjectUnderTestAnalyzerResult;
            var compilationOptions = analyzerResult.GetCompilationOptions();

            FSharpList<ParsedInput> trees = ListModule.OfSeq(syntaxTrees.Reverse());
            FSharpList<string> dependencies = ListModule.OfSeq(analyzerResult.References);

            var checker = FSharpChecker.Create(null, null, null, null, null, null, null, null);

            var pathlist = new List<string>();
            var pdblist = new List<string>();
            foreach (var testProject in _input.ProjectInfo.TestProjectAnalyzerResults)
            {
                var injectionPath = testProject.TargetDirectory;
                if (!_fileSystem.Directory.Exists(injectionPath))
                {
                    _fileSystem.Directory.CreateDirectory(injectionPath);
                }

                // inject the mutated Assembly into the test project
                pathlist.Add(Path.Combine(injectionPath, _input.ProjectInfo.ProjectUnderTestAnalyzerResult.TargetFileName));

                pdblist.Add(Path.Combine(injectionPath,
                        _input.ProjectInfo.ProjectUnderTestAnalyzerResult.SymbolFileName));

                _logger.LogDebug("Injected the mutated assembly file into {0}", injectionPath);
            }
            if (!pathlist.Any())
            {
                throw new GeneralStrykerException("no ProjectUnderTest");
            }

            RollbackProcessResult rollbackProcessResult = null;

            // first try compiling
            //EmitResult emitResult;
            //var retryCount = 1;
            (var compilationSucces, FSharpErrorInfo[] errorinfo) = TryCompilation(checker, trees, pathlist, dependencies, pdblist);

            if (compilationSucces)
            {
                return new CompilingProcessResult()
                {
                    Success = compilationSucces,
                    RollbackResult = rollbackProcessResult
                };
            }

            // compiling failed
            _logger.LogError("Failed to restore the project to a buildable state. Please report the issue. Stryker can not proceed further");
            throw new StrykerCompilationException("Failed to restore build able state.");
        }

        private (bool, FSharpErrorInfo[]) TryCompilation(FSharpChecker checker, FSharpList<ParsedInput> trees, List<string> pathlist, FSharpList<string> dependencies , List<string> pdblist)
        {
            Tuple<FSharpErrorInfo[], int> result = FSharpAsync.RunSynchronously(
                checker.Compile(
                    trees, AssemblyName, pathlist.First(), dependencies, /*[OptionalArgument] FSharpOption<string> pdbFile pdblist.First()*/ null, /*[OptionalArgument] FSharpOption<bool> executable*/true, /*[OptionalArgument] FSharpOption<bool> noframework*/ true, null), null, null);
            return (result.Item2 == 0, result.Item1);
        }
    }
}
