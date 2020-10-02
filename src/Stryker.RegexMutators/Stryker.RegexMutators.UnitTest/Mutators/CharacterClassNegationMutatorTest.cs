﻿using RegexParser.Nodes;
using RegexParser.Nodes.CharacterClass;
using Shouldly;
using Stryker.RegexMutators.Mutators;
using System.Collections.Generic;
using Xunit;

namespace Stryker.RegexMutators.UnitTest.Mutators
{
    public class CharacterClassNegationMutatorTest
    {
        [Fact]
        public void ShouldNegateUnnegatedCharacterClass()
        {
            // Arrange
            var characters = new List<RegexNode> { new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c') };
            var characterSet = new CharacterClassCharacterSetNode(characters);
            var subtractionCharacterSet = new CharacterClassCharacterSetNode(new CharacterNode('a'));
            var subtraction = new CharacterClassNode(subtractionCharacterSet, false);
            var characterClass = new CharacterClassNode(characterSet, subtraction, false);
            var childNodes = new List<RegexNode> { new CharacterNode('x'), characterClass, new CharacterNode('y') };
            var root = new ConcatenationNode(childNodes);
            var target = new CharacterClassNegationMutator();

            // Act
            var result = target.ApplyMutations(characterClass, root);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.OriginalNode.ShouldBe(characterClass);
            mutation.ReplacementNode.ToString().ShouldBe("[^abc-[a]]");
            mutation.ReplacementPattern.ShouldBe("x[^abc-[a]]y");
            mutation.DisplayName.ShouldBe("Regex character class negation mutation");
            mutation.Description.ShouldBe("Character class \"[abc-[a]]\" was replaced with \"[^abc-[a]]\" at offset 1.");
        }

        [Fact]
        public void ShouldUnnegateNegatedCharacterClass()
        {
            // Arrange
            var characters = new List<RegexNode> { new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c') };
            var characterSet = new CharacterClassCharacterSetNode(characters);
            var subtractionCharacterSet = new CharacterClassCharacterSetNode(new CharacterNode('a'));
            var subtraction = new CharacterClassNode(subtractionCharacterSet, false);
            var characterClass = new CharacterClassNode(characterSet, subtraction, true);
            var childNodes = new List<RegexNode> { new CharacterNode('x'), characterClass, new CharacterNode('y') };
            var root = new ConcatenationNode(childNodes);
            var target = new CharacterClassNegationMutator();

            // Act
            var result = target.ApplyMutations(characterClass, root);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.OriginalNode.ShouldBe(characterClass);
            mutation.ReplacementNode.ToString().ShouldBe("[abc-[a]]");
            mutation.ReplacementPattern.ShouldBe("x[abc-[a]]y");
            mutation.DisplayName.ShouldBe("Regex character class negation mutation");
            mutation.Description.ShouldBe("Character class \"[^abc-[a]]\" was replaced with \"[abc-[a]]\" at offset 1.");
        }

        [Fact]
        public void MutateShouldNotMutateNonCharacterClassNode()
        {
            // Arrange
            var characterNode = new CharacterNode('a');
            var root = new ConcatenationNode(characterNode);
            var target = new CharacterClassNegationMutator();

            // Act
            var result = target.Mutate(characterNode, root);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
