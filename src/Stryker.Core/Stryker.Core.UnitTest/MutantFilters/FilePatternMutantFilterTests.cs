using Shouldly;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class FilePatternMutantFilterTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void MutantFilter_ShouldSkipMutationsForExcludedFiles(bool fileExcluded)
        {
            
        }
    }
}