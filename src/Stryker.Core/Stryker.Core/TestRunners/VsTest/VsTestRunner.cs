using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using NuGet.Frameworks;
using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ToolHelpers;
using Stryker.DataCollector;

namespace Stryker.Core.TestRunners.VsTest
{
    public sealed class VsTestRunner : ITestRunner
    {
        private IVsTestConsoleWrapper _vsTestConsole;
        private readonly IFileSystem _fileSystem;
        private readonly StrykerOptions _options;
        private readonly SourceProjectInfo _projectInfo;
        private readonly Func<int, IStrykerTestHostLauncher> _hostBuilder;
        private readonly IVsTestHelper _vsTestHelper;
        private readonly bool _ownHelper;
        private readonly List<string> _messages = new();
        private IDictionary<Guid, VsTestDescription> _vsTests;
        private ICollection<string> _sources;
        private bool _disposedValue; // To detect redundant calls
        private readonly int _id;
        private TestFramework _testFramework;
        private bool _vsTestFailed;

        private readonly ILogger _logger;
        private bool _aborted;
        private readonly TestSet _tests;
        private string RunnerId => $"Runner {_id}";

        public VsTestRunner(StrykerOptions options,
            SourceProjectInfo projectInfo,
            IDictionary<Guid, VsTestDescription> tests,
            TestSet testSet,
            int id,
            IFileSystem fileSystem = null,
            IVsTestHelper helper = null,
            ILogger logger = null,
            IVsTestConsoleWrapper wrapper = null,
            Func<int, IStrykerTestHostLauncher> hostBuilder = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunner>();
            _fileSystem = fileSystem ?? new FileSystem();
            _options = options;
            _projectInfo = projectInfo;
            _hostBuilder = hostBuilder ?? (xId => new StrykerVsTestHostLauncher(xId));
            SetListOfTests(tests);
            _tests = testSet ?? new TestSet();
            _ownHelper = helper == null;
            _vsTestHelper = helper ?? new VsTestHelper();
            _id = id;
            PrepareVsTestEnvironment();
            _vsTestConsole = wrapper;
            if (_vsTestConsole == null)
            {
                PrepareVsTestConsole();
            }
            _vsTests = DiscoverTests(null).Item1;
        }

        private bool CantUseStrykerDataCollector() =>
            _projectInfo.TestProjectAnalyzerResults.Select(x => x.GetNuGetFramework()).Any(t =>
                t.Framework == FrameworkConstants.FrameworkIdentifiers.NetCoreApp && t.Version.Major < 2);

        public TestRunResult InitialTest()
        {
            var testResults = RunTestSession(null, true, GenerateRunSettings(null, false, false, new Dictionary<int, ITestGuids>()));
            // initial test run, register test results
            foreach (var result in testResults.TestResults)
            {
                if (!_vsTests.ContainsKey(result.TestCase.Id))
                {
                    _vsTests[result.TestCase.Id] = new VsTestDescription(result.TestCase);
                    _logger.LogWarning("{RunnerId}: Initial test run encounter a unexpected test case ({TestCaseDisplayName}), mutation tests may be inaccurate. Disable coverage analysis if your have doubts.",
                        RunnerId, result.TestCase.DisplayName);
                }

                _vsTests[result.TestCase.Id].RegisterInitialTestResult(result);
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
                if (_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest))
                {
                    needAll = false;
                    foreach (var mutant in mutants)
                    {
                        ITestListDescription tests;
                        if ((mutant.IsStaticValue && !_options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest)) || mutant.MustRunAgainstAllTests)
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
                        timeOutMs = timeoutCalc.CalculateTimeoutValue((int)testCases.Sum(id => _vsTests[id].InitialRunTime.TotalMilliseconds));
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
            var expectedTests = needAll ? DiscoverTests().Count : numberTestCases;

            void HandleUpdate(IRunResults handler)
            {
                var handlerTestResults = handler.TestResults;
                if (mutants == null)
                {
                    return;
                }
                var tests = handlerTestResults.Count == DiscoverTests().Count
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

            var testResults = RunTestSession(mutantTestsMap, needAll,
                GenerateRunSettings(timeOutMs, mutants != null, false, mutantTestsMap), timeOutMs, HandleUpdate);

            return BuildTestRunResult(testResults, expectedTests);
        }

        private TestRunResult BuildTestRunResult(IRunResults testResults, int expectedTests, bool compressAll = true)
        {
            var resultAsArray = testResults.TestResults.ToArray();
            var testCases = resultAsArray.Select(t => t.TestCase.Id).Distinct();
            var ranTestsCount = testCases.Count();
            var timeout = !_aborted && ranTestsCount < expectedTests;
            var ranTests = (compressAll && ranTestsCount >= DiscoverTests().Count) ? (ITestGuids)TestsGuidList.EveryTest() : new WrappedGuidsEnumeration(testCases);
            var failedTests = resultAsArray.Where(tr => tr.Outcome == TestOutcome.Failed).Select(t => t.TestCase.Id);

            if (ranTests.IsEmpty && (testResults.TestsInTimeout == null || testResults.TestsInTimeout.Count == 0))
            {
                _logger.LogTrace($"{RunnerId}: Initial Test session reports 0 result and 0 stuck tests.");
            }

            var duration =  TimeSpan.FromTicks(_vsTests.Values.Sum(t => t.InitialRunTime.Ticks));

            var message = string.Join(Environment.NewLine,
                resultAsArray.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage))
                    .Select(tr => $"{tr.DisplayName}{Environment.NewLine}{Environment.NewLine}{tr.ErrorMessage}"));
            var failedTestsDescription = new WrappedGuidsEnumeration(failedTests);
            var timedOutTests = new WrappedGuidsEnumeration(testResults.TestsInTimeout?.Select(t => t.Id));
            return timeout
                ? TestRunResult.TimedOut(ranTests, failedTestsDescription, timedOutTests, message, duration)
                : new TestRunResult(ranTests, failedTestsDescription, timedOutTests, message, duration);
        }

