using Shouldly;
using Stryker.Core.Initialisation;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    using System.Linq;

    public class InputFileResolverTests
    {
        private string _currentDirectory { get; set; }
        private string _filesystemRoot { get; set; }
        private readonly string _sourceFile;

        public InputFileResolverTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _filesystemRoot = Path.GetPathRoot(_currentDirectory);
            _sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");
        }

        [Fact]
        public void InputFileResolver_InitializeShouldCrawlFiles()
        {
            string projectFile = @"<Project Sdk=""Microsoft.NET.Sdk"">
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
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(projectFile)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_filesystemRoot, "TestProject", "TestProject.csproj"), new MockFileData(projectFile)},
                { Path.Combine(_filesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") },
                { Path.Combine(_filesystemRoot, "TestProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") }
            });

            var target = new InputFileResolver(fileSystem);

            var result = target.ResolveInput(Path.Combine(_filesystemRoot, "TestProject"), "", new List<string>());

            result.TestProjectPath.ShouldBe(Path.Combine(_filesystemRoot, "TestProject"));
        }

        [Fact]
        public void InputFileResolver_InitializeShouldCrawlFilesRecursively()
        {
            string projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
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

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(_sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(_sourceFile)},
                    { Path.Combine(_filesystemRoot, "TestProject", "TestProject.csproj"), new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "TestProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") },
                });

            var target = new InputFileResolver(fileSystem);

            var result = target.ResolveInput(Path.Combine(_filesystemRoot, "TestProject"), "", new List<string>());

            result.TestProjectPath.ShouldBe(Path.Combine(_filesystemRoot, "TestProject"));
            result.ProjectContents.Children.Count.ShouldBe(2);
        }

        [Theory]
        [InlineData("bin")]
        [InlineData("obj")]
        [InlineData("node_modules")]
        public void InputFileResolver_InitializeShouldIgnoreBinFolder(string folderName)
        {
            string projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
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

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(_sourceFile)},
                    { Path.Combine(_filesystemRoot, "TestProject", "TestProject.csproj"), new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "TestProject", folderName, "somecsharpfile.cs"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", folderName, "subfolder", "somecsharpfile.cs"), new MockFileData("Bytecode") },
                });

            var target = new InputFileResolver(fileSystem);

            var result = target.ResolveInput(Path.Combine(_filesystemRoot, "TestProject"), "", new List<string>());

            result.TestProjectPath.ShouldBe(Path.Combine(_filesystemRoot, "TestProject"));
            result.ProjectContents.Children.Count.ShouldBe(1);
        }

        [Fact]
        public void InputFileResolver_InitializeShouldMarkFilesAsExcluded()
        {
            string projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
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

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(projectFile) },
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(_sourceFile) },
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive2.cs"), new MockFileData(_sourceFile) },
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive3.cs"), new MockFileData(_sourceFile) },
                    { Path.Combine(_filesystemRoot, "TestProject", "TestProject.csproj"), new MockFileData(projectFile) },
                    { Path.Combine(_filesystemRoot, "TestProject", "Debug", "somecsharpfile.cs"), new MockFileData("Bytecode") },
                    { Path.Combine(_filesystemRoot, "TestProject", "Release", "subfolder", "somecsharpfile.cs"), new MockFileData("Bytecode") }
                });

            var target = new InputFileResolver(fileSystem);

            var result = target.ResolveInput(Path.Combine(_filesystemRoot, "TestProject"), "", new List<string> { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs") });

            result.TestProjectPath.ShouldBe(Path.Combine(_filesystemRoot, "TestProject"));
            result.ProjectContents.GetAllFiles().Count(c => c.IsExcluded).ShouldBe(1);
        }

        [Fact]
        public void InputFileResolver_ShouldThrowExceptionOnNoProjectFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
                });

            var target = new InputFileResolver(fileSystem);

            Assert.Throws<FileNotFoundException>(() => target.ScanProjectFile(Path.Combine(_filesystemRoot, "ExampleProject")));
        }

        [Fact]
        public void InputFileResolver_ShouldThrowExceptionOnTwoProjectFiles()
        {
            string projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
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
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject2.csproj"), new MockFileData(projectFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
                });

            var target = new InputFileResolver(fileSystem);

            Assert.Throws<FileNotFoundException>(() => target.ScanProjectFile(Path.Combine(_filesystemRoot, "ExampleProject")));
        }
    }
}
