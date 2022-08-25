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
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.DataCollector;

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
        {}

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
        private readonly ITestGuids _expectedTests;
        private readonly Dictionary<Guid, TestRun> _runs = new();
        private readonly Dictionary<Guid, TestCase> _inProgress = new();
        private readonly List<TestResult> _rawResults = new();
        private readonly SimpleRunResults _currentResults = new();
        private readonly HashSet<Guid> _ranTests = new();
        private readonly HashSet<Guid> _failedTests = new();
        private readonly HashSet<Guid> _timedOutTests = new();
        private readonly HashSet<Guid> _coveringTests = new();
        
        private readonly object _lck = new();
        private bool _completed;

        public event EventHandler VsTestFailed;
        public event EventHandler ResultsUpdated;

        public RunEventHandler(IDictionary<Guid, VsTestDescription> vsTests, ITestGuids expectedTests, ILogger logger,
            string runnerId)
        {
            _vsTests = vsTests;
            _expectedTests = expectedTests;
            _logger = logger;
            _runnerId = runnerId;
        }

        public bool CancelRequested { get; set; }

        public bool TestSessionDone { get; private set; }

        private void CaptureTestResults(IEnumerable<TestResult> results)
        {
            foreach (var testResult in results)
            {
                var id = testResult.TestCase.Id;
                _rawResults.Add(testResult);
                // if this result is failure, we consider the test as failed!
                if (testResult.Outcome == TestOutcome.Failed)
                {
                    _failedTests.Add(id);
                }

                if (EnsureRunExists(id, testResult) && testResult.GetProperties().Any(p => p.Key.Id == CoverageCollector.ActiveMutationSeen))
                {
                    // this is the first result, it may contains custom properties
                    _coveringTests.Add(id);
                }

                // is test case already finished (according to the initial test run)?
                if (_runs[id].IsComplete())
                {
                    // extra result, report it
                    _currentResults.TestResults.Add(testResult);
                }
                else if (_runs[id].AddResult(testResult))
                {
                    // is the case now finished?
                    var result = _runs[id].Result();
                    _ranTests.Add(id);
                    _currentResults.TestResults.Add(result);
                    _inProgress.Remove(id);
                }
            }
        }

        private bool EnsureRunExists(Guid id, TestResult testResult)
        {
            if (_runs.ContainsKey(id))
            {
                return false;
            }
            if (_vsTests.ContainsKey(id))
            {
                _runs[id] = new TestRun(_vsTests[id]);
            }
            else
            {
                // unknown id. Probable cause: test name has changed due to some parameter having changed
                _runs[id] = new TestRun(new VsTestDescription(testResult.TestCase));
            }
            return true;
        }

        public IRunResults GetRawResults() => new SimpleRunResults(_rawResults, _currentResults.TestsInTimeout);

        public IRunResults GetResults() => _currentResults;

        public ITestRunResults GetTestRunResults() => new TestRunResults(GetRanTests(),
            new TestGuidsList(_failedTests),
            new TestGuidsList(_timedOutTests),
            new TestGuidsList(_coveringTests));

        private ITestGuids GetRanTests()
        {
            if (TestSessionDone && _expectedTests.IsEveryTest && _ranTests.Count >= _vsTests.Count)
            {
                return TestGuidsList.EveryTest();
            }
            return new TestGuidsList(_ranTests);
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
                foreach (var id in _inProgress.Keys)
                {
                    _timedOutTests.Add(id);
                }
                foreach (var (id, result) in _runs.Where( p => !p.Value.IsComplete()))
                {
                    _timedOutTests.Add(id);
                }
                _currentResults.SetTestsInTimeOut(_inProgress.Values
                    .Union(_runs.Values.Where(t => !t.IsComplete()).Select(t => t.Result().TestCase)).ToList());
            }

            TestSessionDone = true;

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

            lock (_lck)
            {
                _completed = true;
                Monitor.Pulse(_lck);
            }
        }

        public int LaunchProcessWithDebuggerAttached(TestProcessStartInfo testProcessStartInfo) =>
            throw new NotSupportedException();

        public void HandleRawMessage(string rawMessage) => _logger.LogTrace($"{_runnerId}: {rawMessage} [RAW]");

        public bool WaitEnd(int? timeOut)
        {
            lock (_lck)
            {
                if (timeOut == null)
                {
                    while (!_completed && !Monitor.Wait(_lck));
                    return true;
                }
                else
                {
                    var delay = 0;
                    const int Unit = 500;
                    while (!_completed && delay < timeOut.Value * 3)
                    {
                        if (Monitor.Wait(_lck, Unit))
                        {
                            break;
                        }
                        delay += Unit;
                    }

                    return _completed;
                }
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
    }
}
