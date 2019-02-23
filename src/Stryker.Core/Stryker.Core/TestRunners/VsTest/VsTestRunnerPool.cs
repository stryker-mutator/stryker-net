using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunnerPool : ITestRunner
    {
        private readonly ConcurrentBag<VsTestRunner> _availableRunners = new ConcurrentBag<VsTestRunner>();

        public VsTestRunnerPool(StrykerOptions options, ProjectInfo projectInfo)
        {
            IEnumerable<TestCase> testCases;

            using (var runner = new VsTestRunner(options, projectInfo, null))
            {
                testCases = runner.DiscoverTests();
            }

            for (var i = 0; i < options.ConcurrentTestrunners; i++)
            {
                _availableRunners.Add(new VsTestRunner(options, projectInfo, testCases.Count()));
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
