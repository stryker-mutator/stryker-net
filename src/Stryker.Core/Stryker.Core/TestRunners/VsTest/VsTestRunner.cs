using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Serilog.Events;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ToolHelpers;
using Stryker.DataCollector;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using Stryker.Core.Exceptions;
using Guid = System.Guid;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunner : IMultiTestRunner
    {
        private IVsTestConsoleWrapper _vsTestConsole;

        private readonly IFileSystem _fileSystem;
        private readonly StrykerOptions _options;
        private readonly OptimizationFlags _flags;
        private readonly ProjectInfo _projectInfo;
        private readonly Func<IDictionary<string, string>, int, IStrykerTestHostLauncher> _hostBuilder;
        private readonly IVsTestHelper _vsTestHelper;
        private readonly bool _ownHelper;
        private readonly List<string> _messages = new List<string>();
        private readonly bool _usePipeForCoverage;
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
            OptimizationFlags flags,
            ProjectInfo projectInfo,
            ICollection<TestCase> testCasesDiscovered,
            IFileSystem fileSystem = null,
            IVsTestHelper helper = null,
            ILogger logger = null,
            IVsTestConsoleWrapper wrapper = null,
            Func<IDictionary<string, string>, int, IStrykerTestHostLauncher> hostBuilder = null)
        {
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestRunner>();
            _fileSystem = fileSystem ?? new FileSystem();
            _options = options;
            _flags = flags;
            _projectInfo = projectInfo;
            _hostBuilder = hostBuilder ?? ((dico, id) => new StrykerVsTestHostLauncher(dico, id));
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
            _usePipeForCoverage = !flags.HasFlag(OptimizationFlags.UseEnvVariable);
        }

        public TestRunResult RunAll(int? timeoutMs, Mutant mutant, TestUpdateHandler update)
        {
            return TestMultipleMutants(timeoutMs, mutant == null ? null : new List<Mutant>{ mutant}, update);
        }

        public TestRunResult TestMultipleMutants(int? timeoutMs, IReadOnlyList<Mutant> mutants, TestUpdateHandler update)
        {
            var envVars = new Dictionary<string, string>();
            var mutantTestsMap = new Dictionary<int, IList<string>>();
            ICollection<TestCase> testCases = null;
            if (!_flags.HasFlag(OptimizationFlags.CoverageBasedTest) && mutants.Count>1)
            {
                throw new StrykerInputException("Multiple mutant test requires coverage analysis");
            }
            if (mutants != null)
            {
                //envVars["ActiveMutation"] = string.Join(',', mutants.Select(m => m.Id.ToString()));
                // if we optimize the number of tests to run
                if (_flags.HasFlag(OptimizationFlags.CoverageBasedTest))
                {
                    var needAll = false;
                    foreach (var mutant in mutants)
                    {
                        var tests = mutant.CoveringTests.GetList().Select(t => t.Guid).ToList();
                        if (mutant.IsStaticValue && !_flags.HasFlag(OptimizationFlags.CaptureCoveragePerTest))
                        {
                            tests = null;
                            needAll = true;
                        }
                        mutantTestsMap.Add(mutant.Id, tests);
                    }

                    testCases = needAll ? null :  mutants.SelectMany(m => m.CoveringTests.GetList()).Distinct().Select( t => _discoveredTests.First( tc => tc.Id.ToString() == t.Guid)).ToList();

                    _logger.LogDebug( $"Runner {_id}: Testing [{string.Join(',', mutants.Select(m => m.DisplayName))}] "+
                                      $"against {(testCases == null ? "all tests." : string.Join(", ", testCases.Select(x => x.FullyQualifiedName)))}.");
                    if (testCases?.Count == 0)
                    {
                        return new TestRunResult(true, "Mutants are not covered by any test!");
                    }
                }
            }

            var expectedTests = testCases?.Count ?? DiscoverNumberOfTests();

            void HandleUpdate(RunEventHandler handler)
            {
                if (mutants == null)
                {
                    return;
                }

                var handlerTestResults = handler.TestResults;
                var tests = handlerTestResults.Count == DiscoverNumberOfTests()
                    ? TestListDescription.EveryTest()
                    : new TestListDescription(handlerTestResults.Select(tr => (TestDescription) tr.TestCase));
                var failedTest = new TestListDescription(handlerTestResults.Where(tr => tr.Outcome == TestOutcome.Failed)
                    .Select(tr => (TestDescription) tr.TestCase));
                var remainingMutants = update?.Invoke(mutants, failedTest, tests);
                if (handlerTestResults.Count < expectedTests &&
                    remainingMutants == false && !_aborted )
                {
                    // all mutants status have been resolved, we can stop
                    _logger.LogDebug($"Runner {_id}: each mutant's fate has been established, we can abort.");
                    _vsTestConsole.CancelTestRun();
                    _aborted = true;
                }
            }

            var testResults = RunTestSession(testCases, envVars, GenerateRunSettings(timeoutMs, false, _usePipeForCoverage, mutantTestsMap), HandleUpdate);
            var resultAsArray = testResults as TestResult[] ?? testResults.ToArray();
            var timeout = (!_aborted && resultAsArray.Length < expectedTests);
            var ranTests = resultAsArray.Length == DiscoverNumberOfTests() ? TestListDescription.EveryTest() : new TestListDescription(resultAsArray.Select(tr => (TestDescription)tr.TestCase));
            var failedTests = resultAsArray.Where(tr => tr.Outcome == TestOutcome.Failed).Select(tr => (TestDescription) tr.TestCase).ToImmutableArray();

            var message = string.Join( Environment.NewLine,
                resultAsArray.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage))
                    .Select(tr => tr.ErrorMessage));
            var failedTestsDescription = new TestListDescription(failedTests);
            return timeout ? TestRunResult.TimedOut(ranTests, failedTestsDescription, message) : new TestRunResult(ranTests, failedTestsDescription, message);
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
                var generateRunSettings = GenerateRunSettings(null, false, _usePipeForCoverage, null);
                _vsTestConsole.DiscoverTests(_sources, runSettings ?? generateRunSettings, handler);

                waitHandle.WaitOne();
                if (handler.Aborted)
                {
                    _logger.LogError($"Runner {_id}: Test discovery has been aborted!");
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
            _logger.LogDebug($"Runner {_id}: Capturing coverage.");
            var testResults = RunTestSession(null, null, GenerateRunSettings(null, true, _usePipeForCoverage, null));
            ParseResultsForCoverage(testResults, mutants);
            return new TestRunResult (true );
        }

        private void ParseResultsForCoverage(IEnumerable<TestResult> testResults, IEnumerable<Mutant> mutants)
        {
            // since we analyze mutant coverage, mutants are assumed as not covered
            var mutantArray = mutants as Mutant[] ?? mutants.ToArray();
            foreach(var mutant in mutantArray)
            {
                mutant.CoveringTests = new TestListDescription(null);
            }

            var seenTestCases = new HashSet<Guid>();
            foreach (var testResult in testResults)
            {
                var (key, value) = testResult.GetProperties().FirstOrDefault(x => x.Key.Id == "Stryker.Coverage");
                if (key == null)
                {
                    _logger.LogWarning($"Each mutant will be tested against {testResult.TestCase.DisplayName}), because we can't get coverage info for test case generated at run time");
                }
                else if (value != null)
                {
                    seenTestCases.Add(testResult.TestCase.Id);
                    var propertyPairValue = (value as string);
                    if (string.IsNullOrWhiteSpace(propertyPairValue))
                    {
                        //
                    }
                    else
                    {
                        var parts = propertyPairValue.Split(';');
                        // we need to refer to the initial testCase instance, otherwise xUnit raises internal errors
                        var coveredMutants = string.IsNullOrEmpty(parts[0])
                            ? new List<int>()
                            : parts[0].Split(',').Select(int.Parse).ToList();
                        // we identify mutants that are part of static code, unless we performed pertest capture
                        var staticMutants = (string.IsNullOrEmpty(parts[1]) || _options.Optimizations.HasFlag(OptimizationFlags.CaptureCoveragePerTest))
                            ? new List<int>()
                            : parts[1].Split(',').Select(int.Parse).ToList();
                        foreach (var mutant in mutantArray)
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

        public void CoverageForOneTest(TestCase test, IEnumerable<Mutant> mutants, bool cantUseAppDomain, bool cantUsePipe)
        {
            _logger.LogDebug($"Runner {_id}: Capturing coverage for {test.FullyQualifiedName}.");
            var testResults = RunTestSession(new []{test}, null, GenerateRunSettings(null, true, _usePipeForCoverage, null));
            ParseResultsForCoverage(testResults.Where(x => x.TestCase.Id == test.Id), mutants);
            // we cancel the test. Avoid using 'Abort' method, as we use the Aborted status to identify timeouts.
            _vsTestConsole.CancelTestRun();
        } 

        private IEnumerable<TestResult> RunTestSession(IEnumerable<TestCase> testCases, 
            IDictionary<string, string> envVars,
            string runSettings,
            Action<RunEventHandler> updateHandler = null, 
            int retries = 0)
        {

            using (var eventHandler = new RunEventHandler(_logger, RunnerId))
            {
                void HandlerVsTestFailed(object sender, EventArgs e) =>  _vsTestFailed = true;
                void HandlerUpdate(object sender, EventArgs e) => updateHandler?.Invoke(eventHandler);
                var strykerVsTestHostLauncher = _hostBuilder(envVars, _id);

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
                    return eventHandler.TestResults;
                }
                _vsTestConsole = PrepareVsTestConsole();
                _vsTestFailed = false;

                return RunTestSession(testCases, envVars, runSettings, updateHandler, ++retries);
            }
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

            _logger.LogDebug("{0}: VsTest logging set to {1}", RunnerId, traceLevel);
            return traceLevel;
        }

        private string GenerateRunSettings(int? timeout, bool forCoverage, bool usePipe,
            Dictionary<int, IList<string>> mutantTestsMap)
        {
            var targetFramework = _projectInfo.TestProjectAnalyzerResults.FirstOrDefault().TargetFramework;
            var targetFrameworkVersion = _projectInfo.TestProjectAnalyzerResults.FirstOrDefault().TargetFrameworkVersion;
            string targetFrameworkVersionString;

            switch (targetFramework)
            {
                case Initialisation.Framework.NetCore:
                    targetFrameworkVersionString = $".NETCoreApp,Version = v{targetFrameworkVersion}";
                    break;
                case Initialisation.Framework.NetStandard:
                    throw new StrykerInputException("Unsupported targetframework detected. A unit test project cannot be netstandard!: " + targetFramework);
                default:
                    targetFrameworkVersionString = $".NETFramework,Version = v{targetFrameworkVersion}";
                    break;
            }

            var needCoverage = forCoverage && NeedCoverage();
            var dataCollectorSettings =CoverageCollector.GetVsTestSettings(needCoverage, usePipe, mutantTestsMap);
            var settingsForCoverage = string.Empty;
            //if (needCoverage)
            {
                if (_testFramework.HasFlag(TestFramework.nUnit))
                {
                    settingsForCoverage = "<CollectDataForEachTestSeparately>true</CollectDataForEachTestSeparately>";
                }

                if (_testFramework.HasFlag(TestFramework.xUnit))
                {
                    settingsForCoverage += "<DisableParallelization>true</DisableParallelization>";
                }
            }
            var timeoutSettings = timeout.HasValue ? $"<TestSessionTimeout>{timeout}</TestSessionTimeout>"+Environment.NewLine : "";
            var runSettings = 
$@"<RunSettings>
 <RunConfiguration>
  <MaxCpuCount>{_options.ConcurrentTestrunners}</MaxCpuCount>
  <TargetFrameworkVersion>{targetFrameworkVersionString}</TargetFrameworkVersion>{timeoutSettings}{settingsForCoverage}
 </RunConfiguration>{dataCollectorSettings}
</RunSettings>";
            _logger.LogDebug("VsTest run settings set to: {0}", runSettings);

            return runSettings;
        }

        private bool NeedCoverage()
        {
            return _flags.HasFlag(OptimizationFlags.CoverageBasedTest) || _flags.HasFlag(OptimizationFlags.SkipUncoveredMutants);
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

            _logger.LogDebug("{1}: Logging VsTest output to: {0}", vsTestLogPath, RunnerId);

            return new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath(), new ConsoleParameters
            {
                TraceLevel = DetermineTraceLevel(),
                LogFilePath = vsTestLogPath
            });
        }

        private void InitializeVsTestConsole()
        {
            var testBinariesPaths = _projectInfo.TestProjectAnalyzerResults.Select(testProject => _projectInfo.GetTestBinariesPath(testProject)).ToList();
            var testBinariesLocations = new List<string>();
            _sources = new List<string>();

            foreach (var path in testBinariesPaths)
            {
                if (!_fileSystem.File.Exists(path))
                {
                    throw new GeneralStrykerException($"The test project binaries could not be found at {path}, exiting...");
                }
                testBinariesLocations.Add(Path.GetDirectoryName(path));
                _sources.Add(FilePathUtils.NormalizePathSeparators(path));
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
                    _logger.LogError($"Exception when disposing Runner {_id}: {0}", e);
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
