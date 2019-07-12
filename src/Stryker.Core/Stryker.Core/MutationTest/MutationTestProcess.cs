using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.TestRunners;

namespace Stryker.Core.MutationTest
{
    public interface IMutationTestProcess
    {
        void Mutate(StrykerOptions options);
        StrykerRunResult Test(StrykerOptions options);
        void Optimize(TestCoverageInfos coveredMutants);
    }

    public class MutationTestProcess : IMutationTestProcess
    {
        private MutationTestInput Input { get; }
        private IReporter Reporter { get; }
        private IMutantOrchestrator Orchestrator { get; }
        private IFileSystem FileSystem { get; }
        private ICompilingProcess CompilingProcess { get; }
        private IMutationTestExecutor MutationTestExecutor { get; }
        private ILogger Logger { get; }

        public MutationTestProcess(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IMutantOrchestrator orchestrator = null,
            ICompilingProcess compilingProcess = null,
            IFileSystem fileSystem = null)
        {
            Input = mutationTestInput;
            Reporter = reporter;
            MutationTestExecutor = mutationTestExecutor;
            Orchestrator = orchestrator ?? new MutantOrchestrator();
            CompilingProcess = compilingProcess ?? new CompilingProcess(mutationTestInput, new RollbackProcess());
            FileSystem = fileSystem ?? new FileSystem();
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        public void Mutate(StrykerOptions options)
        {
            var mutatedSyntaxTrees = new List<SyntaxTree>();
                // add helper
            Logger.LogDebug("Injecting helpers into assembly.");
            mutatedSyntaxTrees.AddRange(CodeInjection.MutantHelpers);

            foreach (var file in Input.ProjectInfo.ProjectContents.GetAllFiles())
            {
                // Get the syntax tree for the source file
                var syntaxTree = CSharpSyntaxTree.ParseText(file.SourceCode,
                    path: file.FullPath,
                    options: new CSharpParseOptions(LanguageVersion.Latest));

                if (!file.IsExcluded)
                {
                    // Mutate the syntax tree
                    var mutatedSyntaxTree = Orchestrator.Mutate(syntaxTree.GetRoot());
                    // Add the mutated syntax tree for compilation
                    mutatedSyntaxTrees.Add(mutatedSyntaxTree.SyntaxTree);
                    // Store the generated mutants in the file
                    file.Mutants = Orchestrator.GetLatestMutantBatch();
                }
                else
                {
                    // Add the original syntax tree for future compilation
                    mutatedSyntaxTrees.Add(syntaxTree);
                    // There aren't any mutants generated so a new list of mutants is sufficient
                    file.Mutants = new List<Mutant>();

                    Logger.LogDebug("Excluded file {0}, no mutants created", file.FullPath);
                }
            }

            Logger.LogDebug("{0} mutants created", Input.ProjectInfo.ProjectContents.Mutants.Count());

            using (var ms = new MemoryStream())
            {
                // compile the mutated syntax trees
                var compileResult = CompilingProcess.Compile(mutatedSyntaxTrees, ms, options.DevMode);

                var injectionPath = Input.ProjectInfo.GetInjectionPath();
                if (!FileSystem.Directory.Exists(Path.GetDirectoryName(injectionPath)) && !FileSystem.File.Exists(injectionPath))
                {
                    FileSystem.Directory.CreateDirectory(Path.GetDirectoryName(injectionPath));
                }

                // inject the mutated Assembly into the test project
                using (var fs = FileSystem.File.Create(injectionPath))
                {
                    ms.Position = 0;
                    ms.CopyTo(fs);
                }
                Logger.LogDebug("Injected the mutated assembly file into {0}", injectionPath);

                // if a rollback took place, mark the rollbacked mutants as status:BuildError
                if (compileResult.RollbackResult?.RollbackedIds.Any() ?? false)
                {
                    foreach (var mutant in Input.ProjectInfo.ProjectContents.Mutants
                        .Where(x => compileResult.RollbackResult.RollbackedIds.Contains(x.Id)))
                    {
                        mutant.ResultStatus = MutantStatus.CompileError;
                    }
                }
                var numberOfBuildErrors = compileResult.RollbackResult?.RollbackedIds.Count() ?? 0;
                if (numberOfBuildErrors > 0)
                {
                    Logger.LogInformation("{0} mutants could not compile and got status {1}", numberOfBuildErrors, MutantStatus.CompileError.ToString());
                }

                if (options.ExcludedMutations.Count() != 0)
                {
                    var mutantsToSkip = Input.ProjectInfo.ProjectContents.Mutants
                        .Where(x => options.ExcludedMutations.Contains(x.Mutation.Type)).ToList();
                    foreach (var mutant in mutantsToSkip)
                    {
                        mutant.ResultStatus = MutantStatus.Skipped;
                    }
                    Logger.LogInformation("{0} mutants got status {1}", mutantsToSkip.Count(), MutantStatus.Skipped.ToString());
                }
            }

            Logger.LogInformation("{0} mutants ready for test", Input.ProjectInfo.ProjectContents.TotalMutants.Count());

            Reporter.OnMutantsCreated(Input.ProjectInfo.ProjectContents);
        }

        public StrykerRunResult Test(StrykerOptions options)
        {
            var mutantsNotRun = Input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList();
            if (!mutantsNotRun.Any())
            {
                if (Input.ProjectInfo.ProjectContents.Mutants.Any(x => x.ResultStatus == MutantStatus.Skipped))
                {
                    Logger.LogWarning("It looks like all mutants were excluded, try a re-run with less exclusion.");
                }
                if (Input.ProjectInfo.ProjectContents.Mutants.Any(x => x.ResultStatus == MutantStatus.NoCoverage))
                {
                    Logger.LogWarning("Not a single mutant is covered. Go add some tests!");
                }
                else
                {
                    Logger.LogWarning("It\'s a mutant-free world, nothing to test.");
                    new StrykerRunResult(options, null);
                }
            }

            var mutantsToTest = mutantsNotRun.Where(x => x.ResultStatus != MutantStatus.Skipped && x.ResultStatus != MutantStatus.NoCoverage);
            if (mutantsToTest.Any())
            {
                Reporter.OnStartMutantTestRun(mutantsNotRun);
                Parallel.ForEach(mutantsToTest,
                    new ParallelOptions { MaxDegreeOfParallelism = options.ConcurrentTestrunners },
                    mutant =>
                    {
                        MutationTestExecutor.Test(mutant, Input.TimeoutMs);

                        Reporter.OnMutantTested(mutant);
                    });
            }

            Reporter.OnAllMutantsTested(Input.ProjectInfo.ProjectContents);

            MutationTestExecutor.TestRunner.Dispose();

            return new StrykerRunResult(options, Input.ProjectInfo.ProjectContents.GetMutationScore());
        }

        public void Optimize(TestCoverageInfos coveredMutants)
        {
            if (coveredMutants == null)
            {
                Logger.LogDebug("No mutant is covered by any test, no optimization done.");
                return;
            }
            var covered = new HashSet<int>(coveredMutants.CoveredMutants);

            Logger.LogDebug("Optimize test runs according to coverage info.");

            var nonCoveredMutants = Input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun && !covered.Contains(x.Id)).ToList();
            const MutantStatus mutantResultStatus = MutantStatus.NoCoverage;
            foreach (var mutant in nonCoveredMutants)
            {
                mutant.ResultStatus = mutantResultStatus;
            }

            foreach (var mutant in Input.ProjectInfo.ProjectContents.Mutants)
            {
                var tests = coveredMutants.GetTests<object>(mutant.Id);
                if (tests == null)
                {
                    continue;
                }
                mutant.CoveringTest = tests.Select(x => x.ToString()).ToList();
            }

            Logger.LogInformation(nonCoveredMutants.Count == 0
                ? "Congratulations, all mutants are covered by tests!"
                : $"{nonCoveredMutants.Count} mutants are not reached by any tests and will survive! (Marked as {mutantResultStatus}).");
        }
    }
}
