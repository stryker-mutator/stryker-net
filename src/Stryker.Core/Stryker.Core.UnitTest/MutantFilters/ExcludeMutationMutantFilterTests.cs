using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;

namespace Stryker.Core.UnitTest.MutantFilters;

[TestClass]
public class ExcludeMutationMutantFilterTests : TestBase
{
    [TestMethod]
    public void ShouldHaveName()
    {
        var target = new IgnoreMutationMutantFilter() as IMutantFilter;
        target.DisplayName.ShouldBe("mutation type filter");
    }

    [TestMethod]
    [DataRow(Mutator.Arithmetic, true)]
    [DataRow(Mutator.Assignment, false)]
    public void MutantFilter_ShouldSkipMutationsForExcludedMutatorType(Mutator excludedMutation, bool skipped)
    {
        // Arrange
        var mutant = new Mutant
        {
            Mutation = new Mutation
            {
                Type = Mutator.Arithmetic,
            }
        };

        var sut = new IgnoreMutationMutantFilter();

        var options = new StrykerOptions
        {
            ExcludedMutations = new[] { excludedMutation }
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
