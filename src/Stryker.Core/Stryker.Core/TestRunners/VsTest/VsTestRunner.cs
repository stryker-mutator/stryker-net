using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Serilog.Events;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
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
using Stryker.Core.MutationTest;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunner : ITestRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly StrykerOptions _options;
        private readonly OptimizationFlags _flags;
        private readonly ProjectInfo _projectInfo;

        private readonly IVsTestConsoleWrapper _vsTestConsole;
        private ICollection<TestCase> _discoveredTests;

        private readonly VsTestHelper _vsTestHelper;
        private readonly List<string> _messages = new List<string>();
        private readonly TestCoverageInfos _coverage;

        private IEnumerable<string> _sources;

        private static ILogger Logger { get; }

        static VsTestRunner()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();
        }

        public VsTestRunner(StrykerOptions options, OptimizationFlags flags, ProjectInfo projectInfo,
            ICollection<TestCase> testCasesDiscovered,
            TestCoverageInfos mappingInfos, IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
            _options = options;
            _flags = flags;
            _projectInfo = projectInfo;
            _discoveredTests = testCasesDiscovered;
            _vsTestHelper = new VsTestHelper(options);
            _coverage = mappingInfos ?? new TestCoverageInfos();
            _vsTestConsole = PrepareVsTestConsole();
            InitializeVsTestConsole();
        }

        public IEnumerable<int> CoveredMutants { get; private set; }

        public TestCoverageInfos FinalMapping => _coverage;

        public TestRunResult RunAll(int? timeoutMs, int? mutationId)
        {
            if (_discoveredTests is null)
            {
                throw new Exception("_discoveredTests cannot be null when running tests");
            }

            var envVars = new Dictionary<string, string>();
            if (mutationId != null)
            {
                envVars["ActiveMutation"] = mutationId.ToString();
            }

            var testCases = (mutationId == null || !_flags.HasFlag(OptimizationFlags.CoverageBasedTest)) ? null : _coverage.GetTests<TestCase>(mutationId.Value);
            return RunVsTest(testCases, timeoutMs, envVars);
        }

        private TestRunResult RunVsTest(ICollection<TestCase> testCases, int? timeoutMs,
            Dictionary<string, string> envVars)
        {
            var testResults = RunAllTests(testCases, envVars, GenerateRunSettings(timeoutMs ?? 0));

            // For now we need to throw an OperationCanceledException when a testrun has timed out. 
            // We know the testrun has timed out because we received less test results from the test run than there are test cases in the unit test project.
            var resultAsArray = testResults as TestResult[] ?? testResults.ToArray();
            if (resultAsArray.All(x => x.Outcome != TestOutcome.Failed) && resultAsArray.Count() < (testCases ?? _discoveredTests).Count)
            {
                throw new OperationCanceledException();
            }

            var testResult = new TestRunResult
            {
                Success = resultAsArray.All(tr => tr.Outcome == TestOutcome.Passed),
                ResultMessage = string.Join(
                    Environment.NewLine,
                    resultAsArray.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage))
                        .Select(tr => tr.ErrorMessage)),
                TotalNumberOfTests = _discoveredTests.Count()
            };

            return testResult;
        }

        public TestRunResult CaptureCoverage()
        {
            var envVars = new Dictionary<string, string>
            {
                {MutantControl.EnvironmentCollectorMore, true.ToString()}
            };
            var testResults = RunAllTests(null, envVars, GenerateRunSettings( 0));
            foreach (var testResult in testResults)
            {
                var propertyPair = testResult.GetProperties().First(x => x.Key.Id == "Stryker.Coverage");
                if (propertyPair.Value != null)
                {
                    var coverage = (propertyPair.Value as string).Split(',').Select(int.Parse);
                    _coverage.DeclareMappingForATest(testResult.TestCase, coverage);
                }
                else
                {
                    Logger.LogWarning("No ");
                }
               
            }

            CoveredMutants = _coverage.CoveredMutants;
            return new TestRunResult { Success = true, TotalNumberOfTests = _discoveredTests.Count };
        }

        public ICollection<TestCase> DiscoverTests(string runSettings = null)
        {
            if (_discoveredTests == null)
            {
                using (var waitHandle = new AutoResetEvent(false))
                {
                    var handler = new DiscoveryEventHandler(waitHandle, _messages);
                    _vsTestConsole.DiscoverTests(_sources, runSettings ?? GenerateRunSettings(0), handler);

                    waitHandle.WaitOne();
                    if (handler.Aborted)
                    {
                        Logger.LogError("Test discovery has been aborted!");
                    }

                    _discoveredTests = handler.DiscoveredTestCases;
                }
            }

            return _discoveredTests;
        }

        private void Handler_TestsFailed(object sender, EventArgs e)
        {
            // one test has failed, we can stop
            Logger.LogDebug("At least one test failed, abort current test run.");
            _vsTestConsole.AbortTestRun();
        }

        private IEnumerable<TestResult> RunAllTests(ICollection<TestCase> testCases, Dictionary<string, string> envVars,
            string runSettings)
        {
            using (var runCompleteSignal = new AutoResetEvent(false))
            {
                using (var processExitedSignal = new AutoResetEvent(false))
                {
                    var handler = new RunEventHandler(runCompleteSignal, _messages);
                    var testHostLauncher = new StrykerVsTestHostLauncher(() => processExitedSignal.Set(), envVars);
                    if (_flags.HasFlag(OptimizationFlags.AbortTestOnKill))
                    {
                        handler.TestsFailed += Handler_TestsFailed;
                    }
                    if (testCases != null)
                    {
                        _vsTestConsole.RunTestsWithCustomTestHost(testCases, runSettings, handler, testHostLauncher);
                    }
                    else
                    {
                        _vsTestConsole.RunTestsWithCustomTestHost(_sources, runSettings, handler, testHostLauncher);
                    }

                    if (_flags.HasFlag(OptimizationFlags.AbortTestOnKill))
                    {
                        handler.TestsFailed -= Handler_TestsFailed;
                    }

                    // Test host exited signal comes after the run complete
                    processExitedSignal.WaitOne();

                    // At this point, run must have complete. Check signal for true
                    runCompleteSignal.WaitOne();

                    // dump coverage
                    var results = handler.TestResults;
                    return results;
                }
            }
        }

        private TraceLevel DetermineTraceLevel()
        {
            switch (_options.LogOptions.LogLevel)
            {
                case LogEventLevel.Debug:
                case LogEventLevel.Verbose:
                    return TraceLevel.Verbose;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return TraceLevel.Error;
                case LogEventLevel.Warning:
                    return TraceLevel.Warning;
                case LogEventLevel.Information:
                    return TraceLevel.Info;
                default:
                    return TraceLevel.Off;
            }
        }

        private string GenerateRunSettings(int timeout)
        {
            var targetFramework = _projectInfo.TargetFramework;

            var targetFrameworkVersion = Regex.Replace(targetFramework, @"[^.\d]", "");
            switch (targetFramework)
            {
                case string s when s.Contains("netcoreapp"):
                    targetFrameworkVersion = $".NETCoreApp,Version = v{targetFrameworkVersion}";
                    break;
                case string s when s.Contains("netstandard"):
                    throw new Exception("Unsupported targetframework detected. A unit test project cannot be netstandard!: " + targetFramework);
                default:
                    targetFrameworkVersion = $".NETFramework = v{targetFrameworkVersion}";
                    break;
            }

            var dataCollectorSettings = CoverageCollector.GetVsTestSettings();
            return $@"<RunSettings>
  <RunConfiguration>
    <MaxCpuCount>{_options.ConcurrentTestrunners}</MaxCpuCount>
    <TargetFrameworkVersion>{targetFrameworkVersion}</TargetFrameworkVersion>
    <TestSessionTimeout>{timeout}</TestSessionTimeout>
  </RunConfiguration>
   {dataCollectorSettings}
</RunSettings>";
        }

        private IVsTestConsoleWrapper PrepareVsTestConsole()
        {
            var logPath = FilePathUtils.ConvertPathSeparators(Path.Combine(_options.OutputPath, "vstest", "vstest-log.txt"));
            _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(logPath));

            return new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath(), new ConsoleParameters
            {
                TraceLevel = DetermineTraceLevel(),
                LogFilePath = logPath
            });
        }

        private void InitializeVsTestConsole()
        {
            var testBinariesPath = FilePathUtils.ConvertPathSeparators(Path.Combine(_options.BasePath, "bin", "Debug", _projectInfo.TargetFramework));
            _sources = new List<string>()
            {
                FilePathUtils.ConvertPathSeparators(Path.Combine(testBinariesPath, _projectInfo.TestProjectFileName.Replace("csproj", "dll")))
            };
            //var mine = Path.GetDirectoryName(typeof(CoverageCollector).Assembly.Location);
            _vsTestConsole.StartSession();
            _vsTestConsole.InitializeExtensions(new List<string>
            {
//                mine,
                testBinariesPath,
                _vsTestHelper.GetDefaultVsTestExtensionsPath(_vsTestHelper.GetCurrentPlatformVsTestToolPath())
            });
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _vsTestConsole.EndSession();
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
