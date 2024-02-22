using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Moq;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;
using Stryker.Core.ToolHelpers;
using Stryker.DataCollector;

namespace Stryker.Core.UnitTest.TestRunners;

/// <summary>
/// This class has a set of methods that can be used to mock VsTest behavior. 
/// 
/// </summary>
public class VsTestMockingHelper : TestBase
{
    protected Mutant Mutant { get; }
    protected Mutant OtherMutant { get; }
    private readonly string _testAssemblyPath;
    public SourceProjectInfo SourceProjectInfo { get; }
    private readonly TestProjectsInfo _testProjectsInfo;
    private readonly MockFileSystem _fileSystem;
    private readonly Uri _NUnitUri;
    private readonly Uri _xUnitUri;
    private readonly TestProperty _coverageProperty;
    private readonly TestProperty _unexpectedCoverageProperty;
    protected readonly TimeSpan TestDefaultDuration = TimeSpan.FromSeconds(1);
    private readonly string _filesystemRoot;

    public VsTestMockingHelper()
    {
        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        _filesystemRoot = Path.GetPathRoot(currentDirectory);

        var sourceFile = File.ReadAllText(currentDirectory + "/TestResources/ExampleSourceFile.cs");
        var testProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot!, "TestProject", "TestProject.csproj"));
        var projectUnderTestPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"));
        const string DefaultTestProjectFileContents = @"<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>
    <ItemGroup>
        <PackageReference Include=""Microsoft.NET.Test.Sdk"" Version = ""15.5.0"" />
        <PackageReference Include=""xunit"" Version=""2.3.1"" />
        <PackageReference Include=""xunit.runner.visualstudio"" Version=""2.3.1"" />
        <DotNetCliToolReference Include=""dotnet-xunit"" Version=""2.3.1"" />
    </ItemGroup>
    <ItemGroup>
        <ProjectReference Include=""..\ExampleProject\ExampleProject.csproj"" />
    </ItemGroup>
