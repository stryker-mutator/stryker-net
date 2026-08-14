using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Configuration.Options;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.Json;

namespace Stryker.Core.UnitTest.Reporters;

[TestClass]
public class BaselineReporterTests : TestBase
{
    [TestMethod]
    public void Saves_Under_ProjectVersion_When_Set()
    {
        var baselineProvider = new Mock<IBaselineProvider>();
        var gitInfoProvider = new Mock<IGitInfoProvider>();
        gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("other-branch");

        var options = new StrykerOptions
        {
            ProjectVersion = "new-feature",
            WithBaseline = true
        };

        var target = new BaselineReporter(options, baselineProvider.Object, gitInfoProvider.Object);

        target.OnAllMutantsTested(CreateComponent(), null);

        baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), "baseline/new-feature"), Times.Once);
        baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), "baseline/other-branch"), Times.Never);
    }

    [TestMethod]
    public void Saves_Under_BranchName_When_ProjectVersion_Is_Empty()
    {
        var baselineProvider = new Mock<IBaselineProvider>();
        var gitInfoProvider = new Mock<IGitInfoProvider>();
        gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("new-feature");
        gitInfoProvider.SetupGet(x => x.IsRepository).Returns(true);

        var options = new StrykerOptions { WithBaseline = true };

        var target = new BaselineReporter(options, baselineProvider.Object, gitInfoProvider.Object);

        target.OnAllMutantsTested(CreateComponent(), null);

        baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), "baseline/new-feature"), Times.Once);
    }

    [TestMethod]
    public void Saves_Under_CurrentVersion_And_Never_Overwrites_The_Compare_Version()
    {
        var baselineProvider = new Mock<IBaselineProvider>();
        var gitInfoProvider = new Mock<IGitInfoProvider>();

        var options = new StrykerOptions
        {
            ProjectVersion = "new-feature",
            BaselineCompareVersion = "master",
            WithBaseline = true
        };

        var target = new BaselineReporter(options, baselineProvider.Object, gitInfoProvider.Object);

        target.OnAllMutantsTested(CreateComponent(), null);

        baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), "baseline/new-feature"), Times.Once);
        baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), "baseline/master"), Times.Never);
    }

    private static IReadOnlyProjectComponent CreateComponent()
    {
        var readOnlyInputComponent = new Mock<IReadOnlyProjectComponent>(MockBehavior.Loose);
        readOnlyInputComponent.Setup(s => s.FullPath).Returns("/home/usr/dev/project");

        return readOnlyInputComponent.Object;
    }
}
