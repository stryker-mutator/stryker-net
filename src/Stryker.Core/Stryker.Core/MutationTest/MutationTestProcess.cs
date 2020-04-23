using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.Compiling;
using Stryker.Core.DiffProviders;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Stryker.Core.MutationTest
{
    public interface IMutationTestProcess
    {
        void Mutate();
        StrykerRunResult Test(StrykerOptions options);
        void GetCoverage();
    }

    public class MutationTestProcess : IMutationTestProcess
    {
        private readonly ICompilingProcess _compilingProcess;
        private readonly IFileSystem _fileSystem;
        private readonly MutationTestInput _input;
        private readonly ILogger _logger;
        private readonly IMutantFilter _mutantFilter;
        private readonly IMutationTestExecutor _mutationTestExecutor;
        private readonly IMutantOrchestrator _orchestrator;
        private readonly IReporter _reporter;
        private readonly StrykerOptions _options;

        public MutationTestProcess(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IMutantOrchestrator orchestrator = null,
            ICompilingProcess compilingProcess = null,
            IFileSystem fileSystem = null,
            StrykerOptions options = null,
            IMutantFilter mutantFilter = null)
        {
            _input = mutationTestInput;
            _reporter = reporter;
            _options = options;
            _mutationTestExecutor = mutationTestExecutor;
            _orchestrator = orchestrator ?? new MutantOrchestrator(options: _options);
            _compilingProcess = compilingProcess ?? new CompilingProcess(mutationTestInput, new RollbackProcess());
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
            _mutantFilter = mutantFilter ?? BroadcastMutantFilterFactory.Create(options);
        }

        public void Mutate()
        {
            // Mutate source files
            foreach (var file in _input.ProjectInfo.ProjectContents.GetAllFiles())
            {
                _logger.LogDebug($"Mutating {file.Name}");
                // Mutate the syntax tree
                var mutatedSyntaxTree = _orchestrator.Mutate(file.SyntaxTree.GetRoot());
                // Add the mutated syntax tree for compilation
                file.MutatedSyntaxTree = mutatedSyntaxTree.SyntaxTree;
                if (_options.DevMode)
                {
                    _logger.LogTrace($"Mutated {file.Name}:{Environment.NewLine}{mutatedSyntaxTree.ToFullString()}");
                }
                // Filter the mutants
                var allMutants = _orchestrator.GetLatestMutantBatch();
                IEnumerable<Mutant> filteredMutants = allMutants;

                filteredMutants = _mutantFilter.FilterMutants(filteredMutants, file, _options).ToList();


                // Store the generated mutants in the file
                file.Mutants = allMutants;
            }

            _logger.LogDebug("{0} mutants created", _input.ProjectInfo.ProjectContents.Mutants.Count());

            CompileMutations();

            var skippedMutantGroups = _input.ProjectInfo.ProjectContents.GetAllFiles()
                .SelectMany(f => f.Mutants)
                .Where(x => x.ResultStatus != MutantStatus.NotRun).GroupBy(x => x.ResultStatusReason)
                .OrderBy(x => x.Key);

            foreach (var skippedMutantGroup in skippedMutantGroups)
            {
                _logger.LogInformation("{0} mutants got status {1}. Reason: {2}", skippedMutantGroup.Count(),
                    skippedMutantGroup.First().ResultStatus, skippedMutantGroup.Key);
            }

            _logger.LogInformation("{0} mutants ready for test",
                _input.ProjectInfo.ProjectContents.TotalMutants.Count());

            _reporter.OnMutantsCreated(_input.ProjectInfo.ProjectContents);
        }

        private void CompileMutations()
        {
            using (var ms = new MemoryStream())
            {
                // compile the mutated syntax trees
                var compileResult = _compilingProcess.Compile(_input.ProjectInfo.ProjectContents.CompilationSyntaxTrees, ms, _options.DevMode);

                foreach (var testProject in _input.ProjectInfo.TestProjectAnalyzerResults)
                {
                    var injectionPath = _input.ProjectInfo.GetInjectionPath(testProject);
                    if (!_fileSystem.Directory.Exists(Path.GetDirectoryName(injectionPath)) &&
                        !_fileSystem.File.Exists(injectionPath))
                    {
                        _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(injectionPath));
                    }

                    // inject the mutated Assembly into the test project
                    using (var fs = _fileSystem.File.Create(injectionPath))
                    {
                        ms.Position = 0;
                        ms.CopyTo(fs);
                    }

                    _logger.LogDebug("Injected the mutated assembly file into {0}", injectionPath);
                }

                // if a rollback took place, mark the rollbacked mutants as status:BuildError
                if (compileResult.RollbackResult?.RollbackedIds.Any() ?? false)
                {
                    foreach (var mutant in _input.ProjectInfo.ProjectContents.Mutants
                        .Where(x => compileResult.RollbackResult.RollbackedIds.Contains(x.Id)))
                    {
                        // Ignore compilation errors if the mutation is skipped anyways.
                        if (mutant.ResultStatus == MutantStatus.Ignored)
                        {
                            continue;
                        }

                        mutant.ResultStatus = MutantStatus.CompileError;
                        mutant.ResultStatusReason = "Could not compile";
                    }
                }
            }
        }

        public StrykerRunResult Test(StrykerOptions options)
        {
            var viableMutantsCount = _input.ProjectInfo.ProjectContents.Mutants.Count(x => x.CountForStats);
            var mutantsNotRun = _input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList();

            if (!mutantsNotRun.Any())
            {
                if (_input.ProjectInfo.ProjectContents.Mutants.Any(x => x.ResultStatus == MutantStatus.Ignored))
                {
                    _logger.LogWarning("It looks like all mutants with tests were excluded. Try a re-run with less exclusion!");
                }
                if (_input.ProjectInfo.ProjectContents.Mutants.Any(x => x.ResultStatus == MutantStatus.NoCoverage))
                {
                    _logger.LogWarning("It looks like all non-excluded mutants are not covered by a test. Go add some tests!");
                }
                if (!_input.ProjectInfo.ProjectContents.Mutants.Any())
                {
                    _logger.LogWarning("It\'s a mutant-free world, nothing to test.");
                    return new StrykerRunResult(options, double.NaN);
                }
            }

            var mutantsToTest = mutantsNotRun;
            if (_options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                var testCount = _mutationTestExecutor.TestRunner.DiscoverNumberOfTests();
                var toTest = @mutantsNotRun.Sum(x => x.MustRunAgainstAllTests ? testCount : x.CoveringTests.Count);
                var total = testCount * viableMutantsCount;
                if (total > 0 && total != toTest)
                {
                    _logger.LogInformation($"Coverage analysis will reduce run time by discarding {(total - toTest) / (double)total:P1} of tests because they would not change results.");
                }
            }
            else if (_options.Optimizations.HasFlag(OptimizationFlags.SkipUncoveredMutants))
            {
                var total = viableMutantsCount;
                var toTest = mutantsToTest.Count();
                if (total > 0 && total != toTest)
                {
                    _logger.LogInformation($"Coverage analysis will reduce run time by discarding {(total - toTest) / (double)total:P1} of tests because they would not change results.");
                }
            }

            if (mutantsToTest.Any())
            {
                var mutantGroups = BuildMutantGroupsForTest(mutantsNotRun);

                _reporter.OnStartMutantTestRun(mutantsNotRun, _mutationTestExecutor.TestRunner.Tests);

                Parallel.ForEach(
                    mutantGroups,
                    new ParallelOptions { MaxDegreeOfParallelism = options.ConcurrentTestrunners },
                    mutants =>
                    {
                        var testMutants = new HashSet<Mutant>();
                        _mutationTestExecutor.Test(mutants, _input.TimeoutMs,
                            (testedMutants, failedTests, ranTests, timedOutTest) =>
                            {
                                var mustGoOn = !options.Optimizations.HasFlag(OptimizationFlags.AbortTestOnKill);
                                foreach (var mutant in testedMutants)
                                {
                                    mutant.AnalyzeTestRun(failedTests, ranTests, timedOutTest);
                                    if (mutant.ResultStatus == MutantStatus.NotRun)
                                    {
                                        mustGoOn = true;
                                    }
                                    else if (!testMutants.Contains(mutant))
                                    {
                                        testMutants.Add(mutant);
                                        _reporter.OnMutantTested(mutant);
                                    }
                                }

                                return mustGoOn;
                            });

                        foreach (var mutant in mutants)
                        {
                            if (mutant.ResultStatus == MutantStatus.NotRun)
                            {
                                _logger.LogWarning($"Mutation {mutant.Id} was not fully tested.");
                            }
                            else if (!testMutants.Contains(mutant))
                            {
                                _reporter.OnMutantTested(mutant);
                            }
                        }
                    });
            }

            _reporter.OnAllMutantsTested(_input.ProjectInfo.ProjectContents);

            _mutationTestExecutor.TestRunner.Dispose();

            return new StrykerRunResult(options, _input.ProjectInfo.ProjectContents.GetMutationScore());
        }

        private IEnumerable<List<Mutant>> BuildMutantGroupsForTest(IReadOnlyCollection<Mutant> mutantsNotRun)
        {

            if (_options.Optimizations.HasFlag(OptimizationFlags.DisableTestMix) || !_options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                return mutantsNotRun.Select(x => new List<Mutant> { x });
            }

            _logger.LogInformation("Analyze coverage info to test multiple mutants per session.");
            var blocks = new List<List<Mutant>>(mutantsNotRun.Count);
            var mutantsToGroup = mutantsNotRun.ToList();
            // we deal with mutants needing full testing first
            blocks.AddRange(mutantsToGroup.Where(m => m.MustRunAgainstAllTests).Select(m => new List<Mutant> { m }));
            mutantsToGroup.RemoveAll(m => m.MustRunAgainstAllTests);
            var testsCount = mutantsToGroup.SelectMany(m => m.CoveringTests.GetList()).Distinct().Count();
            mutantsToGroup = mutantsToGroup.OrderByDescending(m => m.CoveringTests.Count).ToList();
            for (var i = 0; i < mutantsToGroup.Count; i++)
            {
                var usedTests = mutantsToGroup[i].CoveringTests.GetList().ToList();
                var nextBlock = new List<Mutant> { mutantsToGroup[i] };
                for (var j = i + 1; j < mutantsToGroup.Count; j++)
                {
                    if (mutantsToGroup[j].CoveringTests.Count + usedTests.Count > testsCount ||
                        mutantsToGroup[j].CoveringTests.ContainsAny(usedTests))
                    {
                        continue;
                    }

                    nextBlock.Add(mutantsToGroup[j]);
                    usedTests.AddRange(mutantsToGroup[j].CoveringTests.GetList());
                    mutantsToGroup.RemoveAt(j--);
                }

                blocks.Add(nextBlock);
            }
            // compute number of tests that will be run
            _logger.LogInformation($"Mutations will be tested in {blocks.Count} test runs, instead of {mutantsNotRun.Count}.");
            return blocks;
        }

        public void GetCoverage()
        {
            var (targetFrameworkDoesNotSupportAppDomain, targetFrameworkDoesNotSupportPipe) = _input.ProjectInfo.ProjectUnderTestAnalyzerResult.CompatibilityModes;
            var mutantsToScan = _input.ProjectInfo.ProjectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList();
            foreach (var mutant in mutantsToScan)
            {
                mutant.CoveringTests = new TestListDescription(null);
            }
            var testResult = _mutationTestExecutor.TestRunner.CaptureCoverage(mutantsToScan, targetFrameworkDoesNotSupportPipe, targetFrameworkDoesNotSupportAppDomain);
            if (testResult.FailingTests.Count == 0)
            {
                // force static mutants to be tested against all tests.
                if (!_options.Optimizations.HasFlag(OptimizationFlags.CaptureCoveragePerTest))
                {
                    foreach (var mutant in mutantsToScan.Where(mutant => mutant.IsStaticValue))
                    {
                        mutant.MustRunAgainstAllTests = true;
                    }
                }
                foreach (var mutant in mutantsToScan)
                {
                    if (!mutant.MustRunAgainstAllTests && mutant.CoveringTests.IsEmpty)
                    {
                        mutant.ResultStatus = MutantStatus.NoCoverage;
                    }
                    else if (!_options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
                    {
                        mutant.CoveringTests = TestListDescription.EveryTest();
                    }
                }

                return;
            }
            _logger.LogWarning("Test run with no active mutation failed. Stryker failed to correctly generate the mutated assembly. Please report this issue on github with a logfile of this run.");
            throw new StrykerInputException("No active mutant testrun was not successful.", testResult.ResultMessage);
        }
    }
}