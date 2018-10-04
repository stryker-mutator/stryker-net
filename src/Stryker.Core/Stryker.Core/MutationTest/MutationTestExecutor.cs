using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;
using System;

namespace Stryker.Core.MutationTest
{
    /// <summary>
    /// Executes exactly one mutationtest and stores the result
    /// </summary>
    public interface IMutationTestExecutor
    {
        void Test(Mutant mutant);
    }

    public class MutationTestExecutor : IMutationTestExecutor
    {
        private ITestRunner _testRunner { get; set; }
        private ILogger _logger { get; set; }
        private int _timeoutMS { get; set; }

        public MutationTestExecutor(ITestRunner testRunner, int timeoutMS)
        {
            _testRunner = testRunner;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
            _timeoutMS = timeoutMS;
        }

        public void Test(Mutant mutant)
        {
            try
            {
                var result = _testRunner.RunAll(_timeoutMS, mutant.Id);
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
