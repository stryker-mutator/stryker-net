using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public partial class VsTestRunner
    {
        public class RunEventHandler : ITestRunEventsHandler
        {
            private AutoResetEvent waitHandle;

            public List<TestResult> TestResults { get; private set; }

            public RunEventHandler(AutoResetEvent waitHandle)
            {
                this.waitHandle = waitHandle;
                TestResults = new List<TestResult>();
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
                messages.Add("Run Raw:" + rawMessage);
            }

            public void HandleLogMessage(TestMessageLevel level, string message)
            {
                messages.Add("Run Clean:" + message);
            }
        }
    }
}
