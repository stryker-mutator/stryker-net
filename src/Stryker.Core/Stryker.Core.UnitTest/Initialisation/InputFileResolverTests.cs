using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuGet.Frameworks;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.Buildalyzer;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using static NuGet.Frameworks.FrameworkConstants;

namespace Stryker.Core.UnitTest.Initialisation;

[TestClass]
public class InputFileResolverTests : BuildAnalyzerTestsBase
{
    private readonly string _currentDirectory;
    private readonly string _filesystemRoot;
    private readonly string _sourceFile;
    private readonly string _testProjectPath;
    private readonly string _testPath;
    private readonly string _sourcePath;
    private readonly string _sourceProjectPath;
    private readonly string _defaultTestProjectFileContents;
    private readonly string _defaultSourceProjectFileContents;
    private readonly StrykerOptions _options;
    private readonly Mock<INugetRestoreProcess> _nugetMock;

    public InputFileResolverTests()
    {
        _currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        _filesystemRoot = Path.GetPathRoot(_currentDirectory);
        _sourcePath = Path.Combine(_filesystemRoot, "ExampleProject");
        _sourceFile = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");
        _testPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_filesystemRoot, "TestProject"));
        _testProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_testPath, "TestProject.csproj"));
        _sourceProjectPath = FilePathUtils.NormalizePathSeparators(Path.Combine(_sourcePath, "ExampleProject.csproj"));
        _options = new StrykerOptions()
        {
            ProjectPath = _testPath
        };
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
        _defaultSourceProjectFileContents = @"<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

