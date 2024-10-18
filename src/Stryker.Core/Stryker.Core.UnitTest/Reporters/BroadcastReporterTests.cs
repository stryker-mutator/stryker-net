using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents.Csharp;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;

namespace Stryker.Core.UnitTest.Reporters
{
    [TestClass]
    public class BroadcastReporterTests : TestBase
    {
        [TestMethod]
        public void BroadcastReporter_ShouldInvokeSameMethodWithSameObject_OnAllMutantsTested()
        {
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));

            var exampleInputComponent = new CsharpFileLeaf();
            var reporters = new Collection<IReporter>()
            {
                reporterMock.Object
            };
            var target = new BroadcastReporter(reporters);

            target.OnAllMutantsTested(exampleInputComponent, It.IsAny<TestProjectsInfo>());

            reporterMock.Verify(x => x.OnAllMutantsTested(exampleInputComponent, It.IsAny<TestProjectsInfo>()), Times.Once);
        }

        [TestMethod]
        public void BroadcastReporter_ShouldInvokeSameMethodWithSameObject_OnMutantsCreated()
        {
            var mockFileSystem = new MockFileSystem();
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));

            var exampleInputComponent = new CsharpFileLeaf();
            var exampleTestProjectsInfo = new TestProjectsInfo(mockFileSystem);

            var reporters = new Collection<IReporter>()
            {
                reporterMock.Object
            };
            var target = new BroadcastReporter(reporters);

            target.OnMutantsCreated(exampleInputComponent, exampleTestProjectsInfo);

            reporterMock.Verify(x => x.OnMutantsCreated(exampleInputComponent, exampleTestProjectsInfo), Times.Once);
        }

        [TestMethod]
        public void BroadcastReporter_ShouldInvokeSameMethodWithSameObject_OnMutantTested()
        {
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<IMutant>()));

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

        [TestMethod]
        public void BroadcastReporter_ShouldInvokeAllReportersInList()
        {
            var mockFileSystem = new MockFileSystem();
            var reporterMock = new Mock<IReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.OnAllMutantsTested(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));
            reporterMock.Setup(x => x.OnMutantsCreated(It.IsAny<IReadOnlyProjectComponent>(), It.IsAny<TestProjectsInfo>()));
            reporterMock.Setup(x => x.OnMutantTested(It.IsAny<IMutant>()));

            var exampleTestProjectsInfo = new TestProjectsInfo(mockFileSystem);
            var exampleInputComponent = new CsharpFileLeaf();
            var exampleMutant = new Mutant();

            var reporters = new Collection<IReporter>()
            {
                reporterMock.Object,
                reporterMock.Object
            };
            var target = new BroadcastReporter(reporters);

            target.OnMutantsCreated(exampleInputComponent, exampleTestProjectsInfo);
            target.OnMutantTested(exampleMutant);
            target.OnAllMutantsTested(exampleInputComponent, It.IsAny<TestProjectsInfo>());

            reporterMock.Verify(x => x.OnAllMutantsTested(exampleInputComponent, It.IsAny<TestProjectsInfo>()), Times.Exactly(2));
            reporterMock.Verify(x => x.OnMutantsCreated(exampleInputComponent, exampleTestProjectsInfo), Times.Exactly(2));
            reporterMock.Verify(x => x.OnMutantTested(exampleMutant), Times.Exactly(2));
        }

        [TestMethod]
        public void BroadcastReporter_NoReportersInList()
        {
            var mockFileSystem = new MockFileSystem();
            var reporters = new Collection<IReporter>() { };

            var exampleTestProjectsInfo = new TestProjectsInfo(mockFileSystem);
            var exampleInputComponent = new CsharpFileLeaf();
            var exampleMutant = new Mutant();

            var target = new BroadcastReporter(reporters);

            target.OnMutantsCreated(exampleInputComponent, exampleTestProjectsInfo);
            target.OnMutantTested(exampleMutant);
            target.OnAllMutantsTested(exampleInputComponent, It.IsAny<TestProjectsInfo>());

            target.Reporters.ShouldBeEmpty();
        }
    }
}
