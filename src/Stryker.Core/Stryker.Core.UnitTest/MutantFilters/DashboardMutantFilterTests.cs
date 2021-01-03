using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.DashboardCompare;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Json;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class DashboardMutantFilterTests
    {
        [Fact]
        public static void ShouldHaveName()
        {
            // Arrange
            var gitInfoProvider = new Mock<IGitInfoProvider>(MockBehavior.Loose);
            var baselineProviderMock = new Mock<IBaselineProvider>(MockBehavior.Loose);

            // Act
            var target = new DashboardMutantFilter(new StrykerOptions(), baselineProviderMock.Object, gitInfoProvider.Object) as IMutantFilter;

            // Assert
            target.DisplayName.ShouldBe("dashboard filter");
        }

        [Fact]
        public void GetBaselineCallsFallbackWhenDashboardClientReturnsNull()
        {
            // Arrange
            var baselineProvider = new Mock<IBaselineProvider>();
            var gitInfoProvider = new Mock<IGitInfoProvider>();

            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                compareToDashboard: true,
               dashboardApiKey: "Acces_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters,
               fallbackVersion: "fallback/version");

            var inputComponent = new Mock<IReadOnlyProjectComponent>().Object;

            var jsonReport = JsonReport.Build(options, inputComponent);

            gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("refs/heads/master");

            baselineProvider.Setup(x => x.Load("dashboard-compare/refs/heads/master")).Returns(Task.FromResult<JsonReport>(null));
            baselineProvider.Setup(x => x.Load("fallback/version")).Returns(Task.FromResult(jsonReport));

            // Act
            var target = new DashboardMutantFilter(options, baselineProvider.Object, gitInfoProvider.Object);

            // Assert
            baselineProvider.Verify(x => x.Load("dashboard-compare/refs/heads/master"), Times.Once);
            baselineProvider.Verify(x => x.Load("fallback/version"), Times.Once);
        }

        [Fact]
        public void GetBaselineDoesNotCallFallbackWhenDashboardClientReturnsReport()
        {
            // Arrange
            var baselineProvider = new Mock<IBaselineProvider>();
            var gitInfoProvider = new Mock<IGitInfoProvider>();

            var reporters = new string[1];
            reporters[0] = "dashboard";

            var options = new StrykerOptions(
                compareToDashboard: true,
               dashboardApiKey: "Acces_Token",
               projectName: "github.com/JohnDoe/project",
               projectVersion: "version/human/readable",
               reporters: reporters,
               fallbackVersion: "fallback/version");

            var inputComponent = new Mock<IReadOnlyProjectComponent>().Object;

            var jsonReport = JsonReport.Build(options, inputComponent);

            gitInfoProvider.Setup(x => x.GetCurrentBranchName()).Returns("refs/heads/master");

            baselineProvider.Setup(x => x.Load("dashboard-compare/refs/heads/master")).Returns(Task.FromResult(jsonReport));

            // Act
            var target = new DashboardMutantFilter(options, gitInfoProvider: gitInfoProvider.Object, baselineProvider: baselineProvider.Object);

            // Assert
            baselineProvider.Verify(x => x.Load("dashboard-compare/refs/heads/master"), Times.Once);
            baselineProvider.Verify(x => x.Load("fallback/version"), Times.Never);
        }

        [Fact]
        public void FilterMutantsReturnAllMutantsWhenCompareToDashboardEnabledAndBaselineNotAvailabe()
        {
            // Arrange
            var baselineProvider = new Mock<IBaselineProvider>();
            var branchProvider = new Mock<IGitInfoProvider>();

            var options = new StrykerOptions(compareToDashboard: true, projectVersion: "version");

            var target = new DashboardMutantFilter(options, baselineProvider.Object, branchProvider.Object);

            var file = new Mock<ReadOnlyFileLeaf>(new FileLeaf());

            var mutants = new List<Mutant>
            {
                new Mutant(),
                new Mutant(),
                new Mutant()
            };

            // Act
            var results = target.FilterMutants(mutants, file.Object, options);

            // Assert
            results.Count().ShouldBe(3);
        }
    }
}
