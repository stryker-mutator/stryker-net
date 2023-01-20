using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Buildalyzer;
using Mono.Collections.Generic;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.TestRunners;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialisationProcessTests : TestBase
    {
        [Fact]
        public void InitialisationProcess_ShouldCallNeededResolvers()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

            testRunnerMock.Setup(x => x.Dispose());
            var projectContents = new CsharpFolderComposite();
            projectContents.Add(new CsharpFileLeaf());
            var folder = new CsharpFolderComposite();
            folder.AddRange(new Collection<IProjectComponent>
                {
                    new CsharpFileLeaf()
                });
            inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfo(It.IsAny<StrykerOptions>(), It.IsAny<TestProjectsInfo>(), It.IsAny<IEnumerable<IAnalyzerResult>>()))
                .Returns(new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(references: new string[0]).Object,
                    ProjectContents = folder
                });
            inputFileResolverMock.Setup(x => x.ResolveTestProjectsInfo(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>()))
                .Returns(new TestProjectsInfo(new MockFileSystem())
                {
                    TestProjects = new List<TestProject>()
                    {
                        // todo add files to mockfilesystem
                        new TestProject(new MockFileSystem(), TestHelper.SetupProjectAnalyzerResult(
                                    projectFilePath: "C://Example/Dir/ProjectFolder",
                                    targetFramework: "netcoreapp2.1").Object),
                    }
                });
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<ITestRunner>())).Returns(new InitialTestRun(new TestRunResult(true), new TimeoutValueCalculator(1)));
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object);

            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            var result = target.Initialize(options, null);

            inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfo(It.IsAny<StrykerOptions>(), It.IsAny<TestProjectsInfo>(), It.IsAny<IEnumerable<IAnalyzerResult>>()), Times.Once);
            inputFileResolverMock.Verify(x => x.ResolveTestProjectsInfo(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>()), Times.Once);
        }

        [Fact]
        public void InitialisationProcess_ShouldThrowOnFailedInitialTestRun()
        {
            var fileSystemMock = new MockFileSystem();
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());

            var testProjectInfo = new TestProjectsInfo(fileSystemMock)
            {
                TestProjects = new List<TestProject> {
                    new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder",
                        targetFramework: "netcoreapp2.1").Object)
                    }
            };

            inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfo(It.IsAny<StrykerOptions>(), It.IsAny<TestProjectsInfo>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(
                new SourceProjectInfo() {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: new string[0]).Object,
                });

            inputFileResolverMock.Setup(x => x.ResolveTestProjectsInfo(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(testProjectInfo);

            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            testRunnerMock.Setup(x => x.DiscoverTests()).Returns(new TestSet());
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<ITestRunner>())).Throws(new InputException("")); // failing test

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object);
            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            target.Initialize(options, null);
            Assert.Throws<InputException>(() => target.InitialTest(options));

            inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfo(It.IsAny<StrykerOptions>(), It.IsAny<TestProjectsInfo>(), It.IsAny<IEnumerable<IAnalyzerResult>>()), Times.Once);
            inputFileResolverMock.Verify(x => x.ResolveTestProjectsInfo(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>()), Times.Once);
            initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(),testRunnerMock.Object), Times.Once);
        }


        [Fact]
        public void InitialisationProcess_ShouldRunTestSession()
        {
            var fileSystemMock = new MockFileSystem();
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());

            var testProjectInfo = new TestProjectsInfo(fileSystemMock)
            {
                TestProjects = new List<TestProject> {
                    new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder",
                        targetFramework: "netcoreapp2.1").Object)
                    }
            };

            inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfo(It.IsAny<StrykerOptions>(), It.IsAny<TestProjectsInfo>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(
                new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: new string[0]).Object,
                });

            inputFileResolverMock.Setup(x => x.ResolveTestProjectsInfo(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(testProjectInfo);

            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            var testSet = new TestSet();
            testSet.RegisterTest(new TestDescription(Guid.Empty, "test", "test.cs"));
            testRunnerMock.Setup(x => x.DiscoverTests()).Returns(testSet);
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<ITestRunner>()))
                .Returns(new InitialTestRun(new TestRunResult(true), null)); // failing test

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object);
            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            target.Initialize(options, null);
            target.InitialTest(options);

            inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfo(It.IsAny<StrykerOptions>(), It.IsAny<TestProjectsInfo>(), It.IsAny<IEnumerable<IAnalyzerResult>>()), Times.Once);
            inputFileResolverMock.Verify(x => x.ResolveTestProjectsInfo(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>()), Times.Once);
            initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(),testRunnerMock.Object), Times.Once);
        }


        [Theory]
        [InlineData("xunit.core")]
        [InlineData("nunit.framework")]
        [InlineData("Microsoft.VisualStudio.TestPlatform.TestFramework")]
        [InlineData("")]
        public void InitialisationProcess_ShouldThrowOnWhenNoTestDetected(string libraryName)
        {
            var fileSystemMock = new MockFileSystem();
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());

            var testProjectInfo = new TestProjectsInfo(fileSystemMock)
            {
                TestProjects = new List<TestProject> {
                    new TestProject(fileSystemMock, TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder",
                        targetFramework: "netcoreapp2.1",
                        references: new[] { libraryName }).Object)
                    }
            };

            inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfo(It.IsAny<StrykerOptions>(), It.IsAny<TestProjectsInfo>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(
                new SourceProjectInfo()
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: new string[0]).Object,
                });

            inputFileResolverMock.Setup(x => x.ResolveTestProjectsInfo(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(testProjectInfo);

            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            testRunnerMock.Setup(x => x.DiscoverTests()).Returns(new TestSet());
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<ITestRunner>()))
                .Returns(new InitialTestRun(new TestRunResult(true), null)); // failing test

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object);
            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            target.Initialize(options, null);
            Assert.Throws<InputException>(() => target.InitialTest(options)).Message.ShouldContain(libraryName);
        }
    }
}
