using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Stryker.Core.ExecutableFinders;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.Options.Values;
using System;
using System.Collections.Generic;
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
        private string _defaultRunSettings;
        private DotnetFramework _runnerFramework;
        private int? _timeoutMS = null;
        private readonly VsTestHelper _vsTestHelper;
        private static List<string> messages = new List<string>();

        public VsTestRunner(StrykerOptions options, ProjectInfo projectInfo)
        {
            _options = options;
            _projectInfo = projectInfo;
            _vsTestHelper = new VsTestHelper(options);

            GenerateRunSettings();
        }

        public TestRunResult RunAll(int? timeoutMS, int? activeMutationId)
        {
            _timeoutMS = timeoutMS;
            GenerateRunSettings();

            var testBinariesPath = FilePathUtils.ConvertPathSeparators(Path.Combine(_options.BasePath, "bin", "Debug", _projectInfo.TargetFramework));
            var testAssemblyPath = FilePathUtils.ConvertPathSeparators(Path.Combine(testBinariesPath, _projectInfo.TestProjectFileName.Replace("csproj", "dll")));
            var vsTestToolPath = _vsTestHelper.GetVsTestToolPaths()[_runnerFramework];
            var vsTestExtensionsPath = _vsTestHelper.GetDefaultVsTestExtensionsPath(vsTestToolPath);

            IVsTestConsoleWrapper consoleWrapper = null;

            consoleWrapper = new VsTestConsoleWrapper(vsTestToolPath, new ConsoleParameters { TraceLevel = System.Diagnostics.TraceLevel.Info, LogFilePath = Path.Combine(_options.BasePath, "StrykerOutput", "logs", "vstest-log.txt") });

            consoleWrapper.StartSession();
            consoleWrapper.InitializeExtensions(new List<string> { testBinariesPath, vsTestExtensionsPath });

            var testCases = DiscoverTests(new List<string>() { testAssemblyPath }, consoleWrapper);

            var testResults = RunAllTests(consoleWrapper, new List<string> { testAssemblyPath }, activeMutationId);

            string resultMessage = null;
            if (testResults.Select(tr => string.Join(Environment.NewLine, tr.Messages.Select(m => m.Text))) is var testResultsMessages && testResultsMessages.Any())
            {
                resultMessage = string.Join(Environment.NewLine, testResultsMessages);
            }

            var testResult = new TestRunResult
            {
                Success = testResults.All(tr => tr.Outcome == TestOutcome.Passed),
                ResultMessage = resultMessage ?? "",
                TotalNumberOfTests = testCases.Count()
            };

            consoleWrapper.EndSession();
            return testResult;
        }

        IEnumerable<TestCase> DiscoverTests(IEnumerable<string> sources, IVsTestConsoleWrapper consoleWrapper)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new DiscoveryEventHandler(waitHandle);
            consoleWrapper.DiscoverTests(sources, _defaultRunSettings, handler);

            waitHandle.WaitOne();

            return handler.DiscoveredTestCases;
        }

        IEnumerable<TestResult> RunSelectedTests(IVsTestConsoleWrapper consoleWrapper, IEnumerable<TestCase> testCases)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new RunEventHandler(waitHandle);
            consoleWrapper.RunTests(testCases, _defaultRunSettings, handler);

            waitHandle.WaitOne();
            return handler.TestResults;
        }

        IEnumerable<TestResult> RunAllTests(IVsTestConsoleWrapper consoleWrapper, IEnumerable<string> sources, int? activeMutationId)
        {
            var runCompleteSignal = new AutoResetEvent(false);
            var processExitedSignal = new AutoResetEvent(false);
            var handler = new RunEventHandler(runCompleteSignal);
            var testHostLauncher = new StrykerVsTestHostLauncher(() => processExitedSignal.Set(), activeMutationId);

            consoleWrapper.RunTestsWithCustomTestHost(sources, _defaultRunSettings, handler, testHostLauncher);

            // Test host exited signal comes after the run complete
            processExitedSignal.WaitOne();

            // At this point, run must have complete. Check signal for true
            runCompleteSignal.WaitOne();

            return handler.TestResults;
        }

        IEnumerable<TestResult> RunAllTestsWithTestCaseFilter(IVsTestConsoleWrapper consoleWrapper, IEnumerable<string> sources)
        {
            var waitHandle = new AutoResetEvent(false);
            var handler = new RunEventHandler(waitHandle);
            consoleWrapper.RunTests(sources, _defaultRunSettings, new TestPlatformOptions() { TestCaseFilter = "FullyQualifiedName=UnitTestProject.UnitTest.PassingTest" }, handler);

            waitHandle.WaitOne();
            return handler.TestResults;
        }

        private void GenerateRunSettings()
        {
            string targetFramework = _projectInfo.TargetFramework;
            if (targetFramework.Contains("netcoreapp") || targetFramework.Contains("netstandard"))
            {
                _runnerFramework = DotnetFramework.NetCore;
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
    {(_timeoutMS is null ? "" : $"<TestSessionTimeout>{_timeoutMS}</TestSessionTimeout>")}
  </RunConfiguration>
</RunSettings>";
        }

        public class DiscoveryEventHandler : ITestDiscoveryEventsHandler
        {
            private AutoResetEvent waitHandle;

            public List<TestCase> DiscoveredTestCases { get; private set; }

            public DiscoveryEventHandler(AutoResetEvent waitHandle)
            {
                this.waitHandle = waitHandle;
                DiscoveredTestCases = new List<TestCase>();
            }

            public void HandleDiscoveredTests(IEnumerable<TestCase> discoveredTestCases)
            {
                if (discoveredTestCases != null)
                {
                    DiscoveredTestCases.AddRange(discoveredTestCases);
                }
            }

            public void HandleDiscoveryComplete(long totalTests, IEnumerable<TestCase> lastChunk, bool isAborted)
            {
                if (lastChunk != null)
                {
                    DiscoveredTestCases.AddRange(lastChunk);
                }
                waitHandle.Set();
            }

            public void HandleRawMessage(string rawMessage)
            {
                messages.Add("Discovery Raw:" + rawMessage);
            }

            public void HandleLogMessage(TestMessageLevel level, string message)
            {
                messages.Add("Discovery Clean:" + message);
            }
        }

        public class RunEventHandler : ITestRunEventsHandler
        {
            private AutoResetEvent waitHandle;

            public List<TestResult> TestResults { get; private set; }

            public RunEventHandler(AutoResetEvent waitHandle)
            {
                this.waitHandle = waitHandle;
                TestResults = new List<TestResult>();
            }

            public void HandleTestRunComplete(
                TestRunCompleteEventArgs testRunCompleteArgs,
                TestRunChangedEventArgs lastChunkArgs,
                ICollection<AttachmentSet> runContextAttachments,
                ICollection<string> executorUris)
            {
                if (lastChunkArgs != null && lastChunkArgs.NewTestResults != null)
                {
                    TestResults.AddRange(lastChunkArgs.NewTestResults);
                }

                waitHandle.Set();
            }

            public void HandleTestRunStatsChange(TestRunChangedEventArgs testRunChangedArgs)
            {
                if (testRunChangedArgs != null && testRunChangedArgs.NewTestResults != null)
                {
                    TestResults.AddRange(testRunChangedArgs.NewTestResults);
                }
            }

            public int LaunchProcessWithDebuggerAttached(TestProcessStartInfo testProcessStartInfo)
            {
                throw new NotImplementedException();
            }

            public void HandleRawMessage(string rawMessage)
            {
                messages.Add("Run Raw:" + rawMessage);
            }

            public void HandleLogMessage(TestMessageLevel level, string message)
            {
                messages.Add("Run Clean:" + message);
            }
        }
    }
}
