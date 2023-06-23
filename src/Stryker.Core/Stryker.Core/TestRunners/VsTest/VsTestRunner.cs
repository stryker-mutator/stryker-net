using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;

namespace Stryker.Core.TestRunners.VsTest
{
    public sealed class VsTestRunner : IDisposable
    {
        private IVsTestConsoleWrapper _vsTestConsole;
        private bool _disposedValue; // To detect redundant calls
        private bool _cancelled;
        private readonly VsTestContextInformation _context;
        private readonly int _id;
        private int _instanceCount;
        private readonly ILogger _logger;
        // safety timeout for VsTestWrapper operations. We assume VsTest crashed if the timeout triggers

        public static int VsTestExtraTimeOutInMs { get; set; } = 10 * 1000;

        // maximum number of attempts for VsTest sessions, just in case we run into some of VsTest quirks
        private const int MaxAttempts = 3;

        private string RunnerId => $"Runner {_id}";

        public VsTestRunner(VsTestContextInformation context,
            int id,
            ILogger logger = null)
        {
            _context = context;
            _id = id;
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunner>();
            _vsTestConsole = _context.BuildVsTestWrapper(RunnerId);
        }

        public TestRunResult InitialTest(IProjectAndTests project)
        {
            var testResults = RunTestSession(TestGuidsList.EveryTest(), project);
            foreach (var test in _context.VsTests.Keys)
            {
                _context.VsTests[test].ClearInitialResult();
            }
            // initial test run, register test results
            foreach (var result in testResults.TestResults)
            {
                if (!_context.VsTests.ContainsKey(result.TestCase.Id))
                {
                    var vsTestDescription = new VsTestDescription(result.TestCase);
                    _context.VsTests[result.TestCase.Id] = vsTestDescription;
                    _context.Tests.RegisterTest(vsTestDescription.Description);
                    _logger.LogWarning($"{RunnerId}: Initial test run encounter an unexpected test case ({vsTestDescription.Description.Name}), mutation tests may be inaccurate. Disable coverage analysis if you have doubts.",
                        RunnerId, result.TestCase.DisplayName);
                }

                _context.VsTests[result.TestCase.Id].RegisterInitialTestResult(result);
            }

            var totalCountOfTests = _context.GetTestsForSources(project.GetTestAssemblies()).Count;
            // get the test results, but prevent compression of 'all tests'
            return BuildTestRunResult(testResults, totalCountOfTests, totalCountOfTests,false);
        }

        public TestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update)
        {
            var mutantTestsMap = new Dictionary<int, ITestGuids>();
            var timeOutMs = timeoutCalc?.DefaultTimeout;

            var testCases = TestCases(mutants, mutantTestsMap);

            if (testCases?.Count == 0)
            {
                return new TestRunResult(_context.VsTests.Values, TestGuidsList.NoTest(), TestGuidsList.NoTest(), TestGuidsList.NoTest(), "Mutants are not covered by any test!", Enumerable.Empty<string>(), TimeSpan.Zero);
            }
            if (timeoutCalc != null && testCases != null)
            {
                // compute time out
                timeOutMs = timeoutCalc.CalculateTimeoutValue((int)testCases.Sum(id => _context.VsTests[id].InitialRunTime.TotalMilliseconds));
            }

            var numberTestCases = testCases?.Count ?? 0;
            var totalCountOfTests = _context.GetTestsForSources(project.GetTestAssemblies()).Count;
            var expectedTests = testCases == null ? totalCountOfTests : numberTestCases;

            void HandleUpdate(IRunResults handler)
            {
                var handlerTestResults = handler.TestResults;
                var tests = handlerTestResults.Select(p =>p.TestCase.Id).Distinct().Count()>= totalCountOfTests
                    ? (ITestGuids)TestGuidsList.EveryTest()
                    : new WrappedGuidsEnumeration(handlerTestResults.Select(t => t.TestCase.Id));
                var failedTest = new WrappedGuidsEnumeration(handlerTestResults.Where(tr => tr.Outcome == TestOutcome.Failed)
                    .Select(t => t.TestCase.Id));
                var timedOutTest = new WrappedGuidsEnumeration(handler.TestsInTimeout?.Select(t => t.Id));
                var remainingMutants = update?.Invoke(mutants, failedTest, tests, timedOutTest);

                if (remainingMutants != false
                    || handlerTestResults.Count >= expectedTests
                    || _cancelled
                    || _context.Options.OptimizationMode.HasFlag(OptimizationModes.DisableBail))
                {
                    return;
                }
                // all mutants status have been resolved, we can stop
                _logger.LogDebug($"{RunnerId}: Each mutant's fate has been established, we can stop.");
                _vsTestConsole.CancelTestRun();
                _cancelled = true;
            }

            if (timeOutMs.HasValue)
            {
                _logger.LogDebug($"{RunnerId}: Using {timeOutMs} ms as test run timeout");
            }

            var testResults = RunTestSession(new TestGuidsList(testCases), project, timeOutMs, mutantTestsMap, HandleUpdate);

            return BuildTestRunResult(testResults, expectedTests, totalCountOfTests);
        }

