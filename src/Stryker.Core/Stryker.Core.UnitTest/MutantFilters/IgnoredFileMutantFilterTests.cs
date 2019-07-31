using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class IgnoredFileMutantFilterTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MutantFilter_ShouldSkipMutationsForExcludedFiles(bool fileExcluded)
        {
            // Arrange
            var mutant = new Mutant();

            var sut = new IgnoredFileMutantFilter();

            // Act
            var filteredMutants = sut.FilterMutants(new[] { mutant }, new FileLeaf { IsExcluded = fileExcluded }, null);

            // Assert
            if (fileExcluded)
                filteredMutants.ShouldNotContain(mutant);
            else
                filteredMutants.ShouldContain(mutant);
        }
    }
}