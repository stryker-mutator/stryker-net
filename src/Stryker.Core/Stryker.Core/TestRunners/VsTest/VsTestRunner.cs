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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;

namespace Stryker.Core.TestRunners.VsTest
{
    public class VsTestRunner : ITestRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly StrykerOptions _options;
        private readonly ProjectInfo _projectInfo;

        private readonly IVsTestConsoleWrapper _vsTestConsole;
        private ICollection<TestCase> _discoveredTests;

        private readonly VsTestHelper _vsTestHelper;
        private readonly List<string> _messages = new List<string>();


        private IEnumerable<string> _sources;

        private static ILogger _logger { get; set; }

        static VsTestRunner()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();

        }

        public VsTestRunner(StrykerOptions options, ProjectInfo projectInfo, ICollection<TestCase> testCasesDiscovered, IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
            _options = options;
            _projectInfo = projectInfo;
            _discoveredTests = testCasesDiscovered;
            _vsTestHelper = new VsTestHelper(options);

            _vsTestConsole = PrepareVsTestConsole();

            InitializeVsTestConsole();
        }

        public IEnumerable<int> CoveredMutants { get; private set; }

        public TestRunResult RunAll(int? timeoutMS, int? mutationId)
        {
            if (_discoveredTests is null)
            {
                throw new Exception("_discoveredTests cannot be null when running tests");
            }

            var envVars = new Dictionary<string, string> {["ActiveMutation"] = mutationId.ToString()};
            return RunVsTest(timeoutMS, envVars);
        }

        private TestRunResult RunVsTest(int? timeoutMS, Dictionary<string, string> envVars)
        {
            TestRunResult testResult;

                var testResults = RunAllTests(envVars, GenerateRunSettings(timeoutMS ?? 0));

            // For now we need to throw an OperationCanceledException when a testrun has timed out. 
            // We know the testrun has timed out because we received less test results from the test run than there are test cases in the unit test project.
                if (testResults.Count() < _discoveredTests.Count())
            {
                throw new OperationCanceledException();
            }

            testResult = new TestRunResult
            {
                Success = testResults.All(tr => tr.Outcome == TestOutcome.Passed),
                ResultMessage = string.Join(
                    Environment.NewLine,
                        testResults.Where(tr => !string.IsNullOrWhiteSpace(tr.ErrorMessage))
                            .Select(tr => tr.ErrorMessage)),
                    TotalNumberOfTests = _discoveredTests.Count()
            };


            return testResult;
        }

        public TestRunResult CaptureCoverage()
        {
            var mapping = new Dictionary<TestCase, IEnumerable<int>>(_discoveredTests.Count());
            foreach (var discoveredTest in _discoveredTests)
            {
                mapping[discoveredTest] = CaptureCoverage(discoveredTest);
            }

            CoveredMutants = mapping.Values.SelectMany(x => x);
            LogMapping(mapping);
            // invert mapping, i.e. identify the list of test relevant for each mutant
            var finalMapping = new Dictionary<int, ICollection<TestCase>>();
            foreach (var discoveredTest in mapping)
            {
                foreach (var mutantID in discoveredTest.Value)
                {
                    if (!finalMapping.ContainsKey(mutantID))
                    {
                        finalMapping[mutantID] = new List<TestCase> {discoveredTest.Key};
                    }
                    else
                    {
                        finalMapping[mutantID].Add(discoveredTest.Key);
                    }
                }
            }

            LogMapping(finalMapping);
            return new TestRunResult {Success = true, TotalNumberOfTests = _discoveredTests.Count()};
        }

        public IEnumerable<int> CaptureCoverage(TestCase test)
        {
            using (var coverageServer = new CoverageServer())
            {
                var envVars = new Dictionary<string, string>
                {
                    {MutantControl.EnvironmentPipeName, coverageServer.PipeName}
                };
                var testCases = new[] {test};

                using (var runCompleteSignal = new AutoResetEvent(false))
                {
                    using (var processExitedSignal = new AutoResetEvent(false))
                    {
                        var handler = new RunEventHandler(runCompleteSignal, _messages);
                        var testHostLauncher = new StrykerVsTestHostLauncher(() => processExitedSignal.Set(), envVars);

                        _vsTestConsole.RunTestsWithCustomTestHost(testCases, GenerateRunSettings(0), handler,
                            testHostLauncher);

                        // Test host exited signal comes after the run complete
                        processExitedSignal.WaitOne();
                        // At this point, run must have complete. Check signal for true
                        runCompleteSignal.WaitOne();
                    }
                }

                if (!coverageServer.WaitReception())
                {
                    _logger.LogWarning("Did not receive mutant coverage data from initial run.");
                    return null;
                }

                return coverageServer.RanMutants;
            }
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
                        _logger.LogError("Test discovery has been aborted!");
                    }

                    _discoveredTests = handler.DiscoveredTestCases;
                }
            }

            return _discoveredTests;
        }

        private void Handler_TestsFailed(object sender, EventArgs e)
        {
            // one test has failed, we can stop
            _logger.LogDebug("At least one test failed, abort current test run.");
            _vsTestConsole.AbortTestRun();
        }

        private IEnumerable<TestResult> RunAllTests(Dictionary<string, string> envVars, string runSettings)
        {
            using (var runCompleteSignal = new AutoResetEvent(false))
            {
                using (var processExitedSignal = new AutoResetEvent(false))
                {
                    var handler = new RunEventHandler(runCompleteSignal, _messages);
                    var testHostLauncher = new StrykerVsTestHostLauncher(() => processExitedSignal.Set(), envVars);

                    handler.TestsFailed += Handler_TestsFailed;
                    _vsTestConsole.RunTestsWithCustomTestHost(_sources, runSettings, handler, testHostLauncher);
                    handler.TestsFailed -= Handler_TestsFailed;

                    // Test host exited signal comes after the run complete
                    processExitedSignal.WaitOne();

                    // At this point, run must have complete. Check signal for true
                    runCompleteSignal.WaitOne();
                    return handler.TestResults;
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

            _vsTestConsole.StartSession();
            _vsTestConsole.InitializeExtensions(new List<string>
            {
                testBinariesPath,
                _vsTestHelper.GetDefaultVsTestExtensionsPath(_vsTestHelper.GetCurrentPlatformVsTestToolPath())
            });
        }

        private static void LogMapping(Dictionary<int, ICollection<TestCase>> finalMapping)
        {
            _logger.LogInformation("Mutant => Tests Coverage information");
            foreach (var (mutantId, tests) in finalMapping)
            {
                var list = new StringBuilder();
                list.AppendJoin(",", tests.Select(x => x.DisplayName));
                _logger.LogInformation($"Mutant '{mutantId}' covered by [{list}].");
            }
            _logger.LogInformation("*****************");
        }

        private static void LogMapping(Dictionary<TestCase, IEnumerable<int>> mapping)
        {
            _logger.LogInformation("Test => mutants coverage information");
            foreach (var (test, mutantIds) in mapping)
            {
                var list = new StringBuilder();
                list.AppendJoin(",", mutantIds);
                _logger.LogInformation($"Test '{test.DisplayName}' covers [{list}].");
            }
            _logger.LogInformation("*****************");
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