        private void SetListOfTests(IDictionary<Guid, VsTestDescription> tests)
        {
            _vsTests = tests;
            DetectTestFramework(_vsTests?.Values);
        }

        public TestSet DiscoverTests() => DiscoverTests(null).Item2;

        public (IDictionary<Guid, VsTestDescription>, TestSet) DiscoverTests(string runSettings)
        {
            if (_vsTests != null)
            {
                return (_vsTests, _tests);
            }
            using (var waitHandle = new AutoResetEvent(false))
            {
                var handler = new DiscoveryEventHandler(waitHandle, _messages);
                var generateRunSettings = GenerateRunSettings(null, false, false, null);
                _vsTestConsole.DiscoverTests(_sources, runSettings ?? generateRunSettings, handler);

                waitHandle.WaitOne();
                if (handler.Aborted)
                {
                    _logger.LogError($"{RunnerId}: Test discovery has been aborted!");
                }

                _vsTests = new Dictionary<Guid, VsTestDescription>(handler.DiscoveredTestCases.Count);
                foreach (var testCase in handler.DiscoveredTestCases)
                {
                    if (!_vsTests.ContainsKey(testCase.Id))
                    {
                        _vsTests[testCase.Id] = new VsTestDescription(testCase);
                    }

                    _vsTests[testCase.Id].AddSubCase();
                }
                DetectTestFramework(_vsTests.Values);
            }

            _tests.RegisterTests(_vsTests.Values.Select(t => t.Description));
            return (_vsTests, _tests);
        }

        private void DetectTestFramework(ICollection<VsTestDescription> tests)
        {
            if (tests == null)
            {
                _testFramework = 0;
                return;
            }
            if (tests.Any(testCase => testCase.Framework == TestFramework.nUnit))
            {
                _testFramework |= TestFramework.nUnit;
            }
            if (tests.Any(testCase => testCase.Framework == TestFramework.xUnit))
            {
                _testFramework |= TestFramework.xUnit;
            }
        }

