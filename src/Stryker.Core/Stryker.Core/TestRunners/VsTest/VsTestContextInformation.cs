using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ToolHelpers;
using Stryker.DataCollector;

namespace Stryker.Core.TestRunners.VsTest
{
    /// <summary>
    ///     Handles VsTest setup and configuration.
    /// </summary>
    public sealed class VsTestContextInformation : IDisposable
    {
        private readonly IFileSystem _fileSystem;
        private readonly Func<string, IStrykerTestHostLauncher> _hostBuilder;
        private readonly ILogger _logger;
        private readonly bool _ownVsTestHelper;
        private readonly IVsTestHelper _vsTestHelper;
        private readonly Func<ConsoleParameters, IVsTestConsoleWrapper> _wrapperBuilder;
        private bool _disposed;
        private TestFrameworks _testFramework;

        /// <summary>
        ///     Creates an instance.
        /// </summary>
        /// <param name="options">Configuration options</param>
        /// <param name="helper"></param>
        /// <param name="fileSystem"></param>
        /// <param name="builder"></param>
        /// <param name="hostBuilder"></param>
        /// <param name="logger"></param>
        public VsTestContextInformation(StrykerOptions options,
            IVsTestHelper helper = null,
            IFileSystem fileSystem = null,
            Func<ConsoleParameters, IVsTestConsoleWrapper> builder = null,
            Func<string, IStrykerTestHostLauncher> hostBuilder = null,
            ILogger logger = null)
        {
            Options = options;
            _ownVsTestHelper = helper == null;
            _fileSystem = fileSystem ?? new FileSystem();
            _vsTestHelper = helper ?? new VsTestHelper(_fileSystem, logger);
            _wrapperBuilder = builder ?? BuildActualVsTestWrapper;
            var devMode = options.DevMode;
            _hostBuilder = hostBuilder ?? (name => new StrykerVsTestHostLauncher(name, devMode));
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestContextInformation>();
        }

        /// <summary>
        ///     Discovered tests (VsTest format)
        /// </summary>
        public IDictionary<Guid, VsTestDescription> VsTests { get; private set; }

        public IDictionary<string, ISet<Guid>> TestsPerSource { get; } = new Dictionary<string, ISet<Guid>>();

        /// <summary>
        ///     Tests (Stryker format)
        /// </summary>
        public TestSet Tests { get; } = new();

        /// <summary>
        ///    Stryker options
        /// </summary>
        public StrykerOptions Options { get; }

        /// <summary>
        ///     Log folder path
        /// </summary>
        public string LogPath =>
            Options.OutputPath == null ? "logs" : _fileSystem.Path.Combine(Options.OutputPath, "logs");

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (_ownVsTestHelper)
            {
                _vsTestHelper.Cleanup();
            }
        }

        /// <summary>
        ///     Starts a new VsTest instance and returns a wrapper to control it.
        /// </summary>
        /// <param name="runnerId">Name of the instance to create (used in log files)</param>
        /// <returns>an <see cref="IVsTestConsoleWrapper" /> controlling the created instance.</returns>
        public IVsTestConsoleWrapper BuildVsTestWrapper(string runnerId)
        {
            var vsTestConsole = _wrapperBuilder(DetermineConsoleParameters(runnerId));
            try
            {
                // Set roll forward on no candidate fx so vstest console can start on incompatible dotnet core runtimes
                Environment.SetEnvironmentVariable("DOTNET_ROLL_FORWARD_ON_NO_CANDIDATE_FX", "2");
                vsTestConsole.StartSession();
                vsTestConsole.InitializeExtensions(Enumerable.Empty<string>());
            }
            catch (Exception e)
            {
                _logger.LogError("Stryker failed to connect to vstest.console with error: {error}", e.Message);
                throw new GeneralStrykerException("Stryker failed to connect to vstest.console", e);
            }
            return vsTestConsole;
        }
        
        /// <summary>
        ///     Builds a new process launcher used for a test session.
        /// </summary>
        /// <param name="runnerId">Name of the instance to create (used in log files)</param>
        /// <returns>an <see cref="IStrykerTestHostLauncher" /> </returns>
        public IStrykerTestHostLauncher BuildHostLauncher(string runnerId) => _hostBuilder(runnerId);

        private IVsTestConsoleWrapper BuildActualVsTestWrapper(ConsoleParameters parameters) =>
            new VsTestConsoleWrapper(_vsTestHelper.GetCurrentPlatformVsTestToolPath(),
                parameters);

        private ConsoleParameters DetermineConsoleParameters(string runnerId)
        {
            var determineConsoleParameters = new ConsoleParameters
            {
                TraceLevel = Options.LogOptions?.LogLevel switch
                {
                    LogEventLevel.Debug => TraceLevel.Verbose,
                    LogEventLevel.Verbose => TraceLevel.Verbose,
                    LogEventLevel.Error => TraceLevel.Error,
                    LogEventLevel.Fatal => TraceLevel.Error,
                    LogEventLevel.Warning => TraceLevel.Warning,
                    LogEventLevel.Information => TraceLevel.Info,
                    _ => TraceLevel.Off
                }
            };

            if (Options.LogOptions?.LogToFile != true)
            {
                return determineConsoleParameters;
            }

            var vsTestLogPath = _fileSystem.Path.Combine(LogPath, $"{runnerId}-log.txt");
            _fileSystem.Directory.CreateDirectory(LogPath);
            determineConsoleParameters.LogFilePath = vsTestLogPath;
            return determineConsoleParameters;
        }

