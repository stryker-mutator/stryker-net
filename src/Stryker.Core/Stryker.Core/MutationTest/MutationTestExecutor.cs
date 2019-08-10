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
        void Test(Mutant mutant, int timeoutMs);
    }

    public class MutationTestExecutor : IMutationTestExecutor
    {
        public ITestRunner TestRunner { get; }
        private ILogger Logger { get; }

        public IEnumerable<TestDescription> Tests => TestRunner?.Tests;

        public MutationTestExecutor(ITestRunner testRunner)
        {
            TestRunner = testRunner;
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        public void Test(Mutant mutant, int timeoutMs)
        {
            var mutantsToTest = new List<Mutant>{mutant};
            try
            {
                Logger.LogDebug($"Testing {mutant.DisplayName}.");
                var result = TestRunner.RunAll(timeoutMs, mutant);
                Logger.LogTrace($"Testrun for {mutant.DisplayName} with output {result.ResultMessage}");
                foreach (var resultFailingTest in result.FailingTests.GetList())
                {
                    foreach (var currentMutant in mutantsToTest.Where( x => resultFailingTest.IsAllTests || x.IsTestedBy(resultFailingTest.Guid)))
                    {
                        currentMutant.CoveringTest[resultFailingTest.Guid] = true;
                        mutant.ResultStatus = result.Success ? MutantStatus.Survived : MutantStatus.Killed;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Logger.LogTrace("Testrun aborted due to timeout");
                mutant.ResultStatus = MutantStatus.Timeout;
            }
        }
    }
}
