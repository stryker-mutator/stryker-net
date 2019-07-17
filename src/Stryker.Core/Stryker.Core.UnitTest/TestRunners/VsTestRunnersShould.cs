using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Text;
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
        private string _currentDirectory;
        private string _filesystemRoot;
        private string _sourceFile;
        private string _testProjectPath;
        private string _projectUnderTestPath;
        private string _defaultTestProjectFileContents;
        private TestCase _firstTest;
        private TestCase _secondTest;
        private string _testAssemblyPath;
        private ProjectInfo _targetProject;
        private MockFileSystem _fileSystem;
        private Mutant _mutant;
        private TestCase[] _testCases;

        // initialize the test context
        public VsTestRunnersShould()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _filesystemRoot = Path.GetPathRoot(_currentDirectory);

            _sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");
            _testProjectPath = FilePathUtils.ConvertPathSeparators(Path.Combine(_filesystemRoot, "TestProject", "TestProject.csproj"));
            _projectUnderTestPath = FilePathUtils.ConvertPathSeparators(Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"));
            _defaultTestProjectFileContents = @"<Project Sdk=""Microsoft.NET.Sdk"">
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
            _testAssemblyPath = Path.Combine(_filesystemRoot, "_firstTest", "bin", "Debug", "TestApp.dll");
            _firstTest = new TestCase("myFirsTest", new Uri("exec://nunit"), _testAssemblyPath);
            _secondTest = new TestCase("myOtherTest", new Uri("exec://nunit"), _testAssemblyPath);
            _targetProject = new ProjectInfo()
            {
                TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = _testAssemblyPath,
                    TargetFramework = "toto"
                },
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = Path.Combine(_filesystemRoot, "app", "bin", "Debug", "AppToTest.dll"),
                    TargetFramework = "toto"
                },
            };
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _projectUnderTestPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(_sourceFile)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { _testAssemblyPath, new MockFileData("Bytecode") },
                { Path.Combine(_filesystemRoot, "app", "bin", "Debug", "AppToTest.dll"), new MockFileData("Bytecode") },
            });

            _mutant = new Mutant {Id = 1};
            _testCases = new []{_firstTest, _secondTest};
        }

        [Fact]
        public void InitializeAndDiscoverTests()
        {
            var mockVsTest = new Mock<IVsTestConsoleWrapper>(MockBehavior.Strict);

            var options = new StrykerOptions();

            mockVsTest.Setup(x => x.StartSession());
            mockVsTest.Setup(x => x.InitializeExtensions(It.IsAny<List<string>>()));
            mockVsTest.Setup(x =>
                x.DiscoverTests(It.Is<IEnumerable<string>>(d => d.Any( e=> e== _testAssemblyPath)), 
                    It.IsAny<string>(), 
                    It.IsAny<ITestDiscoveryEventsHandler>())).Callback(
                (IEnumerable<string> sources, string discoverySettings, ITestDiscoveryEventsHandler discoveryEventsHandler)=>
                    DiscoverTests(sources, discoverySettings, discoveryEventsHandler, _testCases, false));
            var runner = new VsTestRunner(
                options,
                OptimizationFlags.NoOptimization, 
                _targetProject, 
                null,
                null, 
                _fileSystem,
                wrapper: mockVsTest.Object);

            runner.DiscoverNumberOfTests().ShouldBe(2);
        }

        [Fact]
        public void RunTests()
        {
            var mockVsTest = new Mock<IVsTestConsoleWrapper>(MockBehavior.Strict);

            var options = new StrykerOptions();

            mockVsTest.Setup(x => x.StartSession());
            mockVsTest.Setup(x => x.InitializeExtensions(It.IsAny<List<string>>()));
            mockVsTest.Setup(x =>
                x.DiscoverTests(It.Is<IEnumerable<string>>(d => d.Any( e=> e== _testAssemblyPath)), 
                    It.IsAny<string>(), 
                    It.IsAny<ITestDiscoveryEventsHandler>())).Callback(
                (IEnumerable<string> sources, string discoverySettings, ITestDiscoveryEventsHandler discoveryEventsHandler)=>
                    DiscoverTests(sources, discoverySettings, discoveryEventsHandler, _testCases, false));
            var runner = new VsTestRunner(
                options,
                OptimizationFlags.NoOptimization, 
                _targetProject, 
                null,
                null, 
                _fileSystem,
                wrapper: mockVsTest.Object);

            mockVsTest.Setup(x => 
                x.RunTestsWithCustomTestHost(It.Is<IEnumerable<string>>(t => t.Any( source => source == _testAssemblyPath)), 
                    It.IsAny<string>(), 
                    It.IsAny<ITestRunEventsHandler>(), 
                    It.IsAny<ITestHostLauncher>())).Callback(
                (IEnumerable<string> sources, string settings, ITestRunEventsHandler testRunEvents,
                    ITestHostLauncher host) =>
                {
                    MoqTestRun(testRunEvents, _testCases, true);
                });
            runner.RunAll(null, _mutant);
        }

        private void MoqTestRun(ITestRunEventsHandler testRunEvents, TestCase[] testCases, bool pass)
        {
            Task.Run(() =>
            {
                var timer = new Stopwatch();
                testRunEvents.HandleTestRunStatsChange(
                    new TestRunChangedEventArgs(new TestRunStatistics(0, null), null, testCases));

                Thread.Sleep(10);
                for(var i = 0; i<testCases.Length; i++)
                {
                    var testResult = new TestResult(testCases[i]);
                    testResult.EndTime = DateTimeOffset.Now;
                    testResult.Outcome = pass ? TestOutcome.Passed : TestOutcome.Failed;
                    testRunEvents.HandleTestRunStatsChange(new TestRunChangedEventArgs(
                        new TestRunStatistics(i+1, null), new []{testResult}, null));
                }
                Thread.Sleep(10);
                testRunEvents.HandleTestRunComplete(
                    new TestRunCompleteEventArgs(new TestRunStatistics(testCases.Length, null), false, false, null,
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
}
