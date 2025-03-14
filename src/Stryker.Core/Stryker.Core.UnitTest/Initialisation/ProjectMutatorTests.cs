using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using Stryker.TestRunner.VsTest;
using TestCase = Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase;

namespace Stryker.Core.UnitTest.Initialisation;

[TestClass]
public class ProjectMutatorTests : TestBase
{
    private readonly Mock<IMutationTestProcess> _mutationTestProcessMock = new(MockBehavior.Strict);
    private readonly Mock<IReporter> _reporterMock = new(MockBehavior.Strict);
    private readonly Mock<IInitialisationProcess> _initialisationProcessMock = new(MockBehavior.Strict);
    private readonly MutationTestInput _mutationTestInput;
    private readonly IFileSystem _fileSystemMock = new MockFileSystem();
    private readonly string _testFilePath = "c:\\mytestfile.cs";
    private readonly string _testFileContents = @"using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtraProject.XUnit
{
    public class UnitTest1
    {
        [TestMethod]
        public void Test1()
        {
            // example test
        }

        [TestMethod]
        public void Test2()
        {
            // example test
        }
    }
}
";

    public ProjectMutatorTests()
    {
        _mutationTestProcessMock.Setup(x => x.Mutate());
        _fileSystemMock.File.WriteAllText(_testFilePath, _testFileContents);
        _mutationTestInput = new MutationTestInput()
        {
            TestProjectsInfo = new TestProjectsInfo(_fileSystemMock)
            {
                TestProjects = new List<TestProject>
                {
                    new(_fileSystemMock, TestHelper.SetupProjectAnalyzerResult(
                        sourceFiles: new [] { _testFilePath }).Object)
                }
            }
        };
    }

    [TestMethod]
    public void ShouldInitializeEachProjectInSolution()
    {
        // arrange
        var options = new StrykerOptions();
        var target = new ProjectMutator(_mutationTestProcessMock.Object);
        var testCase1 = new VsTestCase(new TestCase("mytestname1", new Uri(_testFilePath), _testFileContents)
        {
            Id = Guid.NewGuid(),
            CodeFilePath = _testFilePath,
            LineNumber = 7,
        });
        var failedTest = testCase1.Id;
        var testCase2 = new VsTestCase(new TestCase("mytestname2", new Uri(_testFilePath), _testFileContents)
        {
            Id = Guid.NewGuid(),
            CodeFilePath = _testFilePath,
            LineNumber = 13,
        });
        var successfulTest = testCase2.Id;
        var tests = new List<VsTestDescription> { new VsTestDescription(testCase1), new VsTestDescription(testCase2) };
        var initialTestRunResult = new TestRunResult(
            vsTestDescriptions: tests,
            executedTests: new TestIdentifierList(failedTest, successfulTest),
            failedTests: new TestIdentifierList(failedTest),
            timedOutTest: TestIdentifierList.NoTest(),
            message: "testrun succesful",
            Enumerable.Empty<string>(),
            timeSpan: TimeSpan.FromSeconds(2));

        var initialTestrun = new InitialTestRun(initialTestRunResult, new TimeoutValueCalculator(500));

        _mutationTestInput.InitialTestRun = initialTestrun;
        // act
        var result = target.MutateProject(options, _mutationTestInput, _reporterMock.Object);

        // assert
        result.ShouldNotBeNull();
        var testFile = _mutationTestInput.TestProjectsInfo.TestFiles.ShouldHaveSingleItem();
        testFile.Tests.Count.ShouldBe(2);
    }
}
