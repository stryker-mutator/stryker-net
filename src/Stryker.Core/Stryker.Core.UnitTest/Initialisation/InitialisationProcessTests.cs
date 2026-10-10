using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Testing;
using Stryker.Configuration.Options;
using Stryker.Core.Initialisation;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Solutions;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using Stryker.TestRunner.VsTest;
using Stryker.Utilities.Buildalyzer;
using TestRunnerOption = Stryker.Abstractions.Options.TestRunner;

namespace Stryker.Core.UnitTest.Initialisation;

[TestClass]
public class InitialisationProcessTests : TestBase
{
    private const string NoMatchingTestsMessage =
        "No test cases matched `test-case-filter`. Skipping this project. Change your configuration and try again.";
    private const string NoMatchingProjectsMessage =
        "No projects have test cases matching `test-case-filter`. Change your configuration and try again.";

    [TestMethod]
    public async Task InitialisationProcess_ShouldNotReportFilteringWhenNoProjectsWereFound()
    {
        var options = new StrykerOptions { TestCaseFilter = "FullyQualifiedName~MatchingTest" };
        var target = new InitialisationProcess(Mock.Of<IInputFileResolver>(), Mock.Of<IInitialBuildProcess>(),
            new Mock<IInitialTestProcess>(MockBehavior.Strict).Object, Mock.Of<ILogger<InitialisationProcess>>());

        var inputs = await target.GetMutationTestInputsAsync(options, new RelatedSourceProjectsInfo(null, []),
            new Mock<ITestRunner>(MockBehavior.Strict).Object);

        inputs.ShouldBeEmpty();
    }

