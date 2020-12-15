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
        private readonly IStrykerOptions _options;
        private readonly CompilingProcessFsharp _compilingProcess;
        private readonly IFileSystem _fileSystem;
        private readonly MutationTestInput _input;
        private readonly FsharpMutantOrchestrator _orchestrator;

        private readonly IMutantFilter _mutantFilter;
        private readonly IReporter _reporter;

        public MutationProcessFsharp(MutationTestInput mutationTestInput,
            FsharpMutantOrchestrator orchestrator = null,
            IFileSystem fileSystem = null,
            IStrykerOptions options = null,
            IMutantFilter mutantFilter = null,
            IReporter reporter = null)
        {
            _input = mutationTestInput;
            _projectInfo = (ProjectComponent<ParsedInput>)_input.ProjectInfo.ProjectContents;

            _options = options;
            _orchestrator = orchestrator ?? new FsharpMutantOrchestrator(options: _options);
            _fileSystem = fileSystem ?? new FileSystem();
            _compilingProcess = new CompilingProcessFsharp(mutationTestInput, new RollbackProcess(), _fileSystem);
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();

            _mutantFilter = mutantFilter ?? MutantFilterFactory.Create(options);
            _reporter = reporter;
        }

        public void Mutate()
        {
            // Mutate source files
            foreach (FileLeafFsharp file in _projectInfo.GetAllFiles())
            {
                Console.WriteLine(file.Name);
                _logger.LogDebug($"Mutating {file.Name}");
                // Mutate the syntax tree
                var treeroot = ((ParsedInput.ImplFile)file.SyntaxTree).Item.modules;
                var mutatedSyntaxTree = _orchestrator.Mutate(treeroot);
                // Add the mutated syntax tree for compilation
                var tree = (ParsedInput.ImplFile)file.SyntaxTree;
                var item = tree.Item;
                var lastcompiled = item.fileName.Equals("Program.fs") ? new Tuple<bool, bool>(true, true) : item.isLastCompiland;
                var treeToSet = ParsedInput.NewImplFile(ParsedImplFileInput.NewParsedImplFileInput(item.fileName, item.isScript, item.qualifiedNameOfFile, item.scopedPragmas, item.hashDirectives, mutatedSyntaxTree, lastcompiled));
                file.MutatedSyntaxTree = treeToSet;
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
            var compileResult = _compilingProcess.Compile(_projectInfo.CompilationSyntaxTrees, _options.DevMode);

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