        public TestSet GetTestsForSources(IEnumerable<string> sources)
        {
            var result = new TestSet();
            foreach (var source in sources)
            {
                result.RegisterTests(TestsPerSource[source].Select(id => Tests[id]));
            }
            return result;
        }

        // keeps only test assemblies which have tests.
        public bool IsValidSourceList(IEnumerable<string> sources) => sources.Any( s=> TestsPerSource.TryGetValue(s, out var result ) && result.Count >0);

        public IEnumerable<string> GetValidSources(IEnumerable<string> sources) =>
            sources.Where(s => TestsPerSource.TryGetValue(s, out var result) && result.Count > 0);

        public bool AddTestSource(string source)
        {
            if (!_fileSystem.File.Exists(source))
            {
                throw new GeneralStrykerException(
                    $"The test project binaries could not be found at {source}, exiting...");
            }

            if (!TestsPerSource.ContainsKey(source))
            {
                DiscoverTestsInSources(source);
            }

            return TestsPerSource[source].Count > 0;
        }
        
        private void DiscoverTestsInSources(string newSource)
        {
            var wrapper = BuildVsTestWrapper("TestDiscoverer");
            var messages = new List<string>();
            var handler = new DiscoveryEventHandler(messages);
            wrapper.DiscoverTestsAsync(new List<string> { newSource }, GenerateRunSettingsForDiscovery(), handler);

            handler.WaitEnd();
            if (handler.Aborted)
            {
                _logger.LogError("TestDiscoverer: Test discovery has been aborted!");
            }

            wrapper.EndSession();

            TestsPerSource[newSource] = handler.DiscoveredTestCases.Select(c => c.Id).ToHashSet();
            VsTests ??= new Dictionary<Guid, VsTestDescription>(handler.DiscoveredTestCases.Count);
            foreach (var testCase in handler.DiscoveredTestCases)
            {
                if (!VsTests.ContainsKey(testCase.Id))
                {
                    VsTests[testCase.Id] = new VsTestDescription(testCase);
                }

                VsTests[testCase.Id].AddSubCase();
                _logger.LogTrace(
                    $"Test Case : name= {testCase.DisplayName} (id= {testCase.Id}, FQN= {testCase.FullyQualifiedName}).");
            }

            DetectTestFrameworks(VsTests.Values);
            Tests.RegisterTests(VsTests.Values.Select(t => t.Description));
        }

        private void DetectTestFrameworks(ICollection<VsTestDescription> tests)
        {
            if (tests == null)
            {
                _testFramework = 0;
                return;
            }

            if (tests.Any(testCase => testCase.Framework == TestFrameworks.NUnit))
            {
                _testFramework |= TestFrameworks.NUnit;
            }

            if (tests.Any(testCase => testCase.Framework == TestFrameworks.xUnit))
            {
                _testFramework |= TestFrameworks.xUnit;
            }
        }

        private string GenerateRunSettingsForDiscovery()
        {
            var testCaseFilter = string.IsNullOrWhiteSpace(Options.TestCaseFilter)
                ? string.Empty
                : $"<TestCaseFilter>{Options.TestCaseFilter}</TestCaseFilter>" + Environment.NewLine;

            return $@"<RunSettings>
 <RunConfiguration>
  <MaxCpuCount>{Math.Max(1, Options.Concurrency)}</MaxCpuCount>
  <DesignMode>true</DesignMode>
{testCaseFilter}
 </RunConfiguration>
</RunSettings>";
        }

        public string GenerateRunSettings(int? timeout, bool forCoverage, Dictionary<int, ITestGuids> mutantTestsMap, string helperNameSpace, bool isFullFramework)
        {
            var settingsForCoverage = string.Empty;
            var needDataCollector = forCoverage || mutantTestsMap is not null;
            var dataCollectorSettings = needDataCollector
                ? CoverageCollector.GetVsTestSettings(
                    forCoverage,
                    mutantTestsMap?.Select(e => (e.Key, e.Value.GetGuids())),
                    helperNameSpace)
                : string.Empty;

            if (_testFramework.HasFlag(TestFrameworks.NUnit))
            {
                settingsForCoverage = "<CollectDataForEachTestSeparately>true</CollectDataForEachTestSeparately>";
            }

            if (_testFramework.HasFlag(TestFrameworks.xUnit) || _testFramework.HasFlag(TestFrameworks.MsTest))
            {
                settingsForCoverage += "<DisableParallelization>true</DisableParallelization>";
            }
            var timeoutSettings = timeout is > 0
                ? $"<TestSessionTimeout>{timeout}</TestSessionTimeout>" + Environment.NewLine
                : string.Empty;

            var testCaseFilter = string.IsNullOrWhiteSpace(Options.TestCaseFilter)
                ? string.Empty
                : $"<TestCaseFilter>{Options.TestCaseFilter}</TestCaseFilter>" + Environment.NewLine;

            // we need to block parallel run to capture coverage and when testing multiple mutants in a single run
            var runSettings =
                $@"<RunSettings>
<RunConfiguration>
  <CollectSourceInformation>false</CollectSourceInformation>
{(isFullFramework ? @"<DisableAppDomain>true</DisableAppDomain>
" : string.Empty)}  <MaxCpuCount>1</MaxCpuCount>
{timeoutSettings}{settingsForCoverage}
<DesignMode>false</DesignMode>
{testCaseFilter}</RunConfiguration>{dataCollectorSettings}
</RunSettings>";

            return runSettings;
        }
    }
}
