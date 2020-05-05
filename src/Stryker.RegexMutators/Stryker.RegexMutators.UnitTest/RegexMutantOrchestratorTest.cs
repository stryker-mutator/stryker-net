using Shouldly;
using Xunit;

namespace Stryker.RegexMutators.UnitTest
{
    public class RegexMutantOrchestratorTest
    {
        [Fact]
        public void Test()
        {
            // Arrange
            var target = new RegexMutantOrchestrator("^abc");

            // Act
            var result = target.Mutate();

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }
    }
}
