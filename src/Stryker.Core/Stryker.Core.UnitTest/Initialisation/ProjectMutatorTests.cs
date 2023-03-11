using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Buildalyzer;
using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class ProjectMutatorTests : TestBase
    {
        private readonly Mock<IMutationTestProcess> _mutationTestProcessMock = new(MockBehavior.Strict);
        private readonly Mock<IReporter> _reporterMock = new(MockBehavior.Strict);
        private readonly MutationTestInput _mutationTestInput;

        public ProjectMutatorTests()
        {

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
            var target = new ProjectMutator(_mutationTestProcessMock.Object);


            // act
            var result = target.MutateProject(options, _mutationTestInput,_reporterMock.Object);

            // assert
            result.ShouldNotBeNull();
        }
    }
}
