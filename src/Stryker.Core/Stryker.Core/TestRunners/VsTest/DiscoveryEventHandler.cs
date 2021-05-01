using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Stryker.Core.TestRunners.VsTest
{
    public class DiscoveryEventHandler : ITestDiscoveryEventsHandler, ITestDiscoveryEventsHandler2
    {
        private readonly AutoResetEvent _waitHandle;
        private readonly List<string> _messages;
        public List<TestCase> DiscoveredTestCases { get; private set; }
        public bool Aborted { get; private set; }

        public DiscoveryEventHandler(AutoResetEvent waitHandle, List<string> messages)
        {
            this._waitHandle = waitHandle;
            DiscoveredTestCases = new List<TestCase>();
            _messages = messages;
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

            Aborted = isAborted;
            _waitHandle.Set();
        }

        public void HandleDiscoveryComplete(
            DiscoveryCompleteEventArgs discoveryCompleteEventArgs,
            IEnumerable<TestCase> lastChunk)
        {
            if (lastChunk != null)
            {
                DiscoveredTestCases.AddRange(lastChunk);
            }

            Aborted = discoveryCompleteEventArgs.IsAborted;
            _waitHandle.Set();
        }

        public void HandleRawMessage(string rawMessage)
        {
            _messages.Add("Test Discovery Raw Message: " + rawMessage);
        }

        public void HandleLogMessage(TestMessageLevel level, string message)
        {
            _messages.Add("Test Discovery Message: " + message);
        }
    }
}
