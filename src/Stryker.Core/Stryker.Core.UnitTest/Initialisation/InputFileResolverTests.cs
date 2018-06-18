using Shouldly;
using Stryker.Core.Initialisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InputFileResolverTests
    {
        private string _currentDirectory { get; set; }

        public InputFileResolverTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        [SkippableFact(typeof(TypeInitializationException))]
        public void InputFileResolver_InitializeShouldCrawlFiles()
        {
            string sourceFile = File.ReadAllText(_currentDirectory + "/Initialisation/TestResources/ExampleSourceFile.cs");
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
                { @"c:\ExampleProject\ExampleProject.csproj", new MockFileData(projectFile)},
                { @"c:\ExampleProject\Recursive.cs", new MockFileData(sourceFile)},
                { @"c:\TestProject\TestProject.csproj", new MockFileData(projectFile)},
                { @"c:\TestProject\bin\Debug\netcoreapp2.0\ExampleProject.dll", new MockFileData("Bytecode") },
                { @"c:\TestProject\obj\Release\netcoreapp2.0\ExampleProject.dll", new MockFileData("Bytecode") }
            });

            var target = new InputFileResolver(fileSystem);

            var result = target.ResolveInput(@"c:\TestProject\", "");

            result.TestProjectPath.ShouldBe(@"c:\TestProject\");
        }

        [SkippableFact(typeof(TypeInitializationException))]
        public void InputFileResolver_InitializeShouldCrawlFilesRecursively()
        {
            string sourceFile = File.ReadAllText(_currentDirectory + "/Initialisation/TestResources/ExampleSourceFile.cs");
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
                    { @"c:\ExampleProject\ExampleProject.csproj", new MockFileData(projectFile)},
                    { @"c:\ExampleProject\Recursive.cs", new MockFileData(sourceFile)},
                    { @"c:\ExampleProject\OneFolderDeeper\Recursive.cs", new MockFileData(sourceFile)},
                    { @"c:\TestProject\TestProject.csproj", new MockFileData(projectFile)},
                    { @"c:\TestProject\bin\Debug\netcoreapp2.0\ExampleProject.dll", new MockFileData("Bytecode") },
                    { @"c:\TestProject\obj\Release\netcoreapp2.0\ExampleProject.dll", new MockFileData("Bytecode") },
                });

            var target = new InputFileResolver(fileSystem);

            var result = target.ResolveInput(@"c:\TestProject\", "");

            result.TestProjectPath.ShouldBe(@"c:\TestProject\");
            result.ProjectContents.Children.Count.ShouldBe(2);
        }

        [SkippableTheory(typeof(TypeInitializationException))]
        [InlineData("bin")]
        [InlineData("obj")]
        [InlineData("node_modules")]
        public void InputFileResolver_InitializeShouldIgnoreBinFolder(string folderName)
        {
            // the bin, obj and node_modules folders should be skipped
            string sourceFile = File.ReadAllText(_currentDirectory + "/Initialisation/TestResources/ExampleSourceFile.cs");
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
                    { @"c:\ExampleProject\ExampleProject.csproj", new MockFileData(projectFile)},
                    { @"c:\ExampleProject\Recursive.cs", new MockFileData(sourceFile)},
                    { @"c:\TestProject\TestProject.csproj", new MockFileData(projectFile)},
                    { $@"c:\TestProject\{folderName}\somecsharpfile.cs", new MockFileData("Bytecode") },
                    { $@"c:\TestProject\{folderName}\subfolder\somecsharpfile.cs", new MockFileData("Bytecode") },

                });

            var target = new InputFileResolver(fileSystem);

            var result = target.ResolveInput(@"c:\TestProject\", "");

            result.TestProjectPath.ShouldBe(@"c:\TestProject\");
            result.ProjectContents.Children.Count.ShouldBe(1);
        }

        [SkippableFact(typeof(TypeInitializationException))]
        public void InputFileResolver_ShouldThrowExceptionOnNoProjectFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { @"c:\ExampleProject\Recursive.cs", new MockFileData("content")}
                });

            var target = new InputFileResolver(fileSystem);

            var exception = Assert.Throws<FileNotFoundException>(() => target.ScanProjectFile(@"c:\ExampleProject"));
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
                    { @"c:\ExampleProject\ExampleProject.csproj", new MockFileData(projectFile)},
                    { @"c:\ExampleProject\ExampleProject2.csproj", new MockFileData(projectFile)},
                    { @"c:\ExampleProject\Recursive.cs", new MockFileData("content")}
                });

            var target = new InputFileResolver(fileSystem);

            var exception = Assert.Throws<FileNotFoundException>(() => target.ScanProjectFile(@"c:\ExampleProject"));
        }
    }
}
