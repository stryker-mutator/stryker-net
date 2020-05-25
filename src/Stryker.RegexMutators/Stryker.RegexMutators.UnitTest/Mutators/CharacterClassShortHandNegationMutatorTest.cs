using RegexParser.Nodes;
using Shouldly;
using Stryker.RegexMutators.Mutators;
using System.Collections.Generic;
using Xunit;

namespace Stryker.RegexMutators.UnitTest.Mutators
{
    public class CharacterClassShortHandNegationMutatorTest
    {
        [Fact]
        public void ShouldNegateUnnegatedShorthand()
        {
            // Arrange
            var shorthandNode = new CharacterClassShorthandNode('d');
            var childNodes = new List<RegexNode>
            {
                shorthandNode,
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c')
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new CharacterClassShorthandNegationMutator(rootNode);

            // Act
            var result = target.ApplyMutations(shorthandNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe(@"\Dabc");
        }

        [Fact]
        public void ShouldUnnegateNegatedShorthand()
        {
            // Arrange
            var shorthandNode = new CharacterClassShorthandNode('D');
            var childNodes = new List<RegexNode>
            {
                shorthandNode,
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c')
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new CharacterClassShorthandNegationMutator(rootNode);

            // Act
            var result = target.ApplyMutations(shorthandNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe(@"\dabc");
        }

        [Fact]
        public void MutateShouldNotMutateNonCharacterClassShorthandNode()
        {
            // Arrange
            var characterNode = new CharacterNode('a');
            var rootNode = new ConcatenationNode(characterNode);
            var target = new CharacterClassShorthandNegationMutator(rootNode);

            // Act
            var result = target.Mutate(characterNode);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
