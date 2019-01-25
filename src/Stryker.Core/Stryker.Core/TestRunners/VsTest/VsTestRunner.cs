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
    public partial class VsTestRunner : ITestRunner
    {
        private readonly StrykerOptions _options;
        private readonly ProjectInfo _projectInfo;
        private string _defaultRunSettings;
        private DotnetFramework _runnerFramework;
        private int _timeoutMS = 0;
        private readonly VsTestHelper _vsTestHelper;
        private List<string> _messages = new List<string>();

        public VsTestRunner(StrykerOptions options, ProjectInfo projectInfo)
        {
            _options = options;
            _projectInfo = projectInfo;
            _vsTestHelper = new VsTestHelper(options);

            GenerateRunSettings();
        }

        public TestRunResult RunAll(int? timeoutMS, int? activeMutationId)
        {
            _timeoutMS = timeoutMS ?? 0;
            GenerateRunSettings();

            var testBinariesPath = FilePathUtils.ConvertPathSeparators(Path.Combine(_options.BasePath, "bin", "Debug", _projectInfo.TargetFramework));
            var testAssemblyPath = FilePathUtils.ConvertPathSeparators(Path.Combine(testBinariesPath, _projectInfo.TestProjectFileName.Replace("csproj", "dll")));

            // Temporarily run full framework runner until core runner is fixed
            var vsTestToolPath = _vsTestHelper.GetVsTestToolPaths()[DotnetFramework.Full];
            var vsTestExtensionsPath = _vsTestHelper.GetDefaultVsTestExtensionsPath(vsTestToolPath);

            IVsTestConsoleWrapper consoleWrapper = null;

            consoleWrapper = new VsTestConsoleWrapper(vsTestToolPath, new ConsoleParameters { TraceLevel = DetermineTraceLevel(), LogFilePath = Path.Combine(_options.BasePath, "StrykerOutput", "logs", "vstest-log.txt") });

            consoleWrapper.StartSession();
            consoleWrapper.InitializeExtensions(new List<string> { testBinariesPath, vsTestExtensionsPath });

            var testCases = DiscoverTests(new List<string>() { testAssemblyPath }, consoleWrapper);

            var testResults = RunAllTests(consoleWrapper, new List<string> { testAssemblyPath }, activeMutationId);

            // For now we need to throw an OperationCanceledException when a testrun has timed out. 
            // We know the testrun has timed out because we received less test results from the test run than there are test cases in the unit test project.
            if (testResults.Count() < testCases.Count())
            {
                throw new OperationCanceledException();
            }

            var testResult = new TestRunResult
            {
                Success = testResults.All(tr => tr.Outcome == TestOutcome.Passed),
                ResultMessage = string.Join(
                    Environment.NewLine,
                    testResults.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage)).Select(tr => tr.ErrorMessage)),
                TotalNumberOfTests = testCases.Count()
            };

            consoleWrapper.EndSession();
            return testResult;
        }

        private IEnumerable<TestCase> DiscoverTests(IEnumerable<string> sources, IVsTestConsoleWrapper consoleWrapper)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new DiscoveryEventHandler(waitHandle, _messages);
            consoleWrapper.DiscoverTests(sources, _defaultRunSettings, handler);

            waitHandle.WaitOne();

            return handler.DiscoveredTestCases;
        }

        private IEnumerable<TestResult> RunSelectedTests(IVsTestConsoleWrapper consoleWrapper, IEnumerable<TestCase> testCases)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new RunEventHandler(waitHandle, _messages);
            consoleWrapper.RunTests(testCases, _defaultRunSettings, handler);

            waitHandle.WaitOne();
            return handler.TestResults;
        }

        IEnumerable<TestResult> RunAllTests(IVsTestConsoleWrapper consoleWrapper, IEnumerable<string> sources, int? activeMutationId)
        {
            var runCompleteSignal = new AutoResetEvent(false);
            var processExitedSignal = new AutoResetEvent(false);
            var handler = new RunEventHandler(runCompleteSignal, _messages);
            var testHostLauncher = new StrykerVsTestHostLauncher(() => processExitedSignal.Set(), activeMutationId);

            consoleWrapper.RunTestsWithCustomTestHost(sources, _defaultRunSettings, handler, testHostLauncher);

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

        private void GenerateRunSettings()
        {
            string targetFramework = _projectInfo.TargetFramework;
            if (targetFramework.Contains("netcoreapp") || targetFramework.Contains("netstandard"))
            {
                _runnerFramework = DotnetFramework.Core;
            }

            string targetFrameworkVersion = Regex.Replace(targetFramework, @"[^.\d]", "");
            switch (targetFramework)
            {
                case string s when s.Contains("netcoreapp"):
                    targetFrameworkVersion = $".NETCoreApp,Version = v{targetFrameworkVersion}";
                    break;
                case string s when s.Contains("netstandard"):
                    targetFrameworkVersion = $".NETCoreApp,Version = v{targetFrameworkVersion}";
                    break;
                default:
                    throw new Exception("Unsupported targetframework detected" + targetFramework);
            }

            _defaultRunSettings = $@"<RunSettings>
  <RunConfiguration>
    <MaxCpuCount>1</MaxCpuCount>
    <TargetFrameworkVersion>{targetFrameworkVersion}</TargetFrameworkVersion>
    <TestSessionTimeout>{_timeoutMS}</TestSessionTimeout>
  </RunConfiguration>
</RunSettings>";
        }
    }
}
