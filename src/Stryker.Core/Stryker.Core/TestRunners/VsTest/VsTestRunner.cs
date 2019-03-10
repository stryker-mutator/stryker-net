using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Serilog.Events;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.ToolHelpers;
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
        private readonly IFileSystem _fileSystem;
        private readonly StrykerOptions _options;
        private readonly ProjectInfo _projectInfo;

        private readonly IVsTestConsoleWrapper _vsTestConsole;

        private readonly VsTestHelper _vsTestHelper;
        private readonly List<string> _messages = new List<string>();


        private int _testCasesDiscovered;
        private IEnumerable<string> _sources;

        public VsTestRunner(StrykerOptions options, ProjectInfo projectInfo, IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
            _options = options;
            _projectInfo = projectInfo;
            _vsTestHelper = new VsTestHelper(projectInfo);

            _vsTestConsole = PrepareVsTestConsole();

            InitializeVsTestConsole();
        }

        public TestRunResult RunAll(int? timeoutMS, int? activeMutationId)
        {
            TestRunResult testResult = new TestRunResult() { Success = false };

            var testResults = RunAllTests(activeMutationId, GenerateRunSettings(timeoutMS ?? 0));

            // For now we need to throw an OperationCanceledException when a testrun has timed out. 
            // We know the testrun has timed out because we received less test results from the test run than there are test cases in the unit test project.
            if (testResults.Count() < _testCasesDiscovered)
            {
                throw new OperationCanceledException();
            }

            testResult = new TestRunResult
            {
                Success = testResults.All(tr => tr.Outcome == TestOutcome.Passed),
                ResultMessage = string.Join(
                    Environment.NewLine,
                    testResults.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage)).Select(tr => tr.ErrorMessage)),
                TotalNumberOfTests = _testCasesDiscovered
            };

            return testResult;
        }

        public void DiscoverTests(string runSettings = null)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new DiscoveryEventHandler(waitHandle, _messages);
            _vsTestConsole.DiscoverTests(_sources, runSettings ?? GenerateRunSettings(0), handler);

            waitHandle.WaitOne();

            _testCasesDiscovered = handler.DiscoveredTestCases.Count;
        }

        private IEnumerable<TestResult> RunSelectedTests(IEnumerable<TestCase> testCases, string runSettings)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new RunEventHandler(waitHandle, _messages);
            _vsTestConsole.RunTests(testCases, runSettings, handler);

            waitHandle.WaitOne();
            return handler.TestResults;
        }

        private IEnumerable<TestResult> RunAllTests(int? activeMutationId, string runSettings)
        {
            var runCompleteSignal = new AutoResetEvent(false);
            var processExitedSignal = new AutoResetEvent(false);
            var handler = new RunEventHandler(runCompleteSignal, _messages);
            var testHostLauncher = new StrykerVsTestHostLauncher(() => processExitedSignal.Set(), activeMutationId);

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
            string targetFramework = _projectInfo.TestProjectAnalyzerResult.TargetFramework;

            string targetFrameworkVersion = Regex.Replace(targetFramework, @"[^.\d]", "");
            switch (targetFramework)
            {
                case string s when s.Contains("netcoreapp"):
                    targetFrameworkVersion = $".NETCoreApp,Version = v{targetFrameworkVersion}";
                    break;
                case string s when s.Contains("netstandard"):
                    throw new Exception("Unsupported targetframework detected. A unit test project cannot be netstandard!: " + targetFramework);
                default:
                    targetFrameworkVersion = $"Framework40";
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
            if (_options.LogOptions.LogToFile)
            {
                var vstestLogPath = Path.Combine(_options.OutputPath, "logs", "vstest-log.txt");
                _fileSystem.Directory.CreateDirectory(Path.GetDirectoryName(vstestLogPath));

                return new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath(), new ConsoleParameters
                {
                    TraceLevel = DetermineTraceLevel(),
                    LogFilePath = vstestLogPath
                });
            }

            return new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath());
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
                FilePathUtils.ConvertPathSeparators(testBinariesPath)
            };

            _vsTestConsole.StartSession();
            _vsTestConsole.InitializeExtensions(new List<string>
            {
                testBinariesLocation,
                _vsTestHelper.GetDefaultVsTestExtensionsPath(_vsTestHelper.GetCurrentPlatformVsTestToolPath())
            });

            DiscoverTests();
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
