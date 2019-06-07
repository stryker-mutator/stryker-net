using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectFileReaderTests
    {
        [Fact]
        public void ProjectFileReader_HappyFlow()
        {
            var processMock = new Mock<IProcessExecutor>(MockBehavior.Strict);

            var target = new ProjectFileReader(processMock.Object);

            var result = target.DetermineProjectUnderTest(new List<string>() { @"..\ExampleProject\ExampleProject.csproj" }, null);
            result.ShouldBe(@"..\ExampleProject\ExampleProject.csproj");
        }

        [Fact]
        public void ProjectFileReader_ShouldThrowOnNoProjectReference()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new ProjectFileReader().DetermineProjectUnderTest(Enumerable.Empty<string>(), null);
            });

            ex.Message.ShouldBe("Project reference issue.");
            ex.Details.ShouldContain("no project", Case.Insensitive);
        }

        [Fact]
        public void ProjectFileReader_ShouldThrowOnMultipleProjects()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new ProjectFileReader().DetermineProjectUnderTest(new List<string>() {
                    @"..\ExampleProject\ExampleProject.csproj",
                    @"..\AnotherProject\AnotherProject.csproj"
                }, null);
            });

            ex.Message.ShouldBe("Project reference issue.");
            ex.Details.ShouldContain("--project-file");
        }

        [Theory]
        [InlineData("ExampleProject.csproj")]
        [InlineData("exampleproject.csproj")]
        [InlineData("ExampleProject")]
        [InlineData("exampleproject")]
        [InlineData("Example")]
        public void ProjectFileReader_ShouldMatchFromMultipleProjectByName(string shouldMatch)
        {
            var result = new ProjectFileReader().DetermineProjectUnderTest(new List<string>() {
                    @"..\ExampleProject\ExampleProject.csproj",
                    @"..\AnotherProject\AnotherProject.csproj"
                }, shouldMatch);

            result.ShouldBe(Path.Combine("..", "ExampleProject", "ExampleProject.csproj"));
        }

        [Theory]
        [InlineData("Project.csproj")]
        [InlineData("project.csproj")]
        [InlineData("Project")]
        [InlineData(".csproj")]
        public void ProjectFileReader_ShouldThrowWhenTheNameMatchesMore(string shouldMatchMoreThanOne)
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new ProjectFileReader().DetermineProjectUnderTest(new List<string>() {
                    @"..\ExampleProject\ExampleProject.csproj",
                    @"..\AnotherProject\AnotherProject.csproj"
                }, shouldMatchMoreThanOne);
            });

            ex.Message.ShouldBe("Project reference issue.");
            ex.Details.ShouldContain("more than one", Case.Insensitive);
        }

        [Theory]
        [InlineData("SomeProject.csproj")]
        [InlineData("??")]
        [InlineData("WrongProject.csproj")]
        public void ProjectFileReader_ShouldThrowWhenTheNameMatchesNone(string shouldMatchNone)
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new ProjectFileReader().DetermineProjectUnderTest(new List<string>() {
                    @"..\ExampleProject\ExampleProject.csproj",
                    @"..\AnotherProject\AnotherProject.csproj"
                }, shouldMatchNone);
            });

            ex.Message.ShouldBe("Project reference issue.");
            ex.Details.ShouldContain("no project", Case.Insensitive);
        }

        [Fact]
        public void ProjectFileReader_ShouldFindCompileLinkReferencedFiles_AndReturnActualAndVirtualFilePaths()
        {
            string projectFileContent = @"
<Project Sdk=""Microsoft.NET.Sdk"">
    <PropertyGroup>
        <TargetFramework>netcoreapp2.0</TargetFramework>
        <IsPackable>false</IsPackable>
    </PropertyGroup>

    <ItemGroup>
        <Compile Include=""..\ExtraFile\ExampleSourceFile.cs"" Link=""ExampleSourceFile.cs"" />
    </ItemGroup>

    <ItemGroup>
        <ProjectReference Include=""../ExampleProject/ExampleProject.csproj"" />
    </ItemGroup>
</Project>";

            var projectFilePath = Path.Combine("C:\\", "ExampleProject", "ExampleProject.csproj");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                {
                    { projectFilePath, new MockFileData(projectFileContent)}
            });

            var projectFileStream = fileSystem.File.OpenText(projectFilePath);

            var projectFile = XDocument.Load(projectFileStream);

            new ProjectFileReader().FindLinkedFiles(projectFile).ShouldContainKeyAndValue(@"..\ExtraFile\ExampleSourceFile.cs", "ExampleSourceFile.cs");
        }
    }
}
