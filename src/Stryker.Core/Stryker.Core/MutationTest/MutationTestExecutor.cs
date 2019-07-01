using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;
using System;

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

        public MutationTestExecutor(ITestRunner testRunner)
        {
            TestRunner = testRunner;
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        public void Test(Mutant mutant, int timeoutMs)
        {
            try
            {
                Logger.LogDebug($"Testing {mutant.DisplayName}.");
                var result = TestRunner.RunAll(timeoutMs, mutant.Id);
                Logger.LogTrace($"Testrun for {mutant.DisplayName} with output {result.ResultMessage}");

                mutant.ResultStatus = result.Success ? MutantStatus.Survived : MutantStatus.Killed;
            }
            catch (OperationCanceledException)
            {
                Logger.LogTrace("Testrun aborted due to timeout");
                mutant.ResultStatus = MutantStatus.Timeout;
            }
        }
    }
}
