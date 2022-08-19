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
            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>()))
                .Returns(new ProjectInfo(new MockFileSystem())
                {
                    ProjectUnderTestAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                        references: new string[0]).Object,
                    TestProjectAnalyzerResults = new List<IAnalyzerResult> { TestHelper.SetupProjectAnalyzerResult(
                        projectFilePath: "C://Example/Dir/ProjectFolder",
                        targetFramework: "netcoreapp2.1").Object
                    },
                    ProjectContents = folder
                });
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(), It.IsAny<ITestRunner>())).Returns(new InitialTestRun(TestGuidsList.EveryTest(), TestGuidsList.NoTest(), new TimeoutValueCalculator(1)));
            initialBuildProcessMock.Setup(x => x.InitialBuild(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), null));
            assemblyReferenceResolverMock.Setup(x => x.LoadProjectReferences(It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>());

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);

            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

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

            var folder = new CsharpFolderComposite();
            folder.Add(new CsharpFileLeaf());

            inputFileResolverMock.Setup(x => x.ResolveInput(It.IsAny<StrykerOptions>())).Returns(
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
            testRunnerMock.Setup(x => x.DiscoverTests()).Returns(new TestSet());
            initialTestProcessMock.Setup(x => x.InitialTest(It.IsAny<StrykerOptions>(),It.IsAny<ITestRunner>())).Throws(new InputException("")); // failing test
            assemblyReferenceResolverMock.Setup(x => x.LoadProjectReferences(It.IsAny<string[]>()))
                .Returns(Enumerable.Empty<PortableExecutableReference>())
                .Verifiable();

            var target = new InitialisationProcess(
                inputFileResolverMock.Object,
                initialBuildProcessMock.Object,
                initialTestProcessMock.Object,
                testRunnerMock.Object,
                assemblyReferenceResolverMock.Object);
            var options = new StrykerOptions
            {
                ProjectName = "TheProjectName",
                ProjectVersion = "TheProjectVersion"
            };

            target.Initialize(options);
            Assert.Throws<InputException>(() => target.InitialTest(options));

            inputFileResolverMock.Verify(x => x.ResolveInput(It.IsAny<StrykerOptions>()), Times.Once);
            assemblyReferenceResolverMock.Verify();
            initialTestProcessMock.Verify(x => x.InitialTest(It.IsAny<StrykerOptions>(),testRunnerMock.Object), Times.Once);
        }
    }
}
