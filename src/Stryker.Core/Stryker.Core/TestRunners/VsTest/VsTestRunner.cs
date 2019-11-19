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
using System.Text.RegularExpressions;
using System.Threading;
using Stryker.Core.Exceptions;
using Mutant = Stryker.Core.Mutants.Mutant;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunner : IMultiTestRunner
    {
        private IVsTestConsoleWrapper _vsTestConsole;

        public IEnumerable<TestDescription> Tests => _discoveredTests.Select(x => (TestDescription)x);
        private readonly IFileSystem _fileSystem;
        private readonly StrykerOptions _options;
        private readonly OptimizationFlags _flags;
        private readonly ProjectInfo _projectInfo;
        private readonly Func<IDictionary<string, string>, int, IStrykerTestHostLauncher> _hostBuilder;
        private readonly IVsTestHelper _vsTestHelper;
        private readonly bool _ownHelper;
        private readonly List<string> _messages = new List<string>();
        private readonly Dictionary<string, string> _coverageEnvironment;

        private ICollection<TestCase> _discoveredTests;
        private ICollection<string> _sources;
        private bool _disposedValue; // To detect redundant calls
        private static int _count;
        private readonly int _id;
        private TestFramework _testFramework;
        private bool _vsTestFailed = false;

        private readonly ILogger _logger;
        private bool _aborted;
        private string RunnerId => $"Runner {_id}";


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
            _coverageEnvironment = new Dictionary<string, string>
            {
                {CoverageCollector.ModeEnvironmentVariable, flags.HasFlag(OptimizationFlags.UseEnvVariable) ? CoverageCollector.EnvMode : CoverageCollector.PipeMode}
            };
        }

        public TestRunResult RunAll(int? timeoutMs, Mutant mutant)
        {
            return TestMultipleMutants(timeoutMs, mutant == null ? null : new List<Mutant>{ mutant});
        }

        public TestRunResult TestMultipleMutants(int? timeoutMs, IReadOnlyList<Mutant> mutants)
        {
            var envVars = new Dictionary<string, string>();
            ICollection<TestCase> testCases = null;
            if (mutants != null)
            {
                envVars["ActiveMutation"] = string.Join(',', mutants.Select(m => m.Id.ToString()));
                // if we optimize the number of tests to run
                if (_flags.HasFlag(OptimizationFlags.CoverageBasedTest))
                {
                    // we must run all tests if the mutants needs it (static) except when coverage has been captured by isolated test
                    var tests = mutants.SelectMany(m => m.CoveringTests.GetList()).Distinct().ToList();
                    testCases = (mutants.Any(m => m.IsStaticValue) && !_flags.HasFlag(OptimizationFlags.CaptureCoveragePerTest))
                        ? null : _discoveredTests.Where( t => tests.Contains(t)).ToList();

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
                var remainingMutants = UpdateMutantStatusAccordingToTestResults(mutants, handlerTestResults);
                if (handlerTestResults.Count < expectedTests &&
                    remainingMutants.Count == 0 
                    && _options.Optimizations.HasFlag(OptimizationFlags.AbortTestOnKill))
                {
                    // all mutants status have been resolved, we can stop
                    _logger.LogDebug($"Runner {_id}: each mutant's fate has been established, we can abort.");
                    _vsTestConsole.AbortTestRun();
                    _aborted = true;
                    
                }
            }

            var testResults = RunTestSession(testCases, envVars, GenerateRunSettings(timeoutMs, false), false, HandleUpdate);

            // For now we need to throw an OperationCanceledException when a testrun has timed out. 
            // We know the test run has timed out because we received less test results from the test run than there are test cases in the unit test project.
            var resultAsArray = testResults as TestResult[] ?? testResults.ToArray();
            var ranTests = resultAsArray.Length == DiscoverNumberOfTests() ? TestListDescription.EveryTest() : new TestListDescription(resultAsArray.Select(tr => (TestDescription)tr.TestCase));
            var failedTests = resultAsArray.Where(tr => tr.Outcome == TestOutcome.Failed).Select(tr => (TestDescription) tr.TestCase).ToImmutableArray();
            var timeout = (!_aborted && resultAsArray.Length < expectedTests);

            if (mutants != null)
            {
                var notTested = UpdateMutantStatusAccordingToTestResults(mutants, testResults.ToList());

                if (timeout)
                {
                    if (notTested.Count == 1)
                    {
                        notTested[0].ResultStatus = MutantStatus.Timeout;
                    }
                    else
                    {
                        // run remaining mutants one by one
                        foreach (var remainingMutant in notTested)
                        {
                            TestMultipleMutants(timeoutMs, new List<Mutant> {remainingMutant});
                        }
                    }
                }
                else
                {
                    if (notTested.Count > 0)
                    {
                        _logger.LogError(
                            $"{mutants.Count} mutants were not fully tested ({string.Join(", ", mutants)}). Please open an issue.");
                    }
                }
            }

            return new TestRunResult(ranTests, new TestListDescription(failedTests), string.Join( Environment.NewLine,
                resultAsArray.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage))
                    .Select(tr => tr.ErrorMessage)));
        }

        private List<Mutant> UpdateMutantStatusAccordingToTestResults(IReadOnlyList<Mutant> mutants, List<TestResult> handlerTestResults)
        {
            var tests = handlerTestResults.Count == DiscoverNumberOfTests()
                ? TestListDescription.EveryTest()
                : new TestListDescription(handlerTestResults.Select(tr => (TestDescription) tr.TestCase));
            var failed = handlerTestResults.Where(tr => tr.Outcome == TestOutcome.Failed)
                .Select(tr => (TestDescription) tr.TestCase).ToImmutableArray();
            var remainingMutants = new List<Mutant>();
            foreach (var mutant in mutants)
            {
                mutant.AnalyzeTestRun(failed, tests);
                if (mutant.ResultStatus == MutantStatus.NotRun)
                {
                    remainingMutants.Add(mutant);
                }
            }

            return remainingMutants;
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
                var generateRunSettings = GenerateRunSettings(null, false);
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
            var testResults = RunTestSession(null, _coverageEnvironment, GenerateRunSettings(null, true), true);
            ParseResultsForCoverage(testResults, mutants);
            return new TestRunResult (true );
        }

        private void ParseResultsForCoverage(IEnumerable<TestResult> testResults, IEnumerable<Mutant> mutants)
        {
            // since we analyze mutant coverage, mutants are assumed as not covered
            foreach(var mutant in mutants)
            {
                mutant.CoveringTests = new TestListDescription(null);
            }

            foreach (var testResult in testResults)
            {
                var (key, value) = testResult.GetProperties().FirstOrDefault(x => x.Key.Id == "Stryker.Coverage");
                if (key == null)
                {
                    _logger.LogWarning($"Failed to retrieve coverage info for {testResult.TestCase.FullyQualifiedName}.");
                }
                else if (value != null)
                {
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
                        foreach (var mutant in mutants)
                        {
                            if (coveredMutants.Contains(mutant.Id))
                            {
                                mutant.CoveringTests.Add(testResult.TestCase);
                            }

                            if (!staticMutants.Contains(mutant.Id)) continue;
                            // the mutant is used in static initialization context
                            mutant.IsStaticValue = true;
                            if (!_flags.HasFlag(OptimizationFlags.CaptureCoveragePerTest))
                            {
                                mutant.MustRunAgainstAllTests = true;
                            }
                        }
                    }
                }
            }
        }

        public void CoverageForOneTest(TestCase test, IEnumerable<Mutant> mutants, bool cantUseAppDomain, bool cantUsePipe)
        {
            _logger.LogDebug($"Runner {_id}: Capturing coverage for {test.FullyQualifiedName}.");
            var testResults = RunTestSession(new []{test}, _coverageEnvironment, GenerateRunSettings(null, true), true);
            ParseResultsForCoverage(testResults.Where(x => x.TestCase.Id == test.Id), mutants);
        }

        private void Handler_TestsFailed(object sender, EventArgs e)
        {
            // one test has failed, we can stop
            _logger.LogDebug($"Runner {_id}: At least one test failed, abort current test run.");
            _vsTestConsole.AbortTestRun();
            _aborted = true;
        }

        private IEnumerable<TestResult> RunTestSession(IEnumerable<TestCase> testCases, 
            IDictionary<string, string> envVars,
            string runSettings, 
            bool forCoverage,
            Action<RunEventHandler> updateHandler = null, 
            int retries = 0)
        {

            using (var eventHandler = new RunEventHandler(_logger, RunnerId))
            {
                void Handler_VsTestFailed(object sender, EventArgs e) =>  _vsTestFailed = true;
                void HandlerUpdate(object sender, EventArgs e) => updateHandler?.Invoke(eventHandler);
                var strykerVsTestHostLauncher = _hostBuilder(envVars, _id);

                eventHandler.VsTestFailed += Handler_VsTestFailed;
                eventHandler.ResultsUpdated += HandlerUpdate;

                if (_flags.HasFlag(OptimizationFlags.AbortTestOnKill) && !forCoverage)
                {
                    eventHandler.TestsFailed += Handler_TestsFailed;
                }

                _aborted = false;
                if (testCases != null)
                {
                    _vsTestConsole.RunTestsWithCustomTestHost(_discoveredTests.Where(discoveredTest => testCases.Any(test => test.Id == discoveredTest.Id)), runSettings, eventHandler, strykerVsTestHostLauncher);
                }
                else
                {
                    _vsTestConsole.RunTestsWithCustomTestHost(_sources, runSettings, eventHandler, strykerVsTestHostLauncher);
                }

                // Test host exited signal comes after the run complete
                strykerVsTestHostLauncher.WaitProcessExit();

                // At this point, run must have complete. Check signal for true
                eventHandler.WaitEnd();

                eventHandler.ResultsUpdated -= HandlerUpdate;
                eventHandler.TestsFailed -= Handler_TestsFailed;
                eventHandler.VsTestFailed -= Handler_VsTestFailed;

                if (!_vsTestFailed || retries > 10) 
                {
                    return eventHandler.TestResults;
                }
                _vsTestConsole = PrepareVsTestConsole();
                _vsTestFailed = false;

                return RunTestSession(testCases, envVars, runSettings, forCoverage, updateHandler, ++retries);
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

            _logger.LogDebug("{1}: VsTest logging set to {0}", traceLevel.ToString(), RunnerId);
            return traceLevel;
        }

        private string GenerateRunSettings(int? timeout, bool forCoverage)
        {
            var targetFramework = _projectInfo.TestProjectAnalyzerResult.TargetFramework;

            string targetFrameworkVersionString;

            switch (targetFramework)
            {
                case Initialisation.Framework.NetCore:
                    targetFrameworkVersionString = $".NETCoreApp,Version = v{_projectInfo.TestProjectAnalyzerResult.TargetFrameworkString}";
                    break;
                case Initialisation.Framework.NetStandard:
                    throw new StrykerInputException("Unsupported targetframework detected. A unit test project cannot be netstandard!: " + targetFramework);
                default:
                    targetFrameworkVersionString = $".NETFramework,Version = v{_projectInfo.TestProjectAnalyzerResult.TargetFrameworkString}";
                    break;
            }

            var needCoverage = forCoverage && NeedCoverage();
            var dataCollectorSettings = needCoverage ? CoverageCollector.GetVsTestSettings() : "";
            var settingsForCoverage = string.Empty;
            if (needCoverage)
            {
                if (_testFramework.HasFlag(TestFramework.nUnit))
                {
                    settingsForCoverage = "<CollectDataForEachTestSeparately>true</CollectDataForEachTestSeparately>"+Environment.NewLine;
                }

                if (_testFramework.HasFlag(TestFramework.xUnit))
                {
                    settingsForCoverage += "<DisableParallelization>true</DisableParallelization>+Environment.NewLine";
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
            var testBinariesPath = _projectInfo.GetTestBinariesPath();
            if (!_fileSystem.File.Exists(testBinariesPath))
            {
                throw new ApplicationException($"The test project binaries could not be found at {testBinariesPath}, exiting...");
            }

            var testBinariesLocation = Path.GetDirectoryName(testBinariesPath);
            _sources = new List<string>
            {
                FilePathUtils.NormalizePathSeparators(testBinariesPath)
            };
            try
            {
                _vsTestConsole.StartSession();
                _vsTestConsole.InitializeExtensions(new List<string>
                {
                    testBinariesLocation,
                    _vsTestHelper.GetDefaultVsTestExtensionsPath(_vsTestHelper.GetCurrentPlatformVsTestToolPath())
                });
            }
            catch (Exception e)
            {
                throw new ApplicationException("Stryker failed to connect to vstest.console", e);
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