</Project>";
        _testAssemblyPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "_firstTest", "bin", "Debug", "TestApp.dll"));
        _NUnitUri = new Uri("exec://nunit");
        _xUnitUri = new Uri("executor://xunit/VsTestRunner2/netcoreapp");
        var firstTest = BuildCase("T0");
        var secondTest = BuildCase("T1");

        _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { projectUnderTestPath, new MockFileData(DefaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                { testProjectPath, new MockFileData(DefaultTestProjectFileContents)},
                { _testAssemblyPath!, new MockFileData("Bytecode") },
                { Path.Combine(_filesystemRoot, "app", "bin", "Debug", "AppToTest.dll"), new MockFileData("Bytecode") },
            });
        _coverageProperty = TestProperty.Register(CoverageCollector.PropertyName, CoverageCollector.PropertyName, typeof(string), typeof(TestResult));
        _unexpectedCoverageProperty = TestProperty.Register(CoverageCollector.OutOfTestsPropertyName, CoverageCollector.OutOfTestsPropertyName, typeof(string), typeof(TestResult));
        Mutant = new Mutant { Id = 0 };
        OtherMutant = new Mutant { Id = 1 };
        _testProjectsInfo = BuildTestProjectsInfo();
        SourceProjectInfo = BuildSourceProjectInfo();

        TestCases = new List<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> { firstTest, secondTest };
    }

    protected SourceProjectInfo BuildSourceProjectInfo(IEnumerable<Mutant> mutants = null)
    {
        var content = new CsharpFolderComposite();
        content.Add(new CsharpFileLeaf { Mutants = mutants ?? new[] { Mutant, OtherMutant } });
        return new SourceProjectInfo
        {
            AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                properties: new Dictionary<string, string>
                {
                    { "TargetDir", Path.Combine(_filesystemRoot, "app", "bin", "Debug") },
                    { "TargetFileName", "AppToTest.dll" },
                    { "Language", "C#" }
                },targetFramework: "netcoreapp2.1").Object,
            ProjectContents = content,
            TestProjectsInfo = _testProjectsInfo
        };
    }

    protected TestProjectsInfo BuildTestProjectsInfo() =>
        new(_fileSystem)
        {
            TestProjects = new List<TestProject> { new(_fileSystem, TestHelper.SetupProjectAnalyzerResult(
                properties: new Dictionary<string, string>
                {
                    { "TargetDir", Path.GetDirectoryName(_testAssemblyPath) },
                    { "TargetFileName", Path.GetFileName(_testAssemblyPath) }
                },
                targetFramework: "netcoreapp2.1").Object)}
        };

    protected IReadOnlyList<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> TestCases { get; }

    private static void DiscoverTests(ITestDiscoveryEventsHandler discoveryEventsHandler, IReadOnlyCollection<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> tests, bool aborted)
    {
        discoveryEventsHandler.HandleDiscoveredTests(tests);
        discoveryEventsHandler.HandleDiscoveryComplete(tests.Count, null, aborted);
    }

    protected Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase BuildCase(string name, TestFrameworks framework = TestFrameworks.xUnit, string displayName = null)
        => new(name, framework == TestFrameworks.xUnit ? _xUnitUri : _NUnitUri, _testAssemblyPath) { Id = new Guid(), DisplayName = displayName ?? name };

    protected Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase FindOrBuildCase(string testResultId) => TestCases.FirstOrDefault(t => t.FullyQualifiedName == testResultId) ?? BuildCase(testResultId);

    private static void MockTestRun(ITestRunEventsHandler testRunEvents, IReadOnlyList<TestResult> testResults,
        Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase timeOutTest = null) 
        {
            if (testResults.Count == 0)
            {
                // no test ==> no event at all
                return;
            }
            var timer = new Stopwatch();
            testRunEvents.HandleTestRunStatsChange(
                new TestRunChangedEventArgs(new TestRunStatistics(0, null), null, timeOutTest == null ? null : new[] { timeOutTest }));

            for (var i = 0; i < testResults.Count; i++)
            {
                testResults[i].StartTime = DateTimeOffset.Now;
                Thread.Sleep(1);
                testResults[i].EndTime = DateTimeOffset.Now + testResults[i].Duration;
                testRunEvents.HandleTestRunStatsChange(new TestRunChangedEventArgs(
                    new TestRunStatistics(i + 1, null), new[] { testResults[i] }, null));
            }

            if (timeOutTest != null)
            {
                testRunEvents.HandleTestRunStatsChange(new TestRunChangedEventArgs(
                    new TestRunStatistics(testResults.Count, null), null, new[] { timeOutTest }));
            }

            Thread.Sleep(1);
            testRunEvents.HandleTestRunComplete(
                new TestRunCompleteEventArgs(new TestRunStatistics(testResults.Count, null), false, false, null,
                    null, timer.Elapsed),
                new TestRunChangedEventArgs(null, Array.Empty<TestResult>(), new List<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase>()),
                null,
                null);
        }

    protected void SetupMockTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, bool testResult, IReadOnlyList<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> testCases)
    {
        var results = new List<(string, bool)>(testCases.Count);
        results.AddRange(testCases.Select(t => (t.FullyQualifiedName, testResult)));
        SetupMockTestRun(mockVsTest, results);
    }

    protected void SetupMockTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IEnumerable<(string id, bool success)> testResults, IReadOnlyList<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> testCases = null)
    {
        var results = new List<TestResult>();
        testCases ??= TestCases.ToList();
        foreach (var (testResultId, success) in testResults)
        {
            var testCase = testCases.FirstOrDefault(t => t.FullyQualifiedName == testResultId) ?? BuildCase(testResultId);
            results.Add(new TestResult(testCase)
            {
                Outcome = success ? TestOutcome.Passed : TestOutcome.Failed,
                ComputerName = ".",
                Duration = TestDefaultDuration
            });
        }
        SetupMockTestRun(mockVsTest, results);
    }

    protected void SetupMockTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyList<TestResult> results) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHost(
                It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                It.Is<string>(settings => !settings.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<string> _, string _, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                ITestHostLauncher _) =>  MockTestRun(testRunEvents, results));

    protected void SetupFailingTestRun(Mock<IVsTestConsoleWrapper> mockVsTest) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHost(
                It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                It.Is<string>(settings => !settings.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<string> _, string _, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                    ITestHostLauncher _) =>
                // generate test results
                Task.Run(() =>
                {
                    var timer = new Stopwatch();
                    testRunEvents.HandleTestRunStatsChange(
                        new TestRunChangedEventArgs(new TestRunStatistics(0, null), null, null));


                    Thread.Sleep(10);
                    testRunEvents.HandleTestRunComplete(
                        new TestRunCompleteEventArgs(new TestRunStatistics(0, null), false, false,
                            new TransationLayerException("VsTest Crashed"),
                            null, timer.Elapsed),
                        new TestRunChangedEventArgs(null, Array.Empty<TestResult>(),
                            new List<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase>()),
                        null,
                        null);
                }));

    protected void SetupFrozenTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, int repeated = 1) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHost(
                It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                It.Is<string>(settings => !settings.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<string> _, string _, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                    ITestHostLauncher _) =>
                // generate test results
                Task.Run(() =>
                {
                    testRunEvents.HandleTestRunStatsChange(
                        new TestRunChangedEventArgs(new TestRunStatistics(0, null), null, null));

                    if (repeated--<=0)
                        testRunEvents.HandleTestRunComplete(
                            new TestRunCompleteEventArgs(new TestRunStatistics(0, null), false, false,
                            null,
                            null, TimeSpan.FromMilliseconds(10)),
                        new TestRunChangedEventArgs(null, Array.Empty<TestResult>(),
                            new List<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase>()),
                        null,
                        null);
                    else
                    {
                        testRunEvents.HandleTestRunComplete(
                            new TestRunCompleteEventArgs(new TestRunStatistics(0, null), false, false,
                            new TransationLayerException("fake", null),
                            null, TimeSpan.FromMilliseconds(10)),
                        new TestRunChangedEventArgs(null, Array.Empty<TestResult>(),
                            new List<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase>()),
                        null,
                        null);
                    }
                }));

   protected void SetupFrozenVsTest(Mock<IVsTestConsoleWrapper> mockVsTest, int repeated = 1) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHost(
                It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                It.Is<string>(settings => !settings.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<string> _, string _, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                    ITestHostLauncher _) =>
                // generate test results
                {
                    testRunEvents.HandleTestRunStatsChange(
                        new TestRunChangedEventArgs(new TestRunStatistics(0, null), null, null));

                    testRunEvents.HandleTestRunComplete(
                        new TestRunCompleteEventArgs(new TestRunStatistics(0, null), false, false,
                            null,
                            null, TimeSpan.FromMilliseconds(10)),
                        new TestRunChangedEventArgs(null, Array.Empty<TestResult>(),
                            new List<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase>()),
                        null,
                        null);

                    if (repeated-->0)
                        Thread.Sleep(1000);
                });

    protected void SetupMockCoverageRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> coverageResults) => SetupMockCoverageRun(mockVsTest, GenerateCoverageTestResults(coverageResults));

    protected void SetupMockCoverageRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyList<TestResult> results) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHost(
                It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                It.Is<string>(settings => settings.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<string> _, string _, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                ITestHostLauncher _) => MockTestRun(testRunEvents, results));

    private List<TestResult> GenerateCoverageTestResults(IReadOnlyDictionary<string, string> coverageResults)
    {
        var results = new List<TestResult>(coverageResults.Count);
        foreach (var (key, value) in coverageResults)
        {
            var result = new TestResult(FindOrBuildCase(key))
            {
                DisplayName = key,
                Outcome = TestOutcome.Passed,
                ComputerName = "."
            };
            if (value != null)
            {
                var coveredList = value.Split('|');
                result.SetPropertyValue(_coverageProperty, coveredList[0]);
                if (coveredList.Length > 1)
                {
                    result.SetPropertyValue(_unexpectedCoverageProperty, coveredList[1]);
                }
            }

            results.Add(result);
        }

        return results;
    }

    protected void SetupMockCoveragePerTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> coverageResults) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHost(
                It.Is<IEnumerable<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase>>(t => t.Any()),
                It.Is<string>(settings => settings.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> testCases, string _, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                ITestHostLauncher _) =>
            {
                // generate test results
                var results = new List<TestResult>(coverageResults.Count);
                foreach (var (key, value) in coverageResults)
                {
                    var coveredList = value.Split('|');
                    if (!testCases.Any(t => t.DisplayName == key))
                    {
                        continue;
                    }
                    var result = BuildCoverageTestResult(key, coveredList);
                    results.Add(result);
                }
                MockTestRun(testRunEvents, results);
            });

    protected TestResult BuildCoverageTestResult(string key, string[] coveredList)
    {
        var result = new TestResult(FindOrBuildCase(key))
        {
            DisplayName = key,
            Outcome = TestOutcome.Passed,
            ComputerName = "."
        };
        result.SetPropertyValue(_coverageProperty, coveredList[0]);
        if (coveredList.Length > 1)
        {
            result.SetPropertyValue(_unexpectedCoverageProperty, coveredList[1]);
        }

        return result;
    }

    protected static void SetupMockPartialTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> results) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHost(
                It.IsAny<IEnumerable<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase>>(),
                It.Is<string>(s => !s.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> sources, string settings, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                ITestHostLauncher _) =>
            {
                var collector = new CoverageCollector();
                var start = new TestSessionStartArgs
                {
                    Configuration = settings
                };
                var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
                collector.Initialize(mock.Object);
                collector.TestSessionStart(start);

                var mutants = collector.MutantList;
                if (!results.ContainsKey(mutants))
                {
                    throw new ArgumentException($"Unexpected mutant run {mutants}");
                }

                var tests = sources.ToList();
                var data = results[mutants].Split(',').Select(e => e.Split('=')).ToList();
                if (data.Count != tests.Count)
                {
                    throw new ArgumentException($"Invalid number of tests for mutant run {mutants}: found {tests.Count}, expected {data.Count}");
                }

                var runResults = new List<TestResult>(data.Count);
                foreach (var strings in data)
                {
                    var matchingTest = tests.FirstOrDefault(t => t.FullyQualifiedName == strings[0]);
                    if (matchingTest == null)
                    {
                        throw new ArgumentException($"Test {strings[0]} not run for mutant {mutants}.");
                    }

                    var result = new TestResult(matchingTest)
                    { Outcome = strings[1] == "F" ? TestOutcome.Failed : TestOutcome.Passed, ComputerName = "." };
                    runResults.Add(result);
                }
                // setup a normal test run
                MockTestRun(testRunEvents, runResults);
                collector.TestSessionEnd(new TestSessionEndArgs());
            });

    protected static void SetupMockTimeOutTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> results, string timeoutTest) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHost(
                It.IsAny<IEnumerable<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase>>(),
                It.IsAny<string>(),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> sources, string settings, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                ITestHostLauncher _) => 
            {
                var collector = new CoverageCollector();
                var start = new TestSessionStartArgs
                {
                    Configuration = settings
                };
                var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
                Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase timeOutTestCase = null;
                collector.Initialize(mock.Object);
                collector.TestSessionStart(start);

                var mutants = collector.MutantList;
                if (!results.ContainsKey(mutants))
                {
                    throw new ArgumentException($"Unexpected mutant run {mutants}");
                }

                var tests = sources.ToList();
                var data = results[mutants].Split(',').Select(e => e.Split('=')).ToList();
                if (data.Count != tests.Count)
                {
                    throw new ArgumentException($"Invalid number of tests for mutant run {mutants}: found {tests.Count}, expected {data.Count}");
                }

                var runResults = new List<TestResult>(data.Count);
                foreach (var strings in data)
                {
                    var matchingTest = tests.FirstOrDefault(t => t.FullyQualifiedName == strings[0]);
                    if (matchingTest == null)
                    {
                        throw new ArgumentException($"Test {strings[0]} not run for mutant {mutants}.");
                    }
                    if (matchingTest.FullyQualifiedName == timeoutTest)
                    {
                        timeOutTestCase = matchingTest;
                    }
                    var result = new TestResult(matchingTest)
                    { Outcome = strings[1] == "F" ? TestOutcome.Failed : TestOutcome.Passed, ComputerName = "." };
                    runResults.Add(result);
                }
                // setup a normal test run
                MockTestRun(testRunEvents, runResults, timeOutTestCase);
                collector.TestSessionEnd(new TestSessionEndArgs());

            });

    protected Mock<IVsTestConsoleWrapper> BuildVsTestRunnerPool(StrykerOptions options,
        out VsTestRunnerPool runner, IReadOnlyCollection<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> testCases = null, TestProjectsInfo testProjectsInfo = null)
    {
        testCases ??= TestCases.ToList();
        var mockedVsTestConsole = new Mock<IVsTestConsoleWrapper>(MockBehavior.Strict);
        mockedVsTestConsole.Setup(x => x.StartSession());
        mockedVsTestConsole.Setup(x => x.InitializeExtensions(It.IsAny<IEnumerable<string>>()));
        mockedVsTestConsole.Setup(x => x.AbortTestRun());
        mockedVsTestConsole.Setup(x => x.EndSession());
        
        mockedVsTestConsole.Setup(x =>
            x.DiscoverTests(It.Is<IEnumerable<string>>(d => d.Any(e => e == _testAssemblyPath)),
                It.IsAny<string>(),
                It.IsAny<ITestDiscoveryEventsHandler>()))
            .Callback((IEnumerable<string> _, string _, ITestDiscoveryEventsHandler handler) => DiscoverTests(handler, testCases, false));
        var context = new VsTestContextInformation(
            options,
            new Mock<IVsTestHelper>().Object,
            _fileSystem,
            _ => mockedVsTestConsole.Object,
            hostBuilder: _ => new MockStrykerTestHostLauncher(false),
            NullLogger.Instance
        );
        foreach (var path in (testProjectsInfo ?? _testProjectsInfo).GetTestAssemblies())
        {
            context.AddTestSource(path);
        }
        runner = new VsTestRunnerPool(context,
            NullLogger.Instance,
            (information, _) => new VsTestRunner(information, 0, NullLogger.Instance));
        return mockedVsTestConsole;
    }

    protected MutationTestProcess BuildMutationTestProcess(VsTestRunnerPool runner, StrykerOptions options, IReadOnlyList<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> tests = null, SourceProjectInfo sourceProject = null)
    {
        var testRunResult = new TestRunResult(new List<VsTestDescription>(), new TestGuidsList((tests ?? TestCases).Select(t => t.Id)),
            TestGuidsList.NoTest(),
            TestGuidsList.NoTest(),
            string.Empty,
            Enumerable.Empty<string>(),
            TimeSpan.Zero);
        var input = new MutationTestInput
        {
            SourceProjectInfo = sourceProject ?? SourceProjectInfo,
            TestRunner = runner,
            InitialTestRun = new InitialTestRun(testRunResult, new TimeoutValueCalculator(500))
        };
        var mutator = new CsharpMutationProcess(_fileSystem, options);

        return new MutationTestProcess(input, options, null, new MutationTestExecutor(runner), mutator);
    }

    private class MockStrykerTestHostLauncher : IStrykerTestHostLauncher
    {
        public MockStrykerTestHostLauncher(bool isDebug) => IsDebug = isDebug;

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo) => throw new NotImplementedException();

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo, CancellationToken cancellationToken) => throw new NotImplementedException();

        public bool IsDebug { get; }

        public int ErrorCode { get; }
    }
}
