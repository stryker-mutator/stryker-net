using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{

    public class RunEventHandler : ITestRunEventsHandler
    {
        private AutoResetEvent waitHandle;
        private readonly List<string> _messages;
        public List<TestResult> TestResults { get; private set; }

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
                TestResults.AddRange(lastChunkArgs.NewTestResults);
            }

            waitHandle.Set();
        }

        public void HandleTestRunStatsChange(TestRunChangedEventArgs testRunChangedArgs)
        {
            if (testRunChangedArgs != null && testRunChangedArgs.NewTestResults != null)
            {
                TestResults.AddRange(testRunChangedArgs.NewTestResults);
            }
        }

        public int LaunchProcessWithDebuggerAttached(TestProcessStartInfo testProcessStartInfo)
        {
            throw new NotImplementedException();
        }

        public void HandleRawMessage(string rawMessage)
        {
            _messages.Add("Test Run Raw Message: " + rawMessage);
        }

        public void HandleLogMessage(TestMessageLevel level, string message)
        {
            _messages.Add("Test Run Message: " + message);
        }
    }
}
