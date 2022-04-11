using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;

namespace Stryker.Core.CoverageAnalysis
{
    public class CoverageAnalyser : ICoverageAnalyser
    {
        private readonly MutationTestInput _input;
        private readonly ILogger<CoverageAnalyser> _logger;
        private readonly IMutationTestExecutor _mutationTestExecutor;
        private readonly StrykerOptions _options;

        public CoverageAnalyser(StrykerOptions options, IMutationTestExecutor mutationTestExecutor, MutationTestInput input)
        {
            _input = input;
            _mutationTestExecutor = mutationTestExecutor;
            _options = options;

            _logger = ApplicationLogging.LoggerFactory.CreateLogger<CoverageAnalyser>();
        }

        public void DetermineTestCoverage()
        {
            if (!_options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants) &&
                !_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
            {
                return;
            }
            var mutantsToScan =
                _input.ProjectInfo.ProjectContents.Mutants
                    .Where(x => x.ResultStatus == MutantStatus.NotRun)
                    .ToList();
            foreach (var mutant in mutantsToScan)
            {
                mutant.ResetCoverage();
            }
            var testResult = _mutationTestExecutor.TestRunner.CaptureCoverage(mutantsToScan);
            if (testResult.FailingTests.Count == 0)
            {
                SetCoveringTests(mutantsToScan);
                return;
            }
            _logger.LogWarning("Test run with no active mutation failed. Stryker failed to correctly generate the mutated assembly. Please report this issue on github with a logfile of this run.");
            throw new InputException("No active mutant testrun was not successful.", testResult.ResultMessage);
        }

        private void SetCoveringTests(IReadOnlyCollection<Mutant> mutantsToScan)
        {
            // force static mutants to be tested against all tests.
            if (!_options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest))
            {
                foreach (var mutant in mutantsToScan.Where(mutant => mutant.IsStaticValue))
                {
                    mutant.CoveringTests = TestsGuidList.EveryTest();
                }
            }
            foreach (var mutant in mutantsToScan)
            {
                if (mutant.CoveringTests.IsEmpty)
                {
                    mutant.ResultStatus = MutantStatus.NoCoverage;
                    mutant.ResultStatusReason = "Mutant has no test coverage";
                }
                else if (!_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
                {
                    mutant.CoveringTests = TestsGuidList.EveryTest();
                }
                if (mutant.CoveringTests.IsEveryTest)
                {
                    _logger.LogDebug($"Mutant {mutant.Id} will be tested against all tests.");
                }
                else if (mutant.CoveringTests.Count>0)
                {
                    _logger.LogDebug($"Mutant {mutant.Id} is covered by {mutant.CoveringTests.Count} tests.");
                    _logger.LogTrace($"Tests are : {string.Join(',', mutant.CoveringTests.GetGuids())}.");
                }
                else
                {
                    _logger.LogDebug($"Mutant {mutant.Id} is not covered by any test.");
                }
            }
        }
    }
}
