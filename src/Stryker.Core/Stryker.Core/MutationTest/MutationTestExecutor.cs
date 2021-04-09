using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;
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
        void Test(IList<Mutant> mutant, int timeoutMs, TestUpdateHandler updateHandler);
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

        public void Test(IList<Mutant> mutantsToTest, int timeoutMs, TestUpdateHandler updateHandler)
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

        private TestRunResult RunTestSession(IList<Mutant> mutantsToTest, int timeoutMs, TestUpdateHandler updateHandler,
            bool forceSingle)
        {
            TestRunResult result = null;
            Logger.LogTrace($"Testing {string.Join(" ,", mutantsToTest.Select(x => x.DisplayName))}.");
            if (TestRunner is IMultiTestRunner multi && !forceSingle)
            {
                result = multi.TestMultipleMutants(timeoutMs, mutantsToTest.ToList(), updateHandler);
            }
            else
            {
                foreach (var mutant in mutantsToTest)
                {
                    var testRunResult = TestRunner.RunAll(timeoutMs, mutant, updateHandler);
                    if (result == null)
                    {
                        result = testRunResult;
                    }
                    else
                    {
                        result.Merge(testRunResult);
                    }
                }
            }

            if (updateHandler == null)
            {
                foreach (var mutant in mutantsToTest)
                {
                    mutant.AnalyzeTestRun(result.FailingTests, result.RanTests, result.TimedOutTests);
                }
            }

            return result;
        }
    }
}
