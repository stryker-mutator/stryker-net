﻿using Microsoft.Extensions.Logging;
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
        void Test(Mutant mutant, int timeoutMs);
        ITestRunner TestRunner { get; }
    }

    public class MutationTestExecutor : IMutationTestExecutor
    {
        public ITestRunner TestRunner { get; }
        private ILogger _logger { get; }

        public MutationTestExecutor(ITestRunner testRunner)
        {
            TestRunner = testRunner;
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationTestProcess>();
        }

        public void Test(Mutant mutant, int timeoutMs)
        {
            try
            {
                var result = TestRunner.RunAll(timeoutMs, mutant.Id);
                _logger.LogTrace("Testrun with output {0}", result.ResultMessage);

                mutant.ResultStatus = result.Success ? MutantStatus.Survived : MutantStatus.Killed;
            }
            catch (OperationCanceledException)
            {
                _logger.LogTrace("Testrun aborted due to timeout");
                mutant.ResultStatus = MutantStatus.Timeout;
            }
        }
    }
}
