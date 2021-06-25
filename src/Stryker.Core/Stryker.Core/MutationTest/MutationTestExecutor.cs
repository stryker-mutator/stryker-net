using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Initialisation;

namespace Stryker.Core.MutationTest
{
    /// <summary>
    /// Executes exactly one mutation test and stores the result
    /// </summary>
    public interface IMutationTestExecutor
    {
        ITestRunner TestRunner { get; }
        void Test(IList<Mutant> mutant, ITimeoutValueCalculator timeoutMs, TestUpdateHandler updateHandler);
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

        public void Test(IList<Mutant> mutantsToTest, ITimeoutValueCalculator timeoutMs, TestUpdateHandler updateHandler)
        {
            var forceSingle = false;
            while (mutantsToTest.Any())
            {
                var result = RunTestSession(mutantsToTest, timeoutMs, updateHandler, forceSingle);

                Logger.LogTrace(
                    $"Test run for {string.Join(" ,", mutantsToTest.Select(x => x.DisplayName))} is {(result.FailingTests.Count == 0 ? "success" : "failed")} with output: {result.ResultMessage}");

                var remainingMutants = mutantsToTest.Where((m) => m.ResultStatus == MutantStatus.NotRun).ToList();
                if (remainingMutants.Count == mutantsToTest.Count)
                {
                    // the test failed to get any conclusive results
                    if (!result.SessionTimedOut)
                    {
                        // something bad happened.
                        Logger.LogError($"Stryker failed to test {remainingMutants.Count} mutant(s).");
                        return;
                    }

                    // test session's results have been corrupted by the time out
                    // we retry and run tests one by one, if necessary
                    if (remainingMutants.Count == 1)
                    {
                        // only one mutant was tested, we mark it as timeout.
                        remainingMutants[0].ResultStatus = MutantStatus.Timeout;
                        remainingMutants.Clear();
                    }
                    else
                    {
                        // we don't know which tests timed out, we rerun all tests in dedicated sessions
                        forceSingle = true;
                    }
                }

                if (remainingMutants.Any())
                {
                    Logger.LogDebug("Not all mutants were tested.");
                }
                mutantsToTest = remainingMutants;
            }
        }

        private TestRunResult RunTestSession(IList<Mutant> mutantsToTest, ITimeoutValueCalculator timeoutMs,
            TestUpdateHandler updateHandler, bool forceSingle)
        {
            TestRunResult result;
            Logger.LogTrace($"Testing {string.Join(" ,", mutantsToTest.Select(x => x.DisplayName))}.");
            if (forceSingle && mutantsToTest.Count > 1)
            {
                foreach (var mutant in mutantsToTest)
                {
                    var localResult = TestRunner.TestMultipleMutants(timeoutMs, new[] { mutant }, updateHandler);
                    mutant.AnalyzeTestRun(localResult.FailingTests, localResult.RanTests, localResult.TimedOutTests);
                }

                return new TestRunResult(true);
            }
            else
            {
                result = TestRunner.TestMultipleMutants(timeoutMs, mutantsToTest.ToList(), updateHandler);
                if (updateHandler == null)
                {
                    foreach (var mutant in mutantsToTest)
                    {
                        mutant.AnalyzeTestRun(result.FailingTests, result.RanTests, result.TimedOutTests);
                    }
                }
            }

            return result;
        }
    }
}