        public TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants)
        {
            _logger.LogDebug($"{RunnerId}: Capturing coverage.");
            if (CantUseStrykerDataCollector())
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
                var testResults = RunTestSession(null, true, GenerateRunSettings(null, false, true, null));
                ParseResultsForCoverage(testResults.TestResults, mutants);
            }
            return new TestRunResult(true);
        }

        private void ParseResultsForCoverage(IEnumerable<TestResult> testResults, IEnumerable<Mutant> mutants)
        {
            // since we analyze mutant coverage, mutants are assumed as not covered
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
                if (!_vsTests.ContainsKey(testResult.TestCase.Id))
                {
                    _logger.LogWarning($"{RunnerId}: Coverage analysis run encountered a unexpected test case ({testResult.TestCase.DisplayName}), mutation tests may be inaccurate. Disable coverage analysis if your have doubts.");
                    _vsTests.Add(testResult.TestCase.Id, new VsTestDescription(testResult.TestCase));
                }
                var testDescription = _vsTests[testResult.TestCase.Id];
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
                        var staticMutants = (parts.Length == 1 || string.IsNullOrEmpty(parts[1]) || _options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest))
                            ? Enumerable.Empty<int>()
                            : parts[1].Split(',').Select(int.Parse);

                        foreach (var id in coveredMutants)
                        {
                            map[id].Add(testDescription.Description);
                        }

                        staticMutantLists.UnionWith(staticMutants);
                    }
                    var (testProperty, mutantOutsideTests) = testResult.GetProperties()
                        .FirstOrDefault(x => x.Key.Id == CoverageCollector.OutOfTestsPropertyName);
                    if (testProperty != null)
                    {
                        // we have some mutations that appeared outside any test, probably some run time test case generation, or some async logic.
                        propertyPairValue = (mutantOutsideTests as string);
                        var coveredMutants = string.IsNullOrEmpty(propertyPairValue)
                            ? Enumerable.Empty<int>()
                            : propertyPairValue.Split(',').Select(int.Parse);
                        _logger.LogWarning($"{RunnerId}: Some mutations were executed outside any test (mutation ids: {propertyPairValue}).");
                        staticMutantLists.UnionWith(coveredMutants);
                    }
                }
            }

            // push coverage data to the mutants
            foreach (var mutant in mutants)
            {
                mutant.CoveringTests = new TestsGuidList(map[mutant.Id]);
                if (staticMutantLists.Contains(mutant.Id))
                {
                    mutant.IsStaticValue = true;
                }
            }
        }

        public void CoverageForOneTest(Guid test, IEnumerable<Mutant> mutants)
        {
            _logger.LogDebug("{RunnerId}: Capturing coverage for {TestCaseFullyQualifiedName}.", RunnerId, _vsTests[test].Case.FullyQualifiedName);
            var map = new Dictionary<int, ITestGuids>(1) { [-1] = new WrappedGuidsEnumeration(new[] { test }) };
            var testResults = RunTestSession(map, true, GenerateRunSettings(null, false, true, null));
            ParseResultsForCoverage(testResults.TestResults.Where(x => x.TestCase.Id == test), mutants);
        }

        private IRunResults RunTestSession(Dictionary<int, ITestGuids> mutantTestsMap,
            bool runAllTests,
            string runSettings,
            int? timeOut = null,
            Action<RunEventHandler> updateHandler = null,
            int retries = 0)
        {
            using var eventHandler = new RunEventHandler(_vsTests, _logger, RunnerId);
            void HandlerVsTestFailed(object sender, EventArgs e)
            {
                _vsTestFailed = true;
            }

            void HandlerUpdate(object sender, EventArgs e)
            {
                updateHandler?.Invoke(eventHandler);
            }

            var strykerVsTestHostLauncher = _hostBuilder(_id);

            eventHandler.VsTestFailed += HandlerVsTestFailed;
            eventHandler.ResultsUpdated += HandlerUpdate;

            _aborted = false;
            var options = new TestPlatformOptions { TestCaseFilter = _options.TestCaseFilter };
            if (runAllTests)
            {
                _vsTestConsole.RunTestsWithCustomTestHostAsync(_sources, runSettings, options, eventHandler,
                    strykerVsTestHostLauncher);
            }
            else
            {
                _vsTestConsole.RunTestsWithCustomTestHostAsync(mutantTestsMap.SelectMany(m => m.Value.GetGuids()).Select(t => _vsTests[t].Case), runSettings,
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
            PrepareVsTestConsole();
            _vsTestFailed = false;

            return RunTestSession(mutantTestsMap, runAllTests, runSettings, timeOut, updateHandler, retries + 1);
        }

        private ConsoleParameters DetermineConsoleParameters()
        {
            if (!_options.LogOptions.LogToFile)
            {
                return new ConsoleParameters();
            }
            var vsTestLogPath = _fileSystem.Path.Combine(_options.OutputPath, "logs", $"{RunnerId}_VsTest-log.txt");
            _fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(vsTestLogPath));

            _logger.LogTrace("{RunnerId}: Logging VsTest output to: {vsTestLogPath}", RunnerId, vsTestLogPath);
            return new ConsoleParameters
            {
                TraceLevel = DetermineTraceLevel(),
                LogFilePath = vsTestLogPath
            };
        }

        private TraceLevel DetermineTraceLevel()
        {
            var traceLevel = _options.LogOptions.LogLevel switch
            {
                LogEventLevel.Debug => TraceLevel.Verbose,
                LogEventLevel.Verbose => TraceLevel.Verbose,
                LogEventLevel.Error => TraceLevel.Error,
                LogEventLevel.Fatal => TraceLevel.Error,
                LogEventLevel.Warning => TraceLevel.Warning,
                LogEventLevel.Information => TraceLevel.Info,
                _ => TraceLevel.Off
            };

            _logger.LogTrace("{RunnerId}: VsTest logging set to {traceLevel}", RunnerId, traceLevel);
            return traceLevel;
        }

        private string GenerateRunSettings(int? timeout, bool forMutantTesting, bool forCoverage, Dictionary<int, ITestGuids> mutantTestsMap)
        {
            var projectAnalyzerResult = _projectInfo.TestProjectAnalyzerResults.FirstOrDefault();
            var needCoverage = forCoverage && NeedCoverage();
            var dataCollectorSettings = (forMutantTesting || forCoverage) ?
                CoverageCollector.GetVsTestSettings(needCoverage, mutantTestsMap?.Select(e => (e.Key, e.Value.GetGuids())), CodeInjection.HelperNamespace)
                : "";
            var settingsForCoverage = string.Empty;

            if (_testFramework.HasFlag(TestFramework.nUnit))
            {
                settingsForCoverage = "<CollectDataForEachTestSeparately>true</CollectDataForEachTestSeparately>";
            }

            if (_testFramework.HasFlag(TestFramework.xUnit))
            {
                settingsForCoverage += "<DisableParallelization>true</DisableParallelization>";
            }

            var timeoutSettings = timeout != null ? $"<TestSessionTimeout>{timeout}</TestSessionTimeout>" + Environment.NewLine : string.Empty;

            var testCaseFilter = string.IsNullOrWhiteSpace(_options.TestCaseFilter) ?
                string.Empty : $"<TestCaseFilter>{_options.TestCaseFilter}</TestCaseFilter>" + Environment.NewLine;

            // we need to block parallel run to capture coverage and when testing multiple mutants in a single run
            var optionsConcurrentTestRunners = (forCoverage || !_options.OptimizationMode.HasFlag(OptimizationModes.DisableMixMutants)) ? 1 : _options.Concurrency;
            var runSettings =
$@"<RunSettings>
 <RunConfiguration>
{(projectAnalyzerResult.TargetsFullFramework() ? "<DisableAppDomain>true</DisableAppDomain>" : "")}
  <MaxCpuCount>{optionsConcurrentTestRunners}</MaxCpuCount>
{timeoutSettings}
{settingsForCoverage}
<DesignMode>false</DesignMode>
<BatchSize>1</BatchSize>
{testCaseFilter}
 </RunConfiguration>
{dataCollectorSettings}
</RunSettings>";
            _logger.LogTrace("{RunnerId}: VsTest run settings set to: {runSettings}", RunnerId, runSettings);

            return runSettings;
        }

        private bool NeedCoverage() => _options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest)
                                       || _options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants);

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

            _vsTestConsole = new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath(), DetermineConsoleParameters());
            try
            {
                // Set roll forward on no candidate fx so vstest console can start on incompatible dotnet core runtimes
                Environment.SetEnvironmentVariable("DOTNET_ROLL_FORWARD_ON_NO_CANDIDATE_FX", "2");
                _vsTestConsole.StartSession();
                _vsTestConsole.InitializeExtensions(_sources.Select(_fileSystem.Path.GetDirectoryName));
            }
            catch (Exception e)
            {
                throw new GeneralStrykerException("Stryker failed to connect to vstest.console", e);
            }
        }

        private void PrepareVsTestEnvironment()
        {
            var testBinariesPaths = _projectInfo.TestProjectAnalyzerResults.Select(testProject => testProject.GetAssemblyPath()).ToList();
            _sources = new List<string>();

            foreach (var path in testBinariesPaths)
            {
                if (!_fileSystem.File.Exists(path))
                {
                    throw new GeneralStrykerException($"The test project binaries could not be found at {path}, exiting...");
                }

                _sources.Add(path);
            }
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
                if (_ownHelper)
                {
                    _vsTestHelper.Cleanup();
                }
            }

            _disposedValue = true;
        }

        ~VsTestRunner()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
