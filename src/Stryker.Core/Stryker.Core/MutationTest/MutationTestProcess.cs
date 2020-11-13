using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;

namespace Stryker.Core.MutationTest
{
    public interface IMutationTestProcessProvider
    {
        IMutationTestProcess Provide(MutationTestInput mutationTestInput, IReporter reporter, IMutationTestExecutor mutationTestExecutor, IStrykerOptions options);
    }

    public class MutationTestProcessProvider : IMutationTestProcessProvider
    {
        public IMutationTestProcess Provide(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IStrykerOptions options)
        {
            return new MutationTestProcess(mutationTestInput, reporter, mutationTestExecutor, options: options);
        }
    }

    public interface IMutationTestProcess
    {
        MutationTestInput Input { get; }
        void Mutate();
        StrykerRunResult Test(IEnumerable<Mutant> mutantsToTest);
        void GetCoverage();
        void FilterMutants();
    }

    public class MutationTestProcess : IMutationTestProcess
    {
        public MutationTestInput Input { get; }
        private readonly IFileSystem _fileSystem;
        private readonly IProjectComponent _projectContents;
        private readonly ILogger _logger;
        private readonly IMutationTestExecutor _mutationTestExecutor;
        private readonly IMutantOrchestrator _orchestrator;
        private readonly IReporter _reporter;
        private readonly ICoverageAnalyser _coverageAnalyser;
        private readonly IStrykerOptions _options;
        private IMutationProcess _mutationProcess;

        public MutationTestProcess(MutationTestInput mutationTestInput,
            IReporter reporter,
            IMutationTestExecutor mutationTestExecutor,
            IMutantOrchestrator orchestrator = null,
            IFileSystem fileSystem = null,
            IMutantFilter mutantFilter = null,
            ICoverageAnalyser coverageAnalyser = null,
            IStrykerOptions options = null)
        {
            Input = mutationTestInput;
            _projectContents = mutationTestInput.ProjectInfo.ProjectContents;
            _reporter = reporter;
            _options = options;
            _mutationTestExecutor = mutationTestExecutor;
            _orchestrator = orchestrator ?? new MutantOrchestrator(options: _options);
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
            _coverageAnalyser = coverageAnalyser ?? new CoverageAnalyser(_options, _mutationTestExecutor, mutationTestInput);

            _mutationProcess = new MutationProcess(Input, _orchestrator, _fileSystem, _options, mutantFilter, _reporter);
        }

        public void Mutate()
        {
            _mutationProcess.Mutate();
        }

        public void FilterMutants()
        {
            _mutationProcess.FilterMutants();
        }

        public StrykerRunResult Test(IEnumerable<Mutant> mutantsToTest)
        {
            if (!mutantsToTest.Any())
            {
                return new StrykerRunResult(_options, double.NaN);
            }
            if (mutantsToTest.Any(x => x.ResultStatus != MutantStatus.NotRun))
            {
                throw new GeneralStrykerException("Only mutants to run should be passed to the mutation test process. If you see this message please report an issue.");
            }
            var mutantGroups = BuildMutantGroupsForTest(mutantsToTest.ToList());

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = _options.ConcurrentTestrunners };
            Parallel.ForEach(mutantGroups, parallelOptions, mutants =>
            {
                var testMutants = new HashSet<Mutant>();

                bool testUpdateHandler(IReadOnlyList<Mutant> testedMutants, ITestListDescription failedTests, ITestListDescription ranTests, ITestListDescription timedOutTest)
                {
                    var mustGoOn = !_options.Optimizations.HasFlag(OptimizationFlags.AbortTestOnKill);
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
                }
                _mutationTestExecutor.Test(mutants, Input.TimeoutMs, testUpdateHandler);

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

            _mutationTestExecutor.TestRunner.Dispose();

            return new StrykerRunResult(_options, _projectContents.ToReadOnlyInputComponent().GetMutationScore());
        }

        private IEnumerable<List<Mutant>> BuildMutantGroupsForTest(IReadOnlyCollection<Mutant> mutantsNotRun)
        {

            if (_options.Optimizations.HasFlag(OptimizationFlags.DisableTestMix) || !_options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                return mutantsNotRun.Select(x => new List<Mutant> { x });
            }

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

            _logger.LogDebug($"Mutations will be tested in {blocks.Count} test runs, instead of {mutantsNotRun.Count}.");
            return blocks;
        }

        public void GetCoverage()
        {
            _coverageAnalyser.DetermineTestCoverage();
        }
    }
}
