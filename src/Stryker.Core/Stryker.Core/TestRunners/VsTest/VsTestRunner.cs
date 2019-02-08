using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Serilog.Events;
using Stryker.Core.ExecutableFinders;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunner : ITestRunner
    {
        private readonly StrykerOptions _options;
        private readonly ProjectInfo _projectInfo;

        private readonly IVsTestConsoleWrapper _vsTestConsole;

        private readonly VsTestHelper _vsTestHelper;
        private List<string> _messages = new List<string>();

        private string _testAssemblies;
        private IEnumerable<TestCase> _testCases;

        public VsTestRunner(StrykerOptions options, ProjectInfo projectInfo)
        {
            _options = options;
            _projectInfo = projectInfo;
            _vsTestHelper = new VsTestHelper(options);

            _vsTestConsole = PrepareVsTestConsole();

            InitializeVsTestConsole();
        }

        public TestRunResult RunAll(int? timeoutMS, int? activeMutationId)
        {
            // Active Mutation ID is null on initial testrun
            if (activeMutationId == null)
            {
                _testCases = DiscoverTests(new List<string>() { _testAssemblies }, _vsTestConsole, GenerateRunSettings(0));
            }

            int timeout = timeoutMS ?? 0;

            var testResults = RunAllTests(_vsTestConsole, new List<string> { _testAssemblies }, activeMutationId, GenerateRunSettings(timeout));

            // For now we need to throw an OperationCanceledException when a testrun has timed out. 
            // We know the testrun has timed out because we received less test results from the test run than there are test cases in the unit test project.
            if (testResults.Count() < _testCases.Count())
            {
                throw new OperationCanceledException();
            }

            var testResult = new TestRunResult
            {
                Success = testResults.All(tr => tr.Outcome == TestOutcome.Passed),
                ResultMessage = string.Join(
                    Environment.NewLine,
                    testResults.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage)).Select(tr => tr.ErrorMessage)),
                TotalNumberOfTests = _testCases.Count()
            };

            return testResult;
        }

        private IEnumerable<TestCase> DiscoverTests(IEnumerable<string> sources, IVsTestConsoleWrapper consoleWrapper, string runSettings)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new DiscoveryEventHandler(waitHandle, _messages);
            consoleWrapper.DiscoverTests(sources, runSettings, handler);

            waitHandle.WaitOne();

            return handler.DiscoveredTestCases;
        }

        private IEnumerable<TestResult> RunSelectedTests(IVsTestConsoleWrapper consoleWrapper, IEnumerable<TestCase> testCases, string runSettings)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new RunEventHandler(waitHandle, _messages);
            consoleWrapper.RunTests(testCases, runSettings, handler);

            waitHandle.WaitOne();
            return handler.TestResults;
        }

        private IEnumerable<TestResult> RunAllTests(IVsTestConsoleWrapper consoleWrapper, IEnumerable<string> sources, int? activeMutationId, string runSettings)
        {
            var runCompleteSignal = new AutoResetEvent(false);
            var processExitedSignal = new AutoResetEvent(false);
            var handler = new RunEventHandler(runCompleteSignal, _messages);
            var testHostLauncher = new StrykerVsTestHostLauncher(() => processExitedSignal.Set(), activeMutationId);

            consoleWrapper.RunTestsWithCustomTestHost(sources, runSettings, handler, testHostLauncher);

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
    <MaxCpuCount>1</MaxCpuCount>
    <TargetFrameworkVersion>{targetFrameworkVersion}</TargetFrameworkVersion>
    <TestSessionTimeout>{timeout}</TestSessionTimeout>
  </RunConfiguration>
</RunSettings>";
        }

        private IVsTestConsoleWrapper PrepareVsTestConsole()
        {
            return new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath(), new ConsoleParameters
            {
                TraceLevel = DetermineTraceLevel(),
                LogFilePath = Path.Combine(_options.BasePath, "StrykerOutput", "logs", "vstest-log.txt")
            });
        }

        private void InitializeVsTestConsole()
        {
            var testBinariesPath = FilePathUtils.ConvertPathSeparators(Path.Combine(_options.BasePath, "bin", "Debug", _projectInfo.TargetFramework));
            _testAssemblies = FilePathUtils.ConvertPathSeparators(Path.Combine(testBinariesPath, _projectInfo.TestProjectFileName.Replace("csproj", "dll")));

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
