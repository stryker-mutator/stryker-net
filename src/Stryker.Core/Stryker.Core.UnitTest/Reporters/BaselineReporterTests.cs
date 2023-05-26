using Moq;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.Json;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class BaselineReporterTests : TestBase
    {
        [Fact]
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
