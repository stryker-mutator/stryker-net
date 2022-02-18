using Xunit;
using Moq;
using Stryker.Core.Initialisation;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Stryker.Core.Options;
using System.IO.Abstractions.TestingHelpers;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation.SolutionAnalyzer;

namespace Stryker.Core.UnitTest.Fsharp
{
    public class FsharpFileDetectionTests : TestBase
    {
        private readonly string _currentDirectory;
        private readonly string _filesystemRoot;
        private readonly string _sourceFile;
        private readonly string _testProjectPath;
        private readonly string _projectUnderTestPath;
        private readonly string _defaultTestProjectFileContents;
        private readonly Mock<ILogger<InputFileResolver>> loggerMock = new Mock<ILogger<InputFileResolver>>();

        public FsharpFileDetectionTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _filesystemRoot = Path.GetPathRoot(_currentDirectory);

            _sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/FsharpExampleSourceFile.fs");
            _testProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "TestProject", "TestProject.fsproj"));
            _projectUnderTestPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.fsproj"));
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
        <ProjectReference Include=""..\ExampleProject\ExampleProject.fsproj"" />
    </ItemGroup>
</Project>";
        }

        public void Stryker_FsharpShouldRetrieveSourcefiles()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { _projectUnderTestPath, new MockFileData(_defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.fs"), new MockFileData(_sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.fs"), new MockFileData(_sourceFile)},
                    { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") }, // bin should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") }, // obj should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "node_modules", "Some package"), new MockFileData("bla") }, // node_modules should be excluded
                });
            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(_testProjectPath, null, "fsharp"))
                .Returns(TestHelper.SetupProjectBuildalyzerResult(
                    projectReferences: new List<string>() { _projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: _testProjectPath,
                    references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(_projectUnderTestPath, null, "fsharp"))
                .Returns(TestHelper.SetupProjectBuildalyzerResult(
                    projectReferences: new List<string>() { _projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: _projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "F#" } }).Object);
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object, loggerMock.Object);

            var result = target.ResolveInput(new StrykerOptions());

            result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
        }
    }
}