        private ICollection<Guid> TestCases(IReadOnlyList<Mutant> mutants, Dictionary<int, ITestGuids> mutantTestsMap)
        {
            ICollection<Guid> testCases;
            // if we optimize the number of tests to run
            if (_context.Options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
            {
                var needAll = false;
                foreach (var mutant in mutants)
                {
                    var tests = mutant.AssessingTests;
                    needAll = needAll || tests.IsEveryTest;
                    mutantTestsMap.Add(mutant.Id, tests);
                }

                testCases = needAll ? null : mutants.SelectMany(m => m.AssessingTests.GetGuids()).ToList();
                _logger.LogTrace($"{RunnerId}: Testing [{string.Join(',', mutants.Select(m => m.DisplayName))}] " +
                                 $"against {(testCases == null ? "all tests." : string.Join(", ", testCases))}.");
            }
            else
            {
                if (mutants.Count > 1)
                {
                    throw new GeneralStrykerException(
                        "Internal error: trying to test multiple mutants simultaneously without 'perTest' coverage analysis.");
                }

                mutantTestsMap.Add(mutants[0].Id, TestGuidsList.EveryTest());
                testCases = null;
            }

            return testCases;
        }

        private TestRunResult BuildTestRunResult(IRunResults testResults, int expectedTests, int totalCountOfTests, bool compressAll = true)
        {
            var resultAsArray = testResults.TestResults.ToArray();
            var testCases = resultAsArray.Select(t => t.TestCase.Id).ToHashSet();
            var ranTestsCount = testCases.Count;
            var timeout = !_cancelled && ranTestsCount < expectedTests;
            // ranTests is the list of test that have been executed. We detect the special case where all (existing and found) tests have been executed.
            // this is needed when the tests list is not stable (mutations can generate variation for theories) and also helps for performance
            // so we assume that if executed at least as much test as have been detected, it means all tests have been executed
            // EXCEPT when no test have been found. Otherwise, an empty test project would transform non covered mutants to survivors.
            var ranTests = compressAll && totalCountOfTests>0 && ranTestsCount >= totalCountOfTests ? (ITestGuids)TestGuidsList.EveryTest() : new WrappedGuidsEnumeration(testCases);
            var failedTests = resultAsArray.Where(tr => tr.Outcome == TestOutcome.Failed).Select(t => t.TestCase.Id);

            if (ranTests.IsEmpty && (testResults.TestsInTimeout == null || testResults.TestsInTimeout.Count == 0))
            {
                _logger.LogTrace($"{RunnerId}: Test session reports 0 result and 0 stuck test.");
            }

            var duration = TimeSpan.FromTicks(_context.VsTests.Values.Sum(t => t.InitialRunTime.Ticks));

            var message = string.Join(Environment.NewLine,
                resultAsArray.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage))
                    .Select(tr => $"{tr.DisplayName}{Environment.NewLine}{Environment.NewLine}{tr.ErrorMessage}"));
            var messages = resultAsArray.Select(tr => $"{tr.DisplayName}{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, tr.Messages.Select(tm => tm.Text))}");
            var failedTestsDescription = new WrappedGuidsEnumeration(failedTests);
            var timedOutTests = new WrappedGuidsEnumeration(testResults.TestsInTimeout?.Select(t => t.Id));
            return timeout
                ? TestRunResult.TimedOut(_context.VsTests.Values, ranTests, failedTestsDescription, timedOutTests, message, messages, duration)
                : new TestRunResult(_context.VsTests.Values, ranTests, failedTestsDescription, timedOutTests, message, messages, duration);
        }

        public IRunResults RunTestSession(ITestGuids testsToRun, IProjectAndTests project, int? timeout = null, Dictionary<int, ITestGuids> mutantTestsMap= null, Action<IRunResults> updateHandler = null) =>
            RunTestSession(testsToRun, project.GetTestAssemblies(),
                _context.GenerateRunSettings(timeout, false, mutantTestsMap, project.HelperNamespace, project.IsFullFramework), timeout, updateHandler).GetResults();

        public IRunResults RunCoverageSession(ITestGuids testsToRun, IReadOnlyCollection<string> testAssemblies, string nameSpace, bool isFullFramework) =>
            RunTestSession(testsToRun,  testAssemblies,
                _context.GenerateRunSettings(null,
        true,
                    null, nameSpace, isFullFramework)).GetRawResults();

        private RunEventHandler RunTestSession(ITestGuids tests,
            IEnumerable<string> sources,
            string runSettings,
            int? timeOut = null,
            Action<IRunResults> updateHandler = null)
        {

            if (tests.IsEveryTest && !_context.IsValidSourceList(sources))
            {
                _logger.LogWarning($"Test assembl{(sources.Count() <2 ? "y does" : "ies do")} not contain any test, skipping.");
                return new RunEventHandler(_context.VsTests, _logger, RunnerId);
            }

            for (var attempt = 0; attempt < MaxAttempts; attempt++)
            {
                var eventHandler = RunVsTest(tests, _context.GetValidSources(sources), runSettings, timeOut, updateHandler);
                if (eventHandler != null)
                {
                    return eventHandler;
                }
                _logger.LogWarning($"{RunnerId}: Retrying the test session.");
            }

            _logger.LogCritical($"{RunnerId}: VsTest failed, settings: {runSettings}");

            throw new GeneralStrykerException(
                $"{RunnerId}: failed to run a test session despite ${MaxAttempts} attempts. Aborting session.");
        }

        private RunEventHandler RunVsTest(ITestGuids tests, IEnumerable<string> sources, string runSettings, int? timeOut,
            Action<IRunResults> updateHandler)
        {
            var eventHandler = new RunEventHandler(_context.VsTests, _logger, RunnerId);
            var vsTestFailed = false;
            var sessionFailed = false;

            void HandlerVsTestFailed(object sender, EventArgs e)
            {
                sessionFailed = true;
            }

            void HandlerUpdate(object sender, EventArgs e)
            {
                updateHandler?.Invoke(eventHandler.GetResults());
            }

            var strykerVsTestHostLauncher = _context.BuildHostLauncher(RunnerId);

            eventHandler.VsTestFailed += HandlerVsTestFailed;
            eventHandler.ResultsUpdated += HandlerUpdate;

            _cancelled = false;
            var options = new TestPlatformOptions
            {
                TestCaseFilter = string.IsNullOrWhiteSpace(_context.Options.TestCaseFilter)
                    ? null
                    : _context.Options.TestCaseFilter
            };
            Task session;
            if (tests.IsEveryTest)
            {
                session = _vsTestConsole.RunTestsWithCustomTestHostAsync(sources, runSettings, options, eventHandler,
                    strykerVsTestHostLauncher);
            }
            else
            {
                session = _vsTestConsole.RunTestsWithCustomTestHostAsync(tests.GetGuids().Select(t => _context.VsTests[t].Case),
                    runSettings, options, eventHandler, strykerVsTestHostLauncher);
            }
            
            if (timeOut.HasValue)
            {
                // we wait for the end notification for the test session
                // ==> if it failed, results are uncertain
                sessionFailed = !eventHandler.Wait(VsTestExtraTimeOutInMs + timeOut.Value);
                // we wait for vsTestProcess to stop
                // ==> if it appears hung, we recycle it.
                vsTestFailed = !session.Wait(VsTestExtraTimeOutInMs);
                // VsTestHost appears stuck
                // VsTestWrapper aborts the current test sessions on timeout, except on critical error, so we have an internal timeout (+ grace period)
                // to detect and properly handle those events. 
                if (sessionFailed)
                {
                    _logger.LogError(
                        $"{RunnerId}: VsTest did not report the end of test session in due time ({timeOut} ms), it may be frozen.");
                    _logger.LogError($"{RunnerId}: ran {eventHandler.GetResults().TestResults.Count} tests.");
                    // workaround VsTest issue #4527
                    if (_cancelled && sources.Count() > 1)
                    {
                        // we assume VsTest did not properly cancelled the test
                        _logger.LogError($"{RunnerId}: ignoring the error as it looks like a known VsTest issue.");
                        sessionFailed = false;
                        vsTestFailed = false;
                    }
                }
            }
            else
            {
                // no timeout provided ==> initial tests
                // we wait for VsTest to end.
                // we could add a configurable timeout, to prevent actual locking to happen during initial tests, but no idea what a good default should be
                session.Wait();
                // we add a grace delay for notifications to be propagated
                eventHandler.Wait(VsTestExtraTimeOutInMs);
            }
            
            eventHandler.ResultsUpdated -= HandlerUpdate;
            eventHandler.VsTestFailed -= HandlerVsTestFailed;

            if (vsTestFailed || sessionFailed)
            {
                // if the session did not end properly for any level (notifications or process), we recycle the session as a precaution
                PrepareVsTestConsole();
            }
            return sessionFailed ? null : eventHandler;
        }

        private void PrepareVsTestConsole()
        {
            if (_vsTestConsole != null)
            {
                _instanceCount++;
                try
                {
                    _vsTestConsole.EndSession();
                }
                catch { /*Ignore exception. vsTestConsole has been disposed outside of our control*/ }
            }

            _vsTestConsole = _context.BuildVsTestWrapper($"{RunnerId}-{_instanceCount}");
        }

        #region IDisposable Support

        private void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    _vsTestConsole.EndSession();
                }
                catch (Exception e)
                {
                    _logger.LogError($"Exception when disposing {RunnerId}: {0}", e);
                }
            }

            _disposedValue = true;
        }

        ~VsTestRunner() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
