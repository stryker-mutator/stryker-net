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
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollector.InProcDataCollector;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
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
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    /// <summary>
    /// This class hosts the VsTestRunner related tests. The design of VsTest implies the creation of many mocking objects, so the tests may be hard to read.
    /// This is sad but expected. Please use caution when changing/creating tests.
    /// </summary>
    public class VsTestRunnersTest : TestBase
    {
        private readonly string _testAssemblyPath;
        private readonly ProjectInfo _targetProject;
        private readonly MockFileSystem _fileSystem;
        private readonly Mutant _mutant;
        private readonly Mutant _otherMutant;
        private readonly CsharpFolderComposite _projectContents;
        private readonly Uri _executorUri;
        private readonly TestProperty _coverageProperty;
        private readonly TestProperty _unexpectedCoverageProperty;
        private readonly Uri _xUnitUri;
        private readonly TimeSpan _testDefaultDuration = TimeSpan.FromSeconds(1);

        // initialize the test context and mock objects
        public VsTestRunnersTest()
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filesystemRoot = Path.GetPathRoot(currentDirectory);

            var sourceFile = File.ReadAllText(currentDirectory + "/TestResources/ExampleSourceFile.cs");
            var testProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(filesystemRoot!, "TestProject", "TestProject.csproj"));
            var projectUnderTestPath = FilePathUtils.NormalizePathSeparators(Path.Combine(filesystemRoot, "ExampleProject", "ExampleProject.csproj"));
            const string defaultTestProjectFileContents = @"<Project Sdk=""Microsoft.NET.Sdk"">
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
            _testAssemblyPath = FilePathUtils.NormalizePathSeparators(Path.Combine(filesystemRoot, "_firstTest", "bin", "Debug", "TestApp.dll"));
            _executorUri = new Uri("exec://nunit");
            _xUnitUri = new Uri("executor://xunit/VsTestRunner2/netcoreapp");
            var firstTest = BuildCase("T0");
            var secondTest = BuildCase("T1");

            var content = new CsharpFolderComposite();
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                { Path.Combine(filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                { _testAssemblyPath!, new MockFileData("Bytecode") },
                { Path.Combine(filesystemRoot, "app", "bin", "Debug", "AppToTest.dll"), new MockFileData("Bytecode") },
            });
            _coverageProperty = TestProperty.Register(CoverageCollector.PropertyName, CoverageCollector.PropertyName, typeof(string), typeof(TestResult));
            _unexpectedCoverageProperty = TestProperty.Register(CoverageCollector.OutOfTestsPropertyName, CoverageCollector.OutOfTestsPropertyName, typeof(string), typeof(TestResult));
            _mutant = new Mutant { Id = 0 };
            _otherMutant = new Mutant { Id = 1 };
            _projectContents = content;
            _projectContents.Add(new CsharpFileLeaf { Mutants = new[] { _otherMutant, _mutant } });
            _targetProject = new ProjectInfo(_fileSystem)
            {
                TestProjectAnalyzerResults = new List<IAnalyzerResult> {
                    TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", Path.GetDirectoryName(_testAssemblyPath) },
                        { "TargetFileName", Path.GetFileName(_testAssemblyPath) }
                    },
                    targetFramework: "netcoreapp2.1").Object
                },
                ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", Path.Combine(filesystemRoot, "app", "bin", "Debug") },
                        { "TargetFileName", "AppToTest.dll" },
                        { "Language", "C#" }
                    }).Object,
                ProjectContents = _projectContents
            };

            TestCases = new List<TestCase> { firstTest, secondTest };
        }

        private List<TestCase> TestCases { get; }

        // simulate the discovery of tests
        private static void DiscoverTests(ITestDiscoveryEventsHandler discoveryEventsHandler, ICollection<TestCase> tests, bool aborted) =>
            Task.Run(() => discoveryEventsHandler.HandleDiscoveredTests(tests)).
                ContinueWith((_, u) => discoveryEventsHandler.HandleDiscoveryComplete((int)u, null, aborted), tests.Count);

        private TestCase BuildCase(string name) => new(name, _executorUri, _testAssemblyPath) { Id = new Guid() };

        private TestCase FindOrBuildCase(string testResultId) => TestCases.FirstOrDefault(t => t.FullyQualifiedName == testResultId) ?? BuildCase(testResultId);

        // mock a VsTest run. Provides test result one by one at 10 ms intervals
        // note: a lot of information is still missing (vs real VsTest). You will have to add them if your test requires them
        private void MoqTestRun(ITestRunEventsHandler testRunEvents, IReadOnlyList<TestResult> testResults,
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

        // setup a simple (mock) test run where all tests fail or succeed
        private void SetupMockTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, bool testResult, EventWaitHandle synchroObject, ICollection<TestCase> testCases)
        {
            var results = new List<(string, bool)>(testCases.Count);
            results.AddRange(testCases.Select(t => (t.FullyQualifiedName, testResult)));
            SetupMockTestRun(mockVsTest, results, synchroObject);
        }

        private void SetupMockTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IEnumerable<(string id, bool success)> testResults,
            EventWaitHandle synchroObject, ICollection<TestCase> testCases = null)
        {
            var results = new List<TestResult>();
            testCases ??= TestCases;
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
                    synchroObject.Set();
                }).Returns(Task.CompletedTask);
        }

        // setup a customized coverage capture run, using provided coverage results
        private void SetupMockCoverageRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> coverageResults, EventWaitHandle endProcess) =>
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
                        var coveredList = value.Split('|');
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
                    endProcess.Set();
                }).Returns(Task.CompletedTask);

        // setup a customized partial test runs, using provided test results
        private void SetupMockPartialTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> results, EventWaitHandle endProcess) =>
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

                    endProcess.Set();
                }).Returns(Task.CompletedTask);

        private void SetupMockTimeOutTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> results, string timeoutTest, EventWaitHandle endProcess) =>
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

                    endProcess.Set();
                }).Returns(Task.CompletedTask);

        private Mock<IVsTestConsoleWrapper> BuildVsTestRunner(StrykerOptions options, WaitHandle endProcess, out VsTestRunner runner, ICollection<TestCase> testCases=null, bool suceed = true)
        {
            testCases ??= TestCases;
            var mockedVsTestConsole = new Mock<IVsTestConsoleWrapper>(MockBehavior.Strict);
            mockedVsTestConsole.Setup(x => x.StartSession());
            mockedVsTestConsole.Setup(x => x.InitializeExtensions(It.IsAny<List<string>>()));
            mockedVsTestConsole.Setup(x => x.AbortTestRun());
            mockedVsTestConsole.Setup(x =>
                x.DiscoverTests(It.Is<IEnumerable<string>>(d => d.Any(e => e == _testAssemblyPath)),
                    It.IsAny<string>(),
                    It.IsAny<ITestDiscoveryEventsHandler>())).Callback(
                (IEnumerable<string> _, string _, ITestDiscoveryEventsHandler discoveryEventsHandler) =>
                    DiscoverTests(discoveryEventsHandler, testCases, false));

            runner = new VsTestRunner(
                options,
                _targetProject,
                null,
                null,
                0,
                _fileSystem,
                new Mock<IVsTestHelper>().Object,
                wrapper: mockedVsTestConsole.Object,
                hostBuilder: _ => new MoqHost(endProcess, suceed, false));
            return mockedVsTestConsole;
        }

        [Fact]
        public void InitializeAndDiscoverTests()
        {
            using var endProcess = new EventWaitHandle(true, EventResetMode.ManualReset);
            BuildVsTestRunner(new StrykerOptions(), endProcess, out var runner);
            // make sure we have discovered first and second tests
            runner.DiscoverTests().Count.ShouldBe(TestCases.Count);
        }

        [Fact]
        public void RunInitialTests()
        {
            using var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset);
            var mockVsTest = BuildVsTestRunner(new StrykerOptions(), endProcess, out var runner);
            SetupMockTestRun(mockVsTest, new[] { ("T0", false), ("T1", true) }, endProcess);
            var result = runner.InitialTest();
            // tests are successful => run should be successful
            result.FailingTests.Count.ShouldBe(1);

        }

        [Fact]
        public void HandleVsTestCreationFailure()
        {
            using var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset);
            var mockVsTest = BuildVsTestRunner(new StrykerOptions(), endProcess, out var runner, TestCases, false);
            SetupMockTestRun(mockVsTest, new[] { ("T0", false), ("T1", true) }, endProcess);

            Action action = () => runner.InitialTest();
            action.ShouldThrow<GeneralStrykerException>();
        }

        [Fact]
        public void RunTests()
        {
            using var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset);
            var mockVsTest = BuildVsTestRunner(new StrykerOptions(), endProcess, out var runner);
            SetupMockTestRun(mockVsTest, true, endProcess, TestCases);
            var result = runner.TestMultipleMutants(null, new []{_mutant}, null);
            // tests are successful => run should be successful
            result.FailingTests.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void DetectTestsErrors()
        {
            var options = new StrykerOptions();
            using (var endProcess = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
                SetupMockTestRun(mockVsTest, false, endProcess, TestCases);
                var result = runner.TestMultipleMutants(null, new []{_mutant}, null);
                // run is failed
                result.FailingTests.IsEmpty.ShouldBeFalse();
            }
        }

        [Fact]
        public void DetectTimeout()
        {
            var options = new StrykerOptions
            {
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            using (var endProcess = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);

                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;", ["T1"] = "1;" }, endProcess);

                runner.CaptureCoverage(_targetProject.ProjectContents.Mutants);

                SetupMockTimeOutTestRun(mockVsTest, new Dictionary<string, string> { ["0"] = "T0=S" }, "T0", endProcess);

                var result = runner.TestMultipleMutants(null, new []{_mutant}, null);
                result.TimedOutTests.IsEmpty.ShouldBeFalse();
            }
        }

        [Fact]
        public void AbortOnError()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);

                mockVsTest.Setup(x => x.CancelTestRun()).Verifiable();
                SetupMockTestRun(mockVsTest, false, endProcess, TestCases);

                var result = runner.TestMultipleMutants(null, new []{_mutant},  ((_, _, _, _) => false));
                // verify Abort has been called
                Mock.Verify(mockVsTest);
                // and test run is failed
                result.FailingTests.IsEmpty.ShouldBeFalse();
            }
        }

        [Fact]
        public void CaptureCoverageWhenSkippingUncovered()
        {
            var options = new StrykerOptions
            {
                OptimizationMode = OptimizationModes.SkipUncoveredMutants
            };

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);

                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;", ["T1"] = ";" }, endProcess);

                runner.CaptureCoverage(_targetProject.ProjectContents.Mutants);
                _mutant.CoveringTests.IsEmpty.ShouldBe(false);
                var id = _mutant.CoveringTests.GetGuids().First();
                TestCases.Find(t => t.Id == id)?.DisplayName.ShouldBe("T0");
            }
        }

        [Fact]
        public void IdentifyNonCoveredMutants()
        {
            var options = new StrykerOptions
            {
                OptimizationMode = OptimizationModes.SkipUncoveredMutants
            };

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
                // test 0 and 1 cover mutant 1
                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;", ["T1"] = "0;" }, endProcess);

                var result = runner.CaptureCoverage(_targetProject.ProjectContents.Mutants);
                var testIds = _mutant.CoveringTests.GetGuids().ToList();
                // one mutant is covered by tests 0 and 1
                _mutant.CoveringTests.IsEmpty.ShouldBe(false);

                TestCases.Find(t => t.Id == testIds[0])?.DisplayName.ShouldBe("T0");
                TestCases.Find(t => t.Id == testIds[1])?.DisplayName.ShouldBe("T1");

                // verify Abort has been called
                result.FailingTests.IsEmpty.ShouldBeTrue();
            }
        }

        [Fact]
        public void WorksWhenAllMutantsAreIgnored()
        {
            var options = new StrykerOptions
            {
                OptimizationMode = OptimizationModes.SkipUncoveredMutants
            };

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
                // test 0 and 1 cover mutant 1
                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = ";", ["T1"] = ";" }, endProcess);

                runner.CaptureCoverage(Enumerable.Empty<Mutant>());
               _mutant.CoveringTests.Count.ShouldBe(0);
            }
        }

        [Fact]
        public void RunOnlyUsefulTest()
        {
            var options = new StrykerOptions
            {
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
                // only first test covers one mutant
                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;", ["T1"] = ";" }, endProcess);

                runner.CaptureCoverage(_targetProject.ProjectContents.Mutants);

                SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string> { ["0"] = "T0=S" }, endProcess);

                // process coverage information
                var result = runner.TestMultipleMutants(null, new []{_mutant}, null);
                // verify Abort has been called
                Mock.Verify(mockVsTest);
                // verify only one test has been run
                result.RanTests.Count.ShouldBe(1);
            }
        }

        [Fact]
        public void NotRunTestWhenNotCovered()
        {
            var options = new StrykerOptions
            {
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
                // only first test covers one mutant
                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;0", ["T1"] = ";" }, endProcess);

                _mutant.ResetCoverage();
                _otherMutant.ResetCoverage();
                runner.CaptureCoverage(_targetProject.ProjectContents.Mutants);

                SetupMockTestRun(mockVsTest, false, endProcess, TestCases);
                // mutant 0 is covered
                _mutant.IsStaticValue.ShouldBeTrue();
                var result = runner.TestMultipleMutants(null, new []{_mutant}, null);
                // mutant is killed
                result.FailingTests.IsEmpty.ShouldBeFalse();
                // mutant 1 is not covered
                 result = runner.TestMultipleMutants(null, new []{_otherMutant}, null);
                // tests are ok
                result.RanTests.IsEmpty.ShouldBeTrue();
            }
        }

        [Fact]
        public void RunTestsSimultaneouslyWhenPossible()
        {
            var options = new StrykerOptions()
            {
                OptimizationMode = OptimizationModes.DisableBail | OptimizationModes.CoverageBasedTest,
                Concurrency = Math.Max(Environment.ProcessorCount / 2, 1)
            };

            var mutantFilter = new Mock<IMutantFilter>(MockBehavior.Loose);

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {

                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
                // make sure we have 4 mutants
                _projectContents.Add(new CsharpFileLeaf { Mutants = new[] { new Mutant { Id = 2 }, new Mutant { Id = 3 } } });
                TestCases.Add(new TestCase("T2", _executorUri, _testAssemblyPath));
                TestCases.Add(new TestCase("T3", _executorUri, _testAssemblyPath));

                var ranTests = new TestsGuidList(TestCases.Select(t => t.Id));
                var testRunResult = new TestRunResult(ranTests, TestsGuidList.NoTest(), TestsGuidList.NoTest(),
                    string.Empty, TimeSpan.Zero);
                var input = new MutationTestInput
                {
                    ProjectInfo = _targetProject,
                    TestRunner = runner,
                    InitialTestRun = new InitialTestRun(testRunResult, new TimeoutValueCalculator(500))
                };
                foreach (var mutant in _targetProject.ProjectContents.Mutants)
                {
                    mutant.ResetCoverage();
                }
                var mockReporter = new Mock<IReporter>();
                var tester = new MutationTestProcess(input, mockReporter.Object, new MutationTestExecutor(input.TestRunner), fileSystem: _fileSystem, options: options, mutantFilter: mutantFilter.Object);
                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;", ["T1"] = "1;" }, endProcess);
                tester.GetCoverage();
                SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string> { ["1,0"] = "T0=S,T1=F" }, endProcess);
                tester.Test(_projectContents.Mutants.Where(x => x.ResultStatus == MutantStatus.NotRun));

                _mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
                _otherMutant.ResultStatus.ShouldBe(MutantStatus.Killed);
            }
        }

        [Fact]
        public void RunRelevantTestsOnStaticWhenPerTestCoverage()
        {
            var options = new StrykerOptions
            {
                OptimizationMode = OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest
            };

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);


                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0,1;1", ["T1"] = ";" }, endProcess);

                runner.CaptureCoverage(_targetProject.ProjectContents.Mutants);

                SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string> { ["0"] = "T0=F", ["1"] = "T0=S" }, endProcess);
                var result = runner.TestMultipleMutants(null, new []{_otherMutant}, null);
                result.FailingTests.IsEmpty.ShouldBeTrue();
                result = runner.TestMultipleMutants(null, new []{_mutant}, null);
                result.FailingTests.IsEmpty.ShouldBeFalse();
            }
        }

        [Fact]
        public void HandleMultipleTestResults()
        {
            var options = new StrykerOptions();
            using var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset);
            var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
            // assume 2 results for T0
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) }, endProcess);
            var result = runner.InitialTest();
            // initial test is fine
            result.FailingTests.IsEmpty.ShouldBeTrue();
            // test session will fail
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T0", true), ("T1", true) }, endProcess);
            result = runner.TestMultipleMutants(null, new []{_mutant}, null);
            result.FailingTests.IsEmpty.ShouldBeTrue();
            result.RanTests.IsEveryTest.ShouldBeTrue();
        }

        [Fact]
        public void HandleMultipleTestResultsForXUnit()
        {
            var options = new StrykerOptions();
            using var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset);
            var tests = new List<TestCase>
            {
                new("X0", _xUnitUri, "test.cs"),
                new("X1", _xUnitUri, "test.cs")
            };
            var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, tests);
            // assume 2 results for T0
            SetupMockTestRun(mockVsTest, new[] { ("X0", true), ("X1", true), ("X0", true) , ("X0", true), ("X0", true)}, endProcess, tests);
            var result = runner.InitialTest();
            // the duration should be less than 3 times the (test) default duration
            result.Duration.ShouldBeLessThan(_testDefaultDuration.Duration()*3);
        }

        [Fact]
        public void HandleFailureWithinMultipleTestResults()
        {
            var options = new StrykerOptions();
            using var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset);
            var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
            // assume 2 results for T0
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) }, endProcess);
            var result = runner.InitialTest();
            // initial test is fine
            result.FailingTests.IsEmpty.ShouldBeTrue();
            // test session will fail on test 1
            SetupMockTestRun(mockVsTest, new[] { ("T0", false), ("T0", true), ("T1", true) }, endProcess);

            result = runner.TestMultipleMutants(null, new []{_mutant}, null);
            result.RanTests.IsEveryTest.ShouldBeTrue();
            result.FailingTests.IsEmpty.ShouldBeFalse();
            result.FailingTests.GetGuids().ShouldContain(TestCases[0].Id);
            // test session will fail on the other test result
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T0", false), ("T1", true) }, endProcess);
            result = runner.TestMultipleMutants(null, new []{_mutant}, null);
            result.RanTests.IsEveryTest.ShouldBeTrue();
            result.FailingTests.IsEmpty.ShouldBeFalse();
            result.FailingTests.GetGuids().ShouldContain(TestCases[0].Id);
        }

        [Fact]
        public void HandleTimeOutWithMultipleTestResults()
        {
            var options = new StrykerOptions();
            using var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset);
            var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
            // assume 2 results for T0
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) }, endProcess);
            var result = runner.InitialTest();
            // initial test is fine
            result.FailingTests.IsEmpty.ShouldBeTrue();
            // test session will fail
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true) }, endProcess);
            result = runner.TestMultipleMutants(null, new []{_mutant}, null);

            result.FailingTests.IsEmpty.ShouldBeTrue();
            result.TimedOutTests.Count.ShouldBe(1);
            result.TimedOutTests.GetGuids().ShouldContain(TestCases[0].Id);
            result.RanTests.IsEveryTest.ShouldBeFalse();
        }

        [Fact]
        public void HandleFailureWhenExtraMultipleTestResults()
        {
            var options = new StrykerOptions();
            using var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset);
            var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
            // assume 2 results for T0
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) }, endProcess);
            var result = runner.InitialTest();
            // initial test is fine
            result.FailingTests.IsEmpty.ShouldBeTrue();
            // test session will fail
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T0", true), ("T0", false), ("T1", true) }, endProcess);
            result = runner.TestMultipleMutants(null, new []{_mutant}, null);
            result.RanTests.IsEveryTest.ShouldBeTrue();
            result.FailingTests.IsEmpty.ShouldBeFalse();
            result.FailingTests.GetGuids().ShouldContain(TestCases[0].Id);
        }

        [Fact]
        public void HandleUnexpectedTestResult()
        {
            var options = new StrykerOptions();
            using var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset);
            var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);
            // assume 2 results for T0
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) }, endProcess);
            var result = runner.InitialTest();
            // initial test is fine
            result.FailingTests.IsEmpty.ShouldBeTrue();
            // test session will fail
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T2", true), ("T1", true), ("T0", true) }, endProcess);
            result = runner.TestMultipleMutants(null, new []{_mutant}, null);
            result.RanTests.IsEveryTest.ShouldBeTrue();
            result.FailingTests.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void MarkSuspiciousCoverage()
        {
            var options = new StrykerOptions
            {
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner);

                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;|1", ["T1"] = ";" }, endProcess);

                runner.CaptureCoverage(_targetProject.ProjectContents.Mutants);
                _otherMutant.IsStaticValue.ShouldBeTrue();
            }
        }

        // class mocking the VsTest Host Launcher
        private class MoqHost : IStrykerTestHostLauncher
        {
            private readonly WaitHandle _handle;

            public MoqHost(WaitHandle handle, bool succeed, bool isDebug)
            {
                _handle = handle;
                IsProcessCreated = succeed;
                IsDebug = isDebug;
            }

            public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo)
            {
                throw new NotImplementedException();
            }

            public int LaunchTestHost(TestProcessStartInfo defaultTestHostStartInfo, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public bool IsDebug { get; }

            public bool IsProcessCreated { get; }
        }
    }
}
