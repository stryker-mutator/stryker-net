using Moq;
using Shouldly;
using Stryker.Core.Clients;
using Stryker.Core.DashboardCompare;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Options;
using System;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class MutantFilterFactoryTests
    {
        [Fact]
        public void MutantFilterFactory_Creates_of_type_BroadcastFilter()
        {
            var result = MutantFilterFactory.Create(new StrykerOptions(diff: false));

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

            // Act
            var result = MutantFilterFactory.Create(strykerOptions);

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
            var branchProviderMock = new Mock<IBranchProvider>(MockBehavior.Loose);
            var dashboardClientMock = new Mock<IDashboardClient>(MockBehavior.Loose);

            // Act
            var result = MutantFilterFactory.Create(strykerOptions, diffProviderMock.Object, dashboardClientMock.Object, branchProviderMock.Object);

            // Assert
            var resultAsBroadcastFilter = result as BroadcastMutantFilter;

            resultAsBroadcastFilter.MutantFilters.Count().ShouldBe(5);

            resultAsBroadcastFilter.MutantFilters.Where(x => x.GetType() == typeof(DiffMutantFilter)).Count().ShouldBe(1);
        }
    }
}
