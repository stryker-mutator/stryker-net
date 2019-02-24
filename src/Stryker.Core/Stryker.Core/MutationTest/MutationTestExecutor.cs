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
        void Test(Mutant mutant);
    }

    public class MutationTestExecutor : IMutationTestExecutor
    {
        public ITestRunner TestRunner { get; }
        private ILogger _logger { get; }
        private int _timeoutMS { get; }

        public MutationTestExecutor(ITestRunner testRunner, int timeoutMS)
        {
            TestRunner = testRunner;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
            _timeoutMS = timeoutMS;
        }

        public void Test(Mutant mutant)
        {
            try
            {
                var result = TestRunner.RunAll(_timeoutMS, mutant.Id);
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
