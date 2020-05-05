using RegexParser.Nodes;
using RegexParser.Nodes.AnchorNodes;
using Shouldly;
using Stryker.RegexMutators;
using System.Collections.Generic;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class RegexMutatorTest
    {
        [Fact]
        public void ShouldRemoveAnchorNode()
        {
            // Arrange
            var anchorNode = new StartOfLineNode();
            var childNodes = new List<RegexNode>
            {
                anchorNode,
                new CharacterNode('a'),
                new CharacterNode('b'),
                new CharacterNode('c')
            };
            var rootNode = new ConcatenationNode(childNodes);
            var target = new AnchorRemovalMutator(rootNode);

            // Act
            var result = target.ApplyMutations(anchorNode);

            // Assert
            var mutation = result.ShouldHaveSingleItem();
            mutation.ShouldBe("abc");
        }
    }
}
