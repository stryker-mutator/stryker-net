using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Stryker.Core.MutationTest
{
    public interface IMutationTestProcess
    {
        void Mutate();
        StrykerRunResult Test(StrykerOptions options);
        void GetCoverage();

        void FilterMutants();
    }

    public class MutationTestProcess : IMutationTestProcess
    {
        private readonly IFileSystem _fileSystem;
        private readonly MutationTestInput _input;
        private readonly IProjectComponent _projectInfo;
        private readonly ILogger _logger;
        private readonly IMutationTestExecutor _mutationTestExecutor;
        private readonly IMutantOrchestrator _orchestrator;
        private readonly IReporter _reporter;
        private readonly ICoverageAnalyser _coverageAnalyser;
        private readonly Language _language;
        private readonly StrykerOptions _options;
        private IMutationTestProcessMethod _mutationTestProcess;

        public MutationTestProcess(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IMutantOrchestrator orchestrator = null,
            IFileSystem fileSystem = null,
            StrykerOptions options = null,
            IMutantFilter mutantFilter = null,
            ICoverageAnalyser coverageAnalyser = null,
            Language language = Language.Undifined)
        {
            _input = mutationTestInput;
            _projectInfo = mutationTestInput.ProjectInfo.ProjectContents;
            _reporter = reporter;
            _options = options;
            _mutationTestExecutor = mutationTestExecutor;
            _orchestrator = orchestrator ?? new MutantOrchestrator(options: _options);
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
            _coverageAnalyser = coverageAnalyser ?? new CoverageAnalyser(_options, _mutationTestExecutor, _input);
            _language = language;

            SetupMutationTestProcess(mutantFilter);
        }

        private void SetupMutationTestProcess(IMutantFilter mutantFilter)
        {

            if (_language == Language.Csharp)
            {
                _mutationTestProcess = new MutationtTestProcessMethod(_input, _orchestrator, _fileSystem, _options, mutantFilter, _reporter);
            }
            else if (_language == Language.Fsharp)
            {
                throw new GeneralStrykerException("no valid language detected || no valid csproj");
            }
            else
            {
                throw new GeneralStrykerException("no valid language detected || no valid csproj was given");
            }
        }

        public void Mutate()
        {
            _mutationTestProcess.Mutate();
        }

        public void FilterMutants()
        {
            _mutationTestProcess.FilterMutants();
        }

        public StrykerRunResult Test(StrykerOptions options)
        {
            var viableMutantsCount = _projectInfo.Mutants.Count(x => x.CountForStats);
            var mutantsNotRun = _projectInfo.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun).ToList();

            if (!mutantsNotRun.Any())
            {
                if (_projectInfo.ReadOnlyMutants.Any(x => x.ResultStatus == MutantStatus.Ignored))
                {
                    _logger.LogWarning("It looks like all mutants with tests were excluded. Try a re-run with less exclusion!");
                }
                if (_projectInfo.ReadOnlyMutants.Any(x => x.ResultStatus == MutantStatus.NoCoverage))
                {
                    _logger.LogWarning("It looks like all non-excluded mutants are not covered by a test. Go add some tests!");
                }
                if (!_projectInfo.ReadOnlyMutants.Any())
                {
                    _logger.LogWarning("It\'s a mutant-free world, nothing to test.");
                    return new StrykerRunResult(options, double.NaN);
                }
            }

            var mutantsToTest = mutantsNotRun;
            if (_options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                var testCount = _mutationTestExecutor.TestRunner.DiscoverNumberOfTests();
                var toTest = mutantsNotRun.Sum(x => x.MustRunAgainstAllTests ? testCount : x.CoveringTests.Count);
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

            _reporter.OnAllMutantsTested(_projectInfo);

            _mutationTestExecutor.TestRunner.Dispose();

            return new StrykerRunResult(options, _projectInfo.GetMutationScore());
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
            _coverageAnalyser.DetermineTestCoverage();
        }


    }
}