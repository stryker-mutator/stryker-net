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
using Stryker.Core.Mutants;
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

            testRunnerMock.Setup(x => x.RunAll(It.IsAny<ITimeoutValueCalculator>(), null, null))
                .Returns(new TestRunResult(true)); // testrun is successful
            testRunnerMock.Setup(x => x.DiscoverTests()).Returns(new TestSet());
            testRunnerMock.Setup(x => x.Dispose());
            var projectContents = new CsharpFolderComposite();
            projectContents.Add(new CsharpFileLeaf());
            var folder = new CsharpFolderComposite();
            folder.AddRange(new Collection<IProjectComponent>
                {
                    new CsharpFileLeaf()
                });
            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<IStrykerOptions>()))
                .Returns(new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: new string[0]).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder").Object
                    },
                    ProjectContents = folder
                });
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<IStrykerOptions>(), It.IsAny<ITestRunner>())).Returns(new InitialTestRun(new TestRunResult(true), new TimeoutValueCalculator(1)));
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

            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<IStrykerOptions>()), Times.Once);
        }

        [Fact]
        public void InitialisationProcess_ShouldThrowOnFailedInitialTestRun()
        {
            var testRunnerMock = new Mock<ITestRunner>(MockBehavior.Strict);
            var inputFileResolverMock = new Mock<IInputFileResolver>(MockBehavior.Strict);
            var initialBuildProcessMock = new Mock<IInitialBuildProcess>(MockBehavior.Strict);
            var initialTestProcessMock = new Mock<IInitialTestProcess>(MockBehavior.Strict);
            var assemblyReferenceResolverMock = new Mock<IAssemblyReferenceResolver>(MockBehavior.Strict);

            testRunnerMock.Setup(x => x.RunAll(It.IsAny<ITimeoutValueCalculator>(), null, null));
            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<IStrykerOptions>())).Returns(
                new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: new string[0]).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder").Object
                    },
                    ProjectContents = folder
                });

            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>()));
            testRunnerMock.Setup(x => x.DiscoverTests()).Returns(new TestSet());
            testRunnerMock.Setup(x => x.Dispose());
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<IStrykerOptions>(),It.IsAny<ITestRunner>())).Throws(new StrykerInputException("")); // failing test
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
            Assert.Throws<StrykerInputException>(() => target.InitialTest(options));

            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<IStrykerOptions>()), Times.Once);
            assemblyReferenceResolverMock.Verify();
            initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<IStrykerOptions>(),testRunnerMock.Object), Times.Once);
        }
    }
}
