using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ToolHelpers;
using Stryker.DataCollector;
using Framework = Stryker.Core.Initialisation.Framework;
using Guid = System.Guid;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunner : IMultiTestRunner
    {
        private IVsTestConsoleWrapper _vsTestConsole;

        private readonly IFileSystem _fileSystem;
        private readonly StrykerOptions _options;
        private readonly OptimizationModes _optimizationFlags;
        private readonly ProjectInfo _projectInfo;
        private readonly Func<int, IStrykerTestHostLauncher> _hostBuilder;
        private readonly IVsTestHelper _vsTestHelper;
        private readonly bool _ownHelper;
        private readonly List<string> _messages = new List<string>();
        private ICollection<TestCase> _discoveredTests;
        private ICollection<string> _sources;
        private bool _disposedValue; // To detect redundant calls
        private static int _count;
        private readonly int _id;
        private TestFramework _testFramework;
        private bool _vsTestFailed;

        private readonly ILogger _logger;
        private bool _aborted;
        private string RunnerId => $"Runner {_id}";

        public IEnumerable<TestDescription> Tests => _discoveredTests.Select(x => (TestDescription)x);

        public VsTestRunner(
            StrykerOptions options,
            ProjectInfo projectInfo,
            ICollection<TestCase> testCasesDiscovered,
            IFileSystem fileSystem = null,
            IVsTestHelper helper = null,
            ILogger logger = null,
            IVsTestConsoleWrapper wrapper = null,
            Func<int, IStrykerTestHostLauncher> hostBuilder = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunner>();
            _fileSystem = fileSystem ?? new FileSystem();
            _options = options;
            _optimizationFlags = options.OptimizationMode;
            _projectInfo = projectInfo;
            _hostBuilder = hostBuilder ?? ((id) => new StrykerVsTestHostLauncher(id));
            SetListOfTests(testCasesDiscovered);
            _ownHelper = helper == null;
            _vsTestHelper = helper ?? new VsTestHelper();
            _vsTestConsole = wrapper ?? PrepareVsTestConsole();
            _id = _count++;
            if (testCasesDiscovered != null)
            {
                _discoveredTests = testCasesDiscovered;
                DetectTestFramework(testCasesDiscovered);
            }
            InitializeVsTestConsole();
        }

        private bool CantUseStrykerDataCollector()
        {
            return _projectInfo.TestProjectAnalyzerResults.Select(x => x.GetTargetFrameworkAndVersion()).Any(t =>
                t.Framework == Framework.DotNet && t.Version.Major < 2);
        }

        public TestRunResult RunAll(int? timeoutMs, Mutant mutant, TestUpdateHandler update)
        {
            return TestMultipleMutants(timeoutMs, mutant == null ? null : new List<Mutant> { mutant }, update);
        }

        public TestRunResult TestMultipleMutants(int? timeoutMs, IReadOnlyList<Mutant> mutants, TestUpdateHandler update)
        {
            var mutantTestsMap = new Dictionary<int, IList<string>>();
            ICollection<TestCase> testCases = null;

            if (mutants != null)
            {
                // if we optimize the number of tests to run
                if (_optimizationFlags.HasFlag(OptimizationModes.CoverageBasedTest))
                {
                    var needAll = false;
                    foreach (var mutant in mutants)
                    {
                        List<string> tests;
                        if ((mutant.IsStaticValue && !_optimizationFlags.HasFlag(OptimizationModes.CaptureCoveragePerTest)) || mutant.MustRunAgainstAllTests)
                        {
                            tests = null;
                            needAll = true;
                        }
                        else
                        {
                            tests = mutant.CoveringTests.GetList().Select(t => t.Guid).ToList();
                        }
                        mutantTestsMap.Add(mutant.Id, tests);
                    }

                    testCases = needAll ? null : mutants.SelectMany(m => m.CoveringTests.GetList()).Distinct().Select(t => _discoveredTests.First(tc => tc.Id.ToString() == t.Guid)).ToList();

                    _logger.LogTrace($"{RunnerId}: Testing [{string.Join(',', mutants.Select(m => m.DisplayName))}] " +
                                      $"against {(testCases == null ? "all tests." : string.Join(", ", testCases.Select(x => x.FullyQualifiedName)))}.");
                    if (testCases?.Count == 0)
                    {
                        return new TestRunResult(TestListDescription.NoTest(), TestListDescription.NoTest(), TestListDescription.NoTest(), "Mutants are not covered by any test!");
                    }
                }
                else
                {
                    if (mutants.Count > 1)
                    {
                        throw new GeneralStrykerException("Internal error: trying to test multiple mutants simultaneously without 'perTest' coverage analysis.");
                    }
                    mutantTestsMap.Add(mutants.FirstOrDefault().Id, new List<string>());
                }
            }

            var expectedTests = testCases?.Count ?? DiscoverNumberOfTests();

            void HandleUpdate(IRunResults handler)
            {
                if (mutants == null)
                {
                    return;
                }
                var handlerTestResults = handler.TestResults;
                var tests = handlerTestResults.Count == DiscoverNumberOfTests()
                    ? TestListDescription.EveryTest()
                    : new TestListDescription(handlerTestResults.Select(tr => (TestDescription)tr.TestCase));
                var failedTest = new TestListDescription(handlerTestResults.Where(tr => tr.Outcome == TestOutcome.Failed)
                    .Select(tr => (TestDescription)tr.TestCase));
                var timedOutTests = new TestListDescription(handler.TestsInTimeout?.Select(t => (TestDescription)t));
                var remainingMutants = update?.Invoke(mutants, failedTest, tests, timedOutTests);
                if (handlerTestResults.Count >= expectedTests || remainingMutants != false || _aborted)
                {
                    return;
                }
                // all mutants status have been resolved, we can stop
                _logger.LogDebug($"{RunnerId}: Each mutant's fate has been established, we can stop.");
                _vsTestConsole.CancelTestRun();
                _aborted = true;
            }

            var testResults = RunTestSession(testCases, GenerateRunSettings(timeoutMs, mutants != null, false, mutantTestsMap), HandleUpdate);
            var resultAsArray = testResults.TestResults.ToArray();
            var timeout = (!_aborted && resultAsArray.Length < expectedTests);
            var ranTests = resultAsArray.Length == DiscoverNumberOfTests() ? TestListDescription.EveryTest() : new TestListDescription(resultAsArray.Select(tr => (TestDescription)tr.TestCase));
            var failedTests = resultAsArray.Where(tr => tr.Outcome == TestOutcome.Failed).Select(tr => (TestDescription)tr.TestCase).ToImmutableArray();

            if (ranTests.Count == 0 && (testResults.TestsInTimeout == null || testResults.TestsInTimeout.Count == 0))
            {
                _logger.LogTrace($"{RunnerId}: Test session reports 0 result and 0 stuck tests.");
            }

            var message = string.Join(Environment.NewLine,
                resultAsArray.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage))
                    .Select(tr => tr.ErrorMessage));
            var failedTestsDescription = new TestListDescription(failedTests);
            var timedOutTests = new TestListDescription(testResults.TestsInTimeout?.Select(t => (TestDescription)t));
            return timeout ? TestRunResult.TimedOut(ranTests, failedTestsDescription, timedOutTests, message) : new TestRunResult(ranTests, failedTestsDescription, timedOutTests, message);
        }

        private void SetListOfTests(ICollection<TestCase> tests)
        {
            _discoveredTests = tests;
            DetectTestFramework(_discoveredTests);
        }

        public int DiscoverNumberOfTests()
        {
            return DiscoverTests().Count;
        }

        public ICollection<TestCase> DiscoverTests(string runSettings = null)
        {
            if (_discoveredTests != null)
            {
                return _discoveredTests;
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

                _discoveredTests = handler.DiscoveredTestCases;
                DetectTestFramework(handler.DiscoveredTestCases);
            }

            return _discoveredTests;
        }

        private void DetectTestFramework(ICollection<TestCase> tests)
        {
            if (tests == null)
            {
                _testFramework = 0;
                return;
            }
            if (tests.Any(testCase => testCase.ExecutorUri.AbsoluteUri.Contains("nunit")))
            {
                _testFramework |= TestFramework.nUnit;
            }
            if (tests.Any(testCase => testCase.Properties.Any(p => p.Id == "XunitTestCase")))
            {
                _testFramework |= TestFramework.xUnit;
            }
        }

        public TestRunResult CaptureCoverage(IEnumerable<Mutant> mutants, bool cantUseAppDomain, bool cantUsePipe)
        {
            _logger.LogDebug($"{RunnerId}: Capturing coverage.");
            if (CantUseStrykerDataCollector())
            {
                _logger.LogDebug($"{RunnerId}: project does not support StrykerDataCollector. Coverage data is simulated. Upgrade test proj" +
                                 $"" +
                                 $" to@                                                                                                                  NetCore 2.0+");
                // can't capture coverage info
                foreach (var mutant in mutants)
                {
                    mutant.CoveringTests = TestListDescription.EveryTest();
                }
            }
            else
            {
                var testResults = RunTestSession(null, GenerateRunSettings(null, false, true, null));
                ParseResultsForCoverage(testResults.TestResults, mutants);
            }
            return new TestRunResult(true);
        }

        private void ParseResultsForCoverage(IEnumerable<TestResult> testResults, IEnumerable<Mutant> mutants)
        {
            // since we analyze mutant coverage, mutants are assumed as not covered
            var seenTestCases = new HashSet<Guid>();
            var dynamicTestCases = new HashSet<Guid>();
            foreach (var testResult in testResults)
            {
                var (key, value) = testResult.GetProperties().FirstOrDefault(x => x.Key.Id == "Stryker.Coverage");
                if (key == null)
                {
                    if (seenTestCases.Contains(testResult.TestCase.Id) && !dynamicTestCases.Contains(testResult.TestCase.Id))
                    {
                        dynamicTestCases.Add(testResult.TestCase.Id);
                        // register dynamic testcases
                        foreach (var mutant in mutants)
                        {
                            mutant.CoveringTests.Add(testResult.TestCase);
                        }
                        _logger.LogWarning($"{RunnerId}: Each mutant will be tested against {testResult.TestCase.DisplayName}), because we can't get coverage info for test case generated at run time");
                    }
                }
                else if (value != null)
                {
                    seenTestCases.Add(testResult.TestCase.Id);
                    var propertyPairValue = (value as string);
                    if (string.IsNullOrWhiteSpace(propertyPairValue))
                    {
                        _logger.LogDebug($"{RunnerId}: Test {testResult.TestCase.DisplayName} does not cover any mutation.");
                    }
                    else
                    {
                        var parts = propertyPairValue.Split(';');
                        // we need to refer to the initial testCase instance, otherwise xUnit raises internal errors
                        var coveredMutants = string.IsNullOrEmpty(parts[0])
                            ? new List<int>()
                            : parts[0].Split(',').Select(int.Parse).ToList();
                        // we identify mutants that are part of static code, unless we performed pertest capture
                        var staticMutants = (string.IsNullOrEmpty(parts[1]) || _options.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest))
                            ? new List<int>()
                            : parts[1].Split(',').Select(int.Parse).ToList();
                        foreach (var mutant in mutants)
                        {
                            if (coveredMutants.Contains(mutant.Id))
                            {
                                mutant.CoveringTests.Add(testResult.TestCase);
                            }

                            if (!staticMutants.Contains(mutant.Id))
                            {
                                continue;
                            }
                            // the mutant is used in static initialization context
                            mutant.IsStaticValue = true;
                        }
                    }
                }
            }
        }

        public void CoverageForOneTest(TestCase test, IEnumerable<Mutant> mutants)
        {
            _logger.LogDebug($"{RunnerId}: Capturing coverage for {test.FullyQualifiedName}.");
            var testResults = RunTestSession(new[] { test }, GenerateRunSettings(null, false, true, null));
            ParseResultsForCoverage(testResults.TestResults.Where(x => x.TestCase.Id == test.Id), mutants);
            // we cancel the test. Avoid using 'Abort' method, as we use the Aborted status to identify timeouts.
            _vsTestConsole.CancelTestRun();
        }

        private IRunResults RunTestSession(IEnumerable<TestCase> testCases,
            string runSettings,
            Action<RunEventHandler> updateHandler = null,
            int retries = 0)
        {
            using var eventHandler = new RunEventHandler(_logger, RunnerId);
            void HandlerVsTestFailed(object sender, EventArgs e) => _vsTestFailed = true;
            void HandlerUpdate(object sender, EventArgs e) => updateHandler?.Invoke(eventHandler);
            var strykerVsTestHostLauncher = _hostBuilder(_id);

            eventHandler.VsTestFailed += HandlerVsTestFailed;
            eventHandler.ResultsUpdated += HandlerUpdate;

            _aborted = false;
            if (testCases != null)
            {
                _vsTestConsole.RunTestsWithCustomTestHost(_discoveredTests.Where(discoveredTest => testCases.Any(test => test.Id == discoveredTest.Id)), runSettings, eventHandler, strykerVsTestHostLauncher);
            }
            else
            {
                _vsTestConsole.RunTestsWithCustomTestHost(_sources, runSettings, eventHandler, strykerVsTestHostLauncher);
            }

            // Test host exited signal comes after the run completed
            strykerVsTestHostLauncher.WaitProcessExit();

            // At this point, run must have complete. Check signal for true
            eventHandler.WaitEnd();

            eventHandler.ResultsUpdated -= HandlerUpdate;
            eventHandler.VsTestFailed -= HandlerVsTestFailed;

            if (!_vsTestFailed || retries > 10)
            {
                return eventHandler;
            }
            _vsTestConsole = PrepareVsTestConsole();
            _vsTestFailed = false;

            return RunTestSession(testCases, runSettings, updateHandler, ++retries);
        }

        private TraceLevel DetermineTraceLevel()
        {
            var traceLevel = TraceLevel.Off;
            switch (_options.LogOptions.LogLevel)
            {
                case LogEventLevel.Debug:
                case LogEventLevel.Verbose:
                    traceLevel = TraceLevel.Verbose;
                    break;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    break;
                case LogEventLevel.Warning:
                    traceLevel = TraceLevel.Warning;
                    break;
                case LogEventLevel.Information:
                    traceLevel = TraceLevel.Info;
                    break;
            }

            _logger.LogTrace("{0}: VsTest logging set to {1}", RunnerId, traceLevel);
            return traceLevel;
        }

        private string GenerateRunSettings(int? timeout, bool forMutantTesting, bool forCoverage, Dictionary<int, IList<string>> mutantTestsMap)
        {
            var projectAnalyzerResult = _projectInfo.TestProjectAnalyzerResults.FirstOrDefault();
            var targetFramework = projectAnalyzerResult.GetTargetFramework();

            var needCoverage = forCoverage && NeedCoverage();
            var dataCollectorSettings = (forMutantTesting || forCoverage) ? CoverageCollector.GetVsTestSettings(needCoverage, mutantTestsMap, CodeInjection.HelperNamespace) : "";
            var settingsForCoverage = string.Empty;
            if (_testFramework.HasFlag(TestFramework.nUnit))
            {
                settingsForCoverage = "<CollectDataForEachTestSeparately>true</CollectDataForEachTestSeparately>";
            }

            if (_testFramework.HasFlag(TestFramework.xUnit))
            {
                settingsForCoverage += "<DisableParallelization>true</DisableParallelization>";
            }
            var timeoutSettings = timeout.HasValue ? $"<TestSessionTimeout>{timeout}</TestSessionTimeout>" + Environment.NewLine : string.Empty;
            // we need to block parallel run to capture coverage and when testing multiple mutants in a single run
            var optionsConcurrentTestrunners = (forCoverage || !_options.OptimizationMode.HasFlag(OptimizationModes.DisableMixMutants)) ? 1 : _options.Concurrency;
            var runSettings =
$@"<RunSettings>
 <RunConfiguration>
{(targetFramework == Framework.DotNetClassic ? "<DisableAppDomain>true</DisableAppDomain>" : "")}
  <MaxCpuCount>{optionsConcurrentTestrunners}</MaxCpuCount>
{timeoutSettings}
{settingsForCoverage}
<DesignMode>false</DesignMode>
<BatchSize>1</BatchSize>
 </RunConfiguration>
{dataCollectorSettings}
</RunSettings>";
            _logger.LogTrace("VsTest run settings set to: {0}", runSettings);

            return runSettings;
        }

        private bool NeedCoverage()
        {
            return _optimizationFlags.HasFlag(OptimizationModes.CoverageBasedTest) || _optimizationFlags.HasFlag(OptimizationModes.SkipUncoveredMutants);
        }

        private IVsTestConsoleWrapper PrepareVsTestConsole()
        {
            var vsTestLogPath = Path.Combine(_options.OutputPath, "logs", "VsTest-log.txt");
            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(vsTestLogPath));
            if (_vsTestConsole != null)
            {
                try
                {
                    _vsTestConsole.EndSession();
                }
                catch { /*Ignore exception. vsTestConsole has been disposed outside of our control*/ }
                _vsTestConsole = null;
            }

            _logger.LogTrace("{1}: Logging VsTest output to: {0}", vsTestLogPath, RunnerId);

            return new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath(), new ConsoleParameters
            {
                TraceLevel = DetermineTraceLevel(),
                LogFilePath = vsTestLogPath
            });
        }

        private void InitializeVsTestConsole()
        {
            var testBinariesPaths = _projectInfo.TestProjectAnalyzerResults.Select(testProject => testProject.GetAssemblyPath()).ToList();
            var testBinariesLocations = new List<string>();
            _sources = new List<string>();

            foreach (var path in testBinariesPaths)
            {
                if (!_fileSystem.File.Exists(path))
                {
                    throw new GeneralStrykerException($"The test project binaries could not be found at {path}, exiting...");
                }
                testBinariesLocations.Add(Path.GetDirectoryName(path));
                _sources.Add(path);
            }

            testBinariesLocations.Add(_vsTestHelper.GetDefaultVsTestExtensionsPath(_vsTestHelper.GetCurrentPlatformVsTestToolPath()));

            try
            {
                // Set roll forward on no candidate fx so vstest console can start on incompatible dotnet core runtimes
                Environment.SetEnvironmentVariable("DOTNET_ROLL_FORWARD_ON_NO_CANDIDATE_FX", "2");
                _vsTestConsole.StartSession();
                _vsTestConsole.InitializeExtensions(testBinariesLocations);
            }
            catch (Exception e)
            {
                throw new GeneralStrykerException("Stryker failed to connect to vstest.console", e);
            }

            _discoveredTests = DiscoverTests();
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
