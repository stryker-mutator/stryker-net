using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation.Buildalyzer;
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
        private readonly IStrykerOptions _options;

        public CoverageAnalyser(IStrykerOptions options, IMutationTestExecutor mutationTestExecutor, MutationTestInput input)
        {
            _input = input;
            _mutationTestExecutor = mutationTestExecutor;
            _options = options;

            _logger = ApplicationLogging.LoggerFactory.CreateLogger<CoverageAnalyser>();
        }

        public void DetermineTestCoverage()
        {
            if (_options.Optimizations.HasFlag(OptimizationFlags.SkipUncoveredMutants) || _options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                var (targetFrameworkDoesNotSupportAppDomain, targetFrameworkDoesNotSupportPipe) = _input.ProjectInfo.ProjectUnderTestAnalyzerResult.CompatibilityModes();
                var mutantsToScan =
                    _input.ProjectInfo.ProjectContents.Mutants
                        .Where(x => x.ResultStatus == MutantStatus.NotRun)
                    .ToList();
                foreach (var mutant in mutantsToScan)
                {
                    mutant.ResetCoverage();
                }
                var testResult = _mutationTestExecutor.TestRunner.CaptureCoverage(mutantsToScan, targetFrameworkDoesNotSupportPipe, targetFrameworkDoesNotSupportAppDomain);
                if (testResult.FailingTests.Count == 0)
                {
                    SetCoveringTests(mutantsToScan);
                    return;
                }
                _logger.LogWarning("Test run with no active mutation failed. Stryker failed to correctly generate the mutated assembly. Please report this issue on github with a logfile of this run.");
                throw new StrykerInputException("No active mutant testrun was not successful.", testResult.ResultMessage);
            }
        }

        private void SetCoveringTests(List<Mutant> mutantsToScan)
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
                    mutant.ResultStatusReason = "Mutant has no test coverage";
                }
                else if (!_options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest))
                {
                    mutant.DeclareCoveringTest(TestDescription.AllTests());
                }
            }
        }
    }
}
