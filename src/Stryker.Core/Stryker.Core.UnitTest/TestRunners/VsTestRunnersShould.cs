using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TestPlatform.VsTestConsole.TranslationLayer.Interfaces;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
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
        private TestCase test = new TestCase("myFirsTest", new Uri("exec://moqexecutor"), "test.cpp");

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
        }

        [Fact]
        public void InitializeAndDiscoverTests()
        {
            var mockVsTest = new Mock<IVsTestConsoleWrapper>(MockBehavior.Strict);
            var testAssemblyPath = Path.Combine(_filesystemRoot, "test", "bin", "Debug", "TestApp.dll");
            var targetProject = new ProjectInfo()
            {
                TestProjectAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = testAssemblyPath,
                    TargetFramework = "toto"
                },
                ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                {
                    AssemblyPath = Path.Combine(_filesystemRoot, "app", "bin", "Debug", "AppToTest.dll"),
                    TargetFramework = "toto"
                },
            };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _projectUnderTestPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(_sourceFile)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { testAssemblyPath, new MockFileData("Bytecode") },
                { Path.Combine(_filesystemRoot, "app", "bin", "Debug", "AppToTest.dll"), new MockFileData("Bytecode") },
            });

            var options = new StrykerOptions();
            var testCases = new []{test};

            mockVsTest.Setup(x => x.StartSession());
            mockVsTest.Setup(x => x.InitializeExtensions(It.IsAny<List<string>>()));
            mockVsTest.Setup(x =>
                x.DiscoverTests(It.Is<IEnumerable<string>>(d => d.Any( e=> e== testAssemblyPath)), 
                    It.IsAny<string>(), 
                    It.IsAny<ITestDiscoveryEventsHandler>())).Callback(
                (IEnumerable<string> sources, string discoverySettings, ITestDiscoveryEventsHandler discoveryEventsHandler)=>
                    DiscoverTests(sources, discoverySettings, discoveryEventsHandler, testCases, false));
            var runner = new VsTestRunner(
                options,
                OptimizationFlags.NoOptimization, 
                targetProject, 
                null,
                null, 
                fileSystem,
                wrapper: mockVsTest.Object);

            runner.DiscoverNumberOfTests().ShouldBe(1);
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