</Project>";

        _nugetMock = new Mock<INugetRestoreProcess>();
        _nugetMock.Setup( x => x.RestorePackages(It.IsAny<string>(), It.IsAny<string>()));

    }

    [TestMethod]
    [DataRow("netcoreapp2.1", FrameworkIdentifiers.NetCoreApp, "", 2, 1, 0, 0)]
    [DataRow("netstandard1.6", FrameworkIdentifiers.NetStandard, "", 1, 6, 0, 0)]
    [DataRow("net4.5", FrameworkIdentifiers.Net, "", 4, 5, 0, 0)]
    [DataRow("net4.5.1", FrameworkIdentifiers.Net, "", 4, 5, 1, 0)]
    [DataRow("net45", FrameworkIdentifiers.Net, "", 4, 5, 0, 0)]
    [DataRow("net452", FrameworkIdentifiers.Net, "", 4, 5, 2, 0)]
    [DataRow("net5.0", FrameworkIdentifiers.NetCoreApp, "", 5, 0, 0, 0)]
    [DataRow("net5.0-windows", FrameworkIdentifiers.NetCoreApp, "windows", 5, 0, 0, 0)]
    [DataRow("net5", FrameworkIdentifiers.NetCoreApp, "", 5, 0, 0, 0)]
    public void ProjectAnalyzerShouldDecodeFramework(string tfm, string framework, string platform, int major, int minor, int build, int revision)
    {
        var version = new Version(major, minor, build, revision);
        var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
            projectReferences: new List<string> { _sourcePath },
            targetFramework: tfm).Object;

        var nuGetFramework = analyzerResult.GetNuGetFramework();
        nuGetFramework.Framework.ShouldBe(framework);
        nuGetFramework.Platform.ShouldBe(platform);
        nuGetFramework.Version.ShouldBe(version);
        nuGetFramework.ShouldBe(new NuGetFramework(framework, version, platform, EmptyVersion));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("nxt")]
    [DataRow("mono4.6")]
    [DataRow("netcoreapp1.2.3.4.5")]
    public void ProjectAnalyzerShouldRaiseExceptionsForIllFormedFramework(string tfm)
    {
        var analyzerResult = TestHelper.SetupProjectAnalyzerResult(
            projectReferences: new List<string> { _sourcePath },
            targetFramework: tfm).Object;

        Action lambda = () => analyzerResult.GetNuGetFramework();

        lambda.ShouldThrow(typeof(InputException));
    }

    [TestMethod]
    public void InitializeShouldFindFilesRecursively()
    {

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "OneFolderDeeperOneFolderDeeper", "Deep", "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "OneFolderDeeperOneFolderDeeper", "Deep2", "Recursive.cs"), new MockFileData(_sourceFile)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "bin", "Debug", "netcoreapp2.0"), new MockFileData("Bytecode") }, // bin should be excluded
                { Path.Combine(_sourcePath, "obj", "Release", "netcoreapp2.0"), new MockFileData("Bytecode") }, // obj should be excluded
                { Path.Combine(_sourcePath, "node_modules", "Some package"), new MockFileData("bla") }, // node_modules should be excluded
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath,  []);
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

        result.ProjectContents.GetAllFiles().Count().ShouldBe(3);
    }

    [TestMethod]
    public void InitializeShouldUseBuildalyzerResult()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "Plain.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "OneFolderDeeperOneFolderDeeper", "Deep", "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "OneFolderDeeperOneFolderDeeper", "Deep2", "Recursive.cs"), new MockFileData(_sourceFile)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "bin", "Debug", "netcoreapp2.1"), new MockFileData("Bytecode") }, // bin should be excluded
                { Path.Combine(_sourcePath, "obj", "Debug", "netcoreapp2.1", "ExampleProject.AssemblyInfo.cs"), new MockFileData("Bytecode") }, // obj should be excluded
                { Path.Combine(_sourcePath, "obj", "Release", "netcoreapp2.1"), new MockFileData("Bytecode") }, // obj should be excluded
                { Path.Combine(_sourcePath, "node_modules", "Some package"), new MockFileData("bla") }, // node_modules should be excluded
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath,  fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray());
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);


        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

        result.ProjectContents.GetAllFiles().Count().ShouldBe(5);
    }

    [TestMethod]
    public void InitializeShouldNotSkipXamlFiles()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultSourceProjectFileContents)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "Plain.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "Plain.xaml.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "OneFolderDeeper", "Recursive.cs"), new MockFileData(_sourceFile)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "obj", "Debug", "netcoreapp2.1", "ExampleProject.AssemblyInfo.cs"), new MockFileData("Bytecode") }
            });
        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath,  fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray());
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

        result.ProjectContents.GetAllFiles().Count().ShouldBe(5);
    }

    [TestMethod]
    public void InitializeShouldMutateAssemblyInfo()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultSourceProjectFileContents)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "Plain.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "OneFolderDeeper", "Recursive.cs"), new MockFileData(_sourceFile)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "bin", "Debug", "netcoreapp2.1"), new MockFileData("Bytecode") }, // bin should be excluded
                { Path.Combine(_sourcePath, "obj", "Debug", "netcoreapp2.1", "ExampleProject.AssemblyInfo.cs"), new MockFileData(@"//------------------------------------------------------------------------------
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
                { Path.Combine(_sourcePath, "obj", "Release", "netcoreapp2.1"), new MockFileData("Bytecode") }, // obj should be excluded
                { Path.Combine(_sourcePath, "node_modules", "Some package"), new MockFileData("bla") }, // node_modules should be excluded
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath,  fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray());
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

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

    [TestMethod]
    public void InitializeShouldNotMutateIncompleteAssemblyInfo()
    {
        var textContents = @"//------------------------------------------------------------------------------
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
                        [assembly: System.Reflection.AssemblyVersionAttribute(""1.0.0.0"")]";
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultSourceProjectFileContents)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(_sourceFile)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "obj", "Debug", "netcoreapp2.1", "ExampleProject.AssemblyInfo.cs"), new MockFileData(textContents) },
                { Path.Combine(_sourcePath, "obj", "Release", "netcoreapp2.1"), new MockFileData("Bytecode") }, // obj should be excluded
                { Path.Combine(_sourcePath, "node_modules", "Some package"), new MockFileData("bla") }, // node_modules should be excluded
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath,  fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray());
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

       ((ProjectComponent<SyntaxTree>)result.ProjectContents).CompilationSyntaxTrees.FirstOrDefault(s => s != null && s.FilePath.Contains("AssemblyInfo.cs")).
            ShouldBeSemantically(CSharpSyntaxTree.ParseText(textContents));
    }

    [TestMethod]
    public void InitializeShouldFindSpecifiedTestProjectFile()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "OneFolderDeeper", "Recursive.cs"), new MockFileData(_sourceFile)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath,  fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray());
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

        result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
    }

    [TestMethod]
    public void InitializeShouldResolveImportedProject()
    {
        var readAllText = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

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
                { Path.Combine(_filesystemRoot, "SharedProject", "Shared.cs"), new MockFileData(readAllText)},
                { _sourceProjectPath, new MockFileData(projectFile)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(readAllText)},
                { Path.Combine(_sourcePath, "OneFolderDeeper", "Recursive.cs"), new MockFileData(readAllText)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath,  fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray());
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

        result.ProjectContents.GetAllFiles().Count().ShouldBe(3);
    }

    [TestMethod]
    public void InitializeShouldNotResolveImportedPropsFile()
    {
        var contents = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

        var projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

    <ItemGroup>
    </ItemGroup>

     <Import Project=""..\NonSharedProject\Example.props"" Label=""Shared"" />

</Project>";

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "NonSharedProject", "Example.props"), new MockFileData("")},
                { Path.Combine(_filesystemRoot, "NonSharedProject", "NonSharedSource.cs"), new MockFileData(contents)},
                { _sourceProjectPath, new MockFileData(projectFile)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(contents)},
                { Path.Combine(_sourcePath, "OneFolderDeeper", "Recursive.cs"), new MockFileData(contents)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

        result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
    }

    [TestMethod]
    public void InitializeShouldResolveMultipleImportedProjects()
    {
        var contents = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

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
        var sharedItems2 = @"<?xml version=""1.0"" encoding=""utf-8""?>
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

     <Import Project=""../SharedProject1/Example.projitems"" Label=""Shared"" />
     <Import Project=""../SharedProject2/Example.projitems"" Label=""Shared"" />

</Project>";

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "SharedProject1", "Example.projitems"), new MockFileData(sharedItems)},
                { Path.Combine(_filesystemRoot, "SharedProject1", "Shared.cs"), new MockFileData(contents)},
                { Path.Combine(_filesystemRoot, "SharedProject2", "Example.projitems"), new MockFileData(sharedItems2)},
                { Path.Combine(_filesystemRoot, "SharedProject2", "Shared.cs"), new MockFileData(contents)},
                { _sourceProjectPath, new MockFileData(projectFile)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(contents)},
                { Path.Combine(_sourcePath, "OneFolderDeeper", "Recursive.cs"), new MockFileData(contents)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)}
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

        result.ProjectContents.GetAllFiles().Count().ShouldBe(4);
    }

    [TestMethod]
    public void InitializeShouldThrowOnMissingSharedProject()
    {
        var contents = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

        var projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

    <ItemGroup>
    </ItemGroup>

     <Import Project=""..\SharedProject\Example.projitems"" Label=""Shared"" />

    <ItemGroup>
        <ProjectReference Include=""..\ExampleProject\ExampleProject.csproj"" />
    </ItemGroup>
</Project>";

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "SharedProject", "Shared.cs"), new MockFileData(contents)},
                { _sourceProjectPath, new MockFileData(projectFile)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(contents)},
                { Path.Combine(_sourcePath, "OneFolderDeeper", "Recursive.cs"), new MockFileData(contents)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)}
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        Should.Throw<FileNotFoundException>(() => target.ResolveSourceProjectInfos(_options));
    }

    [TestMethod]
    public void InitializeShouldResolvePropertiesInSharedProjectImports()
    {
        var contents = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

        var sharedFile = "<Project />";

        var projectFile = @"
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
        <ProjectReference Include=""..\ExampleProject\ExampleProject.csproj"" />
    </ItemGroup>
</Project>";

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "SharedProject", "Example.projitems"), new MockFileData(sharedFile)},
                { Path.Combine(_filesystemRoot, "SharedProject", "Shared.cs"), new MockFileData(contents)},
                { _sourceProjectPath, new MockFileData(projectFile)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(contents)},
                { Path.Combine(_sourcePath, "OneFolderDeeper", "Recursive.cs"), new MockFileData(contents)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)}
            });

        var properties = new Dictionary<string, string>
            { { "IsTestProject", "False" }, { "ProjectTypeGuids", "not testproject" }, { "Language", "C#" }, { "SharedDir", "SharedProject" } };
        var sourceProjectManagerMock = BuildProjectAnalyzerMock(_sourceProjectPath, [], properties, new List<string>());
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

        var allFiles = result.ProjectContents.GetAllFiles();

        allFiles.Count().ShouldBe(3);
    }

    [TestMethod]
    public void InitializeShouldThrowIfImportPropertyCannotBeResolved()
    {
        var contents = File.ReadAllText(_currentDirectory + "/TestResources/ExampleSourceFile.cs");

        var sharedFile = "<Project />";

        var projectFile = @"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

    <ItemGroup>
    </ItemGroup>

    <Import Project=""../$(SharedDir)/Example.projitems"" Label=""Shared"" />

    <ItemGroup>
        <ProjectReference Include=""..\ExampleProject\ExampleProject.csproj"" />
    </ItemGroup>
</Project>";

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_filesystemRoot, "SharedProject", "Example.projitems"), new MockFileData(sharedFile)},
                { Path.Combine(_filesystemRoot, "SharedProject", "Shared.cs"), new MockFileData(contents)},
                { _sourceProjectPath, new MockFileData(projectFile)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(contents)},
                { Path.Combine(_sourcePath, "OneFolderDeeper", "Recursive.cs"), new MockFileData(contents)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)}
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var exception = Should.Throw<InputException>(() => target.ResolveSourceProjectInfos(_options));
        exception.Message.ShouldBe($"Missing MSBuild property (SharedDir) in project reference (..{FilePathUtils.NormalizePathSeparators("/$(SharedDir)/Example.projitems")}). Please check your project file ({_sourceProjectPath}) and try again.");
    }

    [TestMethod]
    [DataRow("bin")]
    [DataRow("obj")]
    [DataRow("node_modules")]
    public void InitializeShouldIgnoreBinFolder(string folderName)
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "somecsharpfile.cs"), new MockFileData("Bytecode") },
                { Path.Combine(_filesystemRoot, "TestProject", "TestProject.csproj"), new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, folderName, "subfolder", "somecsharpfile.cs"), new MockFileData("Bytecode") },
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

        ((CsharpFolderComposite)result.ProjectContents).Children.Count().ShouldBe(1);

        var sub =(CsharpFolderComposite) ((CsharpFolderComposite)result.ProjectContents).Children.First();
        // here sub is the root folder of the project
        sub =(CsharpFolderComposite) sub.Children.First();
        sub.Children.Count().ShouldBe(2);
    }

    [TestMethod]
    public void ShouldThrowExceptionOnNullPath()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
            });

        BuildBuildAnalyzerMock(new Dictionary<string, IProjectAnalyzer>());

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);
        Should.Throw<ArgumentNullException>(() => target.FindTestProject(null));
    }

    [TestMethod]
    public void ShouldThrowExceptionOnNoProjectFile()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData("content") }
            });

        BuildBuildAnalyzerMock(new Dictionary<string, IProjectAnalyzer>());

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);
        Should.Throw<InputException>(() => target.FindTestProject(Path.Combine(_sourcePath)));
    }

    [TestMethod]
    public void ShouldThrowStrykerInputExceptionOnTwoProjectFiles_AndNoTestProjectFileSpecified()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Path.Combine(_sourcePath, "ExampleProject.csproj"), new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "ExampleProject2.csproj"), new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData("content")}
            });

        var target = new InputFileResolver(fileSystem, null);

        var errorMessage =
