using System;
using System.Collections.Generic;
using System.Linq;
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
        private bool _vsTestFailed;
        private bool _aborted;
        private readonly VsTestContextInformation _context;
        private readonly int _id;
        private readonly ILogger _logger;

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

        public TestRunResult InitialTest()
        {
            var testResults = RunTestSession(TestGuidsList.EveryTest());
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

            // get the test results, but prevent compression of 'all tests'
            return BuildTestRunResult(testResults, _context.Tests.Count, false);
        }

        public TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update)
        {
            var mutantTestsMap = new Dictionary<int, ITestGuids>();
            var needAll = true;
            ICollection<Guid> testCases;
            var timeOutMs = timeoutCalc?.DefaultTimeout;

            // if we optimize the number of tests to run
            if (_context.Options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
            {
                needAll = false;
                foreach (var mutant in mutants)
                {
                    var tests = mutant.AssessingTests;
                    needAll =  needAll || tests.IsEveryTest;
                    mutantTestsMap.Add(mutant.Id, tests);
                }

                testCases = needAll ? null : mutants.SelectMany(m => m.AssessingTests.GetGuids()).ToList();

                _logger.LogTrace($"{RunnerId}: Testing [{string.Join(',', mutants.Select(m => m.DisplayName))}] " +
                                 $"against {(testCases == null ? "all tests." : string.Join(", ", testCases))}.");
                if (testCases?.Count == 0)
                {
                    return new TestRunResult(TestGuidsList.NoTest(), TestGuidsList.NoTest(), TestGuidsList.NoTest(), "Mutants are not covered by any test!", TimeSpan.Zero);
                }

                if (timeoutCalc != null && testCases != null)
                {
                    // compute time out
                    timeOutMs = timeoutCalc.CalculateTimeoutValue((int)testCases.Sum(id => _context.VsTests[id].InitialRunTime.TotalMilliseconds));
                }
            }
            else
            {
                if (mutants.Count > 1)
                {
                    throw new GeneralStrykerException("Internal error: trying to test multiple mutants simultaneously without 'perTest' coverage analysis.");
                }
                mutantTestsMap.Add(mutants[0].Id, TestGuidsList.EveryTest());
                testCases = null;
            }

            var numberTestCases = testCases?.Count ?? 0;
            var expectedTests = needAll ? _context.Tests.Count : numberTestCases;

            void HandleUpdate(IRunResults handler)
            {
                var handlerTestResults = handler.TestResults;
                var tests = handlerTestResults.Count == _context.Tests.Count
                    ? (ITestGuids)TestGuidsList.EveryTest()
                    : new WrappedGuidsEnumeration(handlerTestResults.Select(t => t.TestCase.Id));
                var failedTest = new WrappedGuidsEnumeration(handlerTestResults.Where(tr => tr.Outcome == TestOutcome.Failed)
                    .Select(t => t.TestCase.Id));
                var timedOutTest = new WrappedGuidsEnumeration(handler.TestsInTimeout?.Select(t => t.Id));
                var remainingMutants = update?.Invoke(mutants, failedTest, tests, timedOutTest);

                if (remainingMutants != false
                    || handlerTestResults.Count >= expectedTests
                    || _aborted
                    || _context.Options.OptimizationMode.HasFlag(OptimizationModes.DisableBail))
                {
                    return;
                }
                // all mutants status have been resolved, we can stop
                _logger.LogDebug($"{RunnerId}: Each mutant's fate has been established, we can stop.");
                _vsTestConsole.CancelTestRun();
                _aborted = true;
            }

            if (timeOutMs.HasValue)
            {
                _logger.LogDebug($"{RunnerId}: Using {timeOutMs} ms as test run timeout");
            }

            var testResults = RunTestSession(new TestGuidsList(testCases), timeOutMs, mutantTestsMap, HandleUpdate);

            return BuildTestRunResult(testResults, expectedTests);
        }

        private TestRunResult BuildTestRunResult(IRunResults testResults, int expectedTests, bool compressAll = true)
        {
            var resultAsArray = testResults.TestResults.ToArray();
            var testCases = resultAsArray.Select(t => t.TestCase.Id).ToHashSet();
            var ranTestsCount = testCases.Count;
            var timeout = !_aborted && ranTestsCount < expectedTests;
            var ranTests = compressAll && ranTestsCount >= _context.Tests.Count ? (ITestGuids)TestGuidsList.EveryTest() : new WrappedGuidsEnumeration(testCases);
            var failedTests = resultAsArray.Where(tr => tr.Outcome == TestOutcome.Failed).Select(t => t.TestCase.Id);

            if (ranTests.IsEmpty && (testResults.TestsInTimeout == null || testResults.TestsInTimeout.Count == 0))
            {
                _logger.LogTrace($"{RunnerId}: Test session reports 0 result and 0 stuck tests.");
            }

            var duration =  TimeSpan.FromTicks(_context.VsTests.Values.Sum(t => t.InitialRunTime.Ticks));

            var message = string.Join(Environment.NewLine,
                resultAsArray.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage))
                    .Select(tr => $"{tr.DisplayName}{Environment.NewLine}{Environment.NewLine}{tr.ErrorMessage}"));
            var failedTestsDescription = new WrappedGuidsEnumeration(failedTests);
            var timedOutTests = new WrappedGuidsEnumeration(testResults.TestsInTimeout?.Select(t => t.Id));
            return timeout
                ? TestRunResult.TimedOut(ranTests, failedTestsDescription, timedOutTests, message, duration)
                : new TestRunResult(ranTests, failedTestsDescription, timedOutTests, message, duration);
        }

        public IRunResults RunTestSession(ITestGuids testsToRun, int? timeout = null, Dictionary<int, ITestGuids> mutantTestsMap= null, Action<IRunResults> updateHandler = null) =>
            RunTestSession(testsToRun,
                _context.GenerateRunSettings(timeout, false, mutantTestsMap), timeout, updateHandler).GetResults();

        public IRunResults RunCoverageSession(ITestGuids testsToRun) =>
            RunTestSession(testsToRun,
                _context.GenerateRunSettings(null,
                    true,
                    null)).GetRawResults();

        private RunEventHandler RunTestSession(ITestGuids tests,
            string runSettings,
            int? timeOut = null,
            Action<IRunResults> updateHandler = null,
            int retries = 0)
        {
            var eventHandler = new RunEventHandler(_context.VsTests, _logger, RunnerId);
            void HandlerVsTestFailed(object sender, EventArgs e)
            {
                _vsTestFailed = true;
            }

            void HandlerUpdate(object sender, EventArgs e)
            {
                updateHandler?.Invoke(eventHandler.GetResults());
            }

            var strykerVsTestHostLauncher = _context.BuildHostLauncher(RunnerId);

            eventHandler.VsTestFailed += HandlerVsTestFailed;
            eventHandler.ResultsUpdated += HandlerUpdate;

            _aborted = false;
            var options = new TestPlatformOptions { TestCaseFilter = _context.Options.TestCaseFilter };
            if (tests.IsEveryTest)
            {
                _vsTestConsole.RunTestsWithCustomTestHostAsync(_context.TestSources, runSettings, options, eventHandler,
                    strykerVsTestHostLauncher);
            }
            else
            {
                _vsTestConsole.RunTestsWithCustomTestHostAsync(tests.GetGuids().Select(t => _context.VsTests[t].Case), runSettings,
                    options, eventHandler, strykerVsTestHostLauncher);
            }

            // Wait for test completed report
            if (!eventHandler.WaitEnd(timeOut))
            {
                _logger.LogWarning($"{RunnerId}: VsTest did not report the end of test session in due time, it may have hang. Retrying!");
                _vsTestConsole.AbortTestRun();
                _vsTestFailed = true;
            }

            if (!strykerVsTestHostLauncher.IsProcessCreated)
            {
                throw new GeneralStrykerException("*** Failed to create a TestRunner, Stryker cannot recover from this! ***");
            }

            eventHandler.ResultsUpdated -= HandlerUpdate;
            eventHandler.VsTestFailed -= HandlerVsTestFailed;

            if (!_vsTestFailed || retries > 5)
            {
                return eventHandler;
            }

            PrepareVsTestConsole();
            _vsTestFailed = false;

            return RunTestSession(tests, runSettings, timeOut, updateHandler, retries + 1);
        }

        private void PrepareVsTestConsole()
        {
            if (_vsTestConsole != null)
            {
                try
                {
                    _vsTestConsole.EndSession();
                }
                catch { /*Ignore exception. vsTestConsole has been disposed outside of our control*/ }
            }

            _vsTestConsole = _context.BuildVsTestWrapper(RunnerId);
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
