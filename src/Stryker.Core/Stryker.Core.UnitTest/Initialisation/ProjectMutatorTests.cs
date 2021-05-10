using System.IO.Abstractions.TestingHelpers;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectMutatorTests
    {
        private readonly Mock<IInitialisationProcessProvider> _initialisationProcessProviderMock = new Mock<IInitialisationProcessProvider>(MockBehavior.Strict);
        private readonly Mock<IMutationTestProcessProvider> _mutationTestProcessProviderMock = new Mock<IMutationTestProcessProvider>(MockBehavior.Strict);
        private readonly Mock<IMutationTestProcess> _mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);
        private readonly Mock<IInitialisationProcess> _initialisationProcessMock = new Mock<IInitialisationProcess>(MockBehavior.Strict);
        private readonly Mock<IReporter> _reporterMock = new Mock<IReporter>(MockBehavior.Strict);
        private readonly MutationTestInput _mutationTestInput;

        public ProjectMutatorTests()
        {
            _initialisationProcessProviderMock.Setup(x => x.Provide()).Returns(_initialisationProcessMock.Object);

            _mutationTestProcessProviderMock
                .Setup(x => x.Provide(
                    It.IsAny<MutationTestInput>(),
                    It.IsAny<IReporter>(),
                    It.IsAny<IMutationTestExecutor>(),
                    It.IsAny<IStrykerOptions>()))
                .Returns(_mutationTestProcessMock.Object);

            _mutationTestProcessMock.Setup(x => x.Mutate());


            _mutationTestInput = new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo(new MockFileSystem())
                {
                    ProjectContents = new CsharpFolderComposite()
                },
            };
        }

        [Fact]
        public void ShouldInitializeEachProjectInSolution()
        {
            // arrange
            var options = new StrykerOptions();
            var target = new ProjectMutator(_initialisationProcessProviderMock.Object, _mutationTestProcessProviderMock.Object);

            _initialisationProcessMock.Setup(x => x.Initialize(It.IsAny<IStrykerOptions>())).Returns(_mutationTestInput);
            _initialisationProcessMock.Setup(x => x.InitialTest(It.IsAny<IStrykerOptions>())).Returns(new TimeoutValueCalculator(5));

            // act
            var result = target.MutateProject(options, _reporterMock.Object);

            // assert
            result.ShouldNotBeNull();
        }
    }
}
