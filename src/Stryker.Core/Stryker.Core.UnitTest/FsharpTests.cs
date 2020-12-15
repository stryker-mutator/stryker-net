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

namespace Stryker.Core.UnitTest.Fsharp
{
    public class FsharpTests
    {
        private readonly string _currentDirectory;
        private readonly string _filesystemRoot;
        private readonly string _basePath;
        private readonly string sourceFile;
        private readonly string testProjectPath;
        private readonly string projectUnderTestPath;
        private readonly string defaultTestProjectFileContents;
        private readonly string defaultProjectUndertestFileContents;

        public FsharpTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _filesystemRoot = Path.GetPathRoot(_currentDirectory);
            _basePath = Path.Combine(_filesystemRoot, "TestProject");

            sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/FsharpExampleSourceFile.fs");
            testProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "TestProject", "TestProject.fsproj"));
            projectUnderTestPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.fsproj"));
            defaultTestProjectFileContents = @"<Project Sdk=""Microsoft.NET.Sdk"">
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
            defaultProjectUndertestFileContents = @"<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

</Project>";
        }

        [Fact]
        public void Stryker_FsharpShouldRetrieveSourcefiles()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.fs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.fs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") }, // bin should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") }, // obj should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "node_modules", "Some package"), new MockFileData("bla") }, // node_modules should be excluded
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences : new List<string>() { projectUnderTestPath },
                    targetFramework : "netcoreapp2.1",
                    projectFilePath : testProjectPath,
                    references : new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>() { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "F#" } }).Object);
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);
            var result = target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath));

            result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
        }
    }
}
