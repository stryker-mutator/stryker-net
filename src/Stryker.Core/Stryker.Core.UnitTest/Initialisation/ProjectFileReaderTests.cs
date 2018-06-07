using Shouldly;
using Stryker.Core.Initialisation;
using System;
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

            result.ProjectReference.ShouldBe(@"..\ExampleProject\ExampleProject.csproj");
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
            Assert.Throws<NotSupportedException>(() => new ProjectFileReader().ReadProjectFile(xDocument, null));
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
            var exception = Assert.Throws<NotSupportedException>(() => new ProjectFileReader().ReadProjectFile(xDocument, null));
            exception.Message.ShouldContain("--project");
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
            result.ProjectReference.ShouldBe(@"..\ExampleProject\ExampleProject.csproj");
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
            var exception = Assert.Throws<ArgumentException>(() => new ProjectFileReader().ReadProjectFile(xDocument, shouldMatchMoreThanOne));
            exception.Message.ShouldContain("more than one", Case.Insensitive);
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
            var exception = Assert.Throws<ArgumentException>(() => new ProjectFileReader().ReadProjectFile(xDocument, shouldMatchNone));
            exception.Message.ShouldContain("no project", Case.Insensitive);
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
    }
}
