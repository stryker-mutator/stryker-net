using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using NuGet.Frameworks;
using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ToolHelpers;
using Stryker.DataCollector;

namespace Stryker.Core.TestRunners.VsTest
{
    /// <summary>
    /// Store needed data for proper VsTest     
    /// </summary>
    public sealed class VsTestContextInformation : IDisposable
    {
        private bool _disposed;
        private readonly StrykerOptions _options;
        private readonly ProjectInfo _projectInfo;
        private readonly TestSet _tests = new();
        private IDictionary<Guid, VsTestDescription> _vsTests;
        private readonly IVsTestHelper _vsTestHelper;
        private readonly IFileSystem _fileSystem;
        private List<string> _sources;
        private readonly ILogger _logger;
        private TestFramework _testFramework;
        private readonly Func<string, IVsTestConsoleWrapper> _wrapperBuilder;

        public VsTestContextInformation(StrykerOptions options,
            ProjectInfo projectInfo,
            IVsTestHelper helper,
            IFileSystem fileSystem = null,
            Func<string, IVsTestConsoleWrapper> builder = null,
            ILogger logger = null)
        {
            _options = options;
            _projectInfo = projectInfo;
            _vsTestHelper = helper;
            _fileSystem = fileSystem ?? new FileSystem();
            _wrapperBuilder = builder ?? BuildActualVsTestWrapper;
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestContextInformation>();
        }

        public IDictionary<Guid, VsTestDescription> VsTests => _vsTests;

        public IEnumerable<string> TestSources => _sources;

        public TestSet Tests => _tests;

        public ProjectInfo ProjectInfo => _projectInfo;

        public StrykerOptions Options => _options;

        public bool CantUseStrykerDataCollector() =>
            _projectInfo.TestProjectAnalyzerResults.Select(x => x.GetNuGetFramework()).Any(t =>
                t.Framework == FrameworkConstants.FrameworkIdentifiers.NetCoreApp && t.Version.Major < 2);

        public IVsTestConsoleWrapper BuildVsTestWrapper(string runnerId) => _wrapperBuilder(runnerId);

        private IVsTestConsoleWrapper BuildActualVsTestWrapper(string runnerId)
        {
            var vsTestConsole = new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath(), DetermineConsoleParameters(runnerId));
            try
            {
                // Set roll forward on no candidate fx so vstest console can start on incompatible dotnet core runtimes
                Environment.SetEnvironmentVariable("DOTNET_ROLL_FORWARD_ON_NO_CANDIDATE_FX", "2");
                vsTestConsole.StartSession();
                vsTestConsole.InitializeExtensions(_sources.Select(_fileSystem.Path.GetDirectoryName));
            }
            catch (Exception e)
            {
                throw new GeneralStrykerException("Stryker failed to connect to vstest.console", e);
            }

            return vsTestConsole;
        }

        private ConsoleParameters DetermineConsoleParameters(string runnerId)
        {
            if (_options.LogOptions?.LogToFile != true)
            {
                return new ConsoleParameters();
            }
            var vsTestLogPath = _fileSystem.Path.Combine(_options.OutputPath, "logs", $"{runnerId}_VsTest-log.txt");
            _fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(vsTestLogPath));

