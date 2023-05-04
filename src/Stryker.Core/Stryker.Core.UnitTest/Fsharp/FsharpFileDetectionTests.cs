using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;

namespace Stryker.Core.UnitTest.Fsharp
{
    public class FsharpFileDetectionTests : TestBase
    {
        private readonly string _currentDirectory;
        private readonly string _filesystemRoot;
        private readonly string _sourceFile;
        private readonly string _testProjectPath;
        private readonly string _sourceProjectPath;
        private readonly string _defaultTestProjectFileContents;
        private readonly Mock<ILogger<InputFileResolver>> loggerMock = new Mock<ILogger<InputFileResolver>>();

        public FsharpFileDetectionTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _filesystemRoot = Path.GetPathRoot(_currentDirectory);

            _sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/FsharpExampleSourceFile.fs");
            _testProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "TestProject", "TestProject.fsproj"));
            _sourceProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.fsproj"));
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
                    { _sourceProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.fs"), new MockFileData(_sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.fs"), new MockFileData(_sourceFile)},
                    { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") }, // bin should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") }, // obj should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "node_modules", "Some package"), new MockFileData("bla") }, // node_modules should be excluded
                });
            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(_testProjectPath, null, "fsharp", null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>() { _sourceProjectPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: _testProjectPath,
                    references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(_sourceProjectPath, null, "fsharp", null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>() { _sourceProjectPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: _sourceProjectPath,
                    properties: new Dictionary<string, string>() { { "Language", "F#" } }).Object);
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object, loggerMock.Object);

            var result = target.ResolveSourceProjectInfos(new StrykerOptions()).First();

            result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
        }
    }
}
