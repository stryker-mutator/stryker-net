using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using Stryker.Utilities.Buildalyzer;
using Stryker.Utilities.Logging;
using static Stryker.Abstractions.Testing.ITestRunner;

namespace Stryker.TestRunner.VsTest;

public sealed class VsTestRunner : IDisposable
{
    private IVsTestConsoleWrapper _vsTestConsole;
    private bool _disposedValue; // To detect redundant calls
    private bool _currentSessionCancelled;
    private readonly VsTestContextInformation _context;
    private readonly int _id;
    private int _instanceCount;
    private readonly ILogger _logger;
    // safety timeout for VsTestWrapper operations. We assume VsTest crashed if the timeout triggers

    public static int VsTestExtraTimeOutInMs { get; set; } = 10 * 1000;

    // maximum number of attempts for VsTest sessions, just in case we run into some of VsTest quirks
    private const int MaxAttempts = 3;

    private string RunnerId => $"Runner {_id}";
    private string ControlVariableName => $"ACTIVE_MUTATION_{_id}";

    public VsTestRunner(VsTestContextInformation context,
        int id,
        ILogger logger = null)
    {
        _context = context;
        _id = id;
        _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunner>();
        _vsTestConsole = _context.BuildVsTestWrapper(RunnerId, ControlVariableName);
    }

    public TestRunResult InitialTest(IProjectAndTests project)
    {
        var testResults = RunTestSession(TestIdentifierList.EveryTest(), project);
        foreach (var test in _context.VsTests.Keys)
        {
            _context.VsTests[test].ClearInitialResult();
        }

        // initial test run, register test results
        foreach (var result in testResults.TestResults)
        {
            if (!_context.VsTests.ContainsKey(result.TestCase.Id))
            {
                var vsTestDescription = new VsTestDescription(new VsTestCase(result.TestCase));
                _context.VsTests[result.TestCase.Id] = vsTestDescription;
                _context.RegisterDiscoveredTest(vsTestDescription);
                _logger.LogWarning(
                    "{RunnerId}: Initial test run encounter an unexpected test case ({DisplayName}), mutation tests may be inaccurate. Disable coverage analysis if you have doubts.",
                RunnerId, result.TestCase.DisplayName);
            }

            _context.VsTests[result.TestCase.Id].RegisterInitialTestResult(new VsTestResult(result));
        }

        var totalCountOfTests = _context.GetTestsForSources(project.GetTestAssemblies()).Count;
        // get the test results, but prevent compression of 'all tests'
        return BuildTestRunResult(testResults, totalCountOfTests, totalCountOfTests, false);
    }

