using RegexParser.Nodes;
using RegexParser.Nodes.QuantifierNodes;
using Shouldly;
using Stryker.RegexMutators.Mutators;
using System.Collections.Generic;
using Xunit;

namespace Stryker.RegexMutators.UnitTest.Mutators
{
    public class QuantifierRemovalMutatorTest
    {
        [Fact]
        public void ShouldRemoveQuantifierStar()
        {
            // Arrange
            var quantifierNode = new QuantifierStarNode(new CharacterNode('X'));
            var childNodes = new List<RegexNode>
            {
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c'),
                quantifierNode
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new QuantifierRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(quantifierNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abcX");
        }

        [Fact]
        public void ShouldRemoveQuantifierPlus()
        {
            // Arrange
            var quantifierNode = new QuantifierPlusNode(new CharacterNode('X'));
            var childNodes = new List<RegexNode>
            {
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c'),
                quantifierNode
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new QuantifierRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(quantifierNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abcX");
        }

        [Fact]
        public void ShouldRemoveQuantifierQuestionMark()
        {
            // Arrange
            var quantifierNode = new QuantifierQuestionMarkNode(new CharacterNode('X'));
            var childNodes = new List<RegexNode>
            {
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c'),
                quantifierNode
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new QuantifierRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(quantifierNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abcX");
        }

        [Fact]
        public void ShouldRemoveQuantifierN()
        {
            // Arrange
            var quantifierNode = new QuantifierNNode(5, new CharacterNode('X'));
            var childNodes = new List<RegexNode>
            {
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c'),
                quantifierNode
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new QuantifierRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(quantifierNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abcX");
        }

        [Fact]
        public void ShouldRemoveQuantifierNOrMore()
        {
            // Arrange
            var quantifierNode = new QuantifierNOrMoreNode(5, new CharacterNode('X'));
            var childNodes = new List<RegexNode>
            {
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c'),
                quantifierNode
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new QuantifierRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(quantifierNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abcX");
        }

        [Fact]
        public void ShouldRemoveQuantifierNM()
        {
            // Arrange
            var quantifierNode = new QuantifierNMNode(5, 10, new CharacterNode('X'));
            var childNodes = new List<RegexNode>
            {
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c'),
                quantifierNode
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new QuantifierRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(quantifierNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abcX");
        }

        [Fact]
        public void ShouldRemoveLazyQuantifier()
        {
            // Arrange
            var quantifierNode = new QuantifierStarNode(new CharacterNode('X'));
            var lazyNode = new LazyNode(quantifierNode);
            var childNodes = new List<RegexNode>
            {
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c'),
                lazyNode
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new QuantifierRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(quantifierNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abcX");
        }
    }
}
