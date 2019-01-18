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
        void Test(Mutant mutan, int timeotimeoutMsut);
    }

    public class MutationTestExecutor : IMutationTestExecutor
    {
        private ITestRunner _testRunner { get; set; }
        private ILogger _logger { get; set; }

        public MutationTestExecutor(ITestRunner testRunner)
        {
            _testRunner = testRunner;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        public void Test(Mutant mutant, int timeoutMs)
        {
            try
            {
                var result = _testRunner.RunAll(timeoutMs, mutant.Id);
                _logger.LogTrace("Testrun with output {0}", result.ResultMessage);

                if (result.Success)
                {
                    mutant.ResultStatus = MutantStatus.Survived;
                }
                else
                {
                    mutant.ResultStatus = MutantStatus.Killed;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogTrace("Testrun aborted due to timeout");
                mutant.ResultStatus = MutantStatus.Timeout;
            }
        }
    }
}
