using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InputFileResolverTests : TestBase
    {
        private readonly string _currentDirectory;
        private readonly string _filesystemRoot;
        private readonly string _basePath;
        private readonly string sourceFile;
        private readonly string testProjectPath;
        private readonly string projectUnderTestPath;
        private readonly string defaultTestProjectFileContents;
        private readonly string defaultProjectUnderTestFileContents;
        private readonly StrykerOptions _options;

        public InputFileResolverTests()
        {
            _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _filesystemRoot = Path.GetPathRoot(_currentDirectory);
            _basePath = Path.Combine(_filesystemRoot, "TestProject");
            _options = new StrykerOptions()
            {
                BasePath = _basePath
            };
            sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");
            testProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "TestProject", "TestProject.csproj"));
            projectUnderTestPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"));
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
            defaultProjectUnderTestFileContents = @"<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

</Project>";
        }

        [Theory]
        [InlineData("netcoreapp2.1", Framework.DotNet, 2, 1, null)]
        [InlineData("netstandard1.6", Framework.DotNetStandard, 1, 6, null)]
        [InlineData("mono4.6", Framework.Unknown, 4, 6, null)]
        [InlineData("net4.5", Framework.DotNetClassic, 4, 5, null)]
        [InlineData("net4.5.1", Framework.DotNetClassic, 4, 5, 1)]
        [InlineData("net45", Framework.DotNetClassic, 4, 5, null)]
        [InlineData("net452", Framework.DotNetClassic, 4, 5, 2)]
        [InlineData("net5.0", Framework.DotNet, 5, 0, null)]
        [InlineData("net5.0-windows", Framework.DotNet, 5, 0, null)]
        [InlineData("net5", Framework.DotNet, 5, 0, null)]
        public void ProjectAnalyzerShouldDecodeFramework(string version, Framework framework, int major, int minor, int? patch)
        {
            var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
                projectReferences: new List<string> { projectUnderTestPath },
                targetFramework: version).Object;

            if (patch.HasValue)
            {
                analyzerResult.GetTargetFrameworkAndVersion().ShouldBe((framework, new Version(major, minor, patch.Value)));
            }
            else
            {
                analyzerResult.GetTargetFrameworkAndVersion().ShouldBe((framework, new Version(major, minor)));
            }
        }

        [Fact]
        public void InitializeShouldFindFilesRecursively()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") }, // bin should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") }, // obj should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "node_modules", "Some package"), new MockFileData("bla") }, // node_modules should be excluded
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                projectReferences: new List<string> { projectUnderTestPath },
                targetFramework: "netcoreapp2.1",
                projectFilePath: testProjectPath,
                references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(_options);

            result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
        }

        [Fact]
        public void InitializeShouldUseBuildalyzerResult()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Plain.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "bin", "Debug", "netcoreapp2.1"), new MockFileData("Bytecode") }, // bin should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "obj", "Debug", "netcoreapp2.1", "ExampleProject.AssemblyInfo.cs"), new MockFileData("Bytecode") }, // obj should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "obj", "Release", "netcoreapp2.1"), new MockFileData("Bytecode") }, // obj should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "node_modules", "Some package"), new MockFileData("bla") }, // node_modules should be excluded
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                projectReferences: new List<string> { projectUnderTestPath },
                targetFramework: "netcoreapp2.1",
                projectFilePath: testProjectPath,
                references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                projectReferences: new List<string> { projectUnderTestPath },
                targetFramework: "netcoreapp2.1",
                projectFilePath: projectUnderTestPath,
                sourceFiles: fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray()).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object, nugetHelperMock.Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);
            var result = target.ResolveInput(_options);

            result.ProjectContents.GetAllFiles().Count().ShouldBe(4);
        }

        [Fact]
        public void InitializeShouldMutateAssemblyInfo()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Plain.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "bin", "Debug", "netcoreapp2.1"), new MockFileData("Bytecode") }, // bin should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "obj", "Debug", "netcoreapp2.1", "ExampleProject.AssemblyInfo.cs"), new MockFileData(@"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Reflection;