            return new ConsoleParameters
            {
                TraceLevel = _options.LogOptions.LogLevel switch
                {
                    LogEventLevel.Debug => TraceLevel.Verbose,
                    LogEventLevel.Verbose => TraceLevel.Verbose,
                    LogEventLevel.Error => TraceLevel.Error,
                    LogEventLevel.Fatal => TraceLevel.Error,
                    LogEventLevel.Warning => TraceLevel.Warning,
                    LogEventLevel.Information => TraceLevel.Info,
                    _ => TraceLevel.Off
                },
                LogFilePath = vsTestLogPath
            };
        }

        public void Initialize()
        {
            var testBinariesPaths = _projectInfo.TestProjectAnalyzerResults.Select(testProject => testProject.GetAssemblyPath()).ToList();
            _sources = new List<string>();

            foreach (var path in testBinariesPaths)
            {
                if (!_fileSystem.File.Exists(path))
                {
                    throw new GeneralStrykerException($"The test project binaries could not be found at {path}, exiting...");
                }

                _sources.Add(path);
            }

            var wrapper = BuildVsTestWrapper("TestDiscoverer");
            var messages = new List<string>();
            using (var waitHandle = new AutoResetEvent(false))
            {
                var handler = new DiscoveryEventHandler(waitHandle, messages);
                wrapper.DiscoverTests(_sources, GenerateRunSettingsForDiscovery(), handler);

                waitHandle.WaitOne();
                if (handler.Aborted)
                {
                    _logger.LogError("TestDiscoverer: Test discovery has been aborted!");
                }

                _vsTests = new Dictionary<Guid, VsTestDescription>(handler.DiscoveredTestCases.Count);
                foreach (var testCase in handler.DiscoveredTestCases)
                {
                    if (!_vsTests.ContainsKey(testCase.Id))
                    {
                        _vsTests[testCase.Id] = new VsTestDescription(testCase);
                    }

                    _vsTests[testCase.Id].AddSubCase();
                }
                DetectTestFrameworks(_vsTests.Values);
            }

            wrapper.EndSession();
            _tests.RegisterTests(_vsTests.Values.Select(t => t.Description));
        }

        private void DetectTestFrameworks(ICollection<VsTestDescription> tests)
        {
            if (tests == null)
            {
                _testFramework = 0;
                return;
            }
            if (tests.Any(testCase => testCase.Framework == TestFramework.nUnit))
            {
                _testFramework |= TestFramework.nUnit;
            }
            if (tests.Any(testCase => testCase.Framework == TestFramework.xUnit))
            {
                _testFramework |= TestFramework.xUnit;
            }
        }

        private string GenerateRunSettingsForDiscovery()
        {
            var testCaseFilter = string.IsNullOrWhiteSpace(_options.TestCaseFilter) ?
                string.Empty : $"<TestCaseFilter>{_options.TestCaseFilter}</TestCaseFilter>" + Environment.NewLine;

            return $@"<RunSettings>
 <RunConfiguration>
  <CollectSourceInformation>false</CollectSourceInformation>
  <MaxCpuCount>{_options.Concurrency}</MaxCpuCount>
  <DesignMode>true</DesignMode>
{testCaseFilter}
 </RunConfiguration>
</RunSettings>";
        }

        public string GenerateRunSettings(int? timeout, bool forCoverage, Dictionary<int, ITestGuids> mutantTestsMap)
        {
            var projectAnalyzerResult = _projectInfo.TestProjectAnalyzerResults.FirstOrDefault();
            var dataCollectorSettings = CoverageCollector.GetVsTestSettings(
                    forCoverage && (_options.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest) || _options.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants)),
                    mutantTestsMap?.Select(e => (e.Key, e.Value.GetGuids())),
                    CodeInjection.HelperNamespace);
            var settingsForCoverage = string.Empty;
            var needSequentialRun = forCoverage || mutantTestsMap is { Count: > 1 };

            
            if (_testFramework.HasFlag(TestFramework.nUnit))
            {
                settingsForCoverage = "<CollectDataForEachTestSeparately>true</CollectDataForEachTestSeparately>";
            }

            if (_testFramework.HasFlag(TestFramework.xUnit) && needSequentialRun)
            {
                settingsForCoverage += "<DisableParallelization>true</DisableParallelization>";
            }

            var timeoutSettings = timeout is > 0 ? $"<TestSessionTimeout>{timeout}</TestSessionTimeout>" + Environment.NewLine : string.Empty;

            var testCaseFilter = string.IsNullOrWhiteSpace(_options.TestCaseFilter) ?
                string.Empty : $"<TestCaseFilter>{_options.TestCaseFilter}</TestCaseFilter>" + Environment.NewLine;

            // we need to block parallel run to capture coverage and when testing multiple mutants in a single run
            var optionsConcurrentTestRunners = needSequentialRun ? 1 : _options.Concurrency;
            var runSettings =
$@"<RunSettings>
<RunConfiguration>
  <CollectSourceInformation>false</CollectSourceInformation>
{(projectAnalyzerResult.TargetsFullFramework() ? "<DisableAppDomain>true</DisableAppDomain>" : "")}
  <MaxCpuCount>{optionsConcurrentTestRunners}</MaxCpuCount>
{timeoutSettings}
{settingsForCoverage}
<DesignMode>true</DesignMode>
{testCaseFilter}
 </RunConfiguration>
{dataCollectorSettings}
</RunSettings>";

            return runSettings;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _vsTestHelper.Cleanup();
        }
    }
}
