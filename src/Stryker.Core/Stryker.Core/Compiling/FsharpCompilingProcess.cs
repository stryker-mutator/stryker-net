using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using FSharp.Compiler.SourceCodeServices;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Control;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.ProjectAnalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using ParsedInput = FSharp.Compiler.SyntaxTree.ParsedInput;

namespace Stryker.Core.Compiling
{
    public class FsharpCompilingProcess
    {
        private readonly MutationTestInput _input;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public FsharpCompilingProcess(MutationTestInput input,
            IRollbackProcess rollbackProcess, IFileSystem fileSystem)
        {
            _input = input;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<FsharpCompilingProcess>();
            _fileSystem = fileSystem;
        }

        private string AssemblyName =>
            _input.ProjectInfo.ProjectUnderTestAnalyzerResult.GetAssemblyName();

        public CompilingProcessResult Compile(IEnumerable<ParsedInput> syntaxTrees, bool devMode)
        {
            var analyzerResult = _input.ProjectInfo.ProjectUnderTestAnalyzerResult;

            FSharpList<ParsedInput> trees = ListModule.OfSeq(syntaxTrees.Reverse());
            FSharpList<string> dependencies = ListModule.OfSeq(analyzerResult.References);

            //we need a checker if we want to compile 
            var checker = FSharpChecker.Create(projectCacheSize: null, keepAssemblyContents: null, keepAllBackgroundResolutions: null, legacyReferenceResolver: null, tryGetMetadataSnapshot: null, suggestNamesForErrors: null, keepAllBackgroundSymbolUses: null, enableBackgroundItemKeyStoreAndSemanticClassification: null);

            var pathlist = new List<string>();
            var pdblist = new List<string>();
            foreach (var testProject in _input.ProjectInfo.TestProjectAnalyzerResults)
            {
                var injectionPath = _input.ProjectInfo.GetInjectionFilePath(testProject);
                if (!_fileSystem.Directory.Exists(injectionPath.Substring(0, injectionPath.LastIndexOf('\\'))))
                {
                    _fileSystem.Directory.CreateDirectory(injectionPath);
                }

                pathlist.Add(Path.Combine(injectionPath, _input.ProjectInfo.GetInjectionFilePath(testProject)));

                pdblist.Add(Path.Combine(injectionPath,
                        _input.ProjectInfo.ProjectUnderTestAnalyzerResult.GetSymbolFileName()));

                _logger.LogDebug("Injected the mutated assembly file into {0}", injectionPath);
            }
            if (!pathlist.Any())
            {
                throw new GeneralStrykerException("Could not find project under test.");
            }

            //rollback still needs to be implemented
            RollbackProcessResult rollbackProcessResult = null;

            (var compilationSucces, FSharpErrorInfo[] errorinfo) = TryCompilation(checker, trees, pathlist, dependencies);

            if (compilationSucces)
            {
                //we return if compiled succesfully
                //it is however not used as this is the end of the current F# implementation
                return new CompilingProcessResult()
                {
                    Success = compilationSucces,
                    RollbackResult = rollbackProcessResult
                };
            }

            // compiling failed
            _logger.LogError("Failed to restore the project to a buildable state. Please report the issue. Stryker can not proceed further");
            throw new CompilationException("Failed to restore build able state.");
        }

        private (bool, FSharpErrorInfo[]) TryCompilation(FSharpChecker checker, FSharpList<ParsedInput> trees, List<string> pathlist, FSharpList<string> dependencies)
        {
            Tuple<FSharpErrorInfo[], int> result = FSharpAsync.RunSynchronously(
                checker.Compile(
                    trees, AssemblyName, pathlist.First(), dependencies, pdbFile: null, executable: false, noframework: true, userOpName: null), timeout: null, cancellationToken: null);
            return (result.Item2 == 0, result.Item1);
        }
    }
}
