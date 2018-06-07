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
    public class MutationTestExecutor : IMutationTestExecutor
    {
        private ITestRunner _testRunner { get; set; }
        private ILogger _logger { get; set; }

        public MutationTestExecutor(ITestRunner testRunner)
        {
            _testRunner = testRunner;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        public void Test(Mutant mutant)
        {
            _testRunner.SetActiveMutation(mutant.Id);
            var result = _testRunner.RunAll();
            if(result.Success)
            {
                mutant.ResultStatus = MutantStatus.Survived;
            } else
            {
                mutant.ResultStatus = MutantStatus.Killed;
            }
            _logger.LogTrace("Testrun with output {0}", result.ResultMessage);
            _logger.LogDebug("Mutant {0} got status {2}", mutant.Id, mutant.ResultStatus);
        }
    }
}
