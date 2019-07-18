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
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    public class VsTestRunnersShould
    {
        private readonly string _testAssemblyPath;
        private readonly ProjectInfo _targetProject;
        private readonly MockFileSystem _fileSystem;
        private readonly Mutant _mutant;
        private readonly TestCase[] _testCases;

        // initialize the test context
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

        [Fact]
        public void InitializeAndDiscoverTests()
        {
            using (var endProcess = new EventWaitHandle(true, EventResetMode.ManualReset))
            {
                var options = new StrykerOptions();
                var mockVsTest = BuildVsTestMock(options);
                var runner = new VsTestRunner(
                    options,
                    OptimizationFlags.NoOptimization, 
                    _targetProject, 
                    null,
                    null, 
                    _fileSystem,
                    wrapper: mockVsTest.Object, 
                    hostBuilder: ((dictionary, i) => new MoqHost(endProcess, dictionary, i)));
                runner.DiscoverNumberOfTests().ShouldBe(2);
                
            }
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
        public void RunTests()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestMock(options);
                var runner = new VsTestRunner(
                    options,
                    OptimizationFlags.NoOptimization,
                    _targetProject,
                    null,
                    null,
                    _fileSystem,
                    wrapper: mockVsTest.Object,
                    hostBuilder: ((dictionary, i) => new MoqHost(endProcess, dictionary, i)));

                mockVsTest.Setup(x =>
                    x.RunTestsWithCustomTestHost(
                        It.Is<IEnumerable<string>>(t => t.Any(source => source == _testAssemblyPath)),
                        It.IsAny<string>(),
                        It.IsAny<ITestRunEventsHandler>(),
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        MoqTestRun(testRunEvents, _testCases, true);
                        endProcess.Set();
                    });
                var result = runner.RunAll(null, _mutant);

                result.Success.ShouldBe(true);
            }
        }

        [Fact]
        public void DetectTestsErrors()
        {
            var options = new StrykerOptions();
            using (var endProcess = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                var mockVsTest = BuildVsTestMock(options);
                var runner = new VsTestRunner(
                    options,
                    OptimizationFlags.NoOptimization, 
                    _targetProject, 
                    null,
                    null, 
                    _fileSystem,
                    wrapper: mockVsTest.Object,
                hostBuilder: ((dictionary, i) => new MoqHost(endProcess, dictionary, i)));

                mockVsTest.Setup(x => 
                    x.RunTestsWithCustomTestHost(It.Is<IEnumerable<string>>(t => t.Any( source => source == _testAssemblyPath)), 
                        It.IsAny<string>(), 
                        It.IsAny<ITestRunEventsHandler>(), 
                        It.IsAny<ITestHostLauncher>())).Callback(
                    (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                        ITestHostLauncher host) =>
                    {
                        MoqTestRun(testRunEvents, _testCases, false);
                        endProcess.Set();
                    });
                var result = runner.RunAll(null, _mutant);
                result.Success.ShouldBe(false);
                
            }
        }

        [Fact]
        public void DetectTimeout()
        {
            var options = new StrykerOptions();
            using (var endProcess = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                var mockVsTest = BuildVsTestMock(options);
                var runner = new VsTestRunner(
                    options,
                    OptimizationFlags.NoOptimization, 
                    _targetProject, 
                    null,
                    null, 
                    _fileSystem,
                    wrapper: mockVsTest.Object);

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
                
                Should.Throw<OperationCanceledException>(() => runner.RunAll(null, _mutant));
            }
        }

        [Fact]
        public void AbortOnError()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestMock(options);
                var runner = new VsTestRunner(
                    options,
                    OptimizationFlags.AbortTestOnKill,
                    _targetProject,
                    null,
                    null,
                    _fileSystem,
                    wrapper: mockVsTest.Object,
                    hostBuilder: ((dictionary, i) => new MoqHost(endProcess, dictionary, i)));

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
                        MoqTestRun(testRunEvents, _testCases, false);
                        endProcess.Set();
                    });

                var result = runner.RunAll(null, _mutant);
                // verify Abort has been called
                Mock.Verify(mockVsTest);
                result.Success.ShouldBe(false);
            }
        }

        [Fact]
        public void CaptureCoverageGWhenSkippingUncovered()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestMock(options);
                var runner = new VsTestRunner(
                    options,
                    OptimizationFlags.SkipUncoveredMutants,
                    _targetProject,
                    null,
                    null,
                    _fileSystem,
                    wrapper: mockVsTest.Object,
                    hostBuilder: ((dictionary, i) => new MoqHost(endProcess, dictionary, i)));

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
                        results[0].SetPropertyValue(coverageProperty, "1;");
                        MoqTestRun(testRunEvents, results);
                        endProcess.Set();
                    });

                var result = runner.CaptureCoverage();
                runner.CoveredMutants.ShouldHaveSingleItem();


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
                var mockVsTest = BuildVsTestMock(options);
                var runner = new VsTestRunner(
                    options,
                    OptimizationFlags.CoverageBasedTest,
                    _targetProject,
                    null,
                    null,
                    _fileSystem,
                    wrapper: mockVsTest.Object,
                    hostBuilder: ((dictionary, i) => new MoqHost(endProcess, dictionary, i)));

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
                        results[0].SetPropertyValue(coverageProperty, "1;");
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
                mutant.CoveringTest = runner.CoverageMutants.GetTests<TestCase>(mutant).Select( x => x.FullyQualifiedName).ToList();
                var result = runner.RunAll(0, mutant);

                // verify Abort has been called
                result.Success.ShouldBe(true);
            }
        }

        [Fact]
        public void IdentifyNonCoveredMutants()
        {
            var options = new StrykerOptions();

            using (var endProcess = new EventWaitHandle(false, EventResetMode.ManualReset))
            {
                var mockVsTest = BuildVsTestMock(options);
                var runner = new VsTestRunner(
                    options,
                    OptimizationFlags.SkipUncoveredMutants,
                    _targetProject,
                    null,
                    null,
                    _fileSystem,
                    wrapper: mockVsTest.Object,
                    hostBuilder: ((dictionary, i) => new MoqHost(endProcess, dictionary, i)));

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
                            results[i].SetPropertyValue(coverageProperty, "1;");
                        }
                        MoqTestRun(testRunEvents, results);
                        endProcess.Set();
                    });

                var result = runner.CaptureCoverage();
                runner.CoveredMutants.ShouldHaveSingleItem();
                runner.CoverageMutants.GetTests<TestCase>(new Mutant(){Id = 1}).ShouldBe(_testCases);
                // verify Abort has been called
                result.Success.ShouldBe(true);
            }
        }

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
            ;
        }
    }

    internal class MoqHost : IStrykerTestHostLauncher
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
