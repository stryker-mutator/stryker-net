using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Stryker.Core.MutationTest
{
    public interface IMutationTestProcess
    {
        void Mutate(StrykerOptions options);
        StrykerRunResult Test(StrykerOptions options);
    }

    public class MutationTestProcess : IMutationTestProcess
    {
        private MutationTestInput _input { get; set; }
        private IReporter _reporter { get; set; }
        private IMutantOrchestrator _orchestrator { get; set; }
        private IFileSystem _fileSystem { get; }
        private ICompilingProcess _compilingProcess { get; set; }
        private IMutationTestExecutor _mutationTestExecutor { get; set; }
        private ICompilingProcess _rollbackProcess { get; set; }
        private ILogger _logger { get; set; }

        public MutationTestProcess(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IMutantOrchestrator orchestrator = null,
            ICompilingProcess compilingProcess = null,
            IFileSystem fileSystem = null)
        {
            _input = mutationTestInput;
            _reporter = reporter;
            _mutationTestExecutor = mutationTestExecutor;
            _orchestrator = orchestrator ?? new MutantOrchestrator();
            _compilingProcess = compilingProcess ?? new CompilingProcess(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        public void Mutate(StrykerOptions options)
        {
            var mutatedSyntaxTrees = new Collection<SyntaxTree>
            {
                // add helper
                MutantPlacer.ActiveMutantSelectorHelper
            };
            foreach (var file in _input.ProjectInfo.ProjectContents.GetAllFiles())
            {
                // Get the syntax tree for the source file
                var syntaxTree = CSharpSyntaxTree.ParseText(file.SourceCode,
                    path: file.FullPath,
                    options: new CSharpParseOptions(LanguageVersion.Latest));

                if (!file.IsExcluded)
                {
                    // Mutate the syntax tree
                    var mutatedSyntaxTree = _orchestrator.Mutate(syntaxTree.GetRoot());
                    // Add the mutated syntax tree for compilation
                    mutatedSyntaxTrees.Add(mutatedSyntaxTree.SyntaxTree);
                    // Store the generated mutants in the file
                    file.Mutants = _orchestrator.GetLatestMutantBatch();
                }
                else
                {
                    // Add the original syntax tree for future compilation
                    mutatedSyntaxTrees.Add(syntaxTree);
                    // There aren't any mutants generated so a new list of mutants is sufficient
                    file.Mutants = new List<Mutant>();

                    _logger.LogDebug("Excluded file {0}, no mutants created", file.FullPath);
                }
            }

            _logger.LogInformation("{0} mutants created", _input.ProjectInfo.ProjectContents.Mutants.Count());

            using (var ms = new MemoryStream())
            {
                // compile the mutated syntax trees
                var compileResult = _compilingProcess.Compile(mutatedSyntaxTrees, ms, options.DevMode);
                if (!_fileSystem.Directory.Exists(_input.GetInjectionPath()) && !_fileSystem.File.Exists(_input.GetInjectionPath()))
                {
                    _fileSystem.Directory.CreateDirectory(_input.GetInjectionPath());
                }

                // inject the mutated Assembly into the test project
                using (var fs = _fileSystem.File.Create(_input.GetInjectionPath()))
                {
                    ms.Position = 0;
                    ms.CopyTo(fs);
                }
                _logger.LogDebug("Injected the mutated assembly file into {0}", _input.GetInjectionPath());

                // if a rollback took place, mark the rollbacked mutants as status:BuildError
                if (compileResult.RollbackResult?.RollbackedIds.Any() ?? false)
                {
                    foreach (var mutant in _input.ProjectInfo.ProjectContents.Mutants
                        .Where(x => compileResult.RollbackResult.RollbackedIds.Contains(x.Id)))
                    {
                        mutant.ResultStatus = MutantStatus.BuildError;
                    }
                }
                int numberOfBuildErrors = compileResult.RollbackResult?.RollbackedIds.Count() ?? 0;
                if (numberOfBuildErrors > 0)
                {
                    _logger.LogInformation("{0} mutants could not compile and got status {1}", numberOfBuildErrors, MutantStatus.BuildError.ToString());
                }

                if (options.ExcludedMutations.Count() != 0)
                {
                    var mutantsToSkip = _input.ProjectInfo.ProjectContents.Mutants
                        .Where(x => options.ExcludedMutations.Contains(x.Mutation.Type)).ToList();
                    foreach (var mutant in mutantsToSkip)
                    {
                        mutant.ResultStatus = MutantStatus.Skipped;
                    }
                    _logger.LogInformation("{0} mutants got status {1}", mutantsToSkip.Count(), MutantStatus.Skipped.ToString());
                }
            }

            _logger.LogInformation("{0} mutants ready for test", _input.ProjectInfo.ProjectContents.TotalMutants.Count());

            _reporter.OnMutantsCreated(_input.ProjectInfo.ProjectContents);
        }

        public StrykerRunResult Test(StrykerOptions options)
        {
            var logicalProcessorCount = Environment.ProcessorCount;
            var usableProcessorCount = Math.Max(logicalProcessorCount / 2, 1);

            if (options.MaxConcurrentTestrunners <= logicalProcessorCount)
            {
                usableProcessorCount = options.MaxConcurrentTestrunners;
            }

            var mutantsNotRun = _input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList();
            _reporter.OnStartMutantTestRun(mutantsNotRun);

            Parallel.ForEach(mutantsNotRun,
                new ParallelOptions { MaxDegreeOfParallelism = usableProcessorCount },
                mutant =>
                {
                    _mutationTestExecutor.Test(mutant);

                    _reporter.OnMutantTested(mutant);
                });

            _mutationTestExecutor.TestRunner.Dispose();
            _reporter.OnAllMutantsTested(_input.ProjectInfo.ProjectContents);

            return new StrykerRunResult(options, _input.ProjectInfo.ProjectContents.GetMutationScore());
        }
    }
}
