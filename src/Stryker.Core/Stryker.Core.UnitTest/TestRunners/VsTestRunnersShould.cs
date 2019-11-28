﻿using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners.VsTest;
using Stryker.Core.ToolHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    /// <summary>
    /// This class hosts the VsTestRunner related tests. The design of VsTest implies the creation of many mocking objects, so the tests may be hard to read.
    /// This is sad but expected. Please use caution when changing/creating tests.
    /// </summary>
    public class VsTestRunnersShould
    {
        private readonly string _testAssemblyPath;
        private readonly ProjectInfo _targetProject;
        private readonly MockFileSystem _fileSystem;
        private readonly Mutant _mutant;
        private readonly List<TestCase> _testCases;
        private readonly Mutant _otherMutant;
        private readonly FolderComposite _mutants;
        private readonly Uri _executorUri;
        private readonly TestProperty _coverageProperty;

        // initialize the test context and mock objects
        public VsTestRunnersShould()
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filesystemRoot = Path.GetPathRoot(currentDirectory);

            var sourceFile = File.ReadAllText(currentDirectory + "/TestResources/ExampleSourceFile.cs");
            var testProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(filesystemRoot, "TestProject", "TestProject.csproj"));
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
            _testAssemblyPath = Path.Combine(filesystemRoot, "_firstTest", "bin", "Debug", "TestApp.dll");
            _executorUri = new Uri("exec://nunit");
            var firstTest = new TestCase("T0", _executorUri, _testAssemblyPath);
            var secondTest = new TestCase("T1", _executorUri, _testAssemblyPath);

            var content = new FolderComposite();
            _coverageProperty= TestProperty.Register("Stryker.Coverage", "Coverage", typeof(string), typeof(TestResult));
            _mutant = new Mutant{Id = 0};
            _otherMutant = new Mutant{Id = 1};
            _mutants = content;
            _mutants.Add(new FileLeaf{Mutants = new []{_otherMutant, _mutant}});
            _targetProject = new ProjectInfo()
            {
                TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = _testAssemblyPath,
                    TargetFrameworkString = "toto"
                },
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = Path.Combine(filesystemRoot, "app", "bin", "Debug", "AppToTest.dll"),
                    TargetFrameworkString = "toto"
                },
                ProjectContents = _mutants
            };
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                { Path.Combine(filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                { _testAssemblyPath, new MockFileData("Bytecode") },
                { Path.Combine(filesystemRoot, "app", "bin", "Debug", "AppToTest.dll"), new MockFileData("Bytecode") },
            });

            _testCases = new List<TestCase>{firstTest, secondTest};
        }

        // mock a VsTest run. Provides test result one by one at 10 ms intervals
        // note: a lot of information is still missing (vs real VsTest). You will have to add them if your test requires them
        private void MoqTestRun(ITestRunEventsHandler testRunEvents, IReadOnlyList<TestResult> testResults)
        {
            Task.Run(() =>
            {
                var timer = new Stopwatch();
                testRunEvents.HandleTestRunStatsChange(
                    new TestRunChangedEventArgs(new TestRunStatistics(0, null), null, new [] {testResults[0].TestCase}));

                for(var i = 0; i<testResults.Count; i++)
                {
                    Thread.Sleep(10);
                    testResults[i].EndTime = DateTimeOffset.Now;
                    testRunEvents.HandleTestRunStatsChange(new TestRunChangedEventArgs(
                        new TestRunStatistics(i+1, null), new []{testResults[i]}, null));
                }
                Thread.Sleep(10);
                testRunEvents.HandleTestRunComplete(
                    new TestRunCompleteEventArgs(new TestRunStatistics(testResults.Count, null), false, false, null,
                        null, timer.Elapsed),
                    null,
                    null,
                    null);
            });
        }

        // setup a simple (mock) test run where all tests fail or succeed
        private void SetupMockTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, bool testResult, EventWaitHandle synchroObject)
        {
            var results = new TestResult[_testCases.Count];
            for(var i = 0; i < _testCases.Count; i++)
            {
                results[i] = new TestResult(_testCases[i])
                {
                    Outcome = testResult ? TestOutcome.Passed : TestOutcome.Failed, ComputerName = "."
                };
            }
            SetupMockTestRun(mockVsTest, results, synchroObject);
        }

        // setup a customized test run using provided test results
        private void SetupMockTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyList<TestResult> testResults, EventWaitHandle synchroObject)
        {
            mockVsTest.Setup(x =>
                x.RunTestsWithCustomTestHost(
                    It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                    It.Is<string>(settings =>!settings.Contains("DataCollector")),
                    It.IsAny<ITestRunEventsHandler>(),
                    It.IsAny<ITestHostLauncher>())).Callback(
                (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                    ITestHostLauncher host) =>
                {
                    // generate test results
                    MoqTestRun(testRunEvents, testResults);
                    synchroObject.Set();
                });
        }
        
        // setup a customized coverage capture run, using provided coverage results
        private void SetupMockCoverageRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> coverageResults, EventWaitHandle endProcess)
        {
            mockVsTest.Setup(x =>
                x.RunTestsWithCustomTestHost(
                    It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                    It.Is<string>(settings => settings.Contains("DataCollector")),
                    It.IsAny<ITestRunEventsHandler>(),
                    It.IsAny<ITestHostLauncher>())).Callback(
                (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                    ITestHostLauncher host) =>
                {
                    // generate test results
                    var results = new List<TestResult>(coverageResults.Count);
                    foreach (var (key, value) in coverageResults)
                    {
                        var result = new TestResult(_testCases.First(t => t.FullyQualifiedName == key))
                        {
                            Outcome = TestOutcome.Passed, ComputerName = "."
                        };
                        result.SetPropertyValue(_coverageProperty, value);
                        results.Add(result);
                    }
                    MoqTestRun(testRunEvents, results);
                    endProcess.Set();
                });
        }

        // setup a customized partial test runs, using provided test results
        private void SetupMockPartialTestRun(Mock<IVsTestConsoleWrapper> mockVsTest, IReadOnlyDictionary<string, string> results, EventWaitHandle endProcess)
        {

            mockVsTest.Setup(x =>   
                x.RunTestsWithCustomTestHost(
                    It.IsAny<IEnumerable<TestCase>>(),
                    It.Is<string>( s=> !s.Contains("DataCollector")),
                    It.IsAny<ITestRunEventsHandler>(),
                    It.IsAny<ITestHostLauncher>())).Callback(
                (IEnumerable<TestCase> sources, string settings, ITestRunEventsHandler testRunEvents,
                    ITestHostLauncher host) =>
                {
                    var mutants = (host as MoqHost).ActiveMutants;
                    var tests = sources.ToList();
                    if (!results.ContainsKey(mutants))
                    {
                        throw new ArgumentException($"Unexpected mutant run {mutants}");
                    }

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
                            {Outcome = strings[1] == "F" ? TestOutcome.Failed : TestOutcome.Passed, ComputerName = "."};
                        runResults.Add(result);
                    }
                    // setup a normal test run

                    MoqTestRun(testRunEvents, runResults);
                    endProcess.Set();
                });

            mockVsTest.Setup(x =>   
                x.RunTestsWithCustomTestHost(
                    It.IsAny<IEnumerable<string>>(),
                    It.Is<string>( s=> !s.Contains("DataCollector")),
                    It.IsAny<ITestRunEventsHandler>(),
                    It.IsAny<ITestHostLauncher>())).Callback(
                (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                    ITestHostLauncher host) =>
                {
                    var mutants = (host as MoqHost).ActiveMutants;
                    var tests = sources.ToList();
                    if (!results.ContainsKey(mutants))
                    {
                        throw new ArgumentException($"Unexpected mutant run {mutants}");
                    }

                    var data = results[mutants].Split(',').Select(e => e.Split('=')).ToList();
                    var runResults = new List<TestResult>(data.Count);
                    foreach (var strings in data)
                    {
                        var matchingTest = _testCases.FirstOrDefault(t => t.FullyQualifiedName == strings[0]);
                        if (matchingTest == null)
                        {
                            throw new ArgumentException($"Test {strings[0]} not run for mutant {mutants}.");
                        }

                        var result = new TestResult(matchingTest)
                            {Outcome = strings[1] == "F" ? TestOutcome.Failed : TestOutcome.Passed, ComputerName = "."};
                        runResults.Add(result);
                    }
                    // setup a normal test run
                    MoqTestRun(testRunEvents, runResults);
                    endProcess.Set();
                });
        }

        private Mock<IVsTestConsoleWrapper> BuildVsTestRunner(StrykerOptions options, WaitHandle endProcess, out VsTestRunner runner, OptimizationFlags optimizationFlags)
        {
            var mockVsTest = new Mock<IVsTestConsoleWrapper>(MockBehavior.Strict);
            mockVsTest.Setup(x => x.StartSession());
            mockVsTest.Setup(x => x.InitializeExtensions(It.IsAny<List<string>>()));
            mockVsTest.Setup(x => x.AbortTestRun());
            mockVsTest.Setup(x =>
                x.DiscoverTests(It.Is<IEnumerable<string>>(d => d.Any(e => e == _testAssemblyPath)),
                    It.IsAny<string>(),
                    It.IsAny<ITestDiscoveryEventsHandler>())).Callback(
                (IEnumerable<string> sources, string discoverySettings, ITestDiscoveryEventsHandler discoveryEventsHandler) =>
                    DiscoverTests(sources, discoverySettings, discoveryEventsHandler, _testCases, false));

            runner = new VsTestRunner(
                options,
                optimizationFlags,
                _targetProject,
                null,
                _fileSystem,
                helper: new Mock<IVsTestHelper>().Object,
                wrapper: mockVsTest.Object,
                hostBuilder: ((dictionary, i) => new MoqHost(endProcess, dictionary, i)));
            return mockVsTest;
        }

        [Fact]
        public void InitializeAndDiscoverTests()
        {
            using (var endProcess = new EventWaitHandle(true, EventResetMode.ManualReset))
            {
                var options = new StrykerOptions();
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.NoOptimization);
                // make sure we have discovered first and second tests
                runner.DiscoverNumberOfTests().ShouldBe(_testCases.Count);
            }
        }

        [Fact]
        public void RunTests()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.NoOptimization);
                SetupMockTestRun(mockVsTest, true, endProcess);
                var result = runner.RunAll(null, _mutant, null);
                // tests are successful => run should be successful
                result.Success.ShouldBe(true);
            }
        }

        [Fact]
        public void DetectTestsErrors()
        {
            var options = new StrykerOptions();
            using (var endProcess = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.NoOptimization);
                SetupMockTestRun(mockVsTest, false, endProcess);
                var result = runner.RunAll(null, _mutant, null);
                // run is failed
                result.Success.ShouldBe(false);
            }
        }

        [Fact]
        public void DetectTimeout()
        {
            var options = new StrykerOptions();
            using (var endProcess = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.NoOptimization);
                var singleTestResult = new TestResult(_testCases[0])
                {
                    EndTime = DateTimeOffset.Now,
                    Outcome = TestOutcome.Passed
                };
                SetupMockTestRun(mockVsTest, new List<TestResult>{singleTestResult}, endProcess);

                // timeout is notified via exception
                var result = runner.RunAll(null, _mutant, null);
               result.TimeOut.ShouldBe(true);
            }
        }

        [Fact]
        public void AbortOnError()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.AbortTestOnKill);

                mockVsTest.Setup(x => x.AbortTestRun()).Verifiable();
                SetupMockTestRun(mockVsTest, false, endProcess);

                var result = runner.RunAll(null, _mutant, ((mutants, tests, failedTests) => false));
                // verify Abort has been called
                Mock.Verify(mockVsTest);
                // and test run is failed
                result.Success.ShouldBe(false);
            }
        }

        [Fact]
        public void CaptureCoverageWhenSkippingUncovered()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.SkipUncoveredMutants);

                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string>{["T0"] = "0;", ["T1"] = ";"}, endProcess);

                var result = runner.CaptureCoverage(_targetProject.ProjectContents.Mutants, false, false);
                _mutant.CoveringTests.IsEmpty.ShouldBe(false);
                _mutant.CoveringTests.GetList()[0].Name.ShouldBe("T0");

                // capture when ok
                result.Success.ShouldBe(true);
            }
        }

        [Fact]
        public void IdentifyNonCoveredMutants()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.SkipUncoveredMutants);
                // test 0 and 1 cover mutant 1
                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string>{["T0"] = "0;", ["T1"] = "0;"}, endProcess);

                var result = runner.CaptureCoverage(_targetProject.ProjectContents.Mutants, false, false);
                // one mutant is covered by tests 0 and 1
                _mutant.CoveringTests.IsEmpty.ShouldBe(false);
                _mutant.CoveringTests.GetList()[0].Name.ShouldBe("T0");
                _mutant.CoveringTests.GetList()[1].Name.ShouldBe("T1");

                // verify Abort has been called
                result.Success.ShouldBe(true);
            }
        }

        [Fact]
        public void RunOnlyUsefulTest()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.CoverageBasedTest);
                // only first test covers one mutant
                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string>{["T0"] = "0;", ["T1"] = ";"}, endProcess);

                runner.CaptureCoverage(_targetProject.ProjectContents.Mutants, false, false);

                SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string>{["0"]="T0=S"}, endProcess);

                // process coverage information
                var result = runner.RunAll(0, _mutant, null);

                // verify Abort has been called
                result.Success.ShouldBe(true);
            }
        }

        [Fact]
        public void NotRunTestWhenNotCovered()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.CoverageBasedTest);
                // only first test covers one mutant
                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string>{["T0"] = "0;0", ["T1"] = ";"}, endProcess);

                _mutant.CoveringTests = new TestListDescription(null);
                _otherMutant.CoveringTests = new TestListDescription(null);
                runner.CaptureCoverage(_targetProject.ProjectContents.Mutants, false, false);

                SetupMockTestRun(mockVsTest, false, endProcess);
                // mutant 0 is covered
                _mutant.IsStaticValue.ShouldBeTrue();
                var result = runner.RunAll(0, _mutant, null);
                // mutant is killed
                result.Success.ShouldBe(false);
                // mutant 1 is not covered
                result = runner.RunAll(0, _otherMutant, null);
                // tests are ok
                result.Success.ShouldBe(true);
            }
        }

        [Fact]
        public void RunTestsSimultaneouslyWhenPossible()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var strykerOptions = new StrykerOptions(fileSystem:_fileSystem, "perTest", abortTestOnFail:false, allowSimultaneousTesting:true);
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, strykerOptions.Optimizations);
                // make sure we have 4 mutants
                _mutants.Add(new FileLeaf{Mutants = new []{new Mutant{Id = 2}, new Mutant{Id = 3}}});
                _testCases.Add(new TestCase("T2", _executorUri, _testAssemblyPath));
                _testCases.Add(new TestCase("T3", _executorUri, _testAssemblyPath));

                var input = new MutationTestInput {ProjectInfo = _targetProject, TestRunner = runner, TimeoutMs = 100};
                var mockReporter = new Mock<IReporter>();
                var tester = new MutationTestProcess(input, mockReporter.Object, new MutationTestExecutor(input.TestRunner), fileSystem: _fileSystem, options: strykerOptions);
                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string>{["T0"] = "0;", ["T1"] = "1;"}, endProcess);

                runner.CaptureCoverage(_targetProject.ProjectContents.Mutants, false, false);
                SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string>{["1,0,2,3"]="T0=S,T1=F"}, endProcess);
                tester.Test(strykerOptions);

                _mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
                _otherMutant.ResultStatus.ShouldBe(MutantStatus.Killed);
            }
        }

        [Fact]
        public void RunRelevantTestsOnStaticWhenPerTestCoverage()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.CoverageBasedTest|OptimizationFlags.CaptureCoveragePerTest);

                // only first test covers one mutant
                SetupMockCoverageRun(mockVsTest, new Dictionary<string, string>{["T0"] = "0,1;1", ["T1"] = ";"}, endProcess);

                runner.CaptureCoverage(_targetProject.ProjectContents.Mutants, false, false);

                SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string>{["0"] = "T0=F", ["1"]="T0=S"}, endProcess);
                var result = runner.RunAll(0, _otherMutant, null);
                result.Success.ShouldBe(true);
                result = runner.RunAll(null, _mutant, null);
                result.Success.ShouldBe(false);
            }
        }

        // simulate the discovery of tests
        private static void DiscoverTests(IEnumerable<string> sources, string discoverySettings, ITestDiscoveryEventsHandler discoveryEventsHandler, ICollection<TestCase> tests, bool aborted)
        {
            Task.Run(() => discoveryEventsHandler.HandleDiscoveredTests(tests)).
                ContinueWith((t, u) => discoveryEventsHandler.HandleDiscoveryComplete((int)u, null, aborted), tests.Count);
        }

        // class mocking the VsTest Host Launcher
        private class MoqHost : IStrykerTestHostLauncher
        {
            private readonly WaitHandle _handle;
            private readonly IDictionary<string, string> _dico;
            private readonly int _id;

            public MoqHost(WaitHandle handle, IDictionary<string, string> dico, int id)
            {
                _handle = handle;
                _dico = dico;
                _id = id;
            }

            public string ActiveMutants
            {
                get
                {
                    _dico.TryGetValue("ActiveMutation", out var result);
                    return result;
                }
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

            public bool WaitProcessExit()
            {
                return _handle == null || _handle.WaitOne();
            }
        }
    }
}
