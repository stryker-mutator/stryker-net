using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.MutationTest
{
    /// <summary>
    /// Executes exactly one mutation test and stores the result
    /// </summary>
    public interface IMutationTestExecutor
    {
        ITestRunner TestRunner { get; }
        void Test(IList<Mutant> mutant, int timeoutMs);
    }

    public class MutationTestExecutor : IMutationTestExecutor
    {
        public ITestRunner TestRunner { get; }
        private ILogger Logger { get; }

        public MutationTestExecutor(ITestRunner testRunner)
        {
            TestRunner = testRunner;
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        public void Test(IList<Mutant> mutantsToTest, int timeoutMs)
        {
            try
            {
                var result = new TestRunResult(true);
                Logger.LogTrace($"Testing {string.Join(" ,", mutantsToTest.Select(x => x.DisplayName))}.");
                foreach (var mutant in mutantsToTest)
                {
                    result.Merge(TestRunner.RunAll(timeoutMs, mutant));
                }
                Logger.LogDebug($"Test run for {string.Join(" ,", mutantsToTest.Select(x => x.DisplayName))} is {(result.Success ? "success" : "failed")} with output: {result.ResultMessage}");
                var failedTests = result.FailingTests.GetList();
                foreach (var mutant in mutantsToTest)
                {
                    mutant.AnalyzeTestRun(failedTests, result.RanTests);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.LogTrace("Testrun aborted due to timeout");
                if (mutantsToTest.Count == 1)
                {
                    mutantsToTest[0].ResultStatus = MutantStatus.Timeout;
                }
                else
                {
                    // we run each mutant in isolation to identify which one(s) times out
                    foreach (var mutant in mutantsToTest)
                    {
                        Test(new List<Mutant>{mutant}, timeoutMs);
                    }
                }
            }
        }
    }
}
