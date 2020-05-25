using RegexParser.Nodes;
using RegexParser.Nodes.AnchorNodes;
using Shouldly;
using Stryker.RegexMutators.Mutators;
using System.Collections.Generic;
using Xunit;

namespace Stryker.RegexMutators.UnitTest.Mutators
{
    public class AnchorRemovalMutatorTest
    {
        [Fact]
        public void ShouldRemoveStartOfLineNode()
        {
            // Arrange
            var startOfLineNode = new StartOfLineNode();
            var childNodes = new List<RegexNode>
            {
                startOfLineNode,
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c')
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new AnchorRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(startOfLineNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }

        [Fact]
        public void ShouldRemoveEndOfLineNode()
        {
            // Arrange
            var endOfLineNode = new EndOfLineNode();
            var childNodes = new List<RegexNode>
            {
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c'),
                endOfLineNode
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new AnchorRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(endOfLineNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }

        [Fact]
        public void ShouldRemoveStartOfStringNode()
        {
            // Arrange
            var startOfStringNode = new StartOfStringNode();
            var childNodes = new List<RegexNode>
            {
                startOfStringNode,
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c')
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new AnchorRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(startOfStringNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }

        [Fact]
        public void ShouldRemoveEndOfStringNode()
        {
            // Arrange
            var endOfStringNode = new EndOfStringNode();
            var childNodes = new List<RegexNode>
            {
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c'),
                endOfStringNode
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new AnchorRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(endOfStringNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }

        [Fact]
        public void ShouldRemoveEndOfStringZNode()
        {
            // Arrange
            var endOfStringZNode = new EndOfStringZNode();
            var childNodes = new List<RegexNode>
            {
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c'),
                endOfStringZNode
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new AnchorRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(endOfStringZNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }

        [Fact]
        public void ShouldRemoveWordBoundaryNode()
        {
            // Arrange
            var wordBoundaryNode = new WordBoundaryNode();
            var childNodes = new List<RegexNode>
            {
                wordBoundaryNode,
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c')
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new AnchorRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(wordBoundaryNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }

        [Fact]
        public void ShouldRemoveNonWordBoundaryNode()
        {
            // Arrange
            var nonWordBoundaryNode = new NonWordBoundaryNode();
            var childNodes = new List<RegexNode>
            {
                nonWordBoundaryNode,
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c')
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new AnchorRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(nonWordBoundaryNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }

        [Fact]
        public void ShouldRemoveContiguousMatchNode()
        {
            // Arrange
            var contiguousMatchNode = new ContiguousMatchNode();
            var childNodes = new List<RegexNode>
            {
                contiguousMatchNode,
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c')
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new AnchorRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(contiguousMatchNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }

        [Fact]
        public void MutateShouldNotMutateNonAnchorNode()
        {
            // Arrange
            var characterNode = new CharacterNode('a');
            var rootNode = new ConcatenationNode(characterNode);
            var target = new AnchorRemovalMutator(rootNode);

            // Act
            var result = target.Mutate(characterNode);

            // Assert
            result.ShouldBeEmpty();
        }
    }
}
