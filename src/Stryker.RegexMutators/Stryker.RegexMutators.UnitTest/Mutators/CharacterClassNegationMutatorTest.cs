using RegexParser.Nodes;
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
            var characterClass = new CharacterClassNode(characterSet, false);
            var childNodes = new List<RegexNode> { new CharacterNode('x'), characterClass, new CharacterNode('y') };
            var root = new ConcatenationNode(childNodes);
            var target = new CharacterClassNegationMutator(root);

            // Act
            var result = target.ApplyMutations(characterClass);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("x[^abc]y");
        }

        [Fact]
        public void ShouldUnnegateNegatedCharacterClass()
        {
            // Arrange
            var characters = new List<RegexNode> { new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c') };
            var characterSet = new CharacterClassCharacterSetNode(characters);
            var characterClass = new CharacterClassNode(characterSet, true);
            var childNodes = new List<RegexNode> { new CharacterNode('x'), characterClass, new CharacterNode('y') };
            var root = new ConcatenationNode(childNodes);
            var target = new CharacterClassNegationMutator(root);

            // Act
            var result = target.ApplyMutations(characterClass);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("x[abc]y");
        }

        [Fact]
        public void MutateShouldNotMutateNonCharacterClassNode()
        {
            // Arrange
            var characterNode = new CharacterNode('a');
            var rootNode = new ConcatenationNode(characterNode);
            var target = new CharacterClassNegationMutator(rootNode);

            // Act
            var result = target.Mutate(characterNode);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
