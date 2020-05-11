using Shouldly;
using System.Linq;
using Xunit;

namespace Stryker.RegexMutators.UnitTest
{
    public class RegexMutantOrchestratorTest
    {
        [Fact]
        public void ShouldRemoveAnchor()
        {
            // Arrange
            var target = new RegexMutantOrchestrator("^abc");

            // Act
            var result = target.Mutate();

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }

        [Fact]
        public void ShouldRemoveQuantifier()
        {
            // Arrange
            var target = new RegexMutantOrchestrator("abcX?");

            // Act
            var result = target.Mutate();

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abcX");
        }

        [Fact]
        public void ShouldRemoveLazyQuantifier()
        {
            // Arrange
            var target = new RegexMutantOrchestrator("abcX??");

            // Act
            var result = target.Mutate();

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abcX");
        }

        [Fact]
        public void ShouldNegateUnnegatedCharacterClass()
        {
            // Arrange
            var target = new RegexMutantOrchestrator("abc[XY]");

            // Act
            var result = target.Mutate();

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc[^XY]");
        }

        [Fact]
        public void ShouldUnnegateNegatedCharacterClass()
        {
            // Arrange
            var target = new RegexMutantOrchestrator("abc[^XY]");

            // Act
            var result = target.Mutate();

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc[XY]");
        }

        [Fact]
        public void ShouldNegateUnnegatedCharacterClassShorthand()
        {
            // Arrange
            var target = new RegexMutantOrchestrator(@"abc\d");

            // Act
            var result = target.Mutate();

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe(@"abc\D");
        }

        [Fact]
        public void ShouldUnnegateNegatedCharacterClassShorthand()
        {
            // Arrange
            var target = new RegexMutantOrchestrator(@"abc\D");

            // Act
            var result = target.Mutate();

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe(@"abc\d");
        }

        [Fact]
        public void ShouldApplyMultipleMutations()
        {
            // Arrange
            var target = new RegexMutantOrchestrator(@"^[abc]\d?");

            // Act
            var result = target.Mutate();

            // Assert
            result.Count().ShouldBe(4);
            result.ShouldContain(@"[abc]\d?");
            result.ShouldContain(@"^[^abc]\d?");
            result.ShouldContain(@"^[abc]\D?");
            result.ShouldContain(@"^[abc]\d");
        }
    }
}
