using System;
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
            TestRunResult result = null;
            Logger.LogTrace($"Testing {string.Join(" ,", mutantsToTest.Select(x => x.DisplayName))}.");
            if (TestRunner is IMultiTestRunner multi)
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

            Logger.LogDebug(
                $"Test run for {string.Join(" ,", mutantsToTest.Select(x => x.DisplayName))} is {(result.Success ? "success" : "failed")} with output: {result.ResultMessage}");

            var remainingMutants = new List<Mutant>();
            foreach (var mutant in mutantsToTest)
            {
                mutant.AnalyzeTestRun(result.FailingTests, result.RanTests);
                if (mutant.ResultStatus == MutantStatus.NotRun)
                {
                    // test run is not conclusive
                    remainingMutants.Add(mutant);
                }
            }

            if (result.TimeOut)
            {
                if (remainingMutants.Count == 1)
                {
                    remainingMutants[0].ResultStatus = MutantStatus.Timeout;
                }
                else
                {
                    // we will test mutants in isolation.
                    foreach (var remainingMutant in remainingMutants)
                    {
                        result = TestRunner.RunAll(timeoutMs, remainingMutant, updateHandler);
                        if (result.TimeOut)
                        {
                            remainingMutant.ResultStatus = MutantStatus.Timeout;
                        }
                        else
                        {
                            remainingMutant.AnalyzeTestRun(result.FailingTests, result.RanTests);
                        }
                    }
                }
            }
        }
    }
}