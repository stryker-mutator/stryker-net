using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Testing;
using Stryker.Core.Initialisation;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using Stryker.TestRunner.VsTest;

namespace Stryker.Core.UnitTest.Initialisation;

[TestClass]
public class InitialisationProcessTests : TestBase
{
    [TestMethod]
    public void InitialisationProcess_ShouldCallNeededResolvers()
    {
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);

        var projectContents = new CsharpFolderComposite();
        projectContents.Add(new CsharpFileLeaf());
        var folder = new CsharpFolderComposite();
        folder.AddRange(new Mono.Collections.Generic.Collection<IProjectComponent>
        {
            new CsharpFileLeaf()
        });
        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>()))
            .Returns(new[] {new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(references: Array.Empty<string>()).Object,
                ProjectContents = folder
            }
        });

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(new FileSystem());
        var target = new InitialisationProcess(inputFileResolverMock.Object);

        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion"
        };

        var result = target.GetMutableProjectsInfo(options).ToList();
        result.Count.ShouldBe(1);
        inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>()), Times.Once);
    }

    [TestMethod]
    public void InitialisationProcess_ShouldThrowOnFailedInitialTestRun()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new CsharpFolderComposite();
        folder.Add(new CsharpFileLeaf());

        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(
            new[] {new SourceProjectInfo() {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    references: Array.Empty<string>()).Object,
                TestProjectsInfo = new TestProjectsInfo(new MockFileSystem())
            }});

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(new FileSystem());
        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>()));
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
        testRunnerMock.Setup(x => x.DiscoverTests(It.IsAny<string>())).Returns(true);
        initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>())).Throws(new InputException("")); // failing test

        var target = new InitialisationProcess(inputFileResolverMock.Object,
            initialBuildProcessMock.Object,
            initialTestProcessMock.Object);
        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion"
        };

        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        Should.Throw<InputException>(() => target.GetMutationTestInputs(options, projects, testRunnerMock.Object));
        initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), testRunnerMock.Object), Times.Once);
    }

    [TestMethod]
    public void InitialisationProcess_ShouldThrowIfHalfTestsAreFailing()
    {
        var fileSystemMock = new MockFileSystem();
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new CsharpFolderComposite();
        folder.Add(new CsharpFileLeaf());


        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(
            new[] { new SourceProjectInfo { AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(references: Array.Empty<string>()).Object, TestProjectsInfo = new TestProjectsInfo(new MockFileSystem()) } });

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(fileSystemMock);
        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>()));
        var failedTest = "testid";
        var ranTests = new TestIdentifierList(failedTest, "othertest");
        var testSet = new TestSet();
        foreach (var ranTest in ranTests.GetIdentifiers())
        {
            testSet.RegisterTest(new TestDescription(ranTest, "test", "test.cpp"));
        }
        testRunnerMock.Setup(x => x.DiscoverTests(It.IsAny<string>())).Returns(true);
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(testSet);
        var failedTests = new TestIdentifierList(failedTest);
        initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>())).Returns(
            new InitialTestRun(
            new TestRunResult(Array.Empty<VsTestDescription>(), ranTests, failedTests, TestIdentifierList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero), new TimeoutValueCalculator(0))); // failing test

        var target = new InitialisationProcess(inputFileResolverMock.Object,
            initialBuildProcessMock.Object,
            initialTestProcessMock.Object);
        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion"
        };
        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        Should.Throw<InputException>(() => target.GetMutationTestInputs(options, projects, testRunnerMock.Object));
        inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>()), Times.Once);
        initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), testRunnerMock.Object), Times.Once);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void InitialisationProcess_ShouldThrowOnTestTestIfAskedFor(bool breakOnInitialTestFailure)
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new CsharpFolderComposite();
        folder.Add(new CsharpFileLeaf());


        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(
            new[] {new SourceProjectInfo {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    references: Array.Empty<string>()).Object,
                TestProjectsInfo = new TestProjectsInfo(new MockFileSystem())
            }});

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(new FileSystem());
        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>()));
        var failedTest = "testid";
        var ranTests = new TestIdentifierList(failedTest, "othertest", "anothertest");
        var testSet = new TestSet();
        foreach (var ranTest in ranTests.GetIdentifiers())
        {
            testSet.RegisterTest(new TestDescription(ranTest, "test", "test.cpp"));
        }
        testRunnerMock.Setup(x => x.DiscoverTests(It.IsAny<string>())).Returns(true);
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(testSet);
        var failedTests = new TestIdentifierList(failedTest);
        initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>())).Returns(new InitialTestRun(
            new TestRunResult(Array.Empty<VsTestDescription>(), ranTests, failedTests, TestIdentifierList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero), new TimeoutValueCalculator(0))); // failing test

        var target = new InitialisationProcess(inputFileResolverMock.Object,
            initialBuildProcessMock.Object,
            initialTestProcessMock.Object);
        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion",
            BreakOnInitialTestFailure = breakOnInitialTestFailure
        };
        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        if (breakOnInitialTestFailure)
        {
            Should.Throw<InputException>(() => target.GetMutationTestInputs(options, projects, testRunnerMock.Object));
        }
        else
        {
            var testInputs = target.GetMutationTestInputs(options, projects, testRunnerMock.Object);

            testInputs.ShouldNotBeEmpty();
        }
    }


    [TestMethod]
    public void InitialisationProcess_ShouldRunTestSession()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new CsharpFolderComposite();
        folder.Add(new CsharpFileLeaf());

        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(
            new[] { new SourceProjectInfo() { AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(references: Array.Empty<string>()).Object, TestProjectsInfo = new TestProjectsInfo(new MockFileSystem()) } });


        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(new FileSystem());
        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>()));
        var testSet = new TestSet();
        testSet.RegisterTest(new TestDescription("id", "name", "test.cs"));
        testRunnerMock.Setup(x => x.DiscoverTests(It.IsAny<string>())).Returns(true);
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(testSet);
        initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>()))
            .Returns(new InitialTestRun(new TestRunResult(true), null)); // failing test

        var target = new InitialisationProcess(inputFileResolverMock.Object,
            initialBuildProcessMock.Object,
            initialTestProcessMock.Object);
        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion"
        };

        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        var input = target.GetMutationTestInputs(options, projects, testRunnerMock.Object).First();

        inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>()), Times.Once);
        initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), testRunnerMock.Object), Times.Once);
    }


    [TestMethod]
    [DataRow("xunit.core")]
    [DataRow("nunit.framework")]
    [DataRow("Microsoft.VisualStudio.TestPlatform.TestFramework")]
    [DataRow("")]
    public void InitialisationProcess_ShouldThrowOnWhenNoTestDetected(string libraryName)
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new CsharpFolderComposite();
        folder.Add(new CsharpFileLeaf());


        var testProjectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            projectFilePath: "C://Example/Dir/ProjectFolder",
            targetFramework: "netcoreapp2.1",
            references: [libraryName]).Object;

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(new FileSystem());

        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(
            new[] {new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    references: []).Object,
                TestProjectsInfo = new TestProjectsInfo(new MockFileSystem()){TestProjects = new List<TestProject> {new(new MockFileSystem(), testProjectAnalyzerResult)}}
            }});

        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>()));
        testRunnerMock.Setup(x => x.DiscoverTests(It.IsAny<string>())).Returns(false);
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
        initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>()))
            .Returns(new InitialTestRun(new TestRunResult(Array.Empty<VsTestDescription>(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero), null)); // failing test

        var target = new InitialisationProcess(inputFileResolverMock.Object,
            initialBuildProcessMock.Object,
            initialTestProcessMock.Object);
        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion"
        };
        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        Should.Throw<InputException>(() => target.GetMutationTestInputs(options, projects, testRunnerMock.Object)).Message.ShouldContain(libraryName);
    }

    [TestMethod]
    public void InitialisationProcess_ShouldThrowOnWhenNoTestDetectedAndCorrectDependencies()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new CsharpFolderComposite();
        folder.Add(new CsharpFileLeaf());


        var testProjectAnalyzerResultMock = TestHelper.SetupProjectAnalyzerResult(
            projectFilePath: "C://Example/Dir/ProjectFolder",
            targetFramework: "netcoreapp2.1",
            references: ["xunit.core", "nunit.framework", "NUnit3.TestAdapter"]);

        testProjectAnalyzerResultMock.Setup(x => x.PackageReferences).
            Returns(new ReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>(new Dictionary<string, IReadOnlyDictionary<string, string>>
            { ["xunit.core"] = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()), ["xunit.runner.visualstudio"] = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()) }));
        var testProjectAnalyzerResult = testProjectAnalyzerResultMock.Object;

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(new FileSystem());

        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(
            new[] {new SourceProjectInfo
            {
                AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    references: []).Object,
                TestProjectsInfo = new TestProjectsInfo(new MockFileSystem()){TestProjects = new List<TestProject> {new(new MockFileSystem(), testProjectAnalyzerResult)}}
            }});

        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>()));
        testRunnerMock.Setup(x => x.DiscoverTests(It.IsAny<string>())).Returns(false);
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
        initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>()))
            .Returns(new InitialTestRun(new TestRunResult(Array.Empty<VsTestDescription>(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero), null)); // failing test

        var target = new InitialisationProcess(inputFileResolverMock.Object,
            initialBuildProcessMock.Object,
            initialTestProcessMock.Object);
        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion"
        };
        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        Should.Throw<InputException>(() => target.GetMutationTestInputs(options, projects, testRunnerMock.Object)).Message.ShouldContain("failed to deploy or run.");
    }
}
