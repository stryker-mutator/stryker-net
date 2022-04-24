using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using NuGet.Frameworks;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.DataCollector;

namespace Stryker.Core.TestRunners.VsTest
{
    public sealed class VsTestRunner
    {
        private IVsTestConsoleWrapper _vsTestConsole;
        private readonly Func<string, IStrykerTestHostLauncher> _hostBuilder;
        private bool _disposedValue; // To detect redundant calls
        private bool _vsTestFailed;
        private bool _aborted;
        private readonly VsTestContextInformation _context;
        private readonly int _id;
        private readonly ILogger _logger;

        private string RunnerId => $"Runner {_id}";

        public VsTestRunner(VsTestContextInformation context,
            int id,
            ILogger logger = null,
            IVsTestConsoleWrapper wrapper = null,
            Func<string, IStrykerTestHostLauncher> hostBuilder = null)
        {
            _hostBuilder = hostBuilder ?? (xId => new StrykerVsTestHostLauncher(xId));
            _context = context;
            _id = id;
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunner>();
            _vsTestConsole = wrapper;
            if (_vsTestConsole == null)
            {
                PrepareVsTestConsole(context.Options);
            }
        }

        public TestRunResult InitialTest()
        {
            var testResults = RunTestSession(null, true, _context.GenerateRunSettings(null, false, new Dictionary<int, ITestGuids>()));
            // initial test run, register test results
            foreach (var result in testResults.TestResults)
            {
                if (!_context.VsTests.ContainsKey(result.TestCase.Id))
                {
                    _context.VsTests[result.TestCase.Id] = new VsTestDescription(result.TestCase);
                    _logger.LogWarning("{RunnerId}: Initial test run encounter a unexpected test case ({TestCaseDisplayName}), mutation tests may be inaccurate. Disable coverage analysis if your have doubts.",
                        RunnerId, result.TestCase.DisplayName);
                }

                _context.VsTests[result.TestCase.Id].RegisterInitialTestResult(result);
            }

            // get the test results, but prevent compression of 'all tests'
            return BuildTestRunResult(testResults, int.MaxValue, false);
        }

