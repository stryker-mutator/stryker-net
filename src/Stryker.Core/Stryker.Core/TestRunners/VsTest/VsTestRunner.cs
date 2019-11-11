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
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunner : ITestRunner
    {
        private IVsTestConsoleWrapper _vsTestConsole;

        public IEnumerable<int> CoveredMutants { get; private set; }
        public TestCoverageInfos CoverageMutants { get; }
        public IEnumerable<TestDescription> Tests => _discoveredTests.Select(x => (TestDescription)x);

        private readonly IFileSystem _fileSystem;
        private readonly StrykerOptions _options;
        private readonly OptimizationFlags _flags;
        private readonly ProjectInfo _projectInfo;
        private readonly Func<IDictionary<string, string>, int, IStrykerTestHostLauncher> _hostBuilder;
        private readonly IVsTestHelper _vsTestHelper;
        private readonly bool _ownHelper;
        private readonly List<string> _messages = new List<string>();

        private ICollection<TestCase> _discoveredTests;
        private ICollection<string> _sources;
        private bool _disposedValue; // To detect redundant calls
        private static int _count;
        private readonly int _id;
        private TestFramework _testFramework;

        private bool _vsTestFailed = false;

        private readonly ILogger _logger;
        private string _vstestLogPath;

        public VsTestRunner(
            StrykerOptions options,
            OptimizationFlags flags,
            ProjectInfo projectInfo,
            ICollection<TestCase> testCasesDiscovered,
            TestCoverageInfos mappingInfos,
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
            CoverageMutants = mappingInfos ?? new TestCoverageInfos();
            _vsTestConsole = wrapper ?? PrepareVsTestConsole();
            _id = _count++;
            if (testCasesDiscovered != null)
            {
                _discoveredTests = testCasesDiscovered;
                DetectTestFramework(testCasesDiscovered);
            }
            InitializeVsTestConsole();
        }

        private static Dictionary<string, string> BuildCoverageEnvironment(bool useEnv)
        {
            return new Dictionary<string, string>
            {
                {CoverageCollector.ModeEnvironmentVariable, useEnv ? CoverageCollector.EnvMode : CoverageCollector.PipeMode}
            };
        }

        private string RunnerId => $"Runner {_id}";

        public TestRunResult RunAll(int? timeoutMs, IReadOnlyMutant mutant)
        {
            var envVars = new Dictionary<string, string>();

            // if we optimize the number of test to run
            if (mutant == null)
            {
                return RunVsTest(null, timeoutMs, envVars);
            }

            IEnumerable<TestCase> testCases = null;
            if (_flags.HasFlag(OptimizationFlags.CoverageBasedTest) && (mutant.CoveringTest == null || mutant.CoveringTest.Count == 0))
            {
                return new TestRunResult { ResultMessage = "Not covered by any test", Success = true };
            }

            envVars["ActiveMutation"] = mutant.Id.ToString();
            if (_flags.HasFlag(OptimizationFlags.CoverageBasedTest))
            {
                // we must run all tests if the mutants needs it (static) except when coverage has been captured by isolated test
                testCases = (mutant.MustRunAllTests && !_flags.HasFlag(OptimizationFlags.CaptureCoveragePerTest))
                    ? null : _discoveredTests.Where(t => mutant.CoveringTest.ContainsKey(t.Id.ToString())).ToList();
            }

            _logger.LogDebug(testCases == null
                ? $"{RunnerId}: Testing {mutant.DisplayName} against all tests."
                : $"{RunnerId}: Testing {mutant.DisplayName} against: {string.Join(", ", testCases.Select(x => x.FullyQualifiedName))}.");
            return RunVsTest(testCases, timeoutMs, envVars);
        }

        private void SetListOfTests(ICollection<TestCase> tests)
        {
            _discoveredTests = tests;
            DetectTestFramework(_discoveredTests);
        }

        public int DiscoverNumberOfTests()
        {
            return DiscoverTests().Count();
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

        private TestRunResult RunVsTest(IEnumerable<TestCase> testCases, int? timeoutMs,
            Dictionary<string, string> envVars)
        {
            var testResults = RunAllTests(testCases, envVars, GenerateRunSettings(timeoutMs, false), false);

            // For now we need to throw an OperationCanceledException when a testrun has timed out. 
            // We know the test run has timed out because we received less test results from the test run than there are test cases in the unit test project.
            var resultAsArray = testResults as TestResult[] ?? testResults.ToArray();
            if (resultAsArray.All(x => x.Outcome != TestOutcome.Failed) && resultAsArray.Count() < (testCases ?? _discoveredTests).Count())
            {
                throw new OperationCanceledException();
            }

            var testResult = new TestRunResult
            {
                Success = resultAsArray.All(tr => tr.Outcome == TestOutcome.Passed || tr.Outcome == TestOutcome.Skipped),
                FailingTests = resultAsArray.Where(tr => tr.Outcome == TestOutcome.Failed).Select(x => (TestDescription)x.TestCase).ToList(),
                ResultMessage = string.Join(
                    Environment.NewLine,
                    resultAsArray.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage))
                        .Select(tr => tr.ErrorMessage))
            };

            return testResult;
        }

        public TestRunResult CaptureCoverage(bool cantUsePipe, bool cantUseUnloadAppDomain)
        {
            _logger.LogDebug($"{RunnerId}: Capturing coverage.");
            var testResults = RunAllTests(null, BuildCoverageEnvironment( cantUsePipe || _flags.HasFlag(OptimizationFlags.UseEnvVariable)), GenerateRunSettings(null, true), true);
            ParseResultsForCoverage(testResults);
            CoveredMutants = CoverageMutants.CoveredMutants;
            return new TestRunResult { Success = true };
        }

        private void ParseResultsForCoverage(IEnumerable<TestResult> testResults)
        {
            foreach (var testResult in testResults)
            {
                var (key, value) = testResult.GetProperties().FirstOrDefault(x => x.Key.Id == "Stryker.Coverage");
                if (key == null)
                {
                    _logger.LogWarning($"{RunnerId}: Failed to retrieve coverage info for {testResult.TestCase.FullyQualifiedName}.");
                }
                else if (value != null)
                {
                    var propertyPairValue = (value as string);
                    if (string.IsNullOrWhiteSpace(propertyPairValue))
                    {
                        CoverageMutants.DeclareMappingForATest(testResult.TestCase, new List<int>(), new List<int>());
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
                        CoverageMutants.DeclareMappingForATest(testResult.TestCase, coveredMutants, staticMutants);
                    }
                }

            }
        }

        public IEnumerable<TestResult> CoverageForTest(TestCase test, bool cantUsePipe)
        {
            _logger.LogDebug($"{RunnerId}: Capturing coverage for {test.FullyQualifiedName}.");
            var generateRunSettings = GenerateRunSettings(null, true);
            var testResults = RunAllTests(_discoveredTests.Where(x => x.Id == test.Id).ToArray(), BuildCoverageEnvironment( cantUsePipe|| _flags.HasFlag(OptimizationFlags.UseEnvVariable)), generateRunSettings, true);
            var coverageForTest = testResults as TestResult[] ?? testResults.ToArray();
            ParseResultsForCoverage(coverageForTest.Where(x => x.TestCase.Id == test.Id));
            return coverageForTest;
        }

        private void Handler_TestsFailed(object sender, EventArgs e)
        {
            // one test has failed, we can stop
            _logger.LogDebug($"{RunnerId}: At least one test failed, abort current test run.");
            _vsTestConsole.AbortTestRun();
        }

        private IEnumerable<TestResult> RunAllTests(IEnumerable<TestCase> testCases, Dictionary<string, string> envVars,
            string runSettings, bool forCoverage, int retries = 0)
        {
            void Handler_VsTestFailed(object sender, EventArgs e) { _vsTestFailed = true; }

            using (var runCompleteSignal = new AutoResetEvent(false))
            {
                var eventHandler = new RunEventHandler(runCompleteSignal, _logger, $"Runner {this._id}");
                var strykerVsTestHostLauncher = _hostBuilder(envVars, _id);

                eventHandler.VsTestFailed += Handler_VsTestFailed;

                if (_flags.HasFlag(OptimizationFlags.AbortTestOnKill) && !forCoverage)
                {
                    eventHandler.TestsFailed += Handler_TestsFailed;
                }

                if (testCases != null)
                {
                    var finalTestCases = _discoveredTests.Where(discoveredTest => testCases.Any(test => test.Id == discoveredTest.Id));
                    _vsTestConsole.RunTestsWithCustomTestHost(finalTestCases, runSettings, eventHandler, strykerVsTestHostLauncher);
                }
                else
                {
                    _vsTestConsole.RunTestsWithCustomTestHost(_sources, runSettings, eventHandler, strykerVsTestHostLauncher);
                }

                // Test host exited signal comes after the run complete
                strykerVsTestHostLauncher.WaitProcessExit();

                // At this point, run must have complete. Check signal for true
                runCompleteSignal.WaitOne();

                eventHandler.TestsFailed -= Handler_TestsFailed;
                eventHandler.VsTestFailed -= Handler_VsTestFailed;

                if (_vsTestFailed && retries <= 10)
                {
                    _vsTestConsole = PrepareVsTestConsole();
                    _vsTestFailed = false;

                    return RunAllTests(testCases, envVars, runSettings, forCoverage, ++retries);
                }
                return eventHandler.TestResults;
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

            _logger.LogDebug($"{RunnerId}: VsTest logging set to {traceLevel}");
            return traceLevel;
        }

        private string GenerateRunSettings(int? timeout, bool forCoverage)
        {
            var (targetFramework, version) = _projectInfo.TestProjectAnalyzerResult.FrameworkAndVersion;
            string targetFrameworkVersion;
            switch (targetFramework)
            {
                case FrameworkKind.NetCore:
                    targetFrameworkVersion = $".NETCoreApp,Version = v{_projectInfo.TestProjectAnalyzerResult.TargetFramework}";
                    break;
                case FrameworkKind.NetStandard:
                    throw new ApplicationException("Unsupported targetframework detected. A unit test project cannot be netstandard!: " + targetFramework);
                default:
                    targetFrameworkVersion = $"Framework{version.Major}{version.Minor}";
                    break;
            }

            var needCoverage = forCoverage && NeedCoverage();
            var dataCollectorSettings = needCoverage ? CoverageCollector.GetVsTestSettings() : "";
            var settingsForCoverage = string.Empty;
            if (needCoverage)
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
            var timeoutSettings = timeout.HasValue ? $"<TestSessionTimeout>{timeout}</TestSessionTimeout>" : "";
            var runSettings =
                $@"<RunSettings>
 <RunConfiguration>
  <MaxCpuCount>{_options.ConcurrentTestrunners}</MaxCpuCount>
  <TargetFrameworkVersion>{targetFrameworkVersion}</TargetFrameworkVersion>
  {timeoutSettings}
  {settingsForCoverage}
 </RunConfiguration>{dataCollectorSettings}
</RunSettings>";

            _logger.LogDebug($"{RunnerId}: VsTest runsettings set to: {runSettings}");

            return runSettings;
        }

        private bool NeedCoverage()
        {
            return _flags.HasFlag(OptimizationFlags.CoverageBasedTest) || _flags.HasFlag(OptimizationFlags.SkipUncoveredMutants);
        }

        private IVsTestConsoleWrapper PrepareVsTestConsole()
        {
            if (_vsTestConsole != null)
            {
                try
                {
                    _vsTestConsole.EndSession();
                }
                catch { /*Ignore exception. vsTestConsole has been disposed outside of our control*/ }

                _vsTestConsole = null;
            }

            _vstestLogPath = Path.Combine(_options.OutputPath, "logs", "vstest-log.txt");
            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(_vstestLogPath));

            _logger.LogDebug($"{RunnerId}: Logging vstest output to: {_vstestLogPath}");

            return new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath(), new ConsoleParameters
            {
                TraceLevel = DetermineTraceLevel(),
                LogFilePath = _vstestLogPath
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
            _sources = new List<string>()
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
            if (!_disposedValue)
            {
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
