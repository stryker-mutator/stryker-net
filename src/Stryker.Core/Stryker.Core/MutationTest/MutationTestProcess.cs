using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
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
        void Mutate();
        void Test(int maxConcurrentTestRunners);
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
            IEnumerable<IMutator> mutators,
            IMutationTestExecutor mutationTestExecutor,
            IMutantOrchestrator orchestrator = null,
            ICompilingProcess compilingProcess = null,
            IFileSystem fileSystem = null)
        {
            _input = mutationTestInput;
            _reporter = reporter;
            _mutationTestExecutor = mutationTestExecutor;
            _orchestrator = orchestrator ?? new MutantOrchestrator(mutators);
            _compilingProcess = compilingProcess ?? new CompilingProcess(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        public void Mutate()
        {
            var mutatedSyntaxTrees = new Collection<SyntaxTree>();
            foreach (var file in _input.ProjectInfo.ProjectContents.GetAllFiles())
            {
                // Get the syntax tree for the source file
                var syntaxTree = CSharpSyntaxTree.ParseText(file.SourceCode, path: file.FullPath);
                // Mutate the syntax tree
                var mutatedSyntaxTree = _orchestrator.Mutate(syntaxTree.GetRoot());
                // Add the mutated syntax tree for compilation
                mutatedSyntaxTrees.Add(mutatedSyntaxTree.SyntaxTree);
                // Store the generated mutatants in the file
                file.Mutants = _orchestrator.GetLatestMutantBatch();
            }

            _logger.LogInformation("{0} mutants created", _input.ProjectInfo.ProjectContents.Mutants.Count());
            
            using (var ms = new MemoryStream())
            {
                // compile the mutated syntax trees
                var compileResult = _compilingProcess.Compile(mutatedSyntaxTrees, ms);
                if (compileResult.Success)
                {
                    _fileSystem.Directory.CreateDirectory(_input.GetInjectionPath());
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
                        foreach(var mutant in _input.ProjectInfo.ProjectContents.Mutants
                            .Where(x => compileResult.RollbackResult.RollbackedIds.Contains(x.Id)))
                        {
                            mutant.ResultStatus = MutantStatus.BuildError;
                        }
                    }
                    _logger.LogDebug("{0} mutants got status {1}", compileResult.RollbackResult?.RollbackedIds.Count(), MutantStatus.BuildError);
                }
            }

            _logger.LogInformation("{0} mutants ready for test", _input.ProjectInfo.ProjectContents.TotalMutants.Count());

            _reporter.OnMutantsCreated(_input.ProjectInfo.ProjectContents);
        }

        public void Test(int maxConcurrentTestRunners)
        {
            var logicalProcessorCount = Environment.ProcessorCount;                  
            var usableProcessorCount = Math.Max(logicalProcessorCount / 2, 1);

            if (maxConcurrentTestRunners <= logicalProcessorCount)
            {
                usableProcessorCount = maxConcurrentTestRunners;
            }

            Parallel.ForEach(_input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun),
                new ParallelOptions { MaxDegreeOfParallelism = usableProcessorCount },
                mutant =>
                {
                    _mutationTestExecutor.Test(mutant);
                    _reporter.OnMutantTested(mutant);
                });
            _reporter.OnAllMutantsTested(_input.ProjectInfo.ProjectContents);
        }
    }
}
