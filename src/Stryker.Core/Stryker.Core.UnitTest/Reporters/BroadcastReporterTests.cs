using System.Collections.ObjectModel;
using Moq;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class BroadcastReporterTests : TestBase
    {
        [Fact]
        public void BroadcastReporter_ShouldInvokeSameMethodWithSameObject_OnAllMutantsTested()
        {
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));

            var exampleInputComponent = new CsharpFileLeaf();
            var exampleMutant = new Mutant();

            var reporters = new Collection<IReporter>()
            {
                reporterMock.Object
            };
            var target = new BroadcastReporter(reporters);

            target.OnAllMutantsTested(exampleInputComponent, It.IsAny<TestProjectsInfo>());

            reporterMock.Verify(x => x.OnAllMutantsTested(exampleInputComponent, It.IsAny<TestProjectsInfo>()), Times.Once);
        }

        [Fact]
        public void BroadcastReporter_ShouldInvokeSameMethodWithSameObject_OnMutantsCreated()
        {
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()));

            var exampleInputComponent = new CsharpFileLeaf();
            var exampleMutant = new Mutant();

            var reporters = new Collection<IReporter>()
            {
                reporterMock.Object
            };
            var target = new BroadcastReporter(reporters);

            target.OnMutantsCreated(exampleInputComponent);

            reporterMock.Verify(x => x.OnMutantsCreated(exampleInputComponent), Times.Once);
        }

        [Fact]
        public void BroadcastReporter_ShouldInvokeSameMethodWithSameObject_OnMutantTested()
        {
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));

            var exampleInputComponent = new CsharpFileLeaf();
            var exampleMutant = new Mutant();

            var reporters = new Collection<IReporter>()
            {
                reporterMock.Object
            };
            var target = new BroadcastReporter(reporters);

            target.OnMutantTested(exampleMutant);

            reporterMock.Verify(x => x.OnMutantTested(exampleMutant), Times.Once);
        }

        [Fact]
        public void BroadcastReporter_ShouldInvokeAllReportersInList()
        {
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>()));
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<Mutant>()));

            var exampleInputComponent = new CsharpFileLeaf();
            var exampleMutant = new Mutant();

            var reporters = new Collection<IReporter>()
            {
                reporterMock.Object,
                reporterMock.Object
            };
            var target = new BroadcastReporter(reporters);

            target.OnAllMutantsTested(exampleInputComponent, It.IsAny<TestProjectsInfo>());
            target.OnMutantsCreated(exampleInputComponent);
            target.OnMutantTested(exampleMutant);

            reporterMock.Verify(x => x.OnAllMutantsTested(exampleInputComponent, It.IsAny<TestProjectsInfo>()), Times.Exactly(2));
            reporterMock.Verify(x => x.OnMutantsCreated(exampleInputComponent), Times.Exactly(2));
            reporterMock.Verify(x => x.OnMutantTested(exampleMutant), Times.Exactly(2));
        }

        [Fact]
        public void BroadcastReporter_NoReportersInList()
        {
            var reporters = new Collection<IReporter>() { };

            var exampleInputComponent = new CsharpFileLeaf();
            var exampleMutant = new Mutant();

            var target = new BroadcastReporter(reporters);

            target.OnAllMutantsTested(exampleInputComponent, It.IsAny<TestProjectsInfo>());
            target.OnMutantsCreated(exampleInputComponent);
            target.OnMutantTested(exampleMutant);
        }
    }
}
