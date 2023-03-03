using System;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Mono.Collections.Generic;
using Moq;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Mutants;
using Xunit;
using Shouldly;

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
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            testRunnerMock.Setup(x => x.Dispose());
            var projectContents = new CsharpFolderComposite();
            projectContents.Add(new CsharpFileLeaf());
            var folder = new CsharpFolderComposite();
            folder.AddRange(new Collection<IProjectComponent>
                {
                    new CsharpFileLeaf()
                });
            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>()))
                .Returns(new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: Array.Empty<string>()).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder",
                        targetFramework: "netcoreapp2.1").Object
                    },
                    ProjectContents = folder
                });
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTest>(), It.IsAny<ITestRunner>())).Returns(new InitialTestRun(new TestRunResult(true), new TimeoutValueCalculator(1)));
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            assemblyReferenceResolverMock.Setup(x => x.LoadProjectReferences(It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>());

            var target = new InitialisationProcess(inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);

            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            var result = target.Initialize(options, null).ToList();
            result.Count.ShouldBe(1);
            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>()), Times.Once);
        }

        [Fact]
        public void InitialisationProcess_ShouldThrowOnFailedInitialTestRun()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(
                new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: new string[0]).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder",
                        targetFramework: "netcoreapp2.1").Object
                    },
                    ProjectContents = folder
                });

            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            testRunnerMock.Setup(x => x.DiscoverTests( It.IsAny<IProjectAndTest>())).Returns(new TestSet());
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTest>(),It.IsAny<ITestRunner>())).Throws(new InputException("")); // failing test
            assemblyReferenceResolverMock.Setup(x => x.LoadProjectReferences(It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            var input = target.Initialize(options, null).First();
            Assert.Throws<InputException>(() => target.InitialTest(options, input.ProjectInfo));

            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>()), Times.Once);
            assemblyReferenceResolverMock.Verify();
            initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTest>(), testRunnerMock.Object), Times.Once);
        }

        [Fact]
        public void InitialisationProcess_ShouldThrowIfHalfTestsAreFailing()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(
                new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: new string[0]).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder",
                        targetFramework: "netcoreapp2.1").Object
                    },
                    ProjectContents = folder
                });

            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            var failedTest = Guid.NewGuid();
            var ranTests = new TestGuidsList(failedTest, Guid.NewGuid());
            var testSet = new TestSet();
            foreach (var ranTest in ranTests.GetGuids())
            {
                testSet.RegisterTest(new TestDescription(ranTest, "test", "test.cpp"));
            }
            testRunnerMock.Setup(x => x.DiscoverTests(It.IsAny<IProjectAndTest>())).Returns(testSet);
            var failedTests = new TestGuidsList(failedTest);
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTest>(),It.IsAny<ITestRunner>())).Returns( new InitialTestRun(
                new TestRunResult(ranTests, failedTests, TestGuidsList.NoTest(), string.Empty, TimeSpan.Zero), new TimeoutValueCalculator(0) )); // failing test
            assemblyReferenceResolverMock.Setup(x => x.LoadProjectReferences(It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            var input = target.Initialize(options, null).First();
            Assert.Throws<InputException>(() => target.InitialTest(options, input.ProjectInfo));

            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>()), Times.Once);
            assemblyReferenceResolverMock.Verify();
            initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTest>(), testRunnerMock.Object), Times.Once);
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
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(
                new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: new string[0]).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder",
                        targetFramework: "netcoreapp2.1").Object
                    },
                    ProjectContents = folder
                });

            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            var failedTest = Guid.NewGuid();
            var ranTests = new TestGuidsList(failedTest, Guid.NewGuid(), Guid.NewGuid());
            var testSet = new TestSet();
            foreach (var ranTest in ranTests.GetGuids())
            {
                testSet.RegisterTest(new TestDescription(ranTest, "test", "test.cpp"));
            }
            testRunnerMock.Setup(x => x.DiscoverTests( It.IsAny<IProjectAndTest>())).Returns(testSet);
            var failedTests = new TestGuidsList(failedTest);
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTest>(),It.IsAny<ITestRunner>())).Returns( new InitialTestRun(
                new TestRunResult(ranTests, failedTests, TestGuidsList.NoTest(), string.Empty, TimeSpan.Zero), new TimeoutValueCalculator(0) )); // failing test
            assemblyReferenceResolverMock.Setup(x => x.LoadProjectReferences(It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion",
                BreakOnInitialTestFailure = breakOnInitialTestFailure
            };
            var input = target.Initialize(options, null).First();
            if (breakOnInitialTestFailure)
            {
                Assert.Throws<InputException>(() => target.InitialTest(options, input.ProjectInfo));
            }
            else
            {
                target.InitialTest(options, input.ProjectInfo);
            }
        }


        [Fact]
        public void InitialisationProcess_ShouldRunTestSession()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(
                new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: new string[0]).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder",
                        targetFramework: "netcoreapp2.1").Object
                    },
                    ProjectContents = folder
                });

            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            var testSet = new TestSet();
            testSet.RegisterTest(new TestDescription(Guid.Empty, "test", "test.cs"));
            testRunnerMock.Setup(x => x.DiscoverTests( It.IsAny<IProjectAndTest>())).Returns(testSet);
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTest>(),It.IsAny<ITestRunner>()))
                .Returns(new InitialTestRun(new TestRunResult(true), null)); // failing test
            assemblyReferenceResolverMock.Setup(x => x.LoadProjectReferences(It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            var input = target.Initialize(options, null).First();
            target.InitialTest(options, input.ProjectInfo);

            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>()), Times.Once);
            assemblyReferenceResolverMock.Verify();
            initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<IProjectAndTest>(), testRunnerMock.Object), Times.Once);
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
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(
                new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: Array.Empty<string>()).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder",
                        targetFramework: "netcoreapp2.1",
                        references: new[] { libraryName}).Object
                    },
                    ProjectContents = folder
                });

            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            testRunnerMock.Setup(x => x.DiscoverTests( It.IsAny<IProjectAndTest>())).Returns(new TestSet());
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(),  It.IsAny<IProjectAndTest>(),It.IsAny<ITestRunner>()))
                .Returns(new InitialTestRun(new TestRunResult(true), null)); // failing test
            assemblyReferenceResolverMock.Setup(x => x.LoadProjectReferences(It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            var input = target.Initialize(options, null).First();
            Assert.Throws<InputException>(() => target.InitialTest(options, input.ProjectInfo)).Message.ShouldContain(libraryName);
        }
    }
}
