using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public class RunEventHandler : ITestRunEventsHandler, IDisposable
    {
        private readonly AutoResetEvent _waitHandle;
        private readonly ILogger _logger;
        private readonly string _runnerId;

        public event EventHandler VsTestFailed;
        public event EventHandler ResultsUpdated;

        public List<TestResult> TestResults { get; }

        public bool TimeOut { get; private set; }
        public bool CancelRequested { get; set; }

        public RunEventHandler(ILogger logger, string runnerId)
        {
            _waitHandle = new AutoResetEvent(false);
            TestResults = new List<TestResult>();
            _logger = logger;
            _runnerId = runnerId;
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

            TimeOut = testRunCompleteArgs.IsAborted;

            if (testRunCompleteArgs.Error != null)
            {
                if (testRunCompleteArgs.Error.GetType() == typeof(TransationLayerException))
                {
                    _logger.LogDebug(testRunCompleteArgs.Error, "VsTest may have crashed, triggering vstest restart!");
                    VsTestFailed?.Invoke(this, EventArgs.Empty);
                }
                else if (testRunCompleteArgs.Error.InnerException is IOException sock)
                {
                    _logger.LogWarning(sock,"Test session ended unexpectedly.");
                }
                else if (!CancelRequested)
                {
                    _logger.LogDebug(testRunCompleteArgs.Error, "VsTest error:");
                }
            }

            _waitHandle.Set();
        }

        private void CaptureTestResults(IEnumerable<TestResult> results)
        {
            var testResults = results as TestResult[] ?? results.ToArray();
            TestResults.AddRange(testResults);
            ResultsUpdated?.Invoke(this, EventArgs.Empty);  
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
            _logger.LogTrace($"{_runnerId}: {rawMessage} [RAW]");
        }

        public void WaitEnd()
        {
            _waitHandle.WaitOne();
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
            _logger.LogTrace($"{_runnerId}: [{levelFinal}] {message}");
        }

        public void Dispose()
        {
            _waitHandle.Dispose();
        }
    }
}
