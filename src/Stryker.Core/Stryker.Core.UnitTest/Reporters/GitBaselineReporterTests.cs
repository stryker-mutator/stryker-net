﻿using Moq;
using Stryker.Core.Baseline;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters;
using Stryker.Core.Reporters.Json;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class GitBaselineReporterTests
    {
        [Fact]
        public void Uses_ProjectVersion_When_CurrentBranch_Is_Null()
        {
            var gitInfoProvider = new Mock<IGitInfoProvider>();
            var baselineProvider = new Mock<IBaselineProvider>();

            var readOnlyInputComponent = new Mock<IReadOnlyInputComponent>(MockBehavior.Loose);

            var options = new StrykerOptions(projectVersion: "new-feature", gitSource: "master", compareToDashboard: true);

            gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns<string>(null);

            var target = new GitBaselineReporter(options, baselineProvider.Object, gitInfoProvider.Object);

            target.OnAllMutantsTested(readOnlyInputComponent.Object);

            baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), It.Is<string>(x => x == "dashboard-compare/new-feature")), Times.Once);
            baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), It.Is<string>(x => x == null)), Times.Never);
        }


        [Fact]
        public void Doesnt_Use_ProjectVersion_When_CurrentBranch_Is_Not_Null()
        {
            var gitInfoProvider = new Mock<IGitInfoProvider>();
            var baselineProvider = new Mock<IBaselineProvider>();

            var readOnlyInputComponent = new Mock<IReadOnlyInputComponent>(MockBehavior.Loose);

            var options = new StrykerOptions(projectVersion: "new-feature", gitSource: "master", compareToDashboard: true);

            gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("new-feature");

            var target = new GitBaselineReporter(options, baselineProvider.Object, gitInfoProvider.Object);

            target.OnAllMutantsTested(readOnlyInputComponent.Object);

            baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), It.Is<string>(x => x == "dashboard-compare/new-feature")), Times.Once);
            baselineProvider.Verify(x => x.Save(It.IsAny<JsonReport>(), It.Is<string>(x => x == "new-feature")), Times.Never);
        }
    }
}
