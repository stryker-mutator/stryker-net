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
        [Fact]
        public void TestProjectEqualsWhenAllPropertiesEqual()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            var rootPath = Path.Combine("c", "TestProject");
            var fileAPath = Path.Combine(rootPath, "ExampleTestFileA.cs");
            fileSystem.AddDirectory(rootPath);
            var fileA = File.ReadAllText(Path.Combine(".", "TestResources", "ExampleTestFileA.cs"));
            fileSystem.AddFile(fileAPath, new MockFileData(fileA));
            var testProjectAnalyzerResultMock = TestHelper.SetupProjectAnalyzerResult(
                references: Array.Empty<string>(),
                sourceFiles: new string[] { fileAPath }
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
            var rootPath = Path.Combine("c", "TestProject");
            var fileAPath = Path.Combine(rootPath, "ExampleTestFileA.cs");
            var fileBPath = Path.Combine(rootPath, "ExampleTestFileB.cs");
            fileSystem.AddDirectory(rootPath);
            var fileA = File.ReadAllText(Path.Combine(".", "TestResources", "ExampleTestFileA.cs"));
            var fileB = File.ReadAllText(Path.Combine(".", "TestResources", "ExampleTestFileB.cs"));
            fileSystem.AddFile(fileAPath, new MockFileData(fileA));
            fileSystem.AddFile(fileBPath, new MockFileData(fileB));
            var testProjectAnalyzerResultAMock = TestHelper.SetupProjectAnalyzerResult(
                references: Array.Empty<string>(),
                sourceFiles: new string[] { fileAPath }
            );
            var testProjectAnalyzerResultBMock = TestHelper.SetupProjectAnalyzerResult(
                references: Array.Empty<string>(),
                sourceFiles: new string[] { fileBPath }
            );

            var testProjectA = new TestProject(fileSystem, testProjectAnalyzerResultAMock.Object);
            var testProjectB = new TestProject(fileSystem, testProjectAnalyzerResultBMock.Object);

            // Assert
            testProjectA.ShouldNotBe(testProjectB);
        }
    }
}
