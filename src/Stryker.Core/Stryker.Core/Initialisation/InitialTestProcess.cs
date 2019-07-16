﻿using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.TestRunners;
using System.Diagnostics;

namespace Stryker.Core.Initialisation
{
    public interface IInitialTestProcess
    {
        int InitialTest(ITestRunner testRunner);
        TestCoverageInfos GetCoverage(ITestRunner testRunner);
    }

    public class InitialTestProcess : IInitialTestProcess
    {
        private readonly ILogger _logger;

        public InitialTestProcess()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<InitialTestProcess>();
        }

        /// <summary>
        /// Executes the initial testrun using the given testrunner
        /// </summary>
        /// <param name="testRunner"></param>
        /// <returns>The duration of the initial testrun</returns>
        public int InitialTest(ITestRunner testRunner)
        {
            var totalNumberOfTests = testRunner.DiscoverNumberOfTests() is var total && total == -1 ? "Unable to detect" : $"{total}";
            _logger.LogInformation("Total number of tests found: {0}", totalNumberOfTests);

            _logger.LogInformation("Initial testrun started");

            // Setup a stopwatch to record the initial test duration
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var testResult = testRunner.RunAll(null, null);
            // Stop stopwatch immediately after testrun
            stopwatch.Stop();

            _logger.LogDebug("Initial testrun output {0}", testResult.ResultMessage);
            if (!testResult.Success)
            {
                _logger.LogWarning("Initial test run failed. Mutation score cannot be computed.");
                throw new StrykerInputException("Initial testrun was not successful.", testResult.ResultMessage);
            }

            return (int)stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Capture coverage informaiton
        /// </summary>
        /// <param name="testRunner"></param>
        /// <returns></returns>
        public TestCoverageInfos GetCoverage(ITestRunner testRunner)
        {
            var testResult = testRunner.CaptureCoverage();
            if (!testResult.Success)
            {
                _logger.LogWarning("Test run with no active mutation failed. Stryker failed to correctly generate the mutated assembly. Please report this issue on github with a logfile of this run.");
                throw new StrykerInputException("No active mutant testrun was not successful.", testResult.ResultMessage);
            }

            return testRunner.CoverageMutants;
        }
    }
}
