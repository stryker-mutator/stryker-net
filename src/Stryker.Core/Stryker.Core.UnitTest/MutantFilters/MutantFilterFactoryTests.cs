using System;
using System.Linq;
using Moq;
using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.DashboardCompare;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class MutantFilterFactoryTests
    {
        [Fact]
        public void MutantFilterFactory_Creates_of_type_BroadcastFilter()
        {
            var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
            var baselineProvider = new Mock<IBaselineProvider>(MockBehavior.Loose);

            var result = MutantFilterFactory.Create(new StrykerOptions(diff: false), diffProviderMock.Object, baselineProvider.Object, branchProviderMock.Object);

            result.ShouldBeOfType<BroadcastMutantFilter>();
        }

        [Fact]
        public void Create_Throws_ArgumentNullException_When_Stryker_Options_Is_Null()
        {
            var result = Should.Throw<ArgumentNullException>(() => MutantFilterFactory.Create(null));
        }

        [Fact]
        public void MutantFilterFactory_Creates_Standard_Mutant_Filters()
        {
            // Arrange
            var strykerOptions = new StrykerOptions(diff: false);

            var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
            var baselineProvider = new Mock<IBaselineProvider>(MockBehavior.Loose);

            // Act
            var result = MutantFilterFactory.Create(strykerOptions, diffProviderMock.Object, baselineProvider.Object, branchProviderMock.Object);

            // Assert
            var resultAsBroadcastFilter = result as BroadcastMutantFilter;

            resultAsBroadcastFilter.MutantFilters.Count().ShouldBe(4);
        }

        [Fact]
        public void MutantFilterFactory_Creates_DiffMutantFilter_When_Diff_Enabled()
        {
            // Arrange
            var strykerOptions = new StrykerOptions(diff: true);

            var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
            var branchProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
            var baselineProvider = new Mock<IBaselineProvider>(MockBehavior.Loose);

            // Act
            var result = MutantFilterFactory.Create(strykerOptions, diffProviderMock.Object, baselineProvider.Object, branchProviderMock.Object);

            // Assert
            var resultAsBroadcastFilter = result as BroadcastMutantFilter;

            resultAsBroadcastFilter.MutantFilters.Count().ShouldBe(5);

            resultAsBroadcastFilter.MutantFilters.Where(x => x.GetType() == typeof(DiffMutantFilter)).Count().ShouldBe(1);
        }

        [Fact]
        public void MutantFilterFactory_Creates_DashboardMutantFilter_And_DiffMutantFilter_Dashboard_Compare_Enabled() {
            var strykerOptions = new StrykerOptions(compareToDashboard: true, projectVersion: "foo");

            var diffProviderMock = new Mock<IDiffProvider>(MockBehavior.Loose);
            var gitInfoProviderMock = new Mock<IGitInfoProvider>(MockBehavior.Loose);
            var baselineProviderMock = new Mock<IBaselineProvider>(MockBehavior.Loose);

            var result = MutantFilterFactory.Create(strykerOptions, diffProviderMock.Object, baselineProviderMock.Object, gitInfoProviderMock.Object);

            var resultAsBroadcastFilter = result as BroadcastMutantFilter;

            resultAsBroadcastFilter.MutantFilters.Count().ShouldBe(6);
            resultAsBroadcastFilter.MutantFilters.Where(x => x.GetType() == typeof(DashboardMutantFilter)).Count().ShouldBe(1);
            resultAsBroadcastFilter.MutantFilters.Where(x => x.GetType() == typeof(DiffMutantFilter)).Count().ShouldBe(1);
        }
    }
}
