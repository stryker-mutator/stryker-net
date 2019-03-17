using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.TestRunners.VsTest
{
    public class RunEventHandler : ITestRunEventsHandler
    {
        private readonly AutoResetEvent waitHandle;
        private readonly List<string> _messages;
        public List<TestResult> TestResults { get; private set; }
        private bool testFailed;
        private static readonly ILogger Logger;

        public event EventHandler TestsFailed;
        static RunEventHandler()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<RunEventHandler>();
        }

        public RunEventHandler(AutoResetEvent waitHandle, List<string> messages)
        {
            this.waitHandle = waitHandle;
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

            waitHandle.Set();
        }

        private void CaptureTestResults(IEnumerable<TestResult> results)
        {
            if (!testFailed && results.Any(result => result.Outcome == TestOutcome.Failed))
            {
                // at least one test has failed
                testFailed = true;
                TestsFailed?.Invoke(this, EventArgs.Empty);
            }
            TestResults.AddRange(results);
        }

        public void HandleTestRunStatsChange(TestRunChangedEventArgs testRunChangedArgs)
        {
            if (testRunChangedArgs != null && testRunChangedArgs.NewTestResults != null)
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
