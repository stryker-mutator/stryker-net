using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Buildalyzer;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Moq;
using Stryker.Core.Initialisation;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;
using Stryker.Core.ToolHelpers;
using Stryker.DataCollector;

namespace Stryker.Core.UnitTest.TestRunners;

/// <summary>
/// This class hosts helping function for VsTest runner tests
/// 
/// </summary>
public class VsTestMockingHelper : TestBase
{
    protected Mutant Mutant { get; }
    protected Mutant OtherMutant { get; }
    protected readonly CsharpFolderComposite _projectContents;
    private readonly string _testAssemblyPath;
    private readonly ProjectInfo _targetProject;
    private readonly MockFileSystem _fileSystem;
    private readonly Uri _executorUri;
    private readonly TestProperty _coverageProperty;
    private readonly TestProperty _unexpectedCoverageProperty;
    protected readonly TimeSpan _testDefaultDuration = TimeSpan.FromSeconds(1);
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
            _executorUri = new Uri("exec://nunit");
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
            _targetProject = BuildTestProject();

            TestCases = new List<TestCase> { firstTest, secondTest };
    }

    protected ProjectInfo BuildTestProject(IEnumerable<Mutant> mutants = null)
    {
        var content = new CsharpFolderComposite();
        content.Add(new CsharpFileLeaf { Mutants = mutants ?? new []{Mutant, OtherMutant} });
        return new ProjectInfo(_fileSystem)
        {
            TestProjectAnalyzerResults = new List<IAnalyzerResult>
            {
                TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>()
                    {
                        { "TargetDir", Path.GetDirectoryName(_testAssemblyPath) },
                        { "TargetFileName", Path.GetFileName(_testAssemblyPath) }
                    },
                    targetFramework: "netcoreapp2.1").Object
            },
            ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                properties: new Dictionary<string, string>()
                {
                    { "TargetDir", Path.Combine(_filesystemRoot, "app", "bin", "Debug") },
                    { "TargetFileName", "AppToTest.dll" },
                    { "Language", "C#" }
                }).Object,
            ProjectContents = content
        };
    }

    protected IReadOnlyList<TestCase> TestCases { get; }

    private static void DiscoverTests(ITestDiscoveryEventsHandler discoveryEventsHandler, IReadOnlyCollection<TestCase> tests, bool aborted) =>
        Task.Run(() => discoveryEventsHandler.HandleDiscoveredTests(tests)).
            ContinueWith((_, u) => discoveryEventsHandler.HandleDiscoveryComplete((int)u, null, aborted), tests.Count);

    protected TestCase BuildCase(string name) => new(name, _executorUri, _testAssemblyPath) { Id = new Guid() };

    private TestCase FindOrBuildCase(string testResultId) => TestCases.FirstOrDefault(t => t.FullyQualifiedName == testResultId) ?? BuildCase(testResultId);

    private static void MoqTestRun(ITestRunEventsHandler testRunEvents, IReadOnlyList<TestResult> testResults,
        TestCase timeOutTest = null) =>
        Task.Run(() =>
        {
            var timer = new Stopwatch();
            testRunEvents.HandleTestRunStatsChange(
                new TestRunChangedEventArgs(new TestRunStatistics(0, null), null, timeOutTest == null ? null : new[] { timeOutTest }));

            for (var i = 0; i < testResults.Count; i++)
            {
                Thread.Sleep(10);
                testResults[i].EndTime = DateTimeOffset.Now;
                testRunEvents.HandleTestRunStatsChange(new TestRunChangedEventArgs(
                    new TestRunStatistics(i + 1, null), new[] { testResults[i] }, null));
            }

            if (timeOutTest != null)
            {
                testRunEvents.HandleTestRunStatsChange(new TestRunChangedEventArgs(
                    new TestRunStatistics(testResults.Count, null), null, new[] { timeOutTest }));
            }

            Thread.Sleep(10);
            testRunEvents.HandleTestRunComplete(
                new TestRunCompleteEventArgs(new TestRunStatistics(testResults.Count, null), false, false, null,
                    null, timer.Elapsed),
                new TestRunChangedEventArgs(null, Array.Empty<TestResult>(), new List<TestCase>()),
                null,
                null);
        });

    protected void SetupMockTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, bool testResult, IReadOnlyList<TestCase> testCases)
    {
        var results = new List<(string, bool)>(testCases.Count);
        results.AddRange(testCases.Select(t => (t.FullyQualifiedName, testResult)));
        SetupMockTestRun(mockVsTest, results);
    }

    protected void SetupMockTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IEnumerable<(string id, bool success)> testResults, IReadOnlyList<TestCase> testCases = null)
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
                Duration = _testDefaultDuration
            });
        }
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHostAsync(
                It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                It.Is<string>(settings => !settings.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<string> _, string _, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                ITestHostLauncher _) =>
            {
                // generate test results
                MoqTestRun(testRunEvents, results);
            }).Returns(Task.CompletedTask);
    }

    protected void SetupMockCoverageRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> coverageResults) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHostAsync(
                It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                It.Is<string>(settings => settings.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<string> _, string _, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                ITestHostLauncher _) =>
            {
                // generate test results
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
                MoqTestRun(testRunEvents, results);
            }).Returns(Task.CompletedTask);

    protected void SetupMockCoveragePerTestRunP(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> coverageResults) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHostAsync(
                It.Is<IEnumerable<TestCase>>(t => t.Any()),
                It.Is<string>(settings => settings.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<TestCase> testCases, string _, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                ITestHostLauncher _) =>
            {
                // generate test results
                var results = new List<TestResult>(coverageResults.Count);
                foreach (var (key, value) in coverageResults)
                {
                    var coveredList = value.Split('|');
                    if (!testCases.Any(t => t.DisplayName== key))
                    {
                        continue;
                    }
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
                    results.Add(result);
                }
                MoqTestRun(testRunEvents, results);
            }).Returns(Task.CompletedTask);

    protected static void SetupMockPartialTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> results) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHostAsync(
                It.IsAny<IEnumerable<TestCase>>(),
                It.Is<string>(s => !s.Contains("<Coverage")),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<TestCase> sources, string settings, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
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
                MoqTestRun(testRunEvents, runResults);
                collector.TestSessionEnd(new TestSessionEndArgs());
            }).Returns(Task.CompletedTask);

    protected static void SetupMockTimeOutTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> results, string timeoutTest) =>
        mockVsTest.Setup(x =>
            x.RunTestsWithCustomTestHostAsync(
                It.IsAny<IEnumerable<TestCase>>(),
                It.IsAny<string>(),
                It.Is<TestPlatformOptions>(o => o != null && o.TestCaseFilter == null),
                It.IsAny<ITestRunEventsHandler>(),
                It.IsAny<ITestHostLauncher>())).Callback(
            (IEnumerable<TestCase> sources, string settings, TestPlatformOptions _, ITestRunEventsHandler testRunEvents,
                ITestHostLauncher _) =>
            {
                var collector = new CoverageCollector();
                var start = new TestSessionStartArgs
                {
                    Configuration = settings
                };
                var mock = new Mock<IDataCollectionSink>(MockBehavior.Loose);
                TestCase timeOutTestCase = null;
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
                MoqTestRun(testRunEvents, runResults, timeOutTestCase);
                collector.TestSessionEnd(new TestSessionEndArgs());

            }).Returns(Task.CompletedTask);

    protected Mock<IVsTestConsoleWrapper> BuildVsTestRunnerPool(StrykerOptions options,
        out VsTestRunnerPool runner, IReadOnlyCollection<TestCase> testCases=null, ProjectInfo targetProject = null,
        bool succeed = true)
    {
        testCases ??= TestCases.ToList();
        var mockedVsTestConsole = new Mock<IVsTestConsoleWrapper>(MockBehavior.Strict);
        mockedVsTestConsole.Setup(x => x.StartSession());
        mockedVsTestConsole.Setup(x => x.InitializeExtensions(It.IsAny<List<string>>()));
        mockedVsTestConsole.Setup(x => x.AbortTestRun());
        mockedVsTestConsole.Setup(x => x.EndSession());
        mockedVsTestConsole.Setup(x =>
            x.DiscoverTests(It.Is<IEnumerable<string>>(d => d.Any(e => e == _testAssemblyPath)),
                It.IsAny<string>(),
                It.IsAny<ITestDiscoveryEventsHandler>())).Callback(
            (IEnumerable<string> _, string _, ITestDiscoveryEventsHandler discoveryEventsHandler) =>
                DiscoverTests(discoveryEventsHandler, testCases, false));

        var context = new VsTestContextInformation(
            options,
            targetProject ?? _targetProject,
            new Mock<IVsTestHelper>().Object,
            _fileSystem,
            _=> mockedVsTestConsole.Object,
            hostBuilder: _ => new MoqHost(succeed, false),
            NullLogger.Instance
        );
        context.Initialize();
        runner = new VsTestRunnerPool(context,
            NullLogger.Instance,
            (information, _) => new VsTestRunner(information, 0, NullLogger.Instance));
        return mockedVsTestConsole;
    }

    protected MutationTestProcess BuildMutationTestProcess(VsTestRunnerPool runner, StrykerOptions options, IReadOnlyList<TestCase> tests = null, ProjectInfo targetProject = null)
    {
        var testRunResult = new TestRunResult(new TestsGuidList((tests ?? TestCases).Select(t => t.Id)),
            TestsGuidList.NoTest(),
            TestsGuidList.NoTest(),
            string.Empty,
            TimeSpan.Zero);
        var input = new MutationTestInput
        {
            ProjectInfo = targetProject ?? _targetProject,
            TestRunner = runner,
            InitialTestRun = new InitialTestRun(testRunResult, new TimeoutValueCalculator(500))
        };
        return new MutationTestProcess(input, new Mock<IReporter>().Object, new MutationTestExecutor(input.TestRunner),
            fileSystem: _fileSystem, options: options, mutantFilter: new Mock<IMutantFilter>(MockBehavior.Loose).Object);
    }

    private class MoqHost : IStrykerTestHostLauncher
    {
        public MoqHost(bool succeed, bool isDebug)
        {
            IsProcessCreated = succeed;
            IsDebug = isDebug;
        }

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo) => throw new NotImplementedException();

        public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo, CancellationToken cancellationToken) => throw new NotImplementedException();

        public bool IsDebug { get; }

        public bool IsProcessCreated { get; }
    }

}
