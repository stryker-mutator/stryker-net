using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InputFileResolverTests
    {
        private readonly string _currentDirectory;
        private readonly string _filesystemRoot;
        private readonly string _basePath;
        private readonly string sourceFile;
        private readonly string testProjectPath;
        private readonly string projectUnderTestPath;
        private readonly string defaultTestProjectFileContents;

        public InputFileResolverTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _filesystemRoot = Path.GetPathRoot(_currentDirectory);
            _basePath = Path.Combine(_filesystemRoot, "TestProject");

            sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");
            testProjectPath = FilePathUtils.ConvertPathSeparators(Path.Combine(_filesystemRoot, "TestProject", "TestProject.csproj"));
            projectUnderTestPath = FilePathUtils.ConvertPathSeparators(Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"));
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
        <ProjectReference Include=""..\ExampleProject\ExampleProject.csproj"" />
    </ItemGroup>
</Project>";
        }

        [Fact]
        public void InputFileResolver_InitializeShouldFindFilesRecursively()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") },
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { projectUnderTestPath },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    References = new string[] { "" }
                });
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>(),
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = projectUnderTestPath
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath));

            result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
        }

        [Fact]
        public void InputFileResolver_InitializeShouldResolveImportedProject()
        {
            string sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

            string sharedItems = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                <PropertyGroup>
                <MSBuildAllProjects>$(MSBuildAllProjects);$(MSBuildThisFileFullPath)</MSBuildAllProjects>
                <HasSharedItems>true</HasSharedItems>
                <SharedGUID>0425a660-ca7d-43f6-93ab-f72c95d506e3</SharedGUID>
                </PropertyGroup>
                <ItemGroup>
                <Compile Include=""$(MSBuildThisFileDirectory)shared.cs"" />
                </ItemGroup>
                </Project>";
            string projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

    <ItemGroup>
    </ItemGroup>
               
     <Import Project=""../SharedProject/Example.projitems"" Label=""Shared"" />

</Project>";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "SharedProject", "Example.projitems"), new MockFileData(sharedItems)},
                    { Path.Combine(_filesystemRoot, "SharedProject", "Shared.cs"), new MockFileData(sourceFile)},
                    { projectUnderTestPath, new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") },
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { projectUnderTestPath },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    Properties = new Dictionary<string, string>(),
                    References = new string[] { "" }
                });
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>(),
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = projectUnderTestPath,
                    Properties = new Dictionary<string, string>()
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath));

            result.ProjectContents.GetAllFiles().Count().ShouldBe(3);
        }

        [Fact]
        public void InputFileResolver_InitializeShouldNotResolveImportedPropsFile()
        {
            string sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

            string projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

    <ItemGroup>
    </ItemGroup>
               
     <Import Project=""../NonSharedProject/Example.props"" Label=""Shared"" />

</Project>";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "NonSharedProject", "Example.props"), new MockFileData("")},
                    { Path.Combine(_filesystemRoot, "NonSharedProject", "NonSharedSource.cs"), new MockFileData(sourceFile)},
                    { projectUnderTestPath, new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") },
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { projectUnderTestPath },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    Properties = new Dictionary<string, string>(),
                    References = new string[] { "" }
                });
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>(),
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = projectUnderTestPath,
                    Properties = new Dictionary<string, string>()
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath));

            result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
        }

        [Fact]
        public void InputFileResolver_InitializeShouldResolveMultipleImportedProjects()
        {
            string sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

            string sharedItems = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                <PropertyGroup>
                <MSBuildAllProjects>$(MSBuildAllProjects);$(MSBuildThisFileFullPath)</MSBuildAllProjects>
                <HasSharedItems>true</HasSharedItems>
                <SharedGUID>0425a660-ca7d-43f6-93ab-f72c95d506e3</SharedGUID>
                </PropertyGroup>
                <ItemGroup>
                <Compile Include=""$(MSBuildThisFileDirectory)shared.cs"" />
                </ItemGroup>
                </Project>";
            string sharedItems2 = @"<?xml version=""1.0"" encoding=""utf-8""?>
                <Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
                <PropertyGroup>
                <MSBuildAllProjects>$(MSBuildAllProjects);$(MSBuildThisFileFullPath)</MSBuildAllProjects>
                <HasSharedItems>true</HasSharedItems>
                <SharedGUID>0425a660-ca7d-43f6-93ab-f72c95d506e3</SharedGUID>
                </PropertyGroup>
                <ItemGroup>
                <Compile Include=""$(MSBuildThisFileDirectory)shared.cs"" />
                </ItemGroup>
                </Project>";
            string projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

    <ItemGroup>
    </ItemGroup>
               
     <Import Project=""../SharedProject1/Example.projitems"" Label=""Shared"" />
     <Import Project=""../SharedProject2/Example.projitems"" Label=""Shared"" />

</Project>";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "SharedProject1", "Example.projitems"), new MockFileData(sharedItems)},
                    { Path.Combine(_filesystemRoot, "SharedProject1", "Shared.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "SharedProject2", "Example.projitems"), new MockFileData(sharedItems2)},
                    { Path.Combine(_filesystemRoot, "SharedProject2", "Shared.cs"), new MockFileData(sourceFile)},
                    { projectUnderTestPath, new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") },
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { projectUnderTestPath },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    Properties = new Dictionary<string, string>(),
                    References = new string[] { "" }
                });
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>(),
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = projectUnderTestPath,
                    Properties = new Dictionary<string, string>(),
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath));

            result.ProjectContents.GetAllFiles().Count().ShouldBe(4);
        }

        [Fact]
        public void InputFileResolver_InitializeShouldThrowOnMissingSharedProject()
        {
            string sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

            string projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

    <ItemGroup>
    </ItemGroup>
               
     <Import Project=""../SharedProject/Example.projitems"" Label=""Shared"" />

    <ItemGroup>
        <ProjectReference Include=""../ExampleProject/ExampleProject.csproj"" />
    </ItemGroup>
</Project>";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "SharedProject", "Shared.cs"), new MockFileData(sourceFile)},
                    { projectUnderTestPath, new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") },
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { projectUnderTestPath },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    Properties = new Dictionary<string, string>(),
                });
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>(),
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = projectUnderTestPath,
                    Properties = new Dictionary<string, string>(),
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            Assert.Throws<FileNotFoundException>(() => target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath)));

        }

        [Fact]
        public void InputFileResolver_InitializeShouldResolvePropertiesInSharedProjectImports()
        {
            string sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

            string sharedFile = "<Project />";

            string projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
        <SharedDir>SharedProject</SharedDir>
    </PropertyGroup>

    <ItemGroup>
    </ItemGroup>

    <Import Project=""../$(SharedDir)/Example.projitems"" Label=""Shared"" />

    <ItemGroup>
        <ProjectReference Include=""../ExampleProject/ExampleProject.csproj"" />
    </ItemGroup>
</Project>";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "SharedProject", "Example.projitems"), new MockFileData(sharedFile)},
                    { Path.Combine(_filesystemRoot, "SharedProject", "Shared.cs"), new MockFileData(sourceFile)},
                    { projectUnderTestPath, new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") },
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { projectUnderTestPath },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    References = new string[] { "" }
                });
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>(),
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = projectUnderTestPath,
                    Properties = new Dictionary<string, string>()
                    {
                        { "SharedDir", "SharedProject" },
                    },
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath));

            var allFiles = result.ProjectContents.GetAllFiles();

            allFiles.Count().ShouldBe(3);
            allFiles.ShouldContain(f => f.Name == "Shared.cs");
        }

        [Fact]
        public void InputFileResolver_InitializeShouldThrowIfImportPropertyCannotBeResolved()
        {
            string sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

            string sharedFile = "<Project />";

            string projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

    <ItemGroup>
    </ItemGroup>

    <Import Project=""../$(SharedDir)/Example.projitems"" Label=""Shared"" />

    <ItemGroup>
        <ProjectReference Include=""../ExampleProject/ExampleProject.csproj"" />
    </ItemGroup>
</Project>";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "SharedProject", "Example.projitems"), new MockFileData(sharedFile)},
                    { Path.Combine(_filesystemRoot, "SharedProject", "Shared.cs"), new MockFileData(sourceFile)},
                    { projectUnderTestPath, new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") },
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { projectUnderTestPath },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                });
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>(),
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = projectUnderTestPath,
                    Properties = new Dictionary<string, string>(),
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var exception = Assert.Throws<StrykerInputException>(() => target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath)));
            exception.Message.ShouldBe($"Missing MSBuild property (SharedDir) in project reference (../$(SharedDir)/Example.projitems). Please check your project file ({projectUnderTestPath}) and try again.");
        }

        [Theory]
        [InlineData("bin")]
        [InlineData("obj")]
        [InlineData("node_modules")]
        public void InputFileResolver_InitializeShouldIgnoreBinFolder(string folderName)
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "TestProject", "TestProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "TestProject", folderName, "somecsharpfile.cs"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", folderName, "subfolder", "somecsharpfile.cs"), new MockFileData("Bytecode") },
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { "" },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    References = new string[] { "" }
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath));

            result.ProjectContents.Children.Count.ShouldBe(1);
        }

        [Fact]
        public void InputFileResolver_InitializeShouldMarkFilesAsExcluded()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents) },
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile) },
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive2.cs"), new MockFileData(sourceFile) },
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive3.cs"), new MockFileData(sourceFile) },
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents) },
                    { Path.Combine(_filesystemRoot, "TestProject", "Debug", "somecsharpfile.cs"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "Release", "subfolder", "somecsharpfile.cs"), new MockFileData("Bytecode") }
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { projectUnderTestPath },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    References = new string[] { "" }
                });
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>(),
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = projectUnderTestPath
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath, filesToExclude: new[] { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs") }));

            result.ProjectContents.GetAllFiles().Count(c => c.IsExcluded).ShouldBe(1);
        }

        [Fact]
        public void InputFileResolver_ShouldThrowExceptionOnNoProjectFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { "" },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            Assert.Throws<StrykerInputException>(() => target.ScanProjectFile(Path.Combine(_filesystemRoot, "ExampleProject")));
        }

        [Fact]
        public void InputFileResolver_ShouldThrowStrykerInputExceptionOnTwoProjectFiles()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject2.csproj"), new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { "" },
                    TargetFramework = "netcore2.1",
                    ProjectFilePath = testProjectPath
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            Assert.Throws<StrykerInputException>(() => target.ScanProjectFile(Path.Combine(_filesystemRoot, "ExampleProject")));
        }

        [Fact]
        public void InputFileResolver_ShouldNotThrowExceptionOnTwoProjectFilesInDifferentLocations()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject\\ExampleProject2", "ExampleProject2.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
            });

            var target = new InputFileResolver(fileSystem, null);

            var actual = target.ScanProjectFile(Path.Combine(_filesystemRoot, "ExampleProject"));

            actual.ShouldBe(Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"));
        }

        [Fact]
        public void InitialisationProcess_ShouldThrowOnMsTestV1Detected()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                { testProjectPath, new MockFileData(defaultTestProjectFileContents)}
            });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { "" },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    References = new string[] { "Microsoft.VisualStudio.QualityTools.UnitTestFramework" }
                });

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            Assert.Throws<StrykerInputException>(() => target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath)));
        }

        [Fact]
        public void InitialisationProcess_ShouldKeepDotnetTestIfIsTestProjectSet()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                { testProjectPath, new MockFileData(defaultTestProjectFileContents)}
            });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { "" },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    Properties = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> { { "IsTestProject", "true" } }),
                    References = new string[] { "" }
                });

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);
            var options = new StrykerOptions(fileSystem: fileSystem, basePath: _basePath, testRunner: "dotnettest");

            target.ResolveInput(options);

            options.TestRunner.ShouldBe(TestRunner.DotnetTest);
        }


        [Fact]
        public void InitialisationProcess_ShouldForceVsTestIfIsTestProjectNotSetAndFullFramework()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                { testProjectPath, new MockFileData(defaultTestProjectFileContents)}
            });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { "" },
                    TargetFramework = "net4.5",
                    ProjectFilePath = testProjectPath,
                    Properties = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()),
                    References = new string[] { "" }
                });

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);
            var options = new StrykerOptions(fileSystem: fileSystem, basePath: _basePath);

            target.ResolveInput(options);

            options.TestRunner.ShouldBe(TestRunner.VsTest);
        }

        [Fact]
        public void InitialisationProcess_ShouldForceVsTestIfIsTestProjectSetFalseAndFullFramework()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                { testProjectPath, new MockFileData(defaultTestProjectFileContents)}
            });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { "" },
                    TargetFramework = "net4.5",
                    ProjectFilePath = testProjectPath,
                    Properties = new ReadOnlyDictionary<string, string>(new Dictionary<string, string> { { "IsTestProject", "false" } }),
                    References = new string[0]
                });

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);
            var options = new StrykerOptions(fileSystem: fileSystem, basePath: _basePath);

            target.ResolveInput(options);

            options.TestRunner.ShouldBe(TestRunner.VsTest);
        }

        [Fact]
        public void InputFileResolver_ShouldSkipXamlFiles()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "app.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "app.xaml"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "app.xaml.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") },
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>() { projectUnderTestPath },
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = testProjectPath,
                    References = new string[] { "" }
                });
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null))
                .Returns(new ProjectAnalyzerResult(null, null)
                {
                    ProjectReferences = new List<string>(),
                    TargetFramework = "netcoreapp2.1",
                    ProjectFilePath = projectUnderTestPath
                });
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(new StrykerOptions(fileSystem: fileSystem, basePath: _basePath));

            result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
        }
    }
}
