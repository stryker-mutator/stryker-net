using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reflection;
using Buildalyzer;
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
            TestProjectsInfo.GetInjectionFilePath(sourceProjectAnalyzerResults, testProjectAnalyzerResults).ShouldBe(expectedPath);
        }

        [Fact]
        public void MergeTestProjectsInfo()
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
            testProjectsInfoABC.TestFiles.First().Tests.Count().ShouldBe(2);
            testProjectsInfoABC.TestFiles.ElementAt(1).Tests.Count().ShouldBe(1);
        }
    }
}
