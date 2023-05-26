using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
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
using Stryker.Core.TestRunners.VsTest;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialisationProcessTests : TestBase
    {
        [Fact]
        public void InitialisationProcess_ShouldCallNeededResolvers()
        {
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);

            var projectContents = new CsharpFolderComposite();
            projectContents.Add(new CsharpFileLeaf());
            var folder = new CsharpFolderComposite();
            folder.AddRange(new Collection<IProjectComponent>
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

            inputFileResolverMock.SetupGet( x => x.FileSystem).Returns(new FileSystem());
            var target = new InitialisationProcess(inputFileResolverMock.Object);

            var options = new StrykerOptions
            { ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            var result = target.GetMutableProjectsInfo(options).ToList();
            result.Count.ShouldBe(1);
            inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>()), Times.Once);
        }

        [Fact]
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

            inputFileResolverMock.SetupGet( x => x.FileSystem).Returns(new FileSystem());
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            testRunnerMock.Setup(x => x.GetTests( It.IsAny<IProjectAndTests>())).Returns(new TestSet());
            testRunnerMock.Setup(x => x.DiscoverTests( It.IsAny<string>())).Returns(true);
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(),It.IsAny<ITestRunner>())).Throws(new InputException("")); // failing test

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
            Assert.Throws<InputException>(() => target.GetMutationTestInputs(options, projects, testRunnerMock.Object));
            initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), testRunnerMock.Object), Times.Once);
        }

        [Fact]
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

            inputFileResolverMock.SetupGet( x => x.FileSystem).Returns(fileSystemMock);
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            var failedTest = Guid.NewGuid();
            var ranTests = new TestGuidsList(failedTest, Guid.NewGuid());
            var testSet = new TestSet();
            foreach (var ranTest in ranTests.GetGuids())
            {
                testSet.RegisterTest(new TestDescription(ranTest, "test", "test.cpp"));
            }
            testRunnerMock.Setup(x => x.DiscoverTests(It.IsAny<string>())).Returns(true);
            testRunnerMock.Setup(x => x.GetTests(It.IsAny<IProjectAndTests>())).Returns(testSet);
            var failedTests = new TestGuidsList(failedTest);
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(),It.IsAny<ITestRunner>())).Returns(
                new InitialTestRun(
                new TestRunResult(Array.Empty<VsTestDescription>() ,ranTests, failedTests, TestGuidsList.NoTest(), string.Empty, Enumerable.Empty<string>(),TimeSpan.Zero), new TimeoutValueCalculator(0) )); // failing test

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
            Assert.Throws<InputException>(() => target.GetMutationTestInputs(options, projects, testRunnerMock.Object));
            inputFileResolverMock.Verify(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>()), Times.Once);
            initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(), testRunnerMock.Object), Times.Once);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void InitialisationProcess_ShouldThrowOnTestTestIfAskedFor(bool breakOnInitialTestFailure)
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());


            inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(
                new [] {new SourceProjectInfo {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: Array.Empty<string>()).Object,
                    TestProjectsInfo = new TestProjectsInfo(new MockFileSystem())
                }});

            inputFileResolverMock.SetupGet( x => x.FileSystem).Returns(new FileSystem());
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            var failedTest = Guid.NewGuid();
            var ranTests = new TestGuidsList(failedTest, Guid.NewGuid(), Guid.NewGuid());
            var testSet = new TestSet();
            foreach (var ranTest in ranTests.GetGuids())
            {
                testSet.RegisterTest(new TestDescription(ranTest, "test", "test.cpp"));
            }
            testRunnerMock.Setup(x => x.DiscoverTests( It.IsAny<string>())).Returns(true);
            testRunnerMock.Setup(x => x.GetTests( It.IsAny<IProjectAndTests>())).Returns(testSet);
            var failedTests = new TestGuidsList(failedTest);
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(),It.IsAny<ITestRunner>())).Returns( new InitialTestRun(
                new TestRunResult(Array.Empty<VsTestDescription>() ,ranTests, failedTests, TestGuidsList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero), new TimeoutValueCalculator(0) )); // failing test

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
                Assert.Throws<InputException>(() => target.GetMutationTestInputs(options, projects, testRunnerMock.Object));
            }
            else
            {
                var testInputs = target.GetMutationTestInputs(options, projects, testRunnerMock.Object);

                testInputs.ShouldNotBeEmpty();
            }
        }


        [Fact]
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


            inputFileResolverMock.SetupGet( x => x.FileSystem).Returns(new FileSystem());
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            var testSet = new TestSet();
            testSet.RegisterTest(new TestDescription(Guid.Empty, "test", "test.cs"));
            testRunnerMock.Setup(x => x.DiscoverTests( It.IsAny<string>())).Returns(true);
            testRunnerMock.Setup(x => x.GetTests( It.IsAny<IProjectAndTests>())).Returns(testSet);
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTests>(),It.IsAny<ITestRunner>()))
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


        [Theory]
        [InlineData("xunit.core")]
        [InlineData("nunit.framework")]
        [InlineData("Microsoft.VisualStudio.TestPlatform.TestFramework")]
        [InlineData("")]
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
                references: new[] { libraryName }).Object;

            inputFileResolverMock.SetupGet( x => x.FileSystem).Returns(new FileSystem());

            inputFileResolverMock.Setup(x => x.ResolveSourceProjectInfos(It.IsAny<StrykerOptions>())).Returns(
                new[] {new SourceProjectInfo
                {
                    AnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: Array.Empty<string>()).Object,
                    TestProjectsInfo = new TestProjectsInfo(new MockFileSystem()){TestProjects = new List<TestProject> {new(new MockFileSystem(), testProjectAnalyzerResult)}}
                }});

            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            testRunnerMock.Setup(x => x.DiscoverTests( It.IsAny<string>())).Returns(false);
            testRunnerMock.Setup(x => x.GetTests( It.IsAny<IProjectAndTests>())).Returns(new TestSet());
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(),  It.IsAny<IProjectAndTests>(),It.IsAny<ITestRunner>()))
                .Returns(new InitialTestRun(new TestRunResult(Array.Empty<VsTestDescription>(),  TestGuidsList.NoTest(), TestGuidsList.NoTest(), TestGuidsList.NoTest(), string.Empty, Enumerable.Empty<string>(), TimeSpan.Zero), null)); // failing test

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
            Assert.Throws<InputException>(() => target.GetMutationTestInputs(options, projects, testRunnerMock.Object)).Message.ShouldContain(libraryName);
        }
    }
}
