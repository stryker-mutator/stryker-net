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
    public interface IRunResults
    {
        List<TestResult> TestResults { get; }
        IReadOnlyList<TestCase> TestsInTimeout { get; }
    }
    internal class TestRun
    {
        private readonly VsTestDescription _testDescription;
        private readonly IList<TestResult> _results;

        public TestRun(VsTestDescription testDescription)
        {
            _testDescription = testDescription;
            _results = new List<TestResult>(testDescription.NbSubCases);
        }

        public bool AddResult(TestResult result)
        {
            _results.Add(result);
            return _results.Count >= _testDescription.NbSubCases;
        }

        public TestResult Result()
        {
            return _results.Aggregate((TestResult) null, (acc, next) =>
            {
                if (acc == null)
                {
                    return next;
                }
                if (next.Outcome == TestOutcome.Failed || acc.Outcome == TestOutcome.None)
                {
                    acc.Outcome = next.Outcome;
                }
                if (acc.StartTime > next.StartTime)
                {
                    acc.StartTime = next.StartTime;
                }
                if (acc.EndTime < next.EndTime)
                {
                    acc.EndTime = next.EndTime;
                }

                foreach (var message in next.Messages)
                {
                    acc.Messages.Add(message);
                }

                acc.Duration = acc.EndTime - acc.StartTime;
                return acc;
            });
        }
    }

    public class RunEventHandler : ITestRunEventsHandler, IDisposable, IRunResults
    {
        private readonly AutoResetEvent _waitHandle;
        private readonly ILogger _logger;
        private readonly string _runnerId;
        private readonly IDictionary<Guid, VsTestDescription> _vsTests;
        private readonly IDictionary<Guid, TestRun> _runs = new Dictionary<Guid, TestRun>();
        private readonly Dictionary<Guid, TestCase> _inProgress = new Dictionary<Guid, TestCase>();
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
                    _runs[id] = new TestRun(_vsTests[id]);
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
            if (lastChunkArgs?.NewTestResults != null)
            {
                CaptureTestResults(lastChunkArgs.NewTestResults);
            }

            if (_inProgress.Any() && !testRunCompleteArgs.IsCanceled)
            {
                TestsInTimeout = _inProgress.Values.Where(t => lastChunkArgs?.NewTestResults.Any(res => res.TestCase.Id == t.Id) != true).ToList();
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
                    _logger.LogDebug(testRunCompleteArgs.Error, $"{_runnerId}: VsTest may have crashed, triggering VsTest restart!");
                    VsTestFailed?.Invoke(this, EventArgs.Empty);
                }
                else if (testRunCompleteArgs.Error.InnerException is IOException sock)
                {
                    _logger.LogWarning(sock,$"{_runnerId}: Test session ended unexpectedly.");
                }
                else if (!CancelRequested)
                {
                    _logger.LogDebug(testRunCompleteArgs.Error, $"{_runnerId}: VsTest error:");
                }
            }

            _waitHandle.Set();
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
            LogLevel levelFinal = level switch
            {
                TestMessageLevel.Informational => LogLevel.Debug,
                TestMessageLevel.Warning => LogLevel.Warning,
                TestMessageLevel.Error => LogLevel.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
            _logger.LogTrace($"{_runnerId}: [{levelFinal}] {message}");
        }

        public void Dispose()
        {
            _waitHandle.Dispose();
        }
    }
}
