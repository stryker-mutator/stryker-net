using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectFileReaderTests
    {
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
    }
}
