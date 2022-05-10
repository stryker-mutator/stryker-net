using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace Stryker.Core.TestRunners.VsTest
{
    public interface IRunResults
    {
        List<TestResult> TestResults { get; }
        IReadOnlyList<TestCase> TestsInTimeout { get; }
    }

    public class SimpleRunResults : IRunResults
    {
        private List<TestCase> _testsInTimeOut = new();
        public List<TestResult> TestResults { get; } = new();
        public IReadOnlyList<TestCase> TestsInTimeout => _testsInTimeOut.AsReadOnly();

        public SimpleRunResults()
        {
        }

        public SimpleRunResults(ICollection<TestResult> results, IReadOnlyCollection<TestCase> testsInTimeout)
        {
            TestResults = results.ToList();
            _testsInTimeOut = testsInTimeout?.ToList() ?? new ();
        }

        public SimpleRunResults Merge(IRunResults other)
        {
            if (other.TestsInTimeout?.Count > 0)
            {
                if (_testsInTimeOut == null)
                {
                    _testsInTimeOut = other.TestsInTimeout.ToList();
                }
                else
                {
                    _testsInTimeOut.AddRange(other.TestsInTimeout);
                }
            }

            TestResults.AddRange(other.TestResults);
            return this;
        }
    }

    public sealed class RunEventHandler : ITestRunEventsHandler, IDisposable, IRunResults
    {
        private readonly AutoResetEvent _waitHandle;
        private readonly ILogger _logger;
        private readonly string _runnerId;
        private readonly IDictionary<Guid, VsTestDescription> _vsTests;
        private readonly IDictionary<Guid, TestRun> _runs = new Dictionary<Guid, TestRun>();
        private readonly Dictionary<Guid, TestCase> _inProgress = new();
        public event EventHandler VsTestFailed;
        public event EventHandler ResultsUpdated;

        public List<TestResult> TestResults { get; }
        public IReadOnlyList<TestCase> TestsInTimeout { get; private set; }
        public bool TimeOut { get; private set; }
        public bool CancelRequested { get; set; }

        public RunEventHandler(IDictionary<Guid, VsTestDescription> vsTests, ILogger logger, string runnerId)
        {
            _waitHandle = new AutoResetEvent(false);
            TestResults = new List<TestResult>();
            _vsTests = vsTests;
            _logger = logger;
            _runnerId = runnerId;
        }

        private void CaptureTestResults(IEnumerable<TestResult> results)
        {
            var testResults = results as TestResult[] ?? results.ToArray();
            foreach (var testResult in testResults)
            {
                var id = testResult.TestCase.Id;
                if (!_runs.ContainsKey(id))
                {
                    if (_vsTests.ContainsKey(id))
                    {
                        _runs[id] = new TestRun(_vsTests[id]);
                    }
                    else
                    {
                        // unknown id. Probable cause: test name has changed due to some parameter having changed
                        _runs[id] = new TestRun(new VsTestDescription(testResult.TestCase));
                    }
                }

                if (_runs[id].AddResult(testResult))
                {
                    TestResults.Add(_runs[id].Result());
                    _inProgress.Remove(id);
                }
            }
        }

        public void HandleTestRunStatsChange(TestRunChangedEventArgs testRunChangedArgs)
        {
            if (testRunChangedArgs.ActiveTests != null)
            {
                foreach (var activeTest in testRunChangedArgs.ActiveTests)
                {
                    _inProgress[activeTest.Id] = activeTest;
                }
            }

            if (testRunChangedArgs.NewTestResults == null || !testRunChangedArgs.NewTestResults.Any())
            {
                return;
            }

            CaptureTestResults(testRunChangedArgs.NewTestResults);
            ResultsUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void HandleTestRunComplete(
            TestRunCompleteEventArgs testRunCompleteArgs,
            TestRunChangedEventArgs lastChunkArgs,
            ICollection<AttachmentSet> runContextAttachments,
            ICollection<string> executorUris)
        {
            if (lastChunkArgs?.ActiveTests != null)
            {
                foreach (var activeTest in lastChunkArgs.ActiveTests)
                {
                    _inProgress[activeTest.Id] = activeTest;
                }
            }

            if (lastChunkArgs?.NewTestResults != null)
            {
                CaptureTestResults(lastChunkArgs.NewTestResults);
            }

            if (!testRunCompleteArgs.IsCanceled && (_inProgress.Any() || _runs.Values.Any(t => !t.IsComplete())))
            {
                // report ongoing tests and test case with missing results as timeouts.
                TestsInTimeout = _inProgress.Values
                    .Union(_runs.Values.Where(t => !t.IsComplete()).Select(t => t.Result().TestCase)).ToList();
                if (TestsInTimeout.Count > 0)
                {
                    TimeOut = true;
                }
            }

            ResultsUpdated?.Invoke(this, EventArgs.Empty);

            if (testRunCompleteArgs.Error != null)
            {
                if (testRunCompleteArgs.Error.GetType() == typeof(TransationLayerException))
                {
                    _logger.LogDebug(testRunCompleteArgs.Error,
                        $"{_runnerId}: VsTest may have crashed, triggering VsTest restart!");
                    VsTestFailed?.Invoke(this, EventArgs.Empty);
                }
                else if (testRunCompleteArgs.Error.InnerException is IOException sock)
                {
                    _logger.LogWarning(sock, $"{_runnerId}: Test session ended unexpectedly.");
                }
                else if (!CancelRequested)
                {
                    _logger.LogDebug(testRunCompleteArgs.Error, $"{_runnerId}: VsTest error:");
                }
            }

            _waitHandle.Set();
        }

        public int LaunchProcessWithDebuggerAttached(TestProcessStartInfo testProcessStartInfo) =>
            throw new NotSupportedException();

        public void HandleRawMessage(string rawMessage) => _logger.LogTrace($"{_runnerId}: {rawMessage} [RAW]");

        public bool WaitEnd(int? timeOut)
        {
            if (timeOut == null)
            {
                return _waitHandle.WaitOne();
            }
            else
            {
                var delay = 0;
                const int Unit = 500;
                while (delay < timeOut.Value * 3)
                {
                    if (_waitHandle.WaitOne(Unit))
                    {
                        return true;
                    }

                    delay += Unit;
                }

                return false;
            }
        }

        public void HandleLogMessage(TestMessageLevel level, string message)
        {
            var levelFinal = level switch
            {
                TestMessageLevel.Informational => LogLevel.Debug,
                TestMessageLevel.Warning => LogLevel.Warning,
                TestMessageLevel.Error => LogLevel.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
            _logger.LogTrace($"{_runnerId}: [{levelFinal}] {message}");
        }

        public void Dispose() => _waitHandle.Dispose();
    }
}
