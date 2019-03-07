using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Serilog.Events;
using Stryker.Core.ToolHelpers;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunner : ITestRunner
    {
        public bool Running { get; private set; }
        private readonly object runLock = new object();

        private readonly StrykerOptions _options;
        private readonly ProjectInfo _projectInfo;

        private readonly IVsTestConsoleWrapper _vsTestConsole;

        private readonly VsTestHelper _vsTestHelper;
        private readonly List<string> _messages = new List<string>();


        private readonly int? _testCasesDiscovered;
        private IEnumerable<string> _sources;

        private static ILogger _logger { get; set; }

        static VsTestRunner()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();

        }

        public VsTestRunner(StrykerOptions options, ProjectInfo projectInfo, int? testCasesDiscovered)
        {
            _options = options;
            _projectInfo = projectInfo;
            _testCasesDiscovered = testCasesDiscovered;
            _vsTestHelper = new VsTestHelper(options);

            _vsTestConsole = PrepareVsTestConsole();

            InitializeVsTestConsole();
        }

        public IEnumerable<int> CoveredMutants { get; private set; }

        public TestRunResult RunAll(int? timeoutMS, int ?mutationId)
        {
            if (_testCasesDiscovered is null)
            {
                throw new Exception("_testCasesDiscovered cannot be null when running tests");
            }

            var envVars = new Dictionary<string, string>{["ActiveMutation"] = mutationId.ToString()};
            return RunVsTest(timeoutMS, envVars);
        }

        private TestRunResult RunVsTest(int? timeoutMS, Dictionary<string, string> envVars)
        {
            TestRunResult testResult = new TestRunResult {Success = false};
            lock (runLock)
            {
                Running = true;

                var testResults = RunAllTests(envVars, GenerateRunSettings(timeoutMS ?? 0));

                // For now we need to throw an OperationCanceledException when a testrun has timed out. 
                // We know the testrun has timed out because we received less test results from the test run than there are test cases in the unit test project.
                if (testResults.Count() < _testCasesDiscovered.Value)
                {
                    throw new OperationCanceledException();
                }

                testResult = new TestRunResult
                {
                    Success = testResults.All(tr => tr.Outcome == TestOutcome.Passed),
                    ResultMessage = string.Join(
                        Environment.NewLine,
                        testResults.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage)).Select(tr => tr.ErrorMessage)),
                    TotalNumberOfTests = _testCasesDiscovered.Value
                };

                Running = false;
            }

            return testResult;
        }

        public TestRunResult CaptureCoverage()
        {
            using (var coverageServer = new CoverageServer())
            {
                var envVars = new Dictionary<string, string>
                {
                    {MutantControl.EnvironmentPipeName, coverageServer.PipeName}
                };
                var result = RunVsTest(null, envVars);
                if (!coverageServer.WaitReception())
                {
                    _logger.LogWarning("Did not receive mutant coverage data from initial run.");
                }
                else
                {
                    CoveredMutants = coverageServer.RanMutants;
                }

                return result;
            }
        }

        public IEnumerable<TestCase> DiscoverTests(string runSettings = null)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new DiscoveryEventHandler(waitHandle, _messages);
            _vsTestConsole.DiscoverTests(_sources, runSettings ?? GenerateRunSettings(0), handler);

            waitHandle.WaitOne();

            return handler.DiscoveredTestCases;
        }

        private IEnumerable<TestResult> RunSelectedTests(IEnumerable<TestCase> testCases, string runSettings)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new RunEventHandler(waitHandle, _messages);
            _vsTestConsole.RunTests(testCases, runSettings, handler);

            waitHandle.WaitOne();
            return handler.TestResults;
        }
            
        private IEnumerable<TestResult> RunAllTests(Dictionary<string, string> envVars, string runSettings)
        {
            var runCompleteSignal = new AutoResetEvent(false);
            var processExitedSignal = new AutoResetEvent(false);
            var handler = new RunEventHandler(runCompleteSignal, _messages);
            var testHostLauncher = new StrykerVsTestHostLauncher(() => processExitedSignal.Set(), envVars);

            _vsTestConsole.RunTestsWithCustomTestHost(_sources, runSettings, handler, testHostLauncher);

            // Test host exited signal comes after the run complete
            processExitedSignal.WaitOne();

            // At this point, run must have complete. Check signal for true
            runCompleteSignal.WaitOne();

            return handler.TestResults;
        }

        private TraceLevel DetermineTraceLevel()
        {
            switch (_options.LogOptions.LogLevel)
            {
                case LogEventLevel.Debug:
                case LogEventLevel.Verbose:
                    return TraceLevel.Verbose;
                case LogEventLevel.Error:
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
            string targetFramework = _projectInfo.TargetFramework;

            string targetFrameworkVersion = Regex.Replace(targetFramework, @"[^.\d]", "");
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

            return $@"<RunSettings>
  <RunConfiguration>
    <MaxCpuCount>{_options.ConcurrentTestrunners}</MaxCpuCount>
    <TargetFrameworkVersion>{targetFrameworkVersion}</TargetFrameworkVersion>
    <TestSessionTimeout>{timeout}</TestSessionTimeout>
  </RunConfiguration>
</RunSettings>";
        }

        private IVsTestConsoleWrapper PrepareVsTestConsole()
        {
            var logFilePath = Path.Combine(_options.BasePath, "StrykerOutput", "logs", "vstest-log.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            return new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath(), new ConsoleParameters
            {
                TraceLevel = DetermineTraceLevel(),
                LogFilePath = logFilePath
            });
        }

        private void InitializeVsTestConsole()
        {
            var testBinariesPath = FilePathUtils.ConvertPathSeparators(Path.Combine(_options.BasePath, "bin", "Debug", _projectInfo.TargetFramework));
            _sources = new List<string>()
            {
                FilePathUtils.ConvertPathSeparators(Path.Combine(testBinariesPath, _projectInfo.TestProjectFileName.Replace("csproj", "dll")))
            };

            _vsTestConsole.StartSession();
            _vsTestConsole.InitializeExtensions(new List<string>
            {
                testBinariesPath,
                _vsTestHelper.GetDefaultVsTestExtensionsPath(_vsTestHelper.GetCurrentPlatformVsTestToolPath())
            });
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _vsTestConsole.EndSession();
                }

                disposedValue = true;
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