    [TestMethod]
    [DataRow(TestRunnerOption.VsTest, false)]
    [DataRow(TestRunnerOption.VsTest, true)]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, false)]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, true)]
    public async Task InitialisationProcess_ShouldExplainWhenAllTestsAreFilteredOut(TestRunnerOption runner, bool testsDiscovered)
    {
        var options = new StrykerOptions { TestRunner = runner, TestCaseFilter = "FullyQualifiedName~NoMatchingTest" };
        var project = CreateTestProject("Project.csproj");
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).ReturnsAsync(testsDiscovered);
        var tests = new TestSet();
        if (testsDiscovered)
        {
            tests.RegisterTest(new TestDescription("id", "test", "test.cs"));
        }
        testRunnerMock.Setup(x => x.GetTests(project)).Returns(tests);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
        if (testsDiscovered)
        {
            initialTestProcessMock.Setup(x => x.InitialTestAsync(options, project, testRunnerMock.Object))
                .ReturnsAsync(new InitialTestRun(EmptyTestRunResult(), null));
        }
        var loggerMock = new Mock<ILogger<InitialisationProcess>>();
        var target = new InitialisationProcess(Mock.Of<IInputFileResolver>(), Mock.Of<IInitialBuildProcess>(),
            initialTestProcessMock.Object, loggerMock.Object);

        var exception = await Should.ThrowAsync<InputException>(() =>
            target.GetMutationTestInputsAsync(options, new RelatedSourceProjectsInfo(null, [project]), testRunnerMock.Object));

        exception.Message.ShouldBe(NoMatchingProjectsMessage);
        project.Warnings.ShouldBeEmpty();
        initialTestProcessMock.Verify(x => x.InitialTestAsync(options, project, testRunnerMock.Object),
            testsDiscovered ? Times.Once() : Times.Never());
        loggerMock.Invocations.ShouldContain(invocation => invocation.Method.Name == nameof(ILogger.Log) &&
            invocation.Arguments[0].Equals(LogLevel.Warning) &&
            invocation.Arguments[2].ToString() == $"{NoMatchingTestsMessage} Project: Project.csproj");
    }

    [TestMethod]
    [DataRow(TestRunnerOption.VsTest, false)]
    [DataRow(TestRunnerOption.VsTest, true)]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, false)]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, true)]
    public async Task InitialisationProcess_ShouldSkipOnlyFilteredProjects(TestRunnerOption runner, bool allProjectsFiltered)
    {
        var options = new StrykerOptions { TestRunner = runner, TestCaseFilter = "FullyQualifiedName~MatchingTest" };
        var filteredProject = CreateTestProject("Filtered.csproj");
        var otherProject = CreateTestProject("Other.csproj");
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).ReturnsAsync(false);
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(otherProject.GetTestAssemblies().Single())).ReturnsAsync(!allProjectsFiltered);
        var tests = new TestSet();
        if (!allProjectsFiltered)
        {
            tests.RegisterTest(new TestDescription("id", "MatchingTest", "test.cs"));
        }
        testRunnerMock.Setup(x => x.GetTests(otherProject)).Returns(tests);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
        var initialTestRun = new InitialTestRun(new TestRunResult(true), new TimeoutValueCalculator(0));
        if (!allProjectsFiltered)
        {
            initialTestProcessMock.Setup(x => x.InitialTestAsync(options, otherProject, testRunnerMock.Object))
                .ReturnsAsync(initialTestRun);
        }
        var target = new InitialisationProcess(Mock.Of<IInputFileResolver>(), Mock.Of<IInitialBuildProcess>(),
            initialTestProcessMock.Object, Mock.Of<ILogger<InitialisationProcess>>());
        var projects = new RelatedSourceProjectsInfo(null, [filteredProject, otherProject]);

        if (allProjectsFiltered)
        {
            var exception = await Should.ThrowAsync<InputException>(() =>
                target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object));
            exception.Message.ShouldBe(NoMatchingProjectsMessage);
        }
        else
        {
            var inputs = await target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object);
            inputs.Count.ShouldBe(1);
            inputs.Single().SourceProjectInfo.ShouldBeSameAs(otherProject);
            inputs.Single().InitialTestRun.ShouldBeSameAs(initialTestRun);
        }

        initialTestProcessMock.Verify(x => x.InitialTestAsync(options, filteredProject, testRunnerMock.Object), Times.Never);
        filteredProject.Warnings.ShouldBeEmpty();
        otherProject.Warnings.ShouldBeEmpty();
    }

    [TestMethod]
    [DataRow(TestRunnerOption.VsTest, null)]
    [DataRow(TestRunnerOption.VsTest, "")]
    [DataRow(TestRunnerOption.VsTest, "  ")]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, null)]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, "")]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, "  ")]
    public async Task InitialisationProcess_ShouldReportSelectedRunnerWhenNoFilterIsApplied(TestRunnerOption runner, string filter)
    {
        var options = new StrykerOptions { TestRunner = runner, TestCaseFilter = filter };
        var project = CreateTestProject("Project.csproj");
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).ReturnsAsync(false);
        testRunnerMock.Setup(x => x.GetTests(project)).Returns(new TestSet());
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
        initialTestProcessMock.Setup(x => x.InitialTestAsync(options, project, testRunnerMock.Object))
            .ReturnsAsync(new InitialTestRun(EmptyTestRunResult(), null));
        var target = new InitialisationProcess(Mock.Of<IInputFileResolver>(), Mock.Of<IInitialBuildProcess>(),
            initialTestProcessMock.Object, Mock.Of<ILogger<InitialisationProcess>>());

        var exception = await Should.ThrowAsync<InputException>(() =>
            target.GetMutationTestInputsAsync(options, new RelatedSourceProjectsInfo(null, [project]), testRunnerMock.Object));

        var runnerName = runner == TestRunnerOption.VsTest ? "VsTest" : "Microsoft Testing Platform";
        exception.Message.ShouldStartWith($"No test result reported. Make sure your test project contains tests and is compatible with {runnerName}.");
        exception.Message.ShouldNotContain("not yet supported");
        exception.Message.ShouldNotContain("filtered out");
        if (runner == TestRunnerOption.MicrosoftTestPlatform)
        {
            exception.Message.ShouldNotContain("VsTest");
        }
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(true, false)]
    [DataRow(false, true)]
    [DataRow(true, true)]
    public async Task InitialisationProcess_ShouldNotTreatTestSessionFailuresAsFilteredTests(bool timedOut, bool multipleProjects)
    {
        var options = new StrykerOptions { TestCaseFilter = "FullyQualifiedName~MatchingTest" };
        var project = CreateTestProject("Project.csproj");
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).ReturnsAsync(true);
        var tests = new TestSet();
        tests.RegisterTest(new TestDescription("id", "MatchingTest", "test.cs"));
        testRunnerMock.Setup(x => x.GetTests(project)).Returns(tests);
        var result = timedOut
            ? TestRunResult.TimedOut([], TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), "timeout", [], TimeSpan.Zero)
            : TestRunResult.RuntimeError([], TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), "crash", [], TimeSpan.Zero);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
        initialTestProcessMock.Setup(x => x.InitialTestAsync(options, project, testRunnerMock.Object))
            .ReturnsAsync(new InitialTestRun(result, null));
        var target = new InitialisationProcess(Mock.Of<IInputFileResolver>(), Mock.Of<IInitialBuildProcess>(),
            initialTestProcessMock.Object, Mock.Of<ILogger<InitialisationProcess>>());

        var projects = multipleProjects ? new[] { project, project } : [project];
        var exception = await Should.ThrowAsync<InputException>(() =>
            target.GetMutationTestInputsAsync(options, new RelatedSourceProjectsInfo(null, projects), testRunnerMock.Object));

        exception.Message.ShouldBe("Initial test run could not be completed.");
        exception.Details.ShouldBe(timedOut ? "timeout" : "crash");
        exception.Message.ShouldNotContain("matched");
    }

    [TestMethod]
    [DataRow(TestRunnerOption.VsTest)]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform)]
    public async Task InitialisationProcess_ShouldUseAssemblyDiscoveryInsteadOfSharedTestCount(TestRunnerOption runner)
    {
        var options = new StrykerOptions { TestRunner = runner, TestCaseFilter = "FullyQualifiedName~MatchingTest" };
        var project = CreateTestProject("Filtered.csproj");
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).ReturnsAsync(false);
        var sharedTests = new TestSet();
        sharedTests.RegisterTest(new TestDescription("other-project-test", "MatchingTest", "other.cs"));
        testRunnerMock.Setup(x => x.GetTests(project)).Returns(sharedTests);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
        var target = new InitialisationProcess(Mock.Of<IInputFileResolver>(), Mock.Of<IInitialBuildProcess>(),
            initialTestProcessMock.Object, Mock.Of<ILogger<InitialisationProcess>>());

        var exception = await Should.ThrowAsync<InputException>(() =>
            target.GetMutationTestInputsAsync(options, new RelatedSourceProjectsInfo(null, [project]), testRunnerMock.Object));

        exception.Message.ShouldBe(NoMatchingProjectsMessage);
        initialTestProcessMock.VerifyNoOtherCalls();
        testRunnerMock.Verify(x => x.GetTests(project), Times.Never);
    }

    [TestMethod]
    [DataRow(TestRunnerOption.VsTest, false, false)]
    [DataRow(TestRunnerOption.VsTest, false, true)]
    [DataRow(TestRunnerOption.VsTest, true, false)]
    [DataRow(TestRunnerOption.VsTest, true, true)]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, false, false)]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, false, true)]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, true, false)]
    [DataRow(TestRunnerOption.MicrosoftTestPlatform, true, true)]
    public async Task InitialisationProcess_ShouldDiscoverEveryAssemblyBeforeSkipping(TestRunnerOption runner, bool discoveryFails, bool firstHasTests)
    {
        var options = new StrykerOptions { TestRunner = runner, TestCaseFilter = "FullyQualifiedName~MatchingTest" };
        var project = CreateTestProject("Project.csproj");
        var otherProject = CreateTestProject("Other.csproj");
        var firstAssembly = project.GetTestAssemblies().Single();
        var secondAssembly = otherProject.GetTestAssemblies().Single();
        project.TestProjectsInfo.TestProjects = project.TestProjectsInfo.TestProjects.Concat(otherProject.TestProjectsInfo.TestProjects).ToList();
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(firstAssembly)).ReturnsAsync(firstHasTests);
        var discoveryError = new InputException("Discovery failed.", "The test adapter could not load.");
        if (discoveryFails)
        {
            testRunnerMock.Setup(x => x.DiscoverTestsAsync(secondAssembly)).ThrowsAsync(discoveryError);
        }
        else
        {
            testRunnerMock.Setup(x => x.DiscoverTestsAsync(secondAssembly)).ReturnsAsync(!firstHasTests);
        }
        var tests = new TestSet();
        tests.RegisterTest(new TestDescription("id", "MatchingTest", "test.cs"));
        testRunnerMock.Setup(x => x.GetTests(project)).Returns(tests);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
        initialTestProcessMock.Setup(x => x.InitialTestAsync(options, project, testRunnerMock.Object))
            .ReturnsAsync(new InitialTestRun(new TestRunResult(true), null));
        var loggerMock = new Mock<ILogger<InitialisationProcess>>();
        var target = new InitialisationProcess(Mock.Of<IInputFileResolver>(), Mock.Of<IInitialBuildProcess>(),
            initialTestProcessMock.Object, loggerMock.Object);
        var projects = new RelatedSourceProjectsInfo(null, [project]);

        if (discoveryFails)
        {
            var exception = await Should.ThrowAsync<InputException>(() =>
                target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object));
            exception.ShouldBeSameAs(discoveryError);
            initialTestProcessMock.Verify(x => x.InitialTestAsync(options, project, testRunnerMock.Object), Times.Never);
            loggerMock.Invocations.ShouldNotContain(invocation => invocation.Method.Name == nameof(ILogger.Log) &&
                invocation.Arguments[2].ToString().Contains("Skipping this project"));
        }
        else
        {
            var inputs = await target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object);
            inputs.Count.ShouldBe(1);
        }
        testRunnerMock.Verify(x => x.DiscoverTestsAsync(firstAssembly), Times.Once);
        testRunnerMock.Verify(x => x.DiscoverTestsAsync(secondAssembly), Times.Once);
    }

    [TestMethod]
    public async Task InitialisationProcess_ShouldPropagateDiscoveryCancellation()
    {
        var options = new StrykerOptions { TestCaseFilter = "FullyQualifiedName~MatchingTest" };
        var project = CreateTestProject("Project.csproj");
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).ThrowsAsync(new OperationCanceledException());
        var target = new InitialisationProcess(Mock.Of<IInputFileResolver>(), Mock.Of<IInitialBuildProcess>(),
            new Mock<IInitialTestProcess>(MockBehavior.Strict).Object, Mock.Of<ILogger<InitialisationProcess>>());

        await Should.ThrowAsync<OperationCanceledException>(() =>
            target.GetMutationTestInputsAsync(options, new RelatedSourceProjectsInfo(null, [project]), testRunnerMock.Object));
    }

    private static SourceProjectInfo CreateTestProject(string projectFilePath)
    {
        var fileSystem = new MockFileSystem();
        return new SourceProjectInfo(TestHelper.SetupProjectAnalyzerResult(projectFilePath: projectFilePath, references: []).Object,
            new TestProjectsInfo(fileSystem)
            {
                TestProjects = [new TestProject(fileSystem, TestHelper.SetupProjectAnalyzerResult(
                    projectFilePath: $"Tests.{projectFilePath}",
                    references: ["xunit.core", "xunit.runner.visualstudio", "Microsoft.Testing.Platform"]).Object)]
            });
    }

    private static TestRunResult EmptyTestRunResult() =>
        new([], TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), string.Empty, [], TimeSpan.Zero);

    [TestMethod]
    public void InitialisationProcess_ShouldCallNeededResolvers()
    {
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);

        var projectContents = new FolderComposite();
        projectContents.Add(new CsharpFileLeaf());
        var folder = new FolderComposite();
        folder.AddRange(new Mono.Collections.Generic.Collection<IProjectComponent>
        {
            new CsharpFileLeaf()
        });
        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>()))
            .Returns(new RelatedSourceProjectsInfo(null, [
                new SourceProjectInfo(TestHelper.SetupProjectAnalyzerResult(references: []).Object, null)
            {
                ProjectContents = folder
            }
            ]));

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(new FileSystem());
        var loggerMock = new Mock<ILogger<InitialisationProcess>>();
        var target = new InitialisationProcess(inputFileResolverMock.Object, Mock.Of<IInitialBuildProcess>(), Mock.Of<IInitialTestProcess>(), loggerMock.Object);

        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion"
        };

        var result = target.GetMutableProjectsInfo(options).SourceProjectInfos.ToList();
        result.Count.ShouldBe(1);
        inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>()), Times.Once);
    }

    [TestMethod]
    public async Task InitialisationProcess_ShouldThrowOnFailedInitialTestRun()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new FolderComposite();
        folder.Add(new CsharpFileLeaf());

        var loggerMock = new Mock<ILogger<InitialisationProcess>>();
        var target = new InitialisationProcess(inputFileResolverMock.Object, initialBuildProcessMock.Object, initialTestProcessMock.Object, loggerMock.Object);

        var mockFileSystem = new MockFileSystem();

        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion",
            WorkingDirectory = "./"
        };

        var projectTracker = new ProjectsTracker(SolutionFile.BuildFromProjectList("solution.sln", []), options,
            new Mock<IBuildalyzerProvider>(MockBehavior.Strict).Object,
            new Mock<INugetRestoreProcess>().Object,
            mockFileSystem, loggerMock.Object);
        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).
            Returns(new RelatedSourceProjectsInfo(projectTracker,
        [new SourceProjectInfo( TestHelper.SetupProjectAnalyzerResult(references: []).Object,
            new TestProjectsInfo(mockFileSystem))
        ]));

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(new FileSystem());
        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<string>(), null, It.IsAny<string>()));
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
        initialTestProcessMock.Setup(x => x.InitialTestAsync(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>())).ThrowsAsync(new InputException("")); // failing test

        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        await Should.ThrowAsync<InputException>(async () => await target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object));
        initialTestProcessMock.Verify(x => x.InitialTestAsync(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), testRunnerMock.Object), Times.Once);
    }

    [TestMethod]
    public async Task InitialisationProcess_ShouldThrowIfHalfTestsAreFailing()
    {
        var fileSystemMock = new MockFileSystem();
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new FolderComposite();
        folder.Add(new CsharpFileLeaf());
        var loggerMock = new Mock<ILogger<InitialisationProcess>>();
        var target = new InitialisationProcess(inputFileResolverMock.Object, initialBuildProcessMock.Object, initialTestProcessMock.Object, loggerMock.Object);
        var mockFileSystem = new MockFileSystem();

        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion",
            WorkingDirectory = "./"
        };

        var projectTracker = new ProjectsTracker(SolutionFile.BuildFromProjectList("solution.sln", []), options,
            new Mock<IBuildalyzerProvider>(MockBehavior.Strict).Object,
            new Mock<INugetRestoreProcess>().Object,
            mockFileSystem, loggerMock.Object);
        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(new RelatedSourceProjectsInfo(projectTracker,
        [new SourceProjectInfo(TestHelper.SetupProjectAnalyzerResult(references: []).Object,  new TestProjectsInfo(new MockFileSystem()))]));

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(fileSystemMock);
        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
            null,It.IsAny<string>()));
        var failedTest = "testid";
        var ranTests = new TestIdentifierList(failedTest, "othertest");
        var testSet = new TestSet();
        foreach (var ranTest in ranTests.GetIdentifiers())
        {
            testSet.RegisterTest(new TestDescription(ranTest, "test", "test.cpp"));
        }
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(testSet);
        var failedTests = new TestIdentifierList(failedTest);
        initialTestProcessMock.Setup(x => x.InitialTestAsync(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>())).ReturnsAsync(
            new InitialTestRun(
            new TestRunResult(Array.Empty<VsTestDescription>(), ranTests, failedTests, TestIdentifierList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero), new TimeoutValueCalculator(0))); // failing test

        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        await Should.ThrowAsync<InputException>(async () => await target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object));
        inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>()), Times.Once);
        initialTestProcessMock.Verify(x => x.InitialTestAsync(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), testRunnerMock.Object), Times.Once);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task InitialisationProcess_ShouldThrowOnTestTestIfAskedFor(bool breakOnInitialTestFailure)
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new FolderComposite();
        folder.Add(new CsharpFileLeaf());

        var loggerMock = new Mock<ILogger<InitialisationProcess>>();
        var target = new InitialisationProcess(inputFileResolverMock.Object, initialBuildProcessMock.Object,
            initialTestProcessMock.Object, loggerMock.Object);

        var mockFileSystem = new MockFileSystem();

        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion",
            BreakOnInitialTestFailure = breakOnInitialTestFailure,
            WorkingDirectory = "./"
        };

        var projectTracker = new ProjectsTracker(SolutionFile.BuildFromProjectList("solution.sln", []), options,
            new Mock<IBuildalyzerProvider>(MockBehavior.Strict).Object,
            new Mock<INugetRestoreProcess>().Object,
            mockFileSystem, loggerMock.Object);
        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(new RelatedSourceProjectsInfo(projectTracker,
        [new SourceProjectInfo(TestHelper.SetupProjectAnalyzerResult(references: []).Object, new TestProjectsInfo(new MockFileSystem()))]));

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(new FileSystem());
        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>()));
        const string failedTest = "testid";
        var ranTests = new TestIdentifierList(failedTest, "othertest", "anothertest");
        var testSet = new TestSet();
        foreach (var ranTest in ranTests.GetIdentifiers())
        {
            testSet.RegisterTest(new TestDescription(ranTest, "test", "test.cpp"));
        }
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(testSet);
        var failedTests = new TestIdentifierList(failedTest);
        initialTestProcessMock.Setup(x => x.InitialTestAsync(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>())).ReturnsAsync(new InitialTestRun(
            new TestRunResult(Array.Empty<VsTestDescription>(), ranTests, failedTests, TestIdentifierList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero), new TimeoutValueCalculator(0))); // failing test

        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        if (breakOnInitialTestFailure)
        {
            await Should.ThrowAsync<InputException>(async () => await target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object));
        }
        else
        {
            var testInputs = await target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object);

            testInputs.ShouldNotBeEmpty();
        }
    }

    [TestMethod]
    public async Task InitialisationProcess_ShouldRunTestSession()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new FolderComposite();
        folder.Add(new CsharpFileLeaf());

        var loggerMock = new Mock<ILogger<InitialisationProcess>>();
        var target = new InitialisationProcess(inputFileResolverMock.Object, initialBuildProcessMock.Object, initialTestProcessMock.Object, loggerMock.Object);
        var mockFileSystem = new MockFileSystem();

        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion",
            WorkingDirectory = "./"
        };

        var projectTracker = new ProjectsTracker(SolutionFile.BuildFromProjectList("solution.sln", []), options,
            new Mock<IBuildalyzerProvider>(MockBehavior.Strict).Object,
            new Mock<INugetRestoreProcess>().Object,
            mockFileSystem, loggerMock.Object);
        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).
            Returns(new RelatedSourceProjectsInfo(projectTracker,
            [new SourceProjectInfo(TestHelper.SetupProjectAnalyzerResult(references: []).Object,
                new TestProjectsInfo(new MockFileSystem()))]));

        var fileSystem = new MockFileSystem();
        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(fileSystem);
        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>()));
        var testSet = new TestSet();
        testSet.RegisterTest(new TestDescription("id", "name", "test.cs"));
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).Returns(Task.FromResult(true));
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(testSet);
        initialTestProcessMock.Setup(x => x.InitialTestAsync(It.IsAny<StrykerOptions>(),
                It.IsAny<IProjectAndTests>(),
                It.IsAny<ITestRunner>()))
            .Returns(Task.FromResult(new InitialTestRun(new TestRunResult(true), null))); // failing test

        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        await target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object);

        inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>()), Times.Once);
        initialTestProcessMock.Verify(x => x.InitialTestAsync(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), testRunnerMock.Object), Times.Once);
    }


    [TestMethod]
    [DataRow("xunit.core")]
    [DataRow("nunit.framework")]
    [DataRow("Microsoft.VisualStudio.TestPlatform.TestFramework")]
    [DataRow("")]
    public async Task InitialisationProcess_ShouldThrowOnWhenNoTestDetected(string libraryName)
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new FolderComposite();
        folder.Add(new CsharpFileLeaf());

        var testProjectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
            projectFilePath: "C://Example/Dir/ProjectFolder",
            targetFramework: "netcoreapp2.1",
            references: [libraryName]).Object;

        inputFileResolverMock.SetupGet(x => x.FileSystem).Returns(new FileSystem());

        var loggerMock = new Mock<ILogger<InitialisationProcess>>();
        var target = new InitialisationProcess(inputFileResolverMock.Object, initialBuildProcessMock.Object, initialTestProcessMock.Object, loggerMock.Object);
        var mockFileSystem = new MockFileSystem();

        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion",
            WorkingDirectory = "./"
        };

        var projectTracker = new ProjectsTracker(SolutionFile.BuildFromProjectList("solution.sln", []), options,
            new Mock<IBuildalyzerProvider>(MockBehavior.Strict).Object,
            new Mock<INugetRestoreProcess>().Object,
            mockFileSystem, loggerMock.Object);
        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(new RelatedSourceProjectsInfo(projectTracker,
        [
            new SourceProjectInfo(
                TestHelper.SetupProjectAnalyzerResult(references: []).Object,
                new TestProjectsInfo(new MockFileSystem()) {
                    TestProjects = new List<TestProject> {new(new MockFileSystem(), testProjectAnalyzerResult)} }
                )
        ]));

        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>()));
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).Returns(Task.FromResult(false));
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
        initialTestProcessMock.Setup(x => x.InitialTestAsync(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>()))
            .Returns(Task.FromResult(new InitialTestRun(new TestRunResult(Array.Empty<VsTestDescription>(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero), null))); // failing test

        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        var exception = await Should.ThrowAsync<InputException>(async () => await target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object));
        exception.Message.ShouldContain(libraryName);
    }

    [TestMethod]
    public void InitialisationProcess_ShouldThrowOnWhenNoTestDetectedAndCorrectDependencies()
    {
        var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
        var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
        var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
        var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

        var folder = new FolderComposite();
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

        var loggerMock = new Mock<ILogger<InitialisationProcess>>(); var target = new InitialisationProcess(inputFileResolverMock.Object, initialBuildProcessMock.Object, initialTestProcessMock.Object, loggerMock.Object);

        var mockFileSystem = new MockFileSystem();

        var options = new StrykerOptions
        {
            ProjectName = "TheProjectName",
            ProjectVersion = "TheProjectVersion",
            WorkingDirectory = "./"
        };

        var projectTracker = new ProjectsTracker(SolutionFile.BuildFromProjectList("solution.sln", []), options,
            new Mock<IBuildalyzerProvider>(MockBehavior.Strict).Object,
            new Mock<INugetRestoreProcess>().Object,
            mockFileSystem, loggerMock.Object);
        inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).
            Returns(new RelatedSourceProjectsInfo(projectTracker,
        [new SourceProjectInfo(TestHelper.SetupProjectAnalyzerResult(
                references: []).Object
            , new TestProjectsInfo(new MockFileSystem()){TestProjects = new List<TestProject> {new(new MockFileSystem(), testProjectAnalyzerResult)}})]));

        initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<string>()));
        testRunnerMock.Setup(x => x.DiscoverTestsAsync(It.IsAny<string>())).Returns(Task.FromResult(false));
        testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(new TestSet());
        initialTestProcessMock.Setup(x => x.InitialTestAsync(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), It.IsAny<ITestRunner>()))
            .Returns(Task.FromResult(new InitialTestRun(new TestRunResult(Array.Empty<VsTestDescription>(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero), null))); // failing test

        var projects = target.GetMutableProjectsInfo(options);
        target.BuildProjects(options, projects);
        Should.Throw<InputException>(async () => await target.GetMutationTestInputsAsync(options, projects, testRunnerMock.Object)).Message.ShouldContain("failed to deploy or run.");
    }
}