$@"Expected exactly one .csproj file, found more than one:
{Path.GetFullPath("/ExampleProject/ExampleProject.csproj")}
{Path.GetFullPath("/ExampleProject/ExampleProject2.csproj")}

Please specify a test project name filter that results in one project.
";

        var exception = Should.Throw<InputException>(() => target.FindTestProject(Path.Combine(_sourcePath)));
        exception.Message.ShouldBe(errorMessage);
       
    }

    [TestMethod]
    public void ShouldNotThrowExceptionOnTwoProjectFilesInDifferentLocations()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { Path.Combine(_sourcePath, "ExampleProject.csproj"), new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_filesystemRoot, "ExampleProject","ExampleProject2", "ExampleProject2.csproj"), new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData("content")}
        });

        var target = new InputFileResolver(fileSystem, null);

        var actual = target.FindTestProject(Path.Combine(_sourcePath));

        actual.ShouldBe(Path.Combine(_sourcePath, "ExampleProject.csproj"));
    }

    [TestMethod]
    public void ShouldChooseGivenTestProjectFileIfPossible()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { Path.Combine(_sourcePath, "ExampleProject.csproj"), new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "TestProject.csproj"), new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData("content")}
        });
        var target = new InputFileResolver(fileSystem, null);

        var actual = target.FindTestProject(Path.Combine(_sourcePath, "TestProject.csproj"));

        actual.ShouldBe(Path.Combine(_sourcePath, "TestProject.csproj"));
    }

    [TestMethod]
    public void ShouldThrowExceptionIfGivenTestFileDoesNotExist()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { Path.Combine(_sourcePath, "AlternateProject.csproj"), new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData("content")}
        });
        var target = new InputFileResolver(fileSystem, null);

        var exception = Should.Throw<InputException>(() => target.FindTestProject(Path.Combine(_sourcePath, "GivenTestProject.csproj")));
        exception.Message.ShouldStartWith("No .csproj or .fsproj file found, please check your project directory at");
    }

    [TestMethod]
    public void ShouldChooseGivenTestProjectFileIfPossible_AtRelativeLocation()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { Path.Combine(_sourcePath, "ExampleProject.csproj"), new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "SubFolder", "TestProject.csproj"), new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData("content")}
        });
        var target = new InputFileResolver(fileSystem, null);

        var actual = target.FindTestProject(Path.Combine(_sourcePath, "SubFolder", "TestProject.csproj"));

        actual.ShouldBe(Path.Combine(_sourcePath, "SubFolder", "TestProject.csproj"));
    }

    [TestMethod]
    public void ShouldChooseGivenTestProjectFileIfPossible_AtFullPath()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { Path.Combine(_sourcePath, "ExampleProject.csproj"), new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath,"SubFolder", "TestProject.csproj"), new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData("content")}
        });
        var target = new InputFileResolver(fileSystem, null);

        var actual = target.FindTestProject(Path.Combine(_filesystemRoot,
            FilePathUtils.NormalizePathSeparators(Path.Combine(_sourcePath, "SubFolder"))));

        actual.ShouldBe(Path.Combine(_sourcePath, "SubFolder", "TestProject.csproj"));
    }

    [TestMethod]
    [DataRow("net6.0")]
    [DataRow("net3.5")]
    public void ShouldSelectAvailableFramework_WhenDesiredNotFound(string targetFramework)
    {
        // Arrange
        var basePath = Path.Combine(_sourcePath, "ExampleProject");
        var testProjectPath = Path.Combine(_sourcePath, "TestProjectFolder", "TestProject.csproj");
        var sourceProjectPath = Path.Combine(_sourcePath, "ExampleProject", "ExampleProject.csproj");
        var sourceProjectNameFilter = "ExampleProject.csproj";
        
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { sourceProjectPath, new MockFileData(_defaultTestProjectFileContents)},
            { testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData("content")}
        });

        var options = new StrykerOptions()
        {
            ProjectPath = basePath,
            SourceProjectName = sourceProjectNameFilter,
            TestProjects = new List<string> { testProjectPath },
            TargetFramework = targetFramework
        };

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(sourceProjectPath,  fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray());
        var testProjectManagerMock = TestProjectAnalyzerMock(testProjectPath, sourceProjectPath);

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        // Act
        var result = target.ResolveSourceProjectInfos(options).First();

        // Assert
        result.AnalyzerResult.TargetFramework.ShouldBe(DefaultFramework);
    }

    [TestMethod]
    public void ShouldThrowOnMsTestV1Detected()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _sourcePath, new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "Recursive.cs"), new MockFileData(_sourceFile)},
            { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)}
        });

        var testProjectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                projectReferences: new List<string> { _sourcePath },
                targetFramework: "netcoreapp2.1",
                properties: new Dictionary<string, string>() { { "Language", "C#" } },
                projectFilePath: _testProjectPath,
                references: new string[] { "Microsoft.VisualStudio.QualityTools.UnitTestFramework" }).Object;

        // Act
        var ex = Should.Throw<InputException>(() => new TestProject(fileSystem, testProjectAnalyzerResult));

        ex.Message.ShouldBe("Please upgrade your test projects to MsTest V2. Stryker.NET uses VSTest which does not support MsTest V1.");
        ex.Details.ShouldBe(@"See https://devblogs.microsoft.com/devops/upgrade-to-mstest-v2/ for upgrade instructions.");
    }

    [TestMethod]
    public void ShouldSkipXamlFiles()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultTestProjectFileContents)},
                { Path.Combine(_sourcePath, "app.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "app.xaml"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "app.xaml.cs"), new MockFileData(_sourceFile)},
                { Path.Combine(_sourcePath, "OneFolderDeeper", "Recursive.cs"), new MockFileData(_sourceFile)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)}
            });
        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        // Act
        var result = target.ResolveSourceProjectInfos(_options).First();

        result.ProjectContents.GetAllFiles().Count().ShouldBe(2);
    }

    [TestMethod]
    public void ShouldFindAllTestProjects()
    {
        // Arrange
        var testProject1 = Path.Combine(_filesystemRoot, "TestProject1", "ExampleProject.csproj");
        var testProject2 = Path.Combine(_filesystemRoot, "TestProject2", "ExampleProject.csproj");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultSourceProjectFileContents)},
                { Path.Combine(_sourcePath, "myFile.cs"), new MockFileData(_sourceFile)},
                { testProject1, new MockFileData(_defaultTestProjectFileContents) },
                { testProject2, new MockFileData(_defaultTestProjectFileContents) },
            });

        var options = new StrykerOptions
        {
            ProjectPath = Path.Combine(_sourcePath),
            TestProjects = new List<string>
            {
                testProject1,
                testProject2
            }
        };

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var testProjectManagerMock = TestProjectAnalyzerMock(testProject1, _sourceProjectPath, "netcoreapp2.1");
        var testProjectManagerMock2 = TestProjectAnalyzerMock(testProject2, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object },
            { "MyProject.UnitTests2", testProjectManagerMock2.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);


        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);
        // Act
        var result = target.ResolveSourceProjectInfos(options).First();

        // Assert
        result.ProjectContents.GetAllFiles().Count().ShouldBe(1);
    }

    [TestMethod]
    public void ShouldFindSourceProjectWhenSingleProjectReferenceAndNoFilter()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _sourceProjectPath, new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "source.cs"), new MockFileData(_sourceFile)},
            { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
        });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath,  fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray());
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var result = target.ResolveSourceProjectInfos(_options).First();

        result.AnalyzerResult.ProjectFilePath.ShouldBe(_sourceProjectPath);
    }

    [TestMethod]
    public void ShouldThrowOnNoProjectReference()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
        });

        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, null, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var action = () => target.ResolveSourceProjectInfos(_options);

        action.ShouldThrow<InputException>().Message.ShouldContain("no project");
    }

    [TestMethod]
    public void ShouldThrowOnMultipleProjectsWithoutFilter()
    {
        // Arrange
        var project2 = Path.Combine(_filesystemRoot, "Project2", "Project2.csproj");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultSourceProjectFileContents)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents) },
                { project2, new MockFileData(_defaultSourceProjectFileContents) },
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var sourceProject2ManagerMock = SourceProjectAnalyzerMock(project2, []);
        var properties = new Dictionary<string, string>{ { "IsTestProject", "True" }, { "Language", "C#" } };
        var testProjectManagerMock = BuildProjectAnalyzerMock(_testProjectPath, [], properties, [_sourceProjectPath, project2], "netcore2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject2", sourceProject2ManagerMock.Object },
            { "MyProject.UnitTests2", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);
        // Act
        var result = () => target.ResolveSourceProjectInfos(_options);

        // Assert
        var ex = result.ShouldThrow<InputException>();
        ex.Message.ShouldContain("Test project contains more than one project reference. Please set the project option");
        ex.Message.ShouldContain("Choose one of the following references:");
    }

    [TestMethod]
    public void ShouldNotThrowIfMultipleProjectButOneIsAlwaysReferenced()
    {
        // Arrange
        var project2 = Path.Combine(_filesystemRoot, "Project2", "Project2.csproj");
        var test2Path = Path.Combine(_testPath, "AltTest", "Test.csproj");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { _sourceProjectPath, new MockFileData(_defaultSourceProjectFileContents)},
                { _testProjectPath, new MockFileData(_defaultTestProjectFileContents) },
                { test2Path, new MockFileData(_defaultTestProjectFileContents) },
                { project2, new MockFileData(_defaultSourceProjectFileContents) },
            });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var sourceProject2ManagerMock = SourceProjectAnalyzerMock(project2, []);
        var properties = new Dictionary<string, string>{ { "IsTestProject", "True" }, { "Language", "C#" } };
        var testProjectManagerMock = BuildProjectAnalyzerMock(_testProjectPath, [], properties, [_sourceProjectPath, project2], "netcore2.1");
        var properties2 = new Dictionary<string, string>{ { "IsTestProject", "True" }, { "Language", "C#" } };
        var testProjectManagerMock2 = BuildProjectAnalyzerMock(test2Path, [], properties2, [_sourceProjectPath], "netcore2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject2", sourceProject2ManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object },
            { "MyProject.UnitTests2", testProjectManagerMock2.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);
        var options = new StrykerOptions
        {
            TestProjects = [_testProjectPath, test2Path],
            WorkingDirectory = _testPath
        };
        // Act
        var result = target.ResolveSourceProjectInfos(options).First();

        // Assert
        result.AnalyzerResult.ProjectFilePath.ShouldBe(_sourceProjectPath);
    }

    [TestMethod]
    [DataRow("ExampleProject.csproj")]
    [DataRow("exampleproject.csproj")]
    [DataRow("ExampleProject")]
    [DataRow("exampleproject")]
    [DataRow("Example")]
    public void ShouldMatchFromMultipleProjectByName(string shouldMatch)
    {
        // Arrange
        var project2 = Path.Combine(_filesystemRoot, "Project2", "Project2.csproj");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _sourceProjectPath, new MockFileData(_defaultSourceProjectFileContents)},
            { _testProjectPath, new MockFileData(_defaultTestProjectFileContents) },
            { project2, new MockFileData(_defaultSourceProjectFileContents) },
        });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var sourceProject2ManagerMock = SourceProjectAnalyzerMock(project2, []);
        var properties = new Dictionary<string, string>{ { "IsTestProject", "True" }, { "Language", "C#" } };
        var testProjectManagerMock = BuildProjectAnalyzerMock(_testProjectPath, [], properties, [_sourceProjectPath, project2], "netcore2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject2", sourceProject2ManagerMock.Object },
            { "MyProject.UnitTests2", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var options = new StrykerOptions { SourceProjectName = shouldMatch, ProjectPath = _testPath };
        var result = target.ResolveSourceProjectInfos(options).First();

        result.AnalyzerResult.ProjectFilePath.ShouldBe(_sourceProjectPath);
    }

    [TestMethod]
    [DataRow("Project.csproj")]
    [DataRow("project.csproj")]
    [DataRow("Project")]
    [DataRow(".csproj")]
    public void ShouldThrowWhenTheNameMatchesMore(string shouldMatchMoreThanOne)
    {
        // Arrange
        var project2 = Path.Combine(_filesystemRoot, "Project2", "2ndProject.csproj");
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _sourceProjectPath, new MockFileData(_defaultSourceProjectFileContents)},
            { _testProjectPath, new MockFileData(_defaultTestProjectFileContents) },
            { project2, new MockFileData(_defaultSourceProjectFileContents) },
        });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath, []);
        var sourceProject2ManagerMock = SourceProjectAnalyzerMock(project2, []);
        var properties = new Dictionary<string, string>{ { "IsTestProject", "True" }, { "Language", "C#" } };
        var testProjectManagerMock = BuildProjectAnalyzerMock(_testProjectPath, [], properties, [_sourceProjectPath, project2], "netcore2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject2", sourceProject2ManagerMock.Object },
            { "MyProject.UnitTests2", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var options = new StrykerOptions { SourceProjectName = shouldMatchMoreThanOne, ProjectPath = _testPath };
        var result = () => target.ResolveSourceProjectInfos(options);
        result.ShouldThrow<InputException>().Message.ShouldContain("more than one project");
    }

    [TestMethod]
    public void ShouldThrowWhenTheNameMatchesNone()
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _sourceProjectPath, new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "source.cs"), new MockFileData(_sourceFile)},
            { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
        });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath,  fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray());
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var options = new StrykerOptions { SourceProjectName = "wrong.csprj", ProjectPath = _testPath };
        var result = () => target.ResolveSourceProjectInfos(options);
        result.ShouldThrow<InputException>().Message.ShouldContain("No project references found.");

    }

    [TestMethod]
    [DataRow("ExampleProject/ExampleProject.csproj")]
    [DataRow("ExampleProject\\ExampleProject.csproj")]
    public void ShouldMatchOnBothForwardAndBackwardsSlash(string shouldMatch)
    {
        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _sourceProjectPath, new MockFileData(_defaultTestProjectFileContents)},
            { Path.Combine(_sourcePath, "source.cs"), new MockFileData(_sourceFile)},
            { _testProjectPath, new MockFileData(_defaultTestProjectFileContents)},
        });

        var sourceProjectManagerMock = SourceProjectAnalyzerMock(_sourceProjectPath,  fileSystem.AllFiles.Where(s => s.EndsWith(".cs")).ToArray());
        var testProjectManagerMock = TestProjectAnalyzerMock(_testProjectPath, _sourceProjectPath, "netcoreapp2.1");

        var analyzerResults = new Dictionary<string, IProjectAnalyzer>
        {
            { "MyProject", sourceProjectManagerMock.Object },
            { "MyProject.UnitTests", testProjectManagerMock.Object }
        };
        BuildBuildAnalyzerMock(analyzerResults);

        var target = new InputFileResolver(fileSystem, BuildalyzerProviderMock.Object, _nugetMock.Object);

        var options = new StrykerOptions { SourceProjectName = shouldMatch, ProjectPath = _testPath };
        var result = target.ResolveSourceProjectInfos(_options).First();

        result.AnalyzerResult.ProjectFilePath.ShouldBe(_sourceProjectPath);
    }
}
