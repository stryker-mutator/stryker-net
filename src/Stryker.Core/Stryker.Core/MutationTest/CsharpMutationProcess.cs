using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutationTest
{
    public class CsharpMutationProcess : IMutationProcess
    {
        private readonly ProjectComponent<SyntaxTree> _projectInfo;
        private readonly ILogger _logger;
        private readonly StrykerOptions _options;
        private readonly CsharpCompilingProcess _compilingProcess;
        private readonly IFileSystem _fileSystem;
        private readonly MutationTestInput _input;
        private readonly BaseMutantOrchestrator<SyntaxNode> _orchestrator;
        private readonly IMutantFilter _mutantFilter;

        /// <summary>
        /// This constructor is for tests
        /// </summary>
        /// <param name="mutationTestInput"></param>
        /// <param name="fileSystem"></param>
        /// <param name="options"></param>
        /// <param name="mutantFilter"></param>
        /// <param name="orchestrator"></param>
        public CsharpMutationProcess(MutationTestInput mutationTestInput,
            IFileSystem fileSystem = null,
            StrykerOptions options = null,
            IMutantFilter mutantFilter = null,
            BaseMutantOrchestrator<SyntaxNode> orchestrator = null)
        {
            _input = mutationTestInput;
            _projectInfo = (ProjectComponent<SyntaxTree>)mutationTestInput.TargetProjectInfo.ProjectContents;
            _options = options;
            _orchestrator = orchestrator ?? new CsharpMutantOrchestrator(options: _options);
            _compilingProcess = new CsharpCompilingProcess(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();

            _mutantFilter = mutantFilter ?? MutantFilterFactory.Create(options, _input);
        }

        /// <summary>
        /// This constructor is used by the <see cref="MutationTestProcess"/> initialization logic.
        /// </summary>
        /// <param name="mutationTestInput"></param>
        /// <param name="options"></param>
        public CsharpMutationProcess(MutationTestInput mutationTestInput, StrykerOptions options) : this(mutationTestInput, null, options, null, null)
        {}

        public void Mutate()
        {
            // Mutate source files
            foreach (var file in _projectInfo.GetAllFiles().Cast<CsharpFileLeaf>())
            {
                _logger.LogDebug($"Mutating {file.FullPath}");
                // Mutate the syntax tree
                var mutatedSyntaxTree = _orchestrator.Mutate(file.SyntaxTree.GetRoot());
                // Add the mutated syntax tree for compilation
                file.MutatedSyntaxTree = mutatedSyntaxTree.SyntaxTree;
                if (_options.DevMode)
                {
                    _logger.LogTrace($"Mutated {file.FullPath}:{Environment.NewLine}{mutatedSyntaxTree.ToFullString()}");
                }
                // Filter the mutants
                file.Mutants = _orchestrator.GetLatestMutantBatch();
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

            foreach (var testProject in _input.TargetProjectInfo.TestProjectAnalyzerResults)
            {
                var injectionPath = _input.TargetProjectInfo.GetInjectionFilePath(testProject);
                if (!_fileSystem.Directory.Exists(Path.GetDirectoryName(injectionPath)))
                {
                    _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(injectionPath));
                }

                // inject the mutated Assembly into the test project
                using var fs = _fileSystem.File.Create(injectionPath);
                ms.Position = 0;
                ms.CopyTo(fs);

                if (msForSymbols != null)
                {
                    // inject the debug symbols into the test project
                    using var symbolDestination = _fileSystem.File.Create(Path.Combine(Path.GetDirectoryName(injectionPath),
                        _input.TargetProjectInfo.AnalyzerResult.GetSymbolFileName()));
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
            foreach (var file in _projectInfo.GetAllFiles())
            {
                // CompileError is a final status and can not be changed during filtering.
                var mutantsToFilter = file.Mutants.Where(x => x.ResultStatus != MutantStatus.CompileError);
                _mutantFilter.FilterMutants(mutantsToFilter, file, _options);
            }
        }
    }
}
