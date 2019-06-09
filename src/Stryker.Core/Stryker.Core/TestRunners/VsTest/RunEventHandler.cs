using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public class RunEventHandler : ITestRunEventsHandler
    {
        private readonly AutoResetEvent _waitHandle;
        private readonly List<string> _messages;
        public List<TestResult> TestResults { get; }
        private bool _testFailed;

        public event EventHandler TestsFailed;

        public RunEventHandler(AutoResetEvent waitHandle, List<string> messages)
        {
            _waitHandle = waitHandle;
            TestResults = new List<TestResult>();
            _messages = messages;
        }

        public void HandleTestRunComplete(
            TestRunCompleteEventArgs testRunCompleteArgs,
            TestRunChangedEventArgs lastChunkArgs,
            ICollection<AttachmentSet> runContextAttachments,
            ICollection<string> executorUris)
        {
            if (lastChunkArgs != null && lastChunkArgs.NewTestResults != null)
            {
                CaptureTestResults(lastChunkArgs.NewTestResults);
            }

            _waitHandle.Set();
        }

        private void CaptureTestResults(IEnumerable<TestResult> results)
        {
            var testResults = results as TestResult[] ?? results.ToArray();
            TestResults.AddRange(testResults);
            if (!_testFailed && testResults.Any(result => result.Outcome == TestOutcome.Failed))
            {
                // at least one test has failed
                _testFailed = true;
                TestsFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void HandleTestRunStatsChange(TestRunChangedEventArgs testRunChangedArgs)
        {
            if (testRunChangedArgs?.NewTestResults != null)
            {
                CaptureTestResults(testRunChangedArgs.NewTestResults);
            }
        }

        public int LaunchProcessWithDebuggerAttached(TestProcessStartInfo testProcessStartInfo)
        {
            throw new NotImplementedException();
        }

        public void HandleRawMessage(string rawMessage)
        {
            var item = $"Test Run Message: [RAW] {rawMessage}";
            _messages.Add(item);
        }

        public void HandleLogMessage(TestMessageLevel level, string message)
        {
            var item = $"Test Run Message: [{level}] {message}";
            _messages.Add(item);
        }
    }
}
