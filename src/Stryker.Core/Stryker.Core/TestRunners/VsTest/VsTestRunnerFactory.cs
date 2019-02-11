using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerFactory : ITestRunner
    {
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new ConcurrentBag<VsTestRunner>();

        public VsTestRunnerFactory(StrykerOptions options, ProjectInfo projectInfo)
        {
            IEnumerable<TestCase> testCases;

            using (var runner = new VsTestRunner(options, projectInfo, null))
            {
                testCases = runner.DiscoverTests();
            }

            for (var i = 0; i < options.MaxConcurrentTestrunners; i++)
            {
                _availableRunners.Add(new VsTestRunner(options, projectInfo, testCases));
            }
        }

        public TestRunResult RunAll(int? timeoutMS, int? activeMutationId)
        {
            TestRunResult result = null;
            while (result is null)
            {
                if (_availableRunners.TryTake(out var runner))
                {
                    result = runner.RunAll(timeoutMS, activeMutationId);
                }
                else
                {
                    continue;
                }

                _availableRunners.Add(runner);
            }

            return result;
        }

        public void Dispose()
        {
            foreach (var runner in _availableRunners)
            {
                runner.Dispose();
            }
        }
    }
}
