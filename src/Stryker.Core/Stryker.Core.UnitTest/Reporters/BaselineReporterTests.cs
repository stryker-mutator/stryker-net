using Moq;
using Stryker.Configuration.Baseline.Providers;
using Stryker.Configuration;
using Stryker.Configuration.ProjectComponents;
using Stryker.Configuration.ProjectComponents.TestProjects;
using Stryker.Configuration.Reporters;
using Stryker.Configuration.Reporters.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Reporters
{
    [TestClass]
    public class BaselineReporterTests : TestBase
    {
        [TestMethod]
        public void Doesnt_Use_ProjectVersion_When_CurrentBranch_Is_Not_Null()
        {
            var gitInfoProvider = new Mock<IGitInfoProvider>();
            var baselineProvider = new Mock<IBaselineProvider>();

            var readOnlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose);
            readOnlyInputComponent.Setup(s => s.FullPath).Returns("/home/usr/dev/project");

            var options = new StrykerOptions
            {
                ProjectVersion = "new-feature",
                SinceTarget = "master",
                WithBaseline = true
            };

            gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("new-feature");

            var target = new BaselineReporter(options, baselineProvider.Object, gitInfoProvider.Object);

            target.OnAllMutantsTested(readOnlyInputComponent.Object, It.IsAny<TestProjectsInfo>());

            baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), It.Is<string>(x => x == "baseline/new-feature")), Times.Once);
            baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), It.Is<string>(x => x == "new-feature")), Times.Never);
        }
    }
}
