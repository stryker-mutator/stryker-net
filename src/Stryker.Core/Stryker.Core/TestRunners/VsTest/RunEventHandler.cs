using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public SimpleRunResults(IEnumerable<TestResult> results, IEnumerable<TestCase> testsInTimeout)
        {
            TestResults = results.ToList();
            _testsInTimeOut = testsInTimeout?.ToList() ?? new List<TestCase>();
        }

        public void SetTestsInTimeOut(ICollection<TestCase> tests) => _testsInTimeOut = tests.ToList();

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

    public sealed class RunEventHandler : ITestRunEventsHandler
    {
        private readonly ILogger _logger;
        private readonly string _runnerId;
        private readonly IDictionary<Guid, VsTestDescription> _vsTests;
        private readonly IDictionary<Guid, TestRun> _runs = new Dictionary<Guid, TestRun>();
        private readonly Dictionary<Guid, TestCase> _inProgress = new();
        private SimpleRunResults _currentResults = new();
        private readonly List<TestResult> _rawResults = new();
        private int _initialResultsCount;
        private readonly object _lck = new();
        private bool _completed;

        public event EventHandler ResultsUpdated;

        public bool CancelRequested { get; set; }

        public bool Failed { get; private set; }

        public RunEventHandler(IDictionary<Guid, VsTestDescription> vsTests, ILogger logger, string runnerId)
        {
            _vsTests = vsTests;
            _logger = logger;
            _runnerId = runnerId;
        }

        private void CaptureTestResults(IEnumerable<TestResult> results)
        {
            var testResults = results as TestResult[] ?? results.ToArray();
            _rawResults.AddRange(testResults);
            AnalyzeRawTestResults(testResults);
        }

        private void AnalyzeRawTestResults(IEnumerable<TestResult> testResults)
        {
            foreach (var testResult in testResults)
            {
                var id = testResult.TestCase.Id;
                if (!_runs.ContainsKey(id))
                {
                    if (_vsTests.TryGetValue(id, out var test))
                    {
                        _runs[id] = new TestRun(test);
                    }
                    else
                    {
                        // unknown id. Probable cause: test name has changed due to some parameter having changed
                        _runs[id] = new TestRun(new VsTestDescription(testResult.TestCase));
                    }
                }

                if (_runs[id].IsComplete())
                {
                    // unexpected result, report it
                    _currentResults.TestResults.Add(testResult);
                }
                else if (_runs[id].AddResult(testResult))
                {
                    _currentResults.TestResults.Add(_runs[id].Result());
                    _inProgress.Remove(id);
                }
            }
        }

        public IRunResults GetRawResults() => new SimpleRunResults(_rawResults, _currentResults.TestsInTimeout);

        public IRunResults GetResults() => _currentResults;

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
            _logger.LogDebug("{RunnerId}: Received testrun complete.", _runnerId);
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
                _currentResults.SetTestsInTimeOut(_inProgress.Values
                    .Union(_runs.Values.Where(t => !t.IsComplete()).Select(t => t.Result().TestCase)).ToList());
            }

            ResultsUpdated?.Invoke(this, EventArgs.Empty);

            if (testRunCompleteArgs.Error != null)
            {
                if (testRunCompleteArgs.Error.GetType() == typeof(TransationLayerException))
                {
                    _logger.LogDebug(testRunCompleteArgs.Error,
                        "{RunnerId}: VsTest may have crashed, triggering VsTest restart!",_runnerId);
                    Failed = true;
                }
                else if (testRunCompleteArgs.Error.InnerException is IOException sock)
                {
                    _logger.LogWarning(sock, "{RunnerId}: Test session ended unexpectedly.", _runnerId);
                }
                else if (!CancelRequested)
                {
                    _logger.LogDebug(testRunCompleteArgs.Error, "{RunnerId}: VsTest error:", _runnerId);
                }
            }

            lock (_lck)
            {
                _completed = true;
                Monitor.Pulse(_lck);
            }
        }

        public int LaunchProcessWithDebuggerAttached(TestProcessStartInfo testProcessStartInfo) =>
            throw new NotSupportedException();

        public void HandleRawMessage(string rawMessage) =>
            _logger.LogTrace("{RunnerId}: {RawMessage} [RAW]", _runnerId, rawMessage);

        public void StartSession()
        {
            _completed = false;
            Failed = false;
            _initialResultsCount = _rawResults.Count;
            _inProgress.Clear();
            _runs.Clear();
        }

        public void DiscardCurrentRun()
        {
            // remove all raw results from this run
            _rawResults.RemoveRange(_initialResultsCount, _rawResults.Count - _initialResultsCount);
            // we reanalyze results gathered so far, in an event sourced way
            _runs.Clear();
            _currentResults = new SimpleRunResults(new List<TestResult>(), _currentResults.TestsInTimeout);
            AnalyzeRawTestResults(_rawResults);
        }

        public bool Wait(int timeOut, out bool slept)
        {
            lock (_lck)
            {
                var watch = new Stopwatch();
                watch.Start();

                while (!_completed && watch.ElapsedMilliseconds < timeOut)
                {
                    Monitor.Wait(_lck, Math.Max(0, (int)(timeOut - watch.ElapsedMilliseconds)));
                }

                slept = watch.ElapsedMilliseconds - timeOut > 30 * 1000;
                if (slept)
                {
                    _logger.LogWarning("{RunnerId}: the computer slept during the testing, need to retry", _runnerId);
                }

                return _completed;
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
            _logger.LogTrace("{RunnerId}: [{LevelFinal}] {Message}", _runnerId, levelFinal, message);
        }
    }
}
