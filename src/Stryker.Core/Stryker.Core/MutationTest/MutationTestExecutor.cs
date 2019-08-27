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
                    mutant.ResultStatus = failedTests.Any(t => t.IsAllTests || mutant.IsTestedBy(t.Guid)) ? MutantStatus.Killed : MutantStatus.Survived;
                }

                foreach (var resultFailingTest in result.FailingTests.GetList())

                {
                    foreach (var currentMutant in mutantsToTest.Where( x => resultFailingTest.IsAllTests || x.IsTestedBy(resultFailingTest.Guid)))
                    {
                        currentMutant.CoveringTests[resultFailingTest.Guid] = true;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // TODO: when multiple mutants are tested at once, we need to test them in isolation on time out.
                Logger.LogTrace("Testrun aborted due to timeout");
                if (mutantsToTest.Count == 1)
                {
                    mutantsToTest[0].ResultStatus = MutantStatus.Timeout;
                }
            }
        }
    }
}
