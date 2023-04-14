using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FSharp.Compiler.CodeAnalysis;
using FSharp.Compiler.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Control;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents.TestProjects;
using IFileSystem = System.IO.Abstractions.IFileSystem;
using ParsedInput = FSharp.Compiler.Syntax.ParsedInput;

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

        public CompilingProcessResult Compile(IEnumerable<ParsedInput> syntaxTrees, bool devMode)
        {
            var analyzerResult = _input.SourceProjectInfo.AnalyzerResult;

            var trees = ListModule.OfSeq(syntaxTrees.Reverse());
            var dependencies = ListModule.OfSeq(analyzerResult.References);

            //we need a checker if we want to compile 
            var checker = FSharpChecker.Create(
                projectCacheSize: null,
                keepAssemblyContents: null,
                keepAllBackgroundResolutions: null,
                legacyReferenceResolver: null,
                tryGetMetadataSnapshot: null,
                suggestNamesForErrors: null,
                keepAllBackgroundSymbolUses: null,
                enableBackgroundItemKeyStoreAndSemanticClassification: null,
                enablePartialTypeChecking: null);

            var mutatedAssemblyPath = TestProjectsInfo.GetInjectionFilePath(_input.TestProjectsInfo.AnalyzerResults.First(), _input.SourceProjectInfo.AnalyzerResult);
            var pdbPath = Path.Combine(mutatedAssemblyPath, _input.SourceProjectInfo.AnalyzerResult.GetSymbolFileName());
            (var compilationSucces, var errorinfo) = TryCompilation(checker, trees, mutatedAssemblyPath, pdbPath, dependencies);

            foreach (var testProject in _input.TestProjectsInfo.AnalyzerResults)
            {
                var injectionPath = TestProjectsInfo.GetInjectionFilePath(testProject, _input.SourceProjectInfo.AnalyzerResult);
                var pdbInjectionpath = Path.Combine(testProject.GetAssemblyDirectoryPath(), _input.SourceProjectInfo.AnalyzerResult.GetSymbolFileName());
                if (!_fileSystem.Directory.Exists(injectionPath.Substring(0, injectionPath.LastIndexOf('\\'))))
                {
                    _fileSystem.Directory.CreateDirectory(injectionPath);
                }

                _fileSystem.File.Copy(mutatedAssemblyPath, injectionPath, true);
                if (_fileSystem.File.Exists(pdbPath))
                {
                    _fileSystem.File.Copy(pdbPath, pdbInjectionpath, true);
                }

                _logger.LogDebug("Injected the mutated assembly file into {0}", injectionPath);
            }

            //rollback still needs to be implemented
            RollbackProcessResult rollbackProcessResult = null;

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
            foreach (var error in errorinfo)
            {
                _logger.LogError(error.Message);
            }
            throw new CompilationException("Failed to compile.");
        }

        private (bool, FSharpDiagnostic[]) TryCompilation(FSharpChecker checker, FSharpList<ParsedInput> trees, string assemblyPath, string pdbAssemblyPath, FSharpList<string> dependencies)
        {
            var result = FSharpAsync.RunSynchronously(
                checker.Compile(
                    trees, _input.SourceProjectInfo.AnalyzerResult.GetAssemblyName(), assemblyPath, dependencies, pdbFile: pdbAssemblyPath, executable: false, noframework: true, userOpName: null), timeout: null, cancellationToken: null);
            return (result.Item2 == 0, result.Item1);
        }
    }
}