    public TestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, IReadOnlyList<IMutant> mutants, TestUpdateHandler update)
    {
        var mutantTestsMap = new Dictionary<int, ITestIdentifiers>();

        var testCases = TestCases(mutants, mutantTestsMap);

        if (testCases?.Count == 0)
        {
            return new TestRunResult(_context.VsTests.Values, TestIdentifierList.NoTest(), TestIdentifierList.NoTest(),
                TestIdentifierList.NoTest(), "Mutants are not covered by any test!", Enumerable.Empty<string>(),
                TimeSpan.Zero);
        }

        var totalCountOfTests = _context.GetTestsForSources(project.GetTestAssemblies()).Count;
        var expectedTests = testCases?.Count ?? totalCountOfTests;

        var timeOutMs = timeoutCalc?.DefaultTimeout;
        if (timeoutCalc != null && testCases != null)
        {
            // compute time out
            timeOutMs = timeoutCalc.CalculateTimeoutValue((int)testCases.Sum(id => _context.VsTests[Guid.Parse(id)].InitialRunTime.TotalMilliseconds));
        }

        if (timeOutMs.HasValue)
        {
            _logger.LogDebug("{RunnerId}: Using {timeOutMs} ms as test run timeout", RunnerId, timeOutMs);
        }

        var testResults = RunTestSession(new TestIdentifierList(testCases), project, timeOutMs, mutantTestsMap, HandleUpdate);

        return BuildTestRunResult(testResults, expectedTests, totalCountOfTests);

        void HandleUpdate(IRunResults handler)
        {
            var handlerTestResults = handler.TestResults;
            var tests = handlerTestResults.Select(p => p.TestCase.Id).Distinct().Count() >= totalCountOfTests
                ? TestIdentifierList.EveryTest()
                : new WrappedIdentifierEnumeration(handlerTestResults.Select(t => t.TestCase.Id.ToString()));
            var failedTest = new WrappedIdentifierEnumeration(handlerTestResults
                .Where(tr => tr.Outcome == TestOutcome.Failed)
                .Select(t => t.TestCase.Id.ToString()));
            var timedOutTest = new WrappedIdentifierEnumeration(handler.TestsInTimeout?.Select(t => t.Id.ToString()));
            var remainingMutants = update?.Invoke(mutants, failedTest, tests, timedOutTest);

            if (remainingMutants != false
                || handlerTestResults.Count >= expectedTests
                || _currentSessionCancelled
                || _context.Options.OptimizationMode.HasFlag(OptimizationModes.DisableBail))
            {
                return;
            }

            // all mutants status have been resolved, we can stop
            _logger.LogDebug("{RunnerId}: Each mutant's fate has been established, we can stop.", RunnerId);
            try
            {
                _vsTestConsole.CancelTestRun();
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex, "Error while cancelling VsTest session.");
                // recycle the session
                PrepareVsTestConsole();
            }
            _currentSessionCancelled = true;
        }
    }

    private ICollection<string> TestCases(IReadOnlyList<IMutant> mutants, Dictionary<int, ITestIdentifiers> mutantTestsMap)
    {
        ICollection<string> testCases;
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

            testCases = needAll ? null : mutants.SelectMany(m => m.AssessingTests.GetIdentifiers()).ToList();
            _logger.LogDebug("{RunnerId}: Testing [{Mutants}]", RunnerId,
                string.Join(',', mutants.Select(m => m.DisplayName)));
            _logger.LogTrace(
                "{RunnerId}: against {TestCases}.", RunnerId,
                testCases == null ? "all tests." : string.Join(", ", testCases));
        }
        else
        {
            if (mutants.Count > 1)
            {
                throw new GeneralStrykerException(
                    "Internal error: trying to test multiple mutants simultaneously without 'perTest' coverage analysis.");
            }

            mutantTestsMap.Add(mutants[0].Id, TestIdentifierList.EveryTest());
            testCases = null;
        }

        return testCases;
    }

    private TestRunResult BuildTestRunResult(IRunResults testResults, int expectedTests, int totalCountOfTests,
            bool compressAll = true)
    {
        var resultAsArray = testResults.TestResults.ToArray();
        var testCases = resultAsArray.Select(t => t.TestCase.Id.ToString()).ToHashSet();
        var ranTestsCount = testCases.Count;
        var timeout = !_currentSessionCancelled && ranTestsCount < expectedTests;
        // ranTests is the list of test that have been executed. We detect the special case where all (existing and found) tests have been executed.
        // this is needed when the tests list is not stable (mutations can generate variation for theories) and also helps for performance
        // so we assume that if executed at least as much test as have been detected, it means all tests have been executed
        // EXCEPT when no test have been found. Otherwise, an empty test project would transform non-covered mutants to survivors.
        var ranTests = compressAll && totalCountOfTests > 0 && ranTestsCount >= totalCountOfTests
            ? TestIdentifierList.EveryTest()
            : new WrappedIdentifierEnumeration(testCases);
        var failedTests = resultAsArray.Where(tr => tr.Outcome == TestOutcome.Failed).Select(t => t.TestCase.Id.ToString());

        if (ranTests.IsEmpty && (testResults.TestsInTimeout == null || testResults.TestsInTimeout.Count == 0))
        {
            _logger.LogTrace("{RunnerId}: Test session reports 0 result and 0 stuck test.", RunnerId);
        }

        var duration = TimeSpan.FromTicks(_context.VsTests.Values.Sum(t => t.InitialRunTime.Ticks));

        var errorMessages = string.Join(Environment.NewLine,
            resultAsArray.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage))
                .Select(tr => $"{tr.DisplayName}{Environment.NewLine}{Environment.NewLine}{tr.ErrorMessage}"));
        var messages = resultAsArray.Select(tr =>
                $"{tr.DisplayName}{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, tr.Messages.Select(tm => tm.Text))}");
        var failedTestsDescription = new WrappedIdentifierEnumeration(failedTests);
        var timedOutTests = new WrappedIdentifierEnumeration(testResults.TestsInTimeout?.Select(t => t.Id.ToString()));
        return timeout
                ? TestRunResult.TimedOut(_context.VsTests.Values, ranTests, failedTestsDescription, timedOutTests,
                    errorMessages, messages, duration)
                : new TestRunResult(_context.VsTests.Values, ranTests, failedTestsDescription, timedOutTests, errorMessages,
                    messages, duration);
    }

    public IRunResults RunTestSession(ITestIdentifiers testsToRun, IProjectAndTests project, int? timeout = null, Dictionary<int, ITestIdentifiers> mutantTestsMap = null, Action<IRunResults> updateHandler = null) =>
        RunTestSession(testsToRun, project, false, timeout, updateHandler, mutantTestsMap).normal;

    public IRunResults RunCoverageSession(ITestIdentifiers testsToRun, IProjectAndTests project) =>
        RunTestSession(testsToRun, project, true).raw;

    private (IRunResults normal, IRunResults raw) RunTestSession(ITestIdentifiers tests, IProjectAndTests projectAndTests,
        bool forCoverage, int? timeOut = null, Action<IRunResults> updateHandler = null,
        Dictionary<int, ITestIdentifiers> mutantTestsMap = null)
    {
        var sources = projectAndTests.GetTestAssemblies();
        var validSources = _context.GetValidSources(sources).ToList();
        if (tests.IsEveryTest && validSources.Count == 0)
        {
            _logger.LogWarning(
                $"{RunnerId}: Test assembl{(sources.Count() < 2 ? "y does" : "ies do")} not contain any test, skipping.");
            return (new SimpleRunResults(), new SimpleRunResults());
        }

        var runEventHandler = new RunEventHandler(_context.VsTests, _logger, RunnerId);

        var options = new TestPlatformOptions
        {
            TestCaseFilter = string.IsNullOrWhiteSpace(_context.Options.TestCaseFilter)
                ? null
                : _context.Options.TestCaseFilter
        };

        runEventHandler.ResultsUpdated += HandlerUpdate;
        // work around VsTest issues when using multiple test assemblies
        foreach (var source in projectAndTests.TestProjectsInfo.AnalyzerResults)
        {
            var testForSource = _context.TestsPerSource[source.GetAssemblyPath()];
            var testsForAssembly = new TestIdentifierList(tests.GetIdentifiers().Where(id => testForSource.Contains(Guid.Parse(id))));
            if (!tests.IsEveryTest && testsForAssembly.Count == 0)
            {
                // skip empty assemblies
                continue;
            }
            var runSettings = _context.GenerateRunSettings(timeOut, forCoverage, mutantTestsMap,
                projectAndTests.HelperNamespace, source.TargetFramework, source.TargetPlatform());
            _logger.LogTrace("{RunnerId}: testing assembly {source}.", RunnerId, source);
            var activeId = -1;
            if (mutantTestsMap is { Count: 1 })
            {
                activeId = mutantTestsMap.Keys.First();
            }
            Environment.SetEnvironmentVariable(ControlVariableName, activeId.ToString());
            RunVsTest(tests, source.GetAssemblyPath(), runSettings, options, timeOut, runEventHandler);

            if (_currentSessionCancelled)
            {
                break;
            }
        }

        runEventHandler.ResultsUpdated -= HandlerUpdate;
        return (runEventHandler.GetResults(), runEventHandler.GetRawResults());

        void HandlerUpdate(object sender, EventArgs e)
        {
            updateHandler?.Invoke(runEventHandler.GetResults());
        }
    }

    private void RunVsTest(ITestIdentifiers tests, string source, string runSettings, TestPlatformOptions options,
            int? timeOut, RunEventHandler eventHandler)
    {
        var attempt = 0;
        while (attempt < MaxAttempts)
        {
            var strykerVsTestHostLauncher = _context.BuildHostLauncher(RunnerId);

            eventHandler.StartSession();
            _currentSessionCancelled = false;
            var session = Task.Run(() =>
                {
                    if (tests.IsEveryTest)
                    {
                        _vsTestConsole.RunTestsWithCustomTestHost([source], runSettings, options, eventHandler,
                            strykerVsTestHostLauncher);
                    }
                    else
                    {
                        var actualTestCases = tests.GetIdentifiers().Select(id =>
                        {
                            var testCase = (VsTestCase)_context.VsTests[Guid.Parse(id)].Case;
                            return testCase.OriginalTestCase;
                        });
                        var testCases = actualTestCases;
                        _vsTestConsole.RunTestsWithCustomTestHost(
                            testCases,
                            runSettings, options, eventHandler, strykerVsTestHostLauncher);
                    }
                });

            if (WaitForEnd(session, eventHandler, timeOut, ref attempt))
            {
                // if the session did not end properly, we recycle the session as a precaution
                PrepareVsTestConsole();
            }

            if (!eventHandler.Failed)
            {
                return;
            }
            attempt++;
            if (attempt < MaxAttempts)
            {
                _logger.LogInformation("{RunnerId}: Retrying the test session.", RunnerId);
                eventHandler.DiscardCurrentRun();
            }
        }

        _logger.LogWarning("{0}: VsTest failed {2} times, settings: {1}", RunnerId, runSettings, attempt);
    }

    private bool WaitForEnd(Task session, RunEventHandler eventHandler, int? timeOut, ref int attempt)
    {
        var vsTestFailed = false;
        if (timeOut.HasValue)
        {
            // we wait for the end notification for the test session
            // ==> if it failed, results are uncertain
            var suspiciousTimeOut = !eventHandler.Wait(VsTestExtraTimeOutInMs + timeOut.Value, out var slept);
            // we wait for vsTestProcess to stop
            // ==> if it appears hung, we recycle it.
            if (!session.Wait(VsTestExtraTimeOutInMs))
            {
                _vsTestConsole.AbortTestRun();
                vsTestFailed = !session.Wait(VsTestExtraTimeOutInMs);
            }

            if (suspiciousTimeOut && slept)
            {
                // the computer went to sleep during the session
                // we should ignore the result and retry
                _logger.LogWarning(
                    "{RunnerId}: Rerun of the test session because computer entered power saving mode.", RunnerId);
                attempt--;
            }
            else
            {
                _logger.LogDebug("{RunnerId}: Test session finished.", RunnerId);
            }
        }
        else
        {
            // no timeout provided ==> initial tests
            // we wait for VsTest to end.
            // we could add a configurable timeout, to prevent actual locking to happen during initial tests, but no idea what a good default should be
            session.Wait();
            // we add a grace delay for notifications to be propagated
            eventHandler.Wait(VsTestExtraTimeOutInMs, out var _);
            _logger.LogDebug("{RunnerId}: Test session finished.", RunnerId);
        }

        return vsTestFailed;
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
            catch
            {
                /*Ignore exception. vsTestConsole has been disposed outside our control*/
            }
        }

        _vsTestConsole = _context.BuildVsTestWrapper($"{RunnerId}-{_instanceCount}", ControlVariableName);
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
                _logger.LogError(e, "Exception when disposing {RunnerId}", RunnerId);
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
