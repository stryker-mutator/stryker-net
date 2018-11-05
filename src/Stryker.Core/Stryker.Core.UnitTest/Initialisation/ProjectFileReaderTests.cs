using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using System;
using System.IO;
using System.Xml.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectFileReaderTests
    {
        [Theory]
        [InlineData("netcoreapp2.0")]
        [InlineData("netcoreapp1.1")]
        [InlineData("netcoreapp2.1")]
        public void ProjectFileReader_ShouldParseXML(string target)
        {
            XDocument xDocument = XDocument.Parse($@"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>{target}</TargetFramework>
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
                
</Project>");
            var result = new ProjectFileReader().ReadProjectFile(xDocument, null);

            result.ProjectReference.ShouldBe(Path.Combine("..", "ExampleProject", "ExampleProject.csproj"));
            result.TargetFramework.ShouldBe(target);
        }

        [Fact]
        public void ProjectFileReader_ShouldThrowOnNoProjectReference()
        {
            XDocument xDocument = XDocument.Parse(@"
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
                
</Project>");
            var ex = Assert.Throws<StrykerInputException>(() => new ProjectFileReader().ReadProjectFile(xDocument, null));
            Assert.Equal("Project reference issue.", ex.Message);
            ex.Details.ShouldContain("no project", Case.Insensitive);
        }

        [Fact]
        public void ProjectFileReader_ShouldThrowOnMultipleProjects()
        {
            XDocument xDocument = XDocument.Parse(@"
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
        <ProjectReference Include=""..\AnotherProject\AnotherProject.csproj"" />
    </ItemGroup>
</Project>");
            var exception = Assert.Throws<StrykerInputException>(() => new ProjectFileReader().ReadProjectFile(xDocument, null));
            Assert.Equal("Project reference issue.", exception.Message);
            exception.Details.ShouldContain("--project-file");
        }

        [Theory]
        [InlineData("ExampleProject.csproj")]
        [InlineData("exampleproject.csproj")]
        [InlineData("ExampleProject")]
        [InlineData("exampleproject")]
        [InlineData("Example")]
        public void ProjectFileReader_ShouldMatchFromMultipleProjectByName(string shouldMatch)
        {
            XDocument xDocument = XDocument.Parse(@"
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
        <ProjectReference Include=""..\AnotherProject\AnotherProject.csproj"" />
    </ItemGroup>
</Project>");
            var result = new ProjectFileReader().ReadProjectFile(xDocument, shouldMatch);
            result.ProjectReference.ShouldBe(Path.Combine("..", "ExampleProject", "ExampleProject.csproj"));
        }


        [Theory]
        [InlineData("Project.csproj")]
        [InlineData("project.csproj")]
        [InlineData("Project")]
        [InlineData(".csproj")]
        public void ProjectFileReader_ShouldThrowWhenTheNameMatchesMore(string shouldMatchMoreThanOne)
        {
            XDocument xDocument = XDocument.Parse(@"
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
        <ProjectReference Include=""..\AnotherProject\AnotherProject.csproj"" />
    </ItemGroup>
</Project>");
            var exception = Assert.Throws<StrykerInputException>(() => new ProjectFileReader().ReadProjectFile(xDocument, shouldMatchMoreThanOne));
            Assert.Equal("Project reference issue.", exception.Message);
            exception.Details.ShouldContain("more than one", Case.Insensitive);
        }

        [Theory]
        [InlineData("SomeProject.csproj")]
        [InlineData("??")]
        [InlineData("WrongProject.csproj")]
        public void ProjectFileReader_ShouldThrowWhenTheNameMatchesNone(string shouldMatchNone)
        {
            XDocument xDocument = XDocument.Parse(@"
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
        <ProjectReference Include=""..\AnotherProject\AnotherProject.csproj"" />
    </ItemGroup>
</Project>");
            var exception = Assert.Throws<StrykerInputException>(() => new ProjectFileReader().ReadProjectFile(xDocument, shouldMatchNone));
            Assert.Equal("Project reference issue.", exception.Message);
            exception.Details.ShouldContain("no project", Case.Insensitive);
        }

        [Fact]
        public void ProjectFileReader_FindAssemblyNameWhenAvailable()
        {
            XDocument xDocument = XDocument.Parse(@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
    <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
    <PackageId>dotnet-stryker</PackageId>
    <Authors>Richard</Authors>
    <Company>InfoSupport</Company>
    <Product>Mutation Testing</Product>
    <AssemblyName>dotnet-stryker</AssemblyName>
    <RootNamespace>Stryker.CLI</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.Extensions.CommandLineUtils"" Version=""1.1.1"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""..\..\Stryker.Core\Stryker.Core\Stryker.Core.csproj"" />
  </ItemGroup>

  <ItemGroup>
    <None Include=""build\**"" Pack=""True"" PackagePath=""build\"" />
  </ItemGroup>
</Project>
");
            var result = new ProjectFileReader().FindAssemblyName(xDocument);
            result.ShouldBe(@"dotnet-stryker");
        }

        [Fact]
        public void ProjectFileReader_FindAssemblyNameShouldBeNullWhenNotAvailable()
        {
            XDocument xDocument = XDocument.Parse(@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp2.0</TargetFramework>
    <GeneratePackageOnBuild>false</GeneratePackageOnBuild>
    <PackageId>dotnet-stryker</PackageId>
    <Authors>Richard</Authors>
    <Company>InfoSupport</Company>
    <Product>Mutation Testing</Product>
    <RootNamespace>Stryker.CLI</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.Extensions.CommandLineUtils"" Version=""1.1.1"" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include=""..\..\Stryker.Core\Stryker.Core\Stryker.Core.csproj"" />
  </ItemGroup>

  <ItemGroup>
    <None Include=""build\**"" Pack=""True"" PackagePath=""build\"" />
  </ItemGroup>
</Project>
");
            var result = new ProjectFileReader().ReadProjectFile(xDocument, "");
            result.AssemblyName.ShouldBeNull();
        }

        [Theory]
        [InlineData("netcoreapp2.0;net461;netstandard2.0")]
        [InlineData("netcoreapp1.1;net45")]
        [InlineData("netcoreapp2.1")]
        public void ProjectFileReader_ShouldParseWhenMultipleFrameworks(string target)
        {
            var xDocument = XDocument.Parse($@"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>{(
        target.Contains(";") ? $"<TargetFrameworks>{target}</TargetFrameworks>" : $"<TargetFramework>{target}</TargetFramework>"
    )}        
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
                
</Project>");

            var result = new ProjectFileReader().ReadProjectFile(xDocument, null);

            result.TargetFramework.ShouldBe(target.Split(';')[0]);
        }
    }
}
