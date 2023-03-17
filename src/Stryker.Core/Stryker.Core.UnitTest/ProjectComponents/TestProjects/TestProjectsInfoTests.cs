using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.ProjectComponents.TestProjects;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects
{
    public class TestProjectsInfoTests
    {
        [Fact]
        public void ShouldGenerateInjectionPath()
        {
            var sourceProjectAnalyzerResults = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", "/app/bin/Debug/" },
                        { "TargetFileName", "AppToTest.dll" }
                    }).Object;

            var testProjectAnalyzerResults = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "TargetDir", "/test/bin/Debug/" },
                        { "TargetFileName", "TestName.dll" }
                    }).Object;

            var expectedPath = FilePathUtils.NormalizePathSeparators("/test/bin/Debug/AppToTest.dll");

            var result = TestProjectsInfo.GetInjectionFilePath(testProjectAnalyzerResults, sourceProjectAnalyzerResults);

            result.ShouldBe(expectedPath);
        }

        [Fact]
        public void MergeTestProjectsInfo()
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
            testProjectA.TestFiles.First().AddTest(Guid.NewGuid(), "test1", SyntaxFactory.Block());
            testProjectA.TestFiles.First().AddTest(Guid.NewGuid(), "test2", SyntaxFactory.Block());
            testProjectB.TestFiles.First().AddTest(Guid.NewGuid(), "test3", SyntaxFactory.Block());

            var testProjectsInfoA = new TestProjectsInfo(fileSystem)
            {
                TestProjects = new List<TestProject> { testProjectA }
            };
            var testProjectsInfoB = new TestProjectsInfo(fileSystem)
            {
                TestProjects = new List<TestProject> { testProjectB }
            };
            var testProjectsInfoC = new TestProjectsInfo(fileSystem)
            {
                TestProjects = new List<TestProject> { testProjectB }
            };

            // Act
            var testProjectsInfoABC = testProjectsInfoA + testProjectsInfoB + testProjectsInfoC;

            // Assert
            testProjectsInfoABC.TestFiles.Count().ShouldBe(2);
            testProjectsInfoABC.TestFiles.ElementAt(0).Tests.Count().ShouldBe(2);
            testProjectsInfoABC.TestFiles.ElementAt(1).Tests.Count().ShouldBe(1);
        }
    }
}
