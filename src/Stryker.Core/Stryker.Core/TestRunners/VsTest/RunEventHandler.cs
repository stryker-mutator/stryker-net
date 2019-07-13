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
        private readonly ILogger _logger;
        private bool _testFailed;

        public event EventHandler TestsFailed;
        public List<TestResult> TestResults { get; }

        public RunEventHandler(AutoResetEvent waitHandle, ILogger logger)
        {
            _waitHandle = waitHandle;
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
            _logger.LogTrace($"Runner: {rawMessage} [RAW]");
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
                    levelFinal = LogLevel.Information;
                    break;
                case TestMessageLevel.Error:
                    levelFinal = LogLevel.Warning;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
            _logger.LogTrace($"Runner:[{levelFinal}] {message}");
        }
    }
}
