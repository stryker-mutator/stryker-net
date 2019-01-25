using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public partial class VsTestRunner
    {
        public class DiscoveryEventHandler : ITestDiscoveryEventsHandler
        {
            private AutoResetEvent waitHandle;

            public List<TestCase> DiscoveredTestCases { get; private set; }

            public DiscoveryEventHandler(AutoResetEvent waitHandle)
            {
                this.waitHandle = waitHandle;
                DiscoveredTestCases = new List<TestCase>();
            }

            public void HandleDiscoveredTests(IEnumerable<TestCase> discoveredTestCases)
            {
                if (discoveredTestCases != null)
                {
                    DiscoveredTestCases.AddRange(discoveredTestCases);
                }
            }

            public void HandleDiscoveryComplete(long totalTests, IEnumerable<TestCase> lastChunk, bool isAborted)
            {
                if (lastChunk != null)
                {
                    DiscoveredTestCases.AddRange(lastChunk);
                }
                waitHandle.Set();
            }

            public void HandleRawMessage(string rawMessage)
            {
                messages.Add("Discovery Raw:" + rawMessage);
            }

            public void HandleLogMessage(TestMessageLevel level, string message)
            {
                messages.Add("Discovery Clean:" + message);
            }
        }
    }
}