[assembly: Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryContentRootAttribute(""WebApi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"", """",""WebApi.csproj"", ""0"")]
                        [assembly: System.Reflection.AssemblyCompanyAttribute(""WebApi"")]
                        [assembly: System.Reflection.AssemblyConfigurationAttribute(""Debug"")]
                        [assembly: System.Reflection.AssemblyFileVersionAttribute(""1.0.0.0"")]
                        [assembly: System.Reflection.AssemblyInformationalVersionAttribute(""1.0.0"")]
                        [assembly: System.Reflection.AssemblyProductAttribute(""WebApi"")]
                        [assembly: System.Reflection.AssemblyTitleAttribute(""WebApi"")]
                        [assembly: System.Reflection.AssemblyVersionAttribute(""1.0.0.0"")]") },
                    { Path.Combine(_filesystemRoot, "ExampleProject", "obj", "Release", "netcoreapp2.1"), new MockFileData("Bytecode") }, // obj should be excluded
                    { Path.Combine(_filesystemRoot, "ExampleProject", "node_modules", "Some package"), new MockFileData("bla") }, // node_modules should be excluded
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                projectReferences: new List<string> { projectUnderTestPath },
                targetFramework: "netcoreapp2.1",
                projectFilePath: testProjectPath,
                references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    sourceFiles: fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray(),
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(_options);

            result.ProjectContents.GetAllFiles().Count().ShouldBe(3);
            var mutatedFile = ((ProjectComponent<SyntaxTree>)result.ProjectContents).CompilationSyntaxTrees.First(s => s != null && s.FilePath.Contains("AssemblyInfo.cs"));

            var node = ((CompilationUnitSyntax)mutatedFile.GetRoot()).AttributeLists
                .SelectMany(al => al.Attributes).FirstOrDefault(n => n.Name.Kind() == SyntaxKind.QualifiedName
                                                                     && ((QualifiedNameSyntax)n.Name).Right
                                                                     .Kind() == SyntaxKind.IdentifierName
                                                                     && (string)((IdentifierNameSyntax)((QualifiedNameSyntax)n.Name).Right)
                                                                     .Identifier.Value == "AssemblyTitleAttribute");

            node.ArgumentList.Arguments.ShouldContain(t => t.Expression.ToString().Contains("Mutated"));
        }

        [Fact]
        public void InitializeShouldFindSpecifiedTestProjectFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                projectReferences: new List<string> { projectUnderTestPath },
                targetFramework: "netcoreapp2.1",
                projectFilePath: testProjectPath,
                references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(_options);

            result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
        }

        [Fact]
        public void InitializeShouldResolveImportedProject()
        {
            var sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

            var sharedItems = @"<?xml version=""1.0"" encoding=""utf-8""?>
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
            var projectFile = @"
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
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: testProjectPath,
                    properties: new Dictionary<string, string>(),
                    references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(_options);

            result.ProjectContents.GetAllFiles().Count().ShouldBe(3);
        }

        [Fact]
        public void InitializeShouldNotResolveImportedPropsFile()
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
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: testProjectPath,
                    properties: new Dictionary<string, string>(),
                    references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(_options);

            result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
        }

        [Fact]
        public void InitializeShouldResolveMultipleImportedProjects()
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
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)}
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: testProjectPath,
                    properties: new Dictionary<string, string>(),
                    references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(_options);

            result.ProjectContents.GetAllFiles().Count().ShouldBe(4);
        }

        [Fact]
        public void InitializeShouldThrowOnMissingSharedProject()
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
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)}
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: testProjectPath,
                    properties: new Dictionary<string, string>(),
                    references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            Assert.Throws<FileNotFoundException>(() => target.ResolveInput(_options));

        }

        [Fact]
        public void InitializeShouldResolvePropertiesInSharedProjectImports()
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
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)}
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: testProjectPath,
                    properties: new Dictionary<string, string>(),
                    references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>()
                    {
                        { "SharedDir", "SharedProject" },
                        { "Language", "C#" }
                    }).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(_options);

            var allFiles = result.ProjectContents.GetAllFiles();

            allFiles.Count().ShouldBe(3);
        }

        [Fact]
        public void InitializeShouldThrowIfImportPropertyCannotBeResolved()
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
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)}
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: testProjectPath).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var exception = Assert.Throws<InputException>(() => target.ResolveInput(_options));
            exception.Message.ShouldBe($"Missing MSBuild property (SharedDir) in project reference (..{FilePathUtils.NormalizePathSeparators("/$(SharedDir)/Example.projitems")}). Please check your project file ({projectUnderTestPath}) and try again.");
        }

        [Theory]
        [InlineData("bin")]
        [InlineData("obj")]
        [InlineData("node_modules")]
        public void InitializeShouldIgnoreBinFolder(string folderName)
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
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>(), null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: testProjectPath,
                    references: new string[] { "" },
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(_options);

            ((CsharpFolderComposite)result.ProjectContents).Children.Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldThrowExceptionOnNoProjectFile()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content") }
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>(), null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>() { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            Assert.Throws<InputException>(() => target.FindTestProject(Path.Combine(_filesystemRoot, "ExampleProject")));
        }

        [Fact]
        public void ShouldThrowStrykerInputExceptionOnTwoProjectFiles_AndNoTestProjectFileSpecified()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject2.csproj"), new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>(), null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: testProjectPath).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var errorMessage =
$@"Expected exactly one .csproj file, found more than one:
{Path.GetFullPath("/ExampleProject/ExampleProject.csproj")}
{Path.GetFullPath("/ExampleProject/ExampleProject2.csproj")}

Please specify a test project name filter that results in one project.
";

            var exception = Assert.Throws<InputException>(() => target.FindTestProject(Path.Combine(_filesystemRoot, "ExampleProject")));
            exception.Message.ShouldBe(errorMessage);
        }

        [Fact]
        public void ShouldNotThrowExceptionOnTwoProjectFilesInDifferentLocations()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject\\ExampleProject2", "ExampleProject2.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
            });

            var target = new InputFileResolver(fileSystem, null);

            var actual = target.FindTestProject(Path.Combine(_filesystemRoot, "ExampleProject"));

            actual.ShouldBe(Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"));
        }

        [Fact]
        public void ShouldChooseGivenTestProjectFileIfPossible()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "TestProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
            });
            var target = new InputFileResolver(fileSystem, null);

            var actual = target.FindTestProject(Path.Combine(_filesystemRoot, "ExampleProject", "TestProject.csproj"));

            actual.ShouldBe(Path.Combine(_filesystemRoot, "ExampleProject", "TestProject.csproj"));
        }

        [Fact]
        public void ShouldThrowExceptionIfGivenTestFileDoesNotExist()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "ExampleProject", "AlternateProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
            });
            var target = new InputFileResolver(fileSystem, null);

            var exception = Assert.Throws<InputException>(() => target.FindTestProject(Path.Combine(_filesystemRoot, "ExampleProject", "GivenTestProject.csproj")));
            exception.Message.ShouldStartWith("No .csproj file found, please check your project directory at");
        }

        [Fact]
        public void ShouldChooseGivenTestProjectFileIfPossible_AtRelativeLocation()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "SubFolder", "TestProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
            });
            var target = new InputFileResolver(fileSystem, null);

            var actual = target.FindTestProject(Path.Combine(_filesystemRoot, "ExampleProject", "SubFolder", "TestProject.csproj"));

            actual.ShouldBe(Path.Combine(_filesystemRoot, "ExampleProject", "SubFolder", "TestProject.csproj"));
        }

        [Fact]
        public void ShouldChooseGivenTestProjectFileIfPossible_AtFullPath()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject","SubFolder", "TestProject.csproj"), new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
            });
            var target = new InputFileResolver(fileSystem, null);

            var actual = target.FindTestProject(Path.Combine(_filesystemRoot,
                FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "ExampleProject", "SubFolder"))));

            actual.ShouldBe(Path.Combine(_filesystemRoot, "ExampleProject", "SubFolder", "TestProject.csproj"));
        }

        [Fact]
        public void ShouldSelectCorrectProjectUnderTest_WhenTestProjectsAreGiven()
        {
            // Arrange
            var basePath = Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject");
            var testProjectPath = Path.Combine(_filesystemRoot, "ExampleProject", "TestProjectFolder", "TestProject.csproj");
            var projectUnderTestPath = Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject", "ExampleProject.csproj");
            var projectUnderTestNameFilter = "ExampleProject.csproj";

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                { testProjectPath, new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData("content")}
            });

            var options = new StrykerOptions()
            {
                BasePath = basePath,
                ProjectUnderTestName = projectUnderTestNameFilter,
                TestProjects = new List<string> { testProjectPath }
            };

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);

            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    projectFilePath: testProjectPath,
                    properties: new Dictionary<string, string>() { { "Language", "C#" } },
                    references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "C#" } },
                    references: new string[0]).Object);

            // Act
            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var actual = target.ResolveInput(options);

            // Assert
            actual.ProjectUnderTestAnalyzerResult.ProjectFilePath.ShouldBe(projectUnderTestPath);
        }

        [Fact]
        public void ShouldThrowOnMsTestV1Detected()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                { Path.Combine(_filesystemRoot, "ExampleProject", "Recursive.cs"), new MockFileData(sourceFile)},
                { testProjectPath, new MockFileData(defaultTestProjectFileContents)}
            });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);

            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), It.IsAny<string>(), null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: testProjectPath,
                    references: new string[] { "Microsoft.VisualStudio.QualityTools.UnitTestFramework" },
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var ex = Assert.Throws<InputException>(() => target.ResolveInput(_options));

            ex.Message.ShouldBe("Please upgrade to MsTest V2. Stryker.NET uses VSTest which does not support MsTest V1.");
            ex.Details.ShouldBe(@"See https://devblogs.microsoft.com/devops/upgrade-to-mstest-v2/ for upgrade instructions.");
        }


        [Fact]
        public void ShouldSkipXamlFiles()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectUnderTestPath, new MockFileData(defaultTestProjectFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "app.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "app.xaml"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "app.xaml.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "OneFolderDeeper", "Recursive.cs"), new MockFileData(sourceFile)},
                    { testProjectPath, new MockFileData(defaultTestProjectFileContents)}
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);

            projectFileReaderMock.Setup(x => x.AnalyzeProject(testProjectPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: testProjectPath,
                    properties: new Dictionary<string, string>(),
                    references: new string[] { "" }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>(),
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: projectUnderTestPath,
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);

            var result = target.ResolveInput(_options);

            result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
        }

        [Fact]
        public void ShouldFindAllTestProjects()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectUnderTestPath, new MockFileData(defaultProjectUnderTestFileContents)},
                    { Path.Combine(_filesystemRoot, "ExampleProject", "myFile.cs"), new MockFileData(sourceFile)},
                    { Path.Combine(_filesystemRoot, "TestProject1", "ExampleProject.csproj"), new MockFileData(defaultTestProjectFileContents) },
                    { Path.Combine(_filesystemRoot, "TestProject2", "ExampleProject.csproj"), new MockFileData(defaultTestProjectFileContents) },
                });

            var projectFileReaderMock = new Mock<IProjectFileReader>(MockBehavior.Strict);

            projectFileReaderMock.Setup(x => x.AnalyzeProject(It.IsAny<string>(), null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { projectUnderTestPath },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: Path.Combine(_filesystemRoot, "TestProject1", "ExampleProject.csproj"),
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }).Object);
            projectFileReaderMock.Setup(x => x.AnalyzeProject(projectUnderTestPath, null, null))
                .Returns(TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string>() { "" },
                    targetFramework: "netcoreapp2.1",
                    projectFilePath: Path.Combine(_filesystemRoot, "ExampleProject", "ExampleProject.csproj"),
                    references: new string[0],
                    properties: new Dictionary<string, string>() { { "Language", "C#" } }
                ).Object);

            var target = new InputFileResolver(fileSystem, projectFileReaderMock.Object);
            var options = new StrykerOptions()
            {
                BasePath = Path.Combine(_filesystemRoot, "ExampleProject"),
                TestProjects = new List<string>
                {
                    Path.Combine(_filesystemRoot, "TestProject1", "ExampleProject.csproj"),
                    Path.Combine(_filesystemRoot, "TestProject2", "ExampleProject.csproj")
                }
            };
            var result = target.ResolveInput(options);

            result.ProjectContents.GetAllFiles().Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldFindProjectUnderTestHappyFlow()
        {
            var target = new InputFileResolver();

            var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> { "../ExampleProject/ExampleProject.csproj" }).Object;

            var result = target.FindProjectUnderTest(new List<IAnalyzerResult> { analyzerResult }, null);
            result.ShouldBe("../ExampleProject/ExampleProject.csproj");
        }

        [Fact]
        public void ShouldThrowOnNoProjectReference()
        {
            var ex = Assert.Throws<InputException>(() =>
            {
                new InputFileResolver().FindProjectUnderTest(Enumerable.Empty<IAnalyzerResult>(), null);
            });

            ex.Message.ShouldBe("Project reference issue.");
            ex.Details.ShouldContain("no project", Case.Insensitive);
        }

        [Fact]
        public void ShouldThrowOnMultipleProjects()
        {
            var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> {
                        "../ExampleProject/ExampleProject.csproj",
                        "../AnotherProject/AnotherProject.csproj"
                    }).Object;

            var ex = Assert.Throws<InputException>(() =>
            {
                new InputFileResolver().FindProjectUnderTest(new List<IAnalyzerResult> { analyzerResult }, null);
            });

            ex.Message.ShouldBe("Project reference issue.");
            ex.Details.ShouldContain("Test project contains more than one project reference. Please set the project option");
            ex.Details.ShouldContain("Choose one of the following references:");
        }

        [Theory]
        [InlineData("ExampleProject.csproj")]
        [InlineData("exampleproject.csproj")]
        [InlineData("ExampleProject")]
        [InlineData("exampleproject")]
        [InlineData("Example")]
        public void ShouldMatchFromMultipleProjectByName(string shouldMatch)
        {
            var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> {
                        "../ExampleProject/ExampleProject.csproj",
                        "../AnotherProject/AnotherProject.csproj"
                    }).Object;

            var result = new InputFileResolver().FindProjectUnderTest(new List<IAnalyzerResult> { analyzerResult }, shouldMatch);

            result.ShouldBe(Path.Combine("..", "ExampleProject", "ExampleProject.csproj"));
        }

        [Theory]
        [InlineData("Project.csproj")]
        [InlineData("project.csproj")]
        [InlineData("Project")]
        [InlineData(".csproj")]
        public void ShouldThrowWhenTheNameMatchesMore(string shouldMatchMoreThanOne)
        {
            var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> {
                        "../ExampleProject/ExampleProject.csproj",
                        "../AnotherProject/AnotherProject.csproj"
                    }).Object;

            var ex = Assert.Throws<InputException>(() =>
            {
                new InputFileResolver().FindProjectUnderTest(new List<IAnalyzerResult> { analyzerResult }, shouldMatchMoreThanOne);
            });

            ex.Message.ShouldBe("Project reference issue.");
            ex.Details.ShouldContain("more than one", Case.Insensitive);
        }

        [Theory]
        [InlineData("SomeProject.csproj")]
        [InlineData("??")]
        [InlineData("WrongProject.csproj")]
        public void ShouldThrowWhenTheNameMatchesNone(string shouldMatchNone)
        {
            var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    projectReferences: new List<string> {
                        "../ExampleProject/ExampleProject.csproj",
                        "../AnotherProject/AnotherProject.csproj"
                    }).Object;

            var ex = Assert.Throws<InputException>(() =>
            {
                new InputFileResolver().FindProjectUnderTest(new List<IAnalyzerResult> { analyzerResult }, shouldMatchNone);
            });

            ex.Message.ShouldBe("Project reference issue.");
            ex.Details.ShouldContain("no project", Case.Insensitive);
        }
    }
}
