using Microsoft.CodeAnalysis;
using Moq;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class InitialisationProcessTests
    {
        [Fact]
        public void InitialisationProcess_ShouldCallNeededResolvers()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>(), null, null))
                .Returns(new TestRunResult(true)); // testrun is successful
            testRunnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(999);
            testRunnerMock.Setup(x => x.Dispose());
            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>()))
                .Returns(new ProjectInfo
                {
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        References = new string[0]
                    },
                    TestProjectAnalyzerResults = new List<ProjectAnalyzerResult> {
                        new ProjectAnalyzerResult(null, null)
                        {
                            ProjectFilePath = "C://Example/Dir/ProjectFolder"
                        }
                    },
                    ProjectContents = new FolderComposite
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent<FileLeaf, SyntaxTree>>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs"
                            }
                        }
                    }
                });
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<ITestRunner>())).Returns(999);
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>()));
            assemblyReferenceResolverMock.Setup(x => x.LoadProjectReferences(It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>());

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);

            var options = new StrykerOptions();

            var result = target.Initialize(options);

            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<StrykerOptions>()), Times.Once);
        }

        [Fact]
        public void InitialisationProcess_ShouldThrowOnFailedInitialTestRun()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            testRunnerMock.Setup(x => x.RunAll(It.IsAny<int>(), null, null));
            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>())).Returns(
                new ProjectInfo
                {
                    ProjectUnderTestAnalyzerResult = new ProjectAnalyzerResult(null, null)
                    {
                        References = new string[0]
                    },
                    TestProjectAnalyzerResults = new List<ProjectAnalyzerResult> {
                        new ProjectAnalyzerResult(null, null)
                        {
                            ProjectFilePath = "C://Example/Dir/ProjectFolder"
                        }
                    },
                    ProjectContents = new FolderComposite
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent<FileLeaf, SyntaxTree>>
                        {
                            new FileLeaf
                            {
                                Name = "SomeFile.cs"
                            }
                        }
                    }
                });
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>()));
            testRunnerMock.Setup(x => x.DiscoverNumberOfTests()).Returns(999);
            testRunnerMock.Setup(x => x.Dispose());
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<ITestRunner>())).Throws(new StrykerInputException("")); // failing test
            assemblyReferenceResolverMock.Setup(x => x.LoadProjectReferences(It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions();

            target.Initialize(options);
            Assert.Throws<StrykerInputException>(() => target.InitialTest(options, out var nbTests));

            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<StrykerOptions>()), Times.Once);
            assemblyReferenceResolverMock.Verify();
            initialTestProcessMock.Verify(x => x.InitialTest(testRunnerMock.Object), Times.Once);
        }
    }
}
