using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Buildalyzer;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectMutatorTests : TestBase
    {
        private readonly Mock<IMutationTestProcess> _mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
        private readonly Mock<IInitialisationProcess> _initialisationProcessMock = new Mock<IInitialisationProcess>(MockBehavior.Strict);
        private readonly Mock<IReporter> _reporterMock = new Mock<IReporter>(MockBehavior.Strict);
        private readonly MutationTestInput _mutationTestInput;

        public ProjectMutatorTests()
        {

            _mutationTestProcessMock.Setup(x => x.Mutate());

            _mutationTestInput = new MutationTestInput()
            {
                SourceProjectInfo = new SourceProjectInfo()
                {
                    ProjectContents = new CsharpFolderComposite()
                }
            };
        }

        [Fact]
        public void ShouldInitializeEachProjectInSolution()
        {
            // arrange
            var options = new StrykerOptions();
            var target = new ProjectMutator(_initialisationProcessMock.Object, _mutationTestProcessMock.Object);

            _initialisationProcessMock.Setup(x => x.Initialize(It.IsAny<StrykerOptions>(), It.IsAny<IEnumerable<IAnalyzerResult>>())).Returns(_mutationTestInput);
            _initialisationProcessMock.Setup(x => x.InitialTest(options))
                .Returns(new InitialTestRun(new TestRunResult(true), new TimeoutValueCalculator(500)));
            // act
            var result = target.MutateProject(options, _reporterMock.Object);

            // assert
            result.ShouldNotBeNull();
        }
    }
}
