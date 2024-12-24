using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest;

public class DiscoveryEventHandler : ITestDiscoveryEventsHandler
{
    private readonly List<string> _messages;
    private readonly object _lck = new();
    private bool _discoveryDone;
    public List<TestCase> DiscoveredTestCases { get; private set; }
    public bool Aborted { get; private set; }

    public DiscoveryEventHandler(List<string> messages)
    {
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
        lock (_lck)
        {
            _discoveryDone = true;
            Monitor.Pulse(_lck);
        }
    }

    public void WaitEnd()
    {
        lock (_lck)
        {
            while (!_discoveryDone)
            {
                Monitor.Wait(_lck);
            }
        }
    }

    public void HandleRawMessage(string rawMessage) => _messages.Add("Test Discovery Raw Message: " + rawMessage);

    public void HandleLogMessage(TestMessageLevel level, string message) => _messages.Add("Test Discovery Message: " + message);
}
