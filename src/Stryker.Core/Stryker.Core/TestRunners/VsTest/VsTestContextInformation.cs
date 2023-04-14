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
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents.TestProjects;
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
        private readonly TestProjectsInfo _testProjectsInfo;
        private readonly IVsTestHelper _vsTestHelper;
        private readonly Func<ConsoleParameters, IVsTestConsoleWrapper> _wrapperBuilder;
        private bool _disposed;
        private List<string> _sources;
        private TestFramework _testFramework;

        /// <summary>
        ///     Creates an instance.
        /// </summary>
        /// <param name="options">Configuration options</param>
        /// <param name="projectInfo">Project information</param>
        /// <param name="helper"></param>
        /// <param name="fileSystem"></param>
        /// <param name="builder"></param>
        /// <param name="hostBuilder"></param>
        /// <param name="logger"></param>
        public VsTestContextInformation(StrykerOptions options,
            TestProjectsInfo testProjectsInfo,
            IVsTestHelper helper = null,
            IFileSystem fileSystem = null,
            Func<ConsoleParameters, IVsTestConsoleWrapper> builder = null,
            Func<string, IStrykerTestHostLauncher> hostBuilder = null,
            ILogger logger = null)
        {
            Options = options;
            _testProjectsInfo = testProjectsInfo;
            _ownVsTestHelper = helper == null;
            _fileSystem = fileSystem ?? new FileSystem();
            _vsTestHelper = helper ?? new VsTestHelper(_fileSystem, logger);
            _wrapperBuilder = builder ?? BuildActualVsTestWrapper;
            _hostBuilder = hostBuilder ?? (name => new StrykerVsTestHostLauncher(name));
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<VsTestContextInformation>();
        }

        /// <summary>
        ///     Discovered tests (VsTest format)
        /// </summary>
        public IDictionary<Guid, VsTestDescription> VsTests { get; private set; }

        /// <summary>
        ///     Test assemblies
        /// </summary>
        public IEnumerable<string> TestSources => _sources;

        /// <summary>
        ///     Tests (Stryker format)
        /// </summary>
        public TestSet Tests { get; } = new();

        /// <summary>
        ///     Stryker options
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
                vsTestConsole.InitializeExtensions(_sources.Select(_fileSystem.Path.GetDirectoryName));
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

        /// <summary>
        ///     Build a full path name stored in the log folder.
        /// </summary>
        /// <param name="fileName">requested filename</param>
        /// <returns>A full pathname</returns>
        public string BuildFilePathName(string fileName) => fileName == null ? null : _fileSystem.Path.Combine(Options.ReportPath ?? ".", fileName);

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
            var vsTestLogPath = _fileSystem.Path.Combine(LogPath, $"{runnerId}_VsTest-log.txt");
            _fileSystem.Directory.CreateDirectory(LogPath);
            determineConsoleParameters.LogFilePath = vsTestLogPath;
            return determineConsoleParameters;
        }

        /// <summary>
        ///     Initialize VsTest session. Discovers tests
        /// </summary>
        /// <exception cref="GeneralStrykerException"></exception>
        public void Initialize()
        {
            var testBinariesPaths = _testProjectsInfo.AnalyzerResults
                .Select(testProject => testProject.GetAssemblyPath()).ToList();
            _sources = new List<string>();

            foreach (var path in testBinariesPaths)
            {
                if (!_fileSystem.File.Exists(path))
                {
                    throw new GeneralStrykerException(
                        $"The test project binaries could not be found at {path}, exiting...");
                }

                _sources.Add(path);
            }

            DiscoverTests();
            Tests.RegisterTests(VsTests.Values.Select(t => t.Description));
        }

        private void DiscoverTests()
        {
            var wrapper = BuildVsTestWrapper("TestDiscoverer");
            var messages = new List<string>();
            var handler = new DiscoveryEventHandler(messages);
            wrapper.DiscoverTests(_sources, GenerateRunSettingsForDiscovery(), handler);

            handler.WaitEnd();
            if (handler.Aborted)
            {
                _logger.LogError("TestDiscoverer: Test discovery has been aborted!");
            }

            VsTests = new Dictionary<Guid, VsTestDescription>(handler.DiscoveredTestCases.Count);
            foreach (var testCase in handler.DiscoveredTestCases)
            {
                if (!VsTests.ContainsKey(testCase.Id))
                {
                    VsTests[testCase.Id] = new VsTestDescription(testCase);
                }

                VsTests[testCase.Id].AddSubCase();
                _logger.LogDebug($"Test Case : name= {testCase.DisplayName} (id= {testCase.Id}, FQN= {testCase.FullyQualifiedName}).");
            }

            DetectTestFrameworks(VsTests.Values);

            wrapper.EndSession();
        }

        public List<VsTestDescription> FindTestCasesWithinDataSource(VsTestDescription referenceTest)
        {
            List<VsTestDescription> result;
            static string Extractor(string name)
            {
                var indexOf = name.IndexOf('(');
                return indexOf < 0 ? name : name[..indexOf];
            }

            switch (referenceTest.Framework)
            {
                case TestFramework.xUnit:
                    // find all tests that have the same FQN
                    result = VsTests.Values.Where(desc => desc.Case.DisplayName == referenceTest.Case.DisplayName).ToList();
                    break;
                case TestFramework.NUnit:
                    var referenceName = Extractor(referenceTest.Case.FullyQualifiedName);
                    result = VsTests.Values.Where(desc => Extractor(desc.Case.FullyQualifiedName) == referenceName).ToList();
                    break;
                default:
                    result = new List<VsTestDescription> { referenceTest };
                    break;
            }
            return result;
        }

        private void DetectTestFrameworks(ICollection<VsTestDescription> tests)
        {
            if (tests == null)
            {
                _testFramework = 0;
                return;
            }

            if (tests.Any(testCase => testCase.Framework == TestFramework.NUnit))
            {
                _testFramework |= TestFramework.NUnit;
            }

            if (tests.Any(testCase => testCase.Framework == TestFramework.xUnit))
            {
                _testFramework |= TestFramework.xUnit;
            }
        }

        private string GenerateRunSettingsForDiscovery()
        {
            var testCaseFilter = string.IsNullOrWhiteSpace(Options.TestCaseFilter)
                ? string.Empty
                : $"<TestCaseFilter>{Options.TestCaseFilter}</TestCaseFilter>" + Environment.NewLine;

            return $@"<RunSettings>
 <RunConfiguration>
  <CollectSourceInformation>true</CollectSourceInformation>
  <MaxCpuCount>{Options.Concurrency}</MaxCpuCount>
  <DesignMode>false</DesignMode>
{testCaseFilter}
 </RunConfiguration>
</RunSettings>";
        }

        public string GenerateRunSettings(int? timeout, bool forCoverage, Dictionary<int, ITestGuids> mutantTestsMap)
        {
            var projectAnalyzerResult = _testProjectsInfo.AnalyzerResults.FirstOrDefault();
            var settingsForCoverage = string.Empty;
            var needDataCollector = forCoverage || mutantTestsMap is { };
            var dataCollectorSettings = needDataCollector
                ? CoverageCollector.GetVsTestSettings(
                    forCoverage,
                    mutantTestsMap?.Select(e => (e.Key, e.Value.GetGuids())),
                    CodeInjection.HelperNamespace)
                : string.Empty;

            if (_testFramework.HasFlag(TestFramework.NUnit))
            {
                settingsForCoverage = "<CollectDataForEachTestSeparately>true</CollectDataForEachTestSeparately>";
            }

            if (_testFramework.HasFlag(TestFramework.xUnit))
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
{(!projectAnalyzerResult.TargetsFullFramework() ? string.Empty : @"<DisableAppDomain>true</DisableAppDomain>
")}  <MaxCpuCount>1</MaxCpuCount>
{timeoutSettings}{settingsForCoverage}
<DesignMode>false</DesignMode>
{testCaseFilter}</RunConfiguration>{dataCollectorSettings}
</RunSettings>";

            return runSettings;
        }
    }
}
