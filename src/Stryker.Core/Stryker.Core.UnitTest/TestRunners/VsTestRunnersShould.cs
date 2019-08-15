using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Interfaces;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.TestRunners.VsTest;
using Stryker.Core.ToolHelpers;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    /// <summary>
    /// This class hosts the VsTestRunner related tests. The design of VsTest implies the creation of many mocking objects, so the test may be hard to read.
    /// This is sad but expected. Please use caution when changing/creating tests.
    /// </summary>
    public class VsTestRunnersShould
    {
        private readonly string _testAssemblyPath;
        private readonly ProjectInfo _targetProject;
        private readonly MockFileSystem _fileSystem;
        private readonly Mutant _mutant;
        private readonly TestCase[] _testCases;

        // initialize the test context and mock objects
        public VsTestRunnersShould()
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filesystemRoot = Path.GetPathRoot(currentDirectory);

            var sourceFile = File.ReadAllText(currentDirectory + "/TestResources/ExampleSourceFile.cs");
            var testProjectPath = FilePathUtils.ConvertPathSeparators(Path.Combine(filesystemRoot, "TestProject", "TestProject.csproj"));
            var projectUnderTestPath = FilePathUtils.ConvertPathSeparators(Path.Combine(filesystemRoot, "ExampleProject", "ExampleProject.csproj"));
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
            var firstTest = new TestCase("myFirsTest", new Uri("exec://nunit"), _testAssemblyPath);
            var secondTest = new TestCase("myOtherTest", new Uri("exec://nunit"), _testAssemblyPath);
            _targetProject = new ProjectInfo()
            {
                TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = _testAssemblyPath,
                    TargetFramework = "toto"
                },
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = Path.Combine(filesystemRoot, "app", "bin", "Debug", "AppToTest.dll"),
                    TargetFramework = "toto"
                }
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

            _mutant = new Mutant {Id = 1};
            _testCases = new []{firstTest, secondTest};
        }

        private Mock<IVsTestConsoleWrapper> BuildVsTestMock(StrykerOptions options)
        {
            var mockVsTest = new Mock<IVsTestConsoleWrapper>(MockBehavior.Strict);
            mockVsTest.Setup(x => x.StartSession());
            mockVsTest.Setup(x => x.InitializeExtensions(It.IsAny<List<string>>()));
            mockVsTest.Setup(x =>
                x.DiscoverTests(It.Is<IEnumerable<string>>(d => d.Any(e => e == _testAssemblyPath)),
                    It.IsAny<string>(),
                    It.IsAny<ITestDiscoveryEventsHandler>())).Callback(
                (IEnumerable<string> sources, string discoverySettings, ITestDiscoveryEventsHandler discoveryEventsHandler) =>
                    DiscoverTests(sources, discoverySettings, discoveryEventsHandler, _testCases, false));
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
                runner.DiscoverNumberOfTests().ShouldBe(2);
            }
        }

        [Fact]
        public void RunTests()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.NoOptimization);

                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                        It.IsAny<string>(),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        // generate two successful test results
                        MoqTestRun(testRunEvents, _testCases, true);
                        endProcess.Set();
                    });


                var result = runner.RunAll(null, _mutant);

                // tests are successful => run should be successful
                result.Success.ShouldBe(true);
            }
        }

        private Mock<IVsTestConsoleWrapper> BuildVsTestRunner(StrykerOptions options, EventWaitHandle endProcess, out VsTestRunner runner, OptimizationFlags optimizationFlags)
        {
            var mockVsTest = BuildVsTestMock(options);
            runner = new VsTestRunner(
                options,
                optimizationFlags,
                _targetProject,
                null,
                null,
                _fileSystem,
                helper: new Mock<IVsTestHelper>().Object,
                wrapper: mockVsTest.Object,
                hostBuilder: ((dictionary, i) => new MoqHost(endProcess, dictionary, i)));
            return mockVsTest;
        }

        [Fact]
        public void DetectTestsErrors()
        {
            var options = new StrykerOptions();
            using (var endProcess = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.NoOptimization);

                mockVsTest.Setup(x => 
                    x.RunTestsWithCustomTestHost(It.Is<IEnumerable<string>>(t => t.Any( source => source == _testAssemblyPath)), 
                        It.IsAny<string>(), 
                        It.IsAny<ITestRunEventsHandler>(), 
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        // tests are failing
                        MoqTestRun(testRunEvents, _testCases, false);
                        endProcess.Set();
                    });

                var result = runner.RunAll(null, _mutant);
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

                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                        It.IsAny<string>(),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        // generate in incomplete run
                        Task.Run(() =>
                        {
                            var timer = new Stopwatch();
                            testRunEvents.HandleTestRunStatsChange(
                                new TestRunChangedEventArgs(new TestRunStatistics(0, null), null, _testCases));

                            Thread.Sleep(10);
                            var testResult = new TestResult(((IReadOnlyList<TestCase>) _testCases)[0])
                            {
                                EndTime = DateTimeOffset.Now, Outcome = TestOutcome.Passed
                            };
                            testRunEvents.HandleTestRunStatsChange(new TestRunChangedEventArgs(
                                new TestRunStatistics(1, null), new[] {testResult}, null));
                            testRunEvents.HandleTestRunComplete(
                                new TestRunCompleteEventArgs(new TestRunStatistics(1, null), false, false, null,
                                    null, timer.Elapsed),
                                null,
                                null,
                                null);
                            endProcess.Set();
                        });
                    });
                
                // timeout is notified via exception
                Should.Throw<OperationCanceledException>(() => runner.RunAll(null, _mutant));
            }
        }

        [Fact]
        public void AbortOnError()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.AbortTestOnKill);

                mockVsTest.Setup( x=>x.AbortTestRun()).Verifiable();
                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                        It.IsAny<string>(),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        // generate failed test results
                        MoqTestRun(testRunEvents, _testCases, false);
                        endProcess.Set();
                    });

                var result = runner.RunAll(null, _mutant);
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
                var coverageProperty= TestProperty.Register("Stryker.Coverage", "Coverage", typeof(string), typeof(TestResult));
                mockVsTest.Setup( x=>x.AbortTestRun()).Verifiable();
                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                        It.IsAny<string>(),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        Task.Run(() =>
                        {
                            // ensure coverage capture is properly configured
                            settings.ShouldContain("DataCollector");
                            var results = new TestResult[_testCases.Length];
                            for (var i = 0; i < _testCases.Length; i++)
                            {
                                results[i] = new TestResult(_testCases[i])
                                {
                                    Outcome = TestOutcome.Passed, ComputerName = "."
                                };
                            }

                            results[0].SetPropertyValue(coverageProperty, "1;");
                            MoqTestRun(testRunEvents, results);
                            endProcess.Set();
                        });
                    });

                var result = runner.CaptureCoverage();

                // only one mutant is covered
                runner.CoverageMutants.CoveredMutants.ShouldHaveSingleItem();

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

                var coverageProperty= TestProperty.Register("Stryker.Coverage", "Coverage", typeof(string), typeof(TestResult));
                mockVsTest.Setup( x=>x.AbortTestRun()).Verifiable();
                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                        It.IsAny<string>(),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        Task.Run(() =>
                        {
                            // generate coverage data covering only mutant (1)
                            settings.ShouldContain("DataCollector");
                            var results = new TestResult[_testCases.Length];
                            for (var i = 0; i < _testCases.Length; i++)
                            {
                                results[i] = new TestResult(_testCases[i])
                                {
                                    Outcome = TestOutcome.Passed, ComputerName = "."
                                };
                                results[i].SetPropertyValue(coverageProperty, "1;");
                            }

                            MoqTestRun(testRunEvents, results);
                            endProcess.Set();
                        });
                    });

                var result = runner.CaptureCoverage();
                // one mutant is covered
                runner.CoverageMutants.CoveredMutants.ShouldHaveSingleItem();
                // it is covered by both tests
                runner.CoverageMutants.GetTests(new Mutant(){Id = 1}).ShouldBe(_testCases.Select(x => (TestDescription) x));
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

                var coverageProperty= TestProperty.Register("Stryker.Coverage", "Coverage", typeof(string), typeof(TestResult));
                mockVsTest.Setup( x=>x.AbortTestRun()).Verifiable();
                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                        It.IsAny<string>(),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        Task.Run(() =>
                        {
                            // generate coverage results
                            settings.ShouldContain("DataCollector");
                            var results = new TestResult[_testCases.Length];
                            for (var i = 0; i < _testCases.Length; i++)
                            {
                                results[i] = new TestResult(_testCases[i])
                                {
                                    Outcome = TestOutcome.Passed, ComputerName = "."
                                };
                            }

                            // only first test covers one mutant
                            results[0].SetPropertyValue(coverageProperty, "1;");
                            MoqTestRun(testRunEvents, results);
                            endProcess.Set();
                        });
                    });

                runner.CaptureCoverage();

                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        // we expect a run with only one test
                        It.Is<IEnumerable<TestCase>>(t => t.Count()==1),
                        It.IsAny<string>(),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<TestCase> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        settings.ShouldNotContain("DataCollector");
                        // setup a normal test run
                        var results = sources.Select(test => new TestResult(test) {Outcome = TestOutcome.Passed, ComputerName = "."}).ToList();

                        results.ShouldHaveSingleItem();
                        MoqTestRun(testRunEvents, results);
                        endProcess.Set();
                    });

                var mutant = new Mutant{Id = 1};
                var mutants = new List<Mutant> {mutant};
                // process coverage information
                runner.CoverageMutants.UpdateMutants(mutants, _testCases.Length);

                var result = runner.RunAll(0, mutant);

                // verify Abort has been called
                result.Success.ShouldBe(true);
            }
        }

        [Fact]
        public void RunAllTestsOnStatic()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.CoverageBasedTest);

                var coverageProperty= TestProperty.Register("Stryker.Coverage", "Coverage", typeof(string), typeof(TestResult));
                mockVsTest.Setup( x=>x.AbortTestRun()).Verifiable();
                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                        It.Is<string>(settings => settings.Contains("DataCollector")),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        Task.Run(() =>
                        {
                            // provides coverage result with static mutant
                            var results = new TestResult[_testCases.Length];
                            for (var i = 0; i < _testCases.Length; i++)
                            {
                                results[i] = new TestResult(_testCases[i])
                                {
                                    Outcome = TestOutcome.Passed, ComputerName = "."
                                };
                            }

                            results[0].SetPropertyValue(coverageProperty, "1;1");
                            MoqTestRun(testRunEvents, results);
                            endProcess.Set();
                        });
                    });

                runner.CaptureCoverage();

                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.IsAny<IEnumerable<string>>(),
                        It.Is<string>(settings => !settings.Contains("DataCollector")),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        var results = _testCases.Select(test => new TestResult(test) {Outcome = TestOutcome.Failed, ComputerName = "."}).ToList();
                        // we expect only one test
                        results.Count.ShouldBe(2);
                        MoqTestRun(testRunEvents, results);
                        endProcess.Set();
                    });
                var mutant = new Mutant{Id = 1};
                var otherMutant = new Mutant{Id = 0};
                var mutants = new List<Mutant> {mutant, otherMutant};
                // process coverage info
                runner.CoverageMutants.UpdateMutants(mutants, _testCases.Length);
                // mutant 1 is covered
                mutant.MustRunAllTests.ShouldBeTrue();
                var result = runner.RunAll(0, mutant);
                // mutant is killed
                result.Success.ShouldBe(false);
                // mutant 0 is not covered
                result = runner.RunAll(0, otherMutant);
                // tests are ok
                result.Success.ShouldBe(true);
            }
        }

        [Fact]
        public void RunRelevantTestsOnStaticWhenPerTestCoverage()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestRunner(options, endProcess, out var runner, OptimizationFlags.CoverageBasedTest|OptimizationFlags.CaptureCoveragePerTest);

                var coverageProperty= TestProperty.Register("Stryker.Coverage", "Coverage", typeof(string), typeof(TestResult));
                mockVsTest.Setup( x=>x.AbortTestRun()).Verifiable();
                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                        It.IsAny<string>(),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        settings.ShouldContain("DataCollector");
                        var results = new TestResult[_testCases.Length];
                        for(var i = 0; i < _testCases.Length; i++)
                        {
                            results[i] = new TestResult(_testCases[i])
                            {
                                Outcome = TestOutcome.Passed, ComputerName = "."
                            };
                        }
                        results[0].SetPropertyValue(coverageProperty, "0,1;1");
                        MoqTestRun(testRunEvents, results);
                        endProcess.Set();
                    });

                runner.CaptureCoverage();

                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.Is<IEnumerable<TestCase>>(t => t.Count()==1),
                        It.IsAny<string>(),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<TestCase> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        settings.ShouldNotContain("DataCollector");
                        var results = new List<TestResult>();
                        foreach(var test in sources)
                        {
                            results.Add( new TestResult(test)
                            {
                                Outcome = TestOutcome.Passed, ComputerName = "."
                            });
                        }

                        results.ShouldHaveSingleItem();
                        MoqTestRun(testRunEvents, results);
                        endProcess.Set();
                    });
                var mutant = new Mutant{Id = 1};
                runner.CoverageMutants.GetTests(mutant).Count.ShouldBe(2);
                var otherMutant = new Mutant{Id = 0};
                foreach (var testDescription in runner.CoverageMutants.GetTests(otherMutant))
                {
                    otherMutant.CoveringTests[testDescription.Guid] = false;
                }
                var result = runner.RunAll(0, otherMutant);

                // verify Abort has been called
                result.Success.ShouldBe(true);
            }
        }

        // mock a VsTest run. Generate test results from the provided list of tests that either succed of fail depending on 'pass'
        private void MoqTestRun(ITestRunEventsHandler testRunEvents, IReadOnlyList<TestCase> testCases, bool pass)
        {
            var results = new TestResult[testCases.Count];
            for(var i = 0; i < testCases.Count; i++)
            {
                results[i] = new TestResult(testCases[i])
                {
                    Outcome = pass ? TestOutcome.Passed : TestOutcome.Failed, ComputerName = "."
                };
            }
            MoqTestRun(testRunEvents, results);
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

        // simulate the discovery of tests
        private void DiscoverTests(IEnumerable<string> sources, string discoverySettings, ITestDiscoveryEventsHandler discoveryEventsHandler, ICollection<TestCase> tests, bool aborted)
        {
            Task.Run(() => discoveryEventsHandler.HandleDiscoveredTests(tests)).
                ContinueWith((t, u) => discoveryEventsHandler.HandleDiscoveryComplete((int)u, null, aborted), tests.Count);
        }

        // class mocking the VsTest Host Launcher
        private class MoqHost : IStrykerTestHostLauncher
        {
            private WaitHandle handle;
            private readonly IDictionary<string, string> _dico;
            private readonly int _id;

            public MoqHost(WaitHandle handle, IDictionary<string, string> dico, int id)
            {
                this.handle = handle;
                _dico = dico;
                _id = id;
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
                if (handle != null)
                {
                    return handle.WaitOne();
                }

                return true;
            }
        }
    }

}
