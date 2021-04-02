using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class ExcludeMutationMutantFilterTests
    {
        [Fact]
        public static void ShouldHaveName()
        {
            var target = new ExcludeMutationMutantFilter() as IMutantFilter;
            target.DisplayName.ShouldBe("mutation type filter");
        }

        [Theory]
        [InlineData(Mutator.Arithmetic, true)]
        [InlineData(Mutator.Assignment, false)]
        public void MutantFilter_ShouldSkipMutationsForExcludedMutatorType(Mutator excludedMutator, bool skipped)
        {
            // Arrange
            var mutant = new Mutant
            {
                Mutation = new Mutation
                {
                    Type = Mutator.Arithmetic,
                }
            };

            var sut = new ExcludeMutationMutantFilter();

            var options = new StrykerOptions
            {
                ExcludedMutators = new[] { excludedMutator }
            };

            // Act
            var filteredMutants = sut.FilterMutants(
                new[] { mutant },
                null,
                options);

            // Assert
            if (skipped)
            {
                filteredMutants.ShouldNotContain(mutant);
            }
            else
            {
                filteredMutants.ShouldContain(mutant);
            }
        }
    }
}
