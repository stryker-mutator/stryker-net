using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Moq;
using Serilog.Events;
using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.TestRunners.VsTest;
using Stryker.Core.ToolHelpers;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    public class VsTextContextInformationTests: TestBase
    {
        private readonly string _testAssemblyPath;
        private readonly TestProjectsInfo _testProjectsInfo;
        private readonly MockFileSystem _fileSystem;
        private readonly Uri _msTestExecutorUri;
        private readonly Uri _executorUri;
        private ConsoleParameters _consoleParameters;

        public VsTextContextInformationTests()
        {
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filesystemRoot = Path.GetPathRoot(currentDirectory);

            var sourceFile = File.ReadAllText(currentDirectory + "/TestResources/ExampleSourceFile.cs");
            var testProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(filesystemRoot!, "TestProject", "TestProject.csproj"));
            var projectUnderTestPath = FilePathUtils.NormalizePathSeparators(Path.Combine(filesystemRoot, "ExampleProject", "ExampleProject.csproj"));
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
            _testAssemblyPath = FilePathUtils.NormalizePathSeparators(Path.Combine(filesystemRoot, "_firstTest", "bin", "Debug", "TestApp.dll"));
            _executorUri = new Uri("exec://nunit");
            _msTestExecutorUri = new Uri("exec://mstestV2");
            var firstTest = BuildCase("T0");
            var secondTest = BuildCaseMsTest("T1");

            var content = new CsharpFolderComposite();
            _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { projectUnderTestPath, new MockFileData(DefaultTestProjectFileContents)},
                { Path.Combine(filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                { Path.Combine(filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                { testProjectPath, new MockFileData(DefaultTestProjectFileContents)},
                { _testAssemblyPath!, new MockFileData("Bytecode") },
                { Path.Combine(filesystemRoot, "app", "bin", "Debug", "AppToTest.dll"), new MockFileData("Bytecode") },
            });
            content.Add(new CsharpFileLeaf());
            _testProjectsInfo = new TestProjectsInfo(_fileSystem)
            {
                TestProjects = new List<TestProject> {  new TestProject (_fileSystem,
                    TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", Path.GetDirectoryName(_testAssemblyPath) },
                        { "TargetFileName", Path.GetFileName(_testAssemblyPath) }
                    },
                    targetFramework: "netcoreapp2.1").Object)
                }

            };

            TestCases = new List<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> { firstTest, secondTest };
        }

        private static void DiscoverTests(ITestDiscoveryEventsHandler discoveryEventsHandler, ICollection<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> tests, bool aborted)
        {
            if (tests is null)
            {
                throw new ArgumentNullException(nameof(tests));
            }

            Task.Run(() => discoveryEventsHandler.HandleDiscoveredTests(tests)).
                ContinueWith((_, u) => discoveryEventsHandler.HandleDiscoveryComplete((int)u, null, aborted), tests.Count);
        }

        public List<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase> TestCases { get; set; }

        private Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase BuildCase(string name) => new(name, _executorUri, _testAssemblyPath) { Id = new Guid() };

        private Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase BuildCaseMsTest(string name) => new(name, _msTestExecutorUri, _testAssemblyPath) { Id = new Guid() };

        private VsTestContextInformation BuildVsTextContext(StrykerOptions options, out Mock<IVsTestConsoleWrapper> mockedVsTestConsole)
        {
            mockedVsTestConsole = new Mock<IVsTestConsoleWrapper>(MockBehavior.Strict);
            mockedVsTestConsole.Setup(x => x.StartSession());
            mockedVsTestConsole.Setup(x => x.InitializeExtensions(It.IsAny<IEnumerable<string>>()));
            mockedVsTestConsole.Setup(x => x.EndSession());
            mockedVsTestConsole.Setup(x =>
                x.DiscoverTests(It.Is<IEnumerable<string>>(d => d.Any(e => e == _testAssemblyPath)),
                    It.IsAny<string>(),
                    It.IsAny<ITestDiscoveryEventsHandler>())).Callback(
                (IEnumerable<string> _, string _, ITestDiscoveryEventsHandler discoveryEventsHandler) =>
                    DiscoverTests(discoveryEventsHandler, TestCases, false));

            var vsTestConsoleWrapper = mockedVsTestConsole.Object;
            return new VsTestContextInformation(
                options,
                new Mock<IVsTestHelper>().Object,
                _fileSystem,
                parameters =>
                {
                    _consoleParameters = parameters;
                    return vsTestConsoleWrapper;
                },
                null,
                NullLogger.Instance
                );
        }

        [Fact]
        public void InitializeAndDiscoverTests()
        {
            using var runner = BuildVsTextContext(new StrykerOptions(), out _);
            // make sure we have discovered first and second tests
            foreach (var testAssembly in _testProjectsInfo.GetTestAssemblies())
            {
                runner.AddTestSource(testAssembly);
            }
            runner.VsTests.Count.ShouldBe(2);
        }

        [Fact]
        public void CleanupProperly()
        {
            using var runner = BuildVsTextContext(new StrykerOptions(), out var mock);
            // make sure we have discovered first and second tests
            foreach (var testAssembly in _testProjectsInfo.GetTestAssemblies())
            {
                runner.AddTestSource(testAssembly);
            }
            runner.Dispose();
            mock.Verify(m => m.EndSession(), Times.Once);
        }

        [Fact]
        public void InitializeAndSetParameters()
        {
            using var runner = BuildVsTextContext(new StrykerOptions(), out _);
            runner.AddTestSource(_testAssemblyPath);
            _consoleParameters.TraceLevel.ShouldBe(TraceLevel.Off);
            _consoleParameters.LogFilePath.ShouldBeNull();
        }

        [Fact]
        public void InitializeAndSetParametersAccordingToOptions()
        {
            using var runner = BuildVsTextContext(new StrykerOptions { LogOptions = new LogOptions { LogToFile = true } }, out _);
            runner.AddTestSource(_testAssemblyPath);
            // logging should be at defined level
            _consoleParameters.TraceLevel.ShouldBe(TraceLevel.Info);

            // we should have the testdiscoverer log file name
            _consoleParameters.LogFilePath.ShouldBe($"\"logs{_fileSystem.Path.DirectorySeparatorChar}TestDiscoverer-log.txt\"");

            // the log folders should exist
            _fileSystem.AllDirectories.Last().ShouldMatch(".*logs$");
        }

        [Theory]
        [InlineData(LogEventLevel.Debug, TraceLevel.Verbose)]
        [InlineData(LogEventLevel.Verbose, TraceLevel.Verbose)]
        [InlineData(LogEventLevel.Information, TraceLevel.Info)]
        [InlineData(LogEventLevel.Warning, TraceLevel.Warning)]
        [InlineData(LogEventLevel.Error, TraceLevel.Error)]
        [InlineData(LogEventLevel.Fatal, TraceLevel.Error)]
        [InlineData((LogEventLevel)(-1), TraceLevel.Off)]
        public void InitializeAndSetProperLogLevel(LogEventLevel setLevel, TraceLevel expectedLevel)
        {
            using var runner = BuildVsTextContext(new StrykerOptions { LogOptions = new LogOptions { LogLevel = setLevel } }, out _);
            // logging should be a defined level
            runner.AddTestSource(_testAssemblyPath);
            _consoleParameters.TraceLevel.ShouldBe(expectedLevel);
        }
    }
}
