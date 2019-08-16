using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Stryker.Core.TestRunners.VsTest
{
    public class RunEventHandler : ITestRunEventsHandler
    {
        private readonly AutoResetEvent _waitHandle;
        private readonly int _id;
        private readonly ILogger _logger;
        private bool _testFailed;

        public event EventHandler TestsFailed;
        public List<TestResult> TestResults { get; }

        public RunEventHandler(AutoResetEvent waitHandle, int id, ILogger logger)
        {
            _waitHandle = waitHandle;
            _id = id;
            TestResults = new List<TestResult>();
            _logger = logger;
        }

        public void HandleTestRunComplete(
            TestRunCompleteEventArgs testRunCompleteArgs,
            TestRunChangedEventArgs lastChunkArgs,
            ICollection<AttachmentSet> runContextAttachments,
            ICollection<string> executorUris)
        {
            if (lastChunkArgs?.NewTestResults != null)
            {
                CaptureTestResults(lastChunkArgs.NewTestResults);
            }

            if (testRunCompleteArgs.Error != null)
            {
                _logger.LogWarning(testRunCompleteArgs.Error, "Exception in VsTest");
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
            _logger.LogTrace($"Runner {_id}: [RAW] {rawMessage}");
        }

        public void HandleLogMessage(TestMessageLevel level, string message)
        {
            LogLevel levelFinal;
            switch (level)
            {
                case TestMessageLevel.Informational:
                    levelFinal = LogLevel.Debug;
                    break;
                case TestMessageLevel.Warning:
                    levelFinal = LogLevel.Warning;
                    break;
                case TestMessageLevel.Error:
                    levelFinal = LogLevel.Error;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
            _logger.LogTrace($"Runner {_id}: [{levelFinal}] {message}");
        }
    }
}
