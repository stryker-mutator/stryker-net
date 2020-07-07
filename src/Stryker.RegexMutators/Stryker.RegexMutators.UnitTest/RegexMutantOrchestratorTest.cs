﻿using Shouldly;
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
            mutation.OriginalNode.ToString().ShouldBe("^");
            mutation.ReplacementNode.ShouldBeNull();
            mutation.ReplacementPattern.ShouldBe("abc");
            mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
            mutation.Description.ShouldBe("Anchor \"^\" was removed at offset 0.");
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
            mutation.OriginalNode.ToString().ShouldBe("X?");
            mutation.ReplacementNode.ToString().ShouldBe("X");
            mutation.ReplacementPattern.ShouldBe("abcX");
            mutation.DisplayName.ShouldBe("Regex quantifier removal mutation");
            mutation.Description.ShouldBe("Quantifier \"?\" was removed at offset 4.");
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
            mutation.OriginalNode.ToString().ShouldBe("X??");
            mutation.ReplacementNode.ToString().ShouldBe("X");
            mutation.ReplacementPattern.ShouldBe("abcX");
            mutation.DisplayName.ShouldBe("Regex quantifier removal mutation");
            mutation.Description.ShouldBe("Quantifier \"??\" was removed at offset 4.");
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
            mutation.OriginalNode.ToString().ShouldBe("[XY]");
            mutation.ReplacementNode.ToString().ShouldBe("[^XY]");
            mutation.ReplacementPattern.ShouldBe("abc[^XY]");
            mutation.DisplayName.ShouldBe("Regex character class negation mutation");
            mutation.Description.ShouldBe("Character class \"[XY]\" was replaced with \"[^XY]\" at offset 3.");
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
            mutation.OriginalNode.ToString().ShouldBe("[^XY]");
            mutation.ReplacementNode.ToString().ShouldBe("[XY]");
            mutation.ReplacementPattern.ShouldBe("abc[XY]");
            mutation.DisplayName.ShouldBe("Regex character class negation mutation");
            mutation.Description.ShouldBe("Character class \"[^XY]\" was replaced with \"[XY]\" at offset 3.");
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
            mutation.OriginalNode.ToString().ShouldBe("\\d");
            mutation.ReplacementNode.ToString().ShouldBe("\\D");
            mutation.ReplacementPattern.ShouldBe("abc\\D");
            mutation.DisplayName.ShouldBe("Regex character class shorthand negation mutation");
            mutation.Description.ShouldBe("Character class shorthand \"\\d\" was replaced with \"\\D\" at offset 3.");
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
            mutation.OriginalNode.ToString().ShouldBe("\\D");
            mutation.ReplacementNode.ToString().ShouldBe("\\d");
            mutation.ReplacementPattern.ShouldBe("abc\\d");
            mutation.DisplayName.ShouldBe("Regex character class shorthand negation mutation");
            mutation.Description.ShouldBe("Character class shorthand \"\\D\" was replaced with \"\\d\" at offset 3.");
        }

        [Fact]
        public void ShouldApplyMultipleMutations()
        {
            // Arrange
            var target = new RegexMutantOrchestrator(@"^[abc]\d?");

            // Act
            var result = target.Mutate();

            // Assert
            result.Count().ShouldBeGreaterThanOrEqualTo(4);
        }
    }
}
