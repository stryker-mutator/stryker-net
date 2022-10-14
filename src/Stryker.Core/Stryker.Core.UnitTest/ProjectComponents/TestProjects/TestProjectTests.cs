using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using Shouldly;
using Stryker.Core.ProjectComponents.TestProjects;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects
{
    public class TestProjectTests
    {
        private readonly string _filesystemRoot;

        [Fact]
        public void TestProjectEqualsWhenAllPropertiesEqual()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory("/c/testProject");
            var fileA = File.ReadAllText(
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/TestResources/ExampleTestFileA.cs")
                );
            fileSystem.AddFile(
                "/c/testProject/exampleTestFileA.cs",
                new MockFileData(fileA));
            var testProjectAnalyzerResultMock = TestHelper.SetupProjectAnalyzerResult(
                references: Array.Empty<string>(),
                sourceFiles: new string[] { "/c/testProject/exampleTestFileA.cs" }
            );

            var testProjectA = new TestProject(fileSystem, testProjectAnalyzerResultMock.Object);
            var testProjectB = new TestProject(fileSystem, testProjectAnalyzerResultMock.Object);

            // Assert
            testProjectA.ShouldBe(testProjectB);
            testProjectA.GetHashCode().ShouldBe(testProjectB.GetHashCode());
        }

        [Fact]
        public void TestProjectsNotEqualWhenAnalyzerResultsNotEqual()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory("/c/testProject");
            var fileA = File.ReadAllText(
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/TestResources/ExampleTestFileA.cs")
                );
            var fileB = File.ReadAllText(
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "/TestResources/ExampleTestFileB.cs")
                );
            fileSystem.AddFile(
                "/c/testProject/exampleTestFileA.cs",
                new MockFileData(fileA));
            fileSystem.AddFile(
                "/c/testProject/exampleTestFileB.cs",
                new MockFileData(fileB));
            var testProjectAnalyzerResultAMock = TestHelper.SetupProjectAnalyzerResult(
                references: Array.Empty<string>(),
                sourceFiles: new string[] { "/c/testProject/exampleTestFileA.cs" }
            );
            var testProjectAnalyzerResultBMock = TestHelper.SetupProjectAnalyzerResult(
                references: Array.Empty<string>(),
                sourceFiles: new string[] { "/c/testProject/exampleTestFileB.cs" }
            );

            var testProjectA = new TestProject(fileSystem, testProjectAnalyzerResultAMock.Object);
            var testProjectB = new TestProject(fileSystem, testProjectAnalyzerResultBMock.Object);

            // Assert
            testProjectA.ShouldNotBe(testProjectB);
        }
    }
}
