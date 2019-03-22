using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Serilog.Events;
using Stryker.Core.Initialisation;
using Stryker.Core.Logging;
using Stryker.Core.MutationTest;
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
        private readonly CoverageServer _coverageServer;

        private IEnumerable<string> _sources;

        private static ILogger Logger { get; set; }

        static VsTestRunner()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<DotnetTestRunner>();
        }

        public VsTestRunner(int id, StrykerOptions options, OptimizationFlags flags, ProjectInfo projectInfo,
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
            _coverageServer = new CoverageServer($"Coverage{id}");
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
            var mapping = new Dictionary<TestCase, IEnumerable<int>>(_discoveredTests.Count());
            foreach (var discoveredTest in _discoveredTests)
            {
                var captureCoverage = CaptureCoverage(discoveredTest);
                mapping[discoveredTest] = captureCoverage;
                _coverage.DeclareMappingForATest(discoveredTest, captureCoverage);
            }

            CoveredMutants = mapping.Values.SelectMany(x => x);
            LogMapping(mapping);

            _coverage.Log();
            return new TestRunResult { Success = true, TotalNumberOfTests = _discoveredTests.Count };
        }

        public IEnumerable<int> CaptureCoverage(TestCase test)
        {
            var envVars = new Dictionary<string, string>
            {
                {MutantControl.EnvironmentPipeName, _coverageServer.PipeName}
            };
            var testCases = new[] { test };
            Logger.LogInformation($"Running test {test.DisplayName}");
            var coverageOk = false;
            var attempts = 0;
            do
            {
                _coverageServer.Clear();
                using (var runCompleteSignal = new AutoResetEvent(false))
                {
                    using (var processExitedSignal = new AutoResetEvent(false))
                    {
                        var handler = new RunEventHandler(runCompleteSignal, _messages);
                        var testHostLauncher =
                            new StrykerVsTestHostLauncher(() => processExitedSignal.Set(), envVars);

                        _vsTestConsole.RunTestsWithCustomTestHost(testCases, GenerateRunSettings(0), handler,
                            testHostLauncher);

                        // Test host exited signal comes after the run complete
                        processExitedSignal.WaitOne();
                        // At this point, run must have complete. Check signal for true
                        runCompleteSignal.WaitOne();
                        if (handler.TestResults.Count != 1)
                        {
                            Logger.LogWarning(
                                $"{test.DisplayName}: Did not get the expected number of test results: {handler.TestResults.Count}");
                        }
                        else if (handler.TestResults[0].Outcome != TestOutcome.Passed)
                        {
                            Logger.LogWarning(
                                $"{test.DisplayName}: did not pass: {handler.TestResults[0].ErrorMessage}");
                        }
                    }
                }

                coverageOk = _coverageServer.WaitReception();
            } while (attempts++ < 2 && !coverageOk);

            if (!coverageOk)
            {
                Logger.LogWarning($"Did not receive mutant coverage data for test {test.DisplayName}.");
                return null;
            }
            return _coverageServer.RanMutants;
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
     <DataCollectionRunSettings>
        <DataCollectors>
            <DataCollector friendlyName=""StrykerCoverageCollector"" />
        </DataCollectors>
    </DataCollectionRunSettings>
</RunSettings>";
            /*
   <InProcDataCollectionRunSettings>  
    <InProcDataCollectors>
      <InProcDataCollector {InProcCoverageCollector.GetVsTestSettings()} >
      </InProcDataCollector>
    </InProcDataCollectors>
  </InProcDataCollectionRunSettings>
             */
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


        private static void LogMapping(IDictionary<TestCase, IEnumerable<int>> mapping)
        {
            Logger.LogInformation("Test => mutants coverage information");
            foreach (var (test, mutantIds) in mapping)
            {
                var list = new StringBuilder();
                list.AppendJoin(",", mutantIds);
                Logger.LogInformation($"Test '{test.DisplayName}' covers [{list}].");
            }
            Logger.LogInformation("*****************");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _coverageServer.Dispose();
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
