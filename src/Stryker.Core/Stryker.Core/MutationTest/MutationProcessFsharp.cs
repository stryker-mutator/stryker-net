using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using static FSharp.Compiler.SyntaxTree;

namespace Stryker.Core.MutationTest
{
    class MutationProcessFsharp : IMutationProcess
    {
        private readonly ProjectComponent<ParsedInput> _projectInfo;
        private readonly ILogger _logger;
        private readonly StrykerOptions _options;
        private readonly CompilingProcessFsharp _compilingProcess;
        private readonly IFileSystem _fileSystem;
        private readonly MutationTestInput _input;
        private readonly IMutantOrchestrator _orchestrator;

        private readonly IMutantFilter _mutantFilter;
        private readonly IReporter _reporter;

        public MutationProcessFsharp(MutationTestInput mutationTestInput,
            IMutantOrchestrator orchestrator = null,
            IFileSystem fileSystem = null,
            StrykerOptions options = null,

            IMutantFilter mutantFilter = null,
            IReporter reporter = null)
        {
            _input = mutationTestInput;
            _projectInfo = (ProjectComponent<ParsedInput>)mutationTestInput.ProjectInfo.ProjectContents;
            _options = options;
            _orchestrator = orchestrator ?? new MutantOrchestrator(options: _options);
            _compilingProcess = new CompilingProcessFsharp(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();

            _mutantFilter = mutantFilter ?? MutantFilterFactory.Create(options);
            _reporter = reporter;
        }

        public void Mutate()
        {
            // Mutate source files
            foreach (FileLeafFsharp file in _projectInfo.GetAllFiles())
            {
                _logger.LogDebug($"Mutating {file.Name}");
                // Mutate the syntax tree
                var treeroot = ((ParsedInput.ImplFile)file.SyntaxTree).Item.modules;
                var mutatedSyntaxTree = file; //_orchestrator.Mutate((ParsedInput.ImplFile)file.SyntaxTree).GetRoot());      //getroot for fsharp && _orchestrator.Mutate
                // Add the mutated syntax tree for compilation
                file.MutatedSyntaxTree = mutatedSyntaxTree.SyntaxTree;
                if (_options.DevMode)
                {
                    _logger.LogTrace($"Mutated {file.Name}:{Environment.NewLine}{mutatedSyntaxTree}"); //.ToFullString()
                }
                // Filter the mutants
                var allMutants = _orchestrator.GetLatestMutantBatch();
                file.Mutants = allMutants;
            }

            _logger.LogDebug("{0} mutants created", _projectInfo.Mutants.Count());

            CompileMutations();
        }

        private void CompileMutations()
        {
            using var ms = new MemoryStream();
            using var msForSymbols = _options.DevMode ? new MemoryStream() : null;
            // compile the mutated syntax trees
            var compileResult = _compilingProcess.Compile(_projectInfo.CompilationSyntaxTrees, ms, msForSymbols, _options.DevMode);

            foreach (var testProject in _input.ProjectInfo.TestProjectAnalyzerResults)
            {
                var injectionPath = testProject.TargetDirectory;
                if (!_fileSystem.Directory.Exists(injectionPath))
                {
                    _fileSystem.Directory.CreateDirectory(injectionPath);
                }

                // inject the mutated Assembly into the test project
                using var fs = _fileSystem.File.Create(Path.Combine(injectionPath, _input.ProjectInfo.ProjectUnderTestAnalyzerResult.TargetFileName));
                ms.Position = 0;
                ms.CopyTo(fs);

                if (msForSymbols != null)
                {
                    // inject the debug symbols into the test project
                    using var symbolDestination = _fileSystem.File.Create(Path.Combine(injectionPath,
                        _input.ProjectInfo.ProjectUnderTestAnalyzerResult.SymbolFileName));
                    msForSymbols.Position = 0;
                    msForSymbols.CopyTo(symbolDestination);
                }

                _logger.LogDebug("Injected the mutated assembly file into {0}", injectionPath);
            }

            // if a rollback took place, mark the rolled back mutants as status:BuildError
            if (compileResult.RollbackResult?.RollbackedIds.Any() ?? false)
            {
                foreach (var mutant in _projectInfo.Mutants
                    .Where(x => compileResult.RollbackResult.RollbackedIds.Contains(x.Id)))
                {
                    // Ignore compilation errors if the mutation is skipped anyways.
                    if (mutant.ResultStatus == MutantStatus.Ignored)
                    {
                        continue;
                    }

                    mutant.ResultStatus = MutantStatus.CompileError;
                    mutant.ResultStatusReason = "Mutant caused compile errors";
                }
            }
        }

        public void FilterMutants()
        {
            throw new NotImplementedException();
        }
    }
}
