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
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.MutationTest
{
    public class CsharpMutationProcess : IMutationProcess
    {
        private readonly ILogger _logger;
        private readonly StrykerOptions _options;
        private readonly IFileSystem _fileSystem;
        private readonly BaseMutantOrchestrator<SyntaxNode> _orchestrator;
        private readonly IMutantFilter _mutantFilter;

        /// <summary>
        /// This constructor is for tests
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="options"></param>
        /// <param name="mutantFilter"></param>
        /// <param name="orchestrator"></param>
        public CsharpMutationProcess(
            IFileSystem fileSystem = null,
            StrykerOptions options = null,
            IMutantFilter mutantFilter = null,
            BaseMutantOrchestrator<SyntaxNode> orchestrator = null)
        {
            _options = options;
            _orchestrator = orchestrator;
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();

            _mutantFilter = mutantFilter;
        }

        /// <summary>
        /// This constructor is used by the <see cref="MutationTestProcess"/> initialization logic.
        /// </summary>
        /// <param name="options"></param>
        public CsharpMutationProcess(StrykerOptions options) : this( null, options)
        { }

        public void Mutate(MutationTestInput input)
        {
            var projectInfo = input.SourceProjectInfo.ProjectContents;
            var orchestrator = _orchestrator ?? new CsharpMutantOrchestrator(new MutantPlacer(input.SourceProjectInfo.CodeInjector), options: _options);
            // Mutate source files
            foreach (var file in projectInfo.GetAllFiles().Cast<CsharpFileLeaf>())
            {
                _logger.LogDebug($"Mutating {file.FullPath}");
                // Mutate the syntax tree
                var mutatedSyntaxTree = orchestrator.Mutate(file.SyntaxTree.GetRoot());
                // Add the mutated syntax tree for compilation
                file.MutatedSyntaxTree = mutatedSyntaxTree.SyntaxTree;
                if (_options.DevMode)
                {
                    _logger.LogTrace($"Mutated {file.FullPath}:{Environment.NewLine}{mutatedSyntaxTree.ToFullString()}");
                }
                // Filter the mutants
                file.Mutants = orchestrator.GetLatestMutantBatch();
            }

            _logger.LogDebug("{0} mutants created", projectInfo.Mutants.Count());

            CompileMutations(input);
        }

        private void CompileMutations(MutationTestInput input)
        {
            var info = input.SourceProjectInfo;
            var projectInfo =  (ProjectComponent<SyntaxTree>) info.ProjectContents;
            using var ms = new MemoryStream();
            using var msForSymbols = _options.DevMode ? new MemoryStream() : null;
            // compile the mutated syntax trees
            var compilingProcess = new CsharpCompilingProcess(input, options: _options);
            var compileResult = compilingProcess.Compile(projectInfo.CompilationSyntaxTrees, ms, msForSymbols);

            foreach (var testProject in info.TestProjectsInfo.AnalyzerResults)
            {
                var injectionPath = TestProjectsInfo.GetInjectionFilePath(testProject, input.SourceProjectInfo.AnalyzerResult);
                if (!_fileSystem.Directory.Exists(testProject.GetAssemblyDirectoryPath()))
                {
                    _fileSystem.Directory.CreateDirectory(testProject.GetAssemblyDirectoryPath());
                }

                // inject the mutated Assembly into the test project
                using var fs = _fileSystem.File.Create(injectionPath);
                ms.Position = 0;
                ms.CopyTo(fs);

                if (msForSymbols != null)
                {
                    // inject the debug symbols into the test project
                    using var symbolDestination = _fileSystem.File.Create(Path.Combine(testProject.GetAssemblyDirectoryPath(), input.SourceProjectInfo.AnalyzerResult.GetSymbolFileName()));
                    msForSymbols.Position = 0;
                    msForSymbols.CopyTo(symbolDestination);
                }

                _logger.LogDebug("Injected the mutated assembly file into {0}", injectionPath);
            }

            // if a rollback took place, mark the rolled back mutants as status:BuildError
            if (compileResult.RollbackResult?.RollbackedIds.Any() ?? false)
            {
                foreach (var mutant in projectInfo.Mutants
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

        public void FilterMutants(MutationTestInput input)
        {
            var mutantFilter = _mutantFilter ?? MutantFilterFactory.Create(_options, input);
            foreach (var file in input.SourceProjectInfo.ProjectContents.GetAllFiles())
            {
                // CompileError is a final status and can not be changed during filtering.
                var mutantsToFilter = file.Mutants.Where(x => x.ResultStatus != MutantStatus.CompileError);
                mutantFilter.FilterMutants(mutantsToFilter, file, _options);
            }
        }
    }
}
