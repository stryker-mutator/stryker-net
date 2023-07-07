using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;

namespace Stryker.Core.MutationTest
{
    /// <summary>
    /// Executes exactly one mutation test and stores the result
    /// </summary>
    public interface IMutationTestExecutor
    {
        ITestRunner TestRunner { get; }
        void Test(IProjectAndTests project, IList<Mutant> mutantsToTest, ITimeoutValueCalculator timeoutMs, TestUpdateHandler updateHandler);
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

        public void Test(IProjectAndTests project, IList<Mutant> mutantsToTest, ITimeoutValueCalculator timeoutMs, TestUpdateHandler updateHandler)
        {
            var forceSingle = false;
            while (mutantsToTest.Any())
            {
                var result = RunTestSession(project, mutantsToTest, timeoutMs, updateHandler, forceSingle);

                Logger.LogDebug(
                    $"Test run for {string.Join(", ", mutantsToTest.Select(x => x.DisplayName))} is {(result.FailingTests.Count == 0 ? "success" : "failed")} ");
                Logger.LogTrace($"Test run : {result.ResultMessage}");
                if (result.Messages is not null && result.Messages.Any())
                {
                    Logger.LogTrace($"Messages for {string.Join(", ", mutantsToTest.Select(x => x.DisplayName))}: {Environment.NewLine}{string.Join("", result.Messages)}");
                }
                var remainingMutants = mutantsToTest.Where((m) => m.ResultStatus == MutantStatus.Pending).ToList();
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

        private TestRunResult RunTestSession(IProjectAndTests projectAndTests, ICollection<Mutant> mutantsToTest,
            ITimeoutValueCalculator timeoutMs,
            TestUpdateHandler updateHandler, bool forceSingle)
        {
            Logger.LogTrace($"Testing {string.Join(" ,", mutantsToTest.Select(x => x.DisplayName))}.");
            if (forceSingle)
            {
                foreach (var mutant in mutantsToTest)
                {
                    var localResult = TestRunner.TestMultipleMutants(projectAndTests, timeoutMs, new[] { mutant }, updateHandler);
                    if (updateHandler == null || localResult.SessionTimedOut)
                    {
                        mutant.AnalyzeTestRun(localResult.FailingTests,
                            localResult.ExecutedTests,
                            localResult.TimedOutTests,
                            localResult.SessionTimedOut);
                    }
                }

                return new TestRunResult(true);
            }

            var result = TestRunner.TestMultipleMutants(projectAndTests, timeoutMs, mutantsToTest.ToList(), updateHandler);
            if (updateHandler != null && !result.SessionTimedOut)
            {
                return result;
            }

            foreach (var mutant in mutantsToTest)
            {
                mutant.AnalyzeTestRun(result.FailingTests,
                    result.ExecutedTests,
                    result.TimedOutTests,
                    mutantsToTest.Count == 1 && result.SessionTimedOut);
            }

            return result;
        }
    }
}