        public TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants, TestUpdateHandler update)
        {
            var mutantTestsMap = new Dictionary<int, ITestGuids>();
            var needAll = true;
            ICollection<Guid> testCases;
            var timeOutMs = timeoutCalc?.DefaultTimeout;

            if (mutants != null)
            {
                // if we optimize the number of tests to run
                if (_context.Options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
                {
                    needAll = false;
                    foreach (var mutant in mutants)
                    {
                        ITestGuids tests;
                        if ((mutant.IsStaticValue && !_context.Options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest)) || mutant.MustRunAgainstAllTests)
                        {
                            tests = TestsGuidList.EveryTest();
                            needAll = true;
                        }
                        else
                        {
                            tests = mutant.CoveringTests;
                        }
                        mutantTestsMap.Add(mutant.Id, tests);
                    }

                    testCases = needAll ? null : mutants.SelectMany(m => m.CoveringTests.GetGuids()).ToList();

                    _logger.LogTrace($"{RunnerId}: Testing [{string.Join(',', mutants.Select(m => m.DisplayName))}] " +
                                     $"against {(testCases == null ? "all tests." : string.Join(", ", testCases))}.");
                    if (testCases?.Count == 0)
                    {
                        return new TestRunResult(TestsGuidList.NoTest(), TestsGuidList.NoTest(), TestsGuidList.NoTest(), "Mutants are not covered by any test!", TimeSpan.Zero);
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
                    mutantTestsMap.Add(mutants[0].Id, TestsGuidList.EveryTest());
                    testCases = null;
                }
            }
            else
            {
                testCases = null;
            }

            var numberTestCases = testCases?.Count ?? 0;
            var expectedTests = needAll ? _context.Tests.Count : numberTestCases;

            void HandleUpdate(IRunResults handler)
            {
                var handlerTestResults = handler.TestResults;
                if (mutants == null)
                {
                    return;
                }
                var tests = handlerTestResults.Count == _context.Tests.Count
                    ? (ITestGuids)TestsGuidList.EveryTest()
                    : new WrappedGuidsEnumeration(handlerTestResults.Select(t => t.TestCase.Id));
                var failedTest = new WrappedGuidsEnumeration(handlerTestResults.Where(tr => tr.Outcome == TestOutcome.Failed)
                    .Select(t => t.TestCase.Id));
                var timedOutTest = new WrappedGuidsEnumeration(handler.TestsInTimeout?.Select(t => t.Id));
                var remainingMutants = update?.Invoke(mutants, failedTest, tests, timedOutTest);
                if (handlerTestResults.Count >= expectedTests || remainingMutants != false || _aborted)
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

            var testResults = RunTestSession(new TestsGuidList(testCases), needAll,
                _context.GenerateRunSettings(timeOutMs, false, mutantTestsMap), timeOutMs, HandleUpdate);

            return BuildTestRunResult(testResults, expectedTests);
        }

        private TestRunResult BuildTestRunResult(IRunResults testResults, int expectedTests, bool compressAll = true)
        {
            var resultAsArray = testResults.TestResults.ToArray();
            var testCases = resultAsArray.Select(t => t.TestCase.Id).Distinct();
            var ranTestsCount = testCases.Count();
            var timeout = !_aborted && ranTestsCount < expectedTests;
            var ranTests = (compressAll && ranTestsCount >= _context.Tests.Count) ? (ITestGuids)TestsGuidList.EveryTest() : new WrappedGuidsEnumeration(testCases);
            var failedTests = resultAsArray.Where(tr => tr.Outcome == TestOutcome.Failed).Select(t => t.TestCase.Id);

            if (ranTests.IsEmpty && (testResults.TestsInTimeout == null || testResults.TestsInTimeout.Count == 0))
            {
                _logger.LogTrace($"{RunnerId}: Initial Test session reports 0 result and 0 stuck tests.");
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

        public TestRunResult CaptureCoverage(TestsGuidList normalTests, IEnumerable<Mutant> mutants)
        {
            _logger.LogDebug($"{RunnerId}: Capturing coverage.");
            if (_context.CantUseStrykerDataCollector())
            {
                _logger.LogDebug($"{RunnerId}: project does not support StrykerDataCollector. Coverage data is simulated. Upgrade test proj to NetCore 2.0+");
                // can't capture coverage info
                foreach (var mutant in mutants)
                {
                    mutant.CoveringTests = TestsGuidList.EveryTest();
                }
            }
            else
            {
                var testResults = RunTestSession(normalTests , normalTests.IsEveryTest, _context.GenerateRunSettings(null, true, null));
                
                ParseResultsForCoverage(testResults.TestResults, mutants);
            }
            return new TestRunResult(true);
        }

        private void ParseResultsForCoverage(IEnumerable<TestResult> testResults, IEnumerable<Mutant> mutants)
        {
            var seenTestCases = new HashSet<Guid>();
            var dynamicTestCases = new HashSet<Guid>();
            var maxMutantId = mutants.Any() ? mutants.Max(m => m.Id) + 1 : 0;
            var map = new List<ICollection<TestDescription>>(maxMutantId);
            var staticMutantLists = new HashSet<int>();
            // initialize the map
            for (var i = 0; i < maxMutantId; i++)
            {
                map.Add(new List<TestDescription>());
            }
            foreach (var testResult in testResults)
            {
                var (key, value) = testResult.GetProperties().FirstOrDefault(x => x.Key.Id == CoverageCollector.PropertyName);
                if (!_context.VsTests.ContainsKey(testResult.TestCase.Id))
                {
                    _logger.LogWarning($"{RunnerId}: Coverage analysis run encountered a unexpected test case ({testResult.TestCase.DisplayName}), mutation tests may be inaccurate. Disable coverage analysis if you have doubts.");
                    _context.VsTests.Add(testResult.TestCase.Id, new VsTestDescription(testResult.TestCase));
                }
                var testDescription = _context.VsTests[testResult.TestCase.Id];
                if (key == null)
                {
                    // the coverage collector did not report anything for this test ==> it has not been tracked by it, so we do not have coverage data
                    // ==> we need it to use this test against every mutation
                    if (!seenTestCases.Contains(testResult.TestCase.Id) ||
                        dynamicTestCases.Contains(testResult.TestCase.Id))
                    {
                        continue;
                    }

                    dynamicTestCases.Add(testDescription.Id);
                    // assume the test (may) cover every mutation
                    foreach (var entry in map)
                    {
                        entry.Add(testDescription.Description);
                    }
                    _logger.LogWarning($"{RunnerId}: Each mutant will be tested against {testResult.TestCase.DisplayName}), because we can't get coverage info for test case generated at run time");
                }
                else if (value != null)
                {
                    // we have coverage data
                    seenTestCases.Add(testDescription.Id);

                    var propertyPairValue = value as string;
                    if (string.IsNullOrWhiteSpace(propertyPairValue))
                    {
                        _logger.LogDebug($"{RunnerId}: Test {testResult.TestCase.DisplayName} does not cover any mutation.");
                    }
                    else
                    {
                        var parts = propertyPairValue.Split(';');
                        var coveredMutants = string.IsNullOrEmpty(parts[0])
                            ? Enumerable.Empty<int>()
                            : parts[0].Split(',').Select(int.Parse);
                        // we identify mutants that are part of static code, unless we performed pertest capture
                        var staticMutants = (parts.Length == 1 || string.IsNullOrEmpty(parts[1]) || _context.Options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest))
                            ? Enumerable.Empty<int>()
                            : parts[1].Split(',').Select(int.Parse);

                        foreach (var id in coveredMutants)
                        {
                            map[id].Add(testDescription.Description);
                        }

                        staticMutantLists.UnionWith(staticMutants);
                    }
                    // look for suspicious mutants
                    var (testProperty, mutantOutsideTests) = testResult.GetProperties()
                        .FirstOrDefault(x => x.Key.Id == CoverageCollector.OutOfTestsPropertyName);
                    if (testProperty == null)
                    {
                        continue;
                    }
                    // we have some mutations that appeared outside any test, probably some run time test case generation, or some async logic.
                    propertyPairValue = (mutantOutsideTests as string);
                    var suspiciousMutants = string.IsNullOrEmpty(propertyPairValue)
                        ? Enumerable.Empty<int>()
                        : propertyPairValue.Split(',').Select(int.Parse);
                    _logger.LogWarning($"{RunnerId}: Some mutations were executed outside any test (mutation ids: {propertyPairValue}).");
                    staticMutantLists.UnionWith(suspiciousMutants);
                }
            }

            // push coverage data to the mutants
            foreach (var mutant in mutants)
            {
                mutant.CoveringTests= mutant.CoveringTests.Merge(new TestsGuidList(map[mutant.Id]));
                if (staticMutantLists.Contains(mutant.Id))
                {
                    mutant.IsStaticValue = true;
                }
            }
        }

        public void CoverageForOneTest(Guid test, IEnumerable<Mutant> mutants)
        {
            _logger.LogDebug("{RunnerId}: Capturing coverage for {TestCaseFullyQualifiedName}.", RunnerId, _context.VsTests[test].Case.FullyQualifiedName);
            var testResults = RunTestSession(new TestsGuidList(test), false, _context.GenerateRunSettings(null, true, null));
            ParseResultsForCoverage(testResults.TestResults.Where(x => x.TestCase.Id == test), mutants);
        }

        private IRunResults RunTestSession(ITestGuids tests,
            bool runAllTests,
            string runSettings,
            int? timeOut = null,
            Action<RunEventHandler> updateHandler = null,
            int retries = 0)
        {
            using var eventHandler = new RunEventHandler(_context.VsTests, _logger, RunnerId);
            void HandlerVsTestFailed(object sender, EventArgs e)
            {
                _vsTestFailed = true;
            }

            void HandlerUpdate(object sender, EventArgs e)
            {
                updateHandler?.Invoke(eventHandler);
            }

            var strykerVsTestHostLauncher = _hostBuilder(RunnerId);

            eventHandler.VsTestFailed += HandlerVsTestFailed;
            eventHandler.ResultsUpdated += HandlerUpdate;

            _aborted = false;
            var options = new TestPlatformOptions { TestCaseFilter = _context.Options.TestCaseFilter };
            if (runAllTests)
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
                _logger.LogWarning($"{RunnerId}: VsTest did not report the end of test session in due time, it may have hang. Retrying");
                _vsTestConsole.AbortTestRun();
                _vsTestFailed = true;
            }

            if (!strykerVsTestHostLauncher.IsProcessCreated)
            {
                throw new GeneralStrykerException("*** Failed to create a TestRunner, Stryker cannot recover from this!***");
            }

            eventHandler.ResultsUpdated -= HandlerUpdate;
            eventHandler.VsTestFailed -= HandlerVsTestFailed;

            if (!_vsTestFailed || retries > 5)
            {
                return eventHandler;
            }
            PrepareVsTestConsole(_context.Options);
            _vsTestFailed = false;

            return RunTestSession(tests, runAllTests, runSettings, timeOut, updateHandler, retries + 1);
        }

        private void PrepareVsTestConsole(StrykerOptions options)
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
