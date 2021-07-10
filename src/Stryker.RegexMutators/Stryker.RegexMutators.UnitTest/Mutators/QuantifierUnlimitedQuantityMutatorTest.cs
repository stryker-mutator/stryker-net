using RegexParser.Nodes;
using RegexParser.Nodes.QuantifierNodes;
using Shouldly;
using Stryker.RegexMutators.Mutators;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Stryker.RegexMutators.UnitTest.Mutators
{
    public class QuantifierUnlimitedQuantityMutatorTest
    {
        [Fact]
        public void ShouldApplyVariationsOnRegularInput()
        {
            // Arrange
            var characterNode = new CharacterNode('a');
            var quantifierNode = new QuantifierNOrMoreNode(5, characterNode);
            var rootNode = new ConcatenationNode(quantifierNode);
            var target = new QuantifierUnlimitedQuantityMutator();

            var expectedResults = new List<string>
            {
                "a{4,}",
                "a{6,}",
            };

            // Act
            var mutations = target.ApplyMutations(quantifierNode, rootNode).ToList();

            // Assert
            var index = 0;
            var originalQuantifier = "a{5,}";
            foreach (var mutation in mutations)
            {
                mutation.OriginalNode.ShouldBe(quantifierNode);
                mutation.ReplacementNode.ToString().ShouldBe(expectedResults[index]);
                mutation.ReplacementPattern.ShouldBe(expectedResults[index]);
                mutation.DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");
                mutation.Description.ShouldBe($"Quantifier \"{originalQuantifier}\" was replaced with \"{expectedResults[index]}\" at offset 1.");
                index++;
            }
            mutations.Count.ShouldBe(2);
        }

        [Fact]
        public void ShouldSkipDecrementOnZeroStartValue()
        {
            // Arrange
            var characterNode = new CharacterNode('a');
            var quantifierNode = new QuantifierNOrMoreNode(0, characterNode);
            var rootNode = new ConcatenationNode(quantifierNode);
            var target = new QuantifierUnlimitedQuantityMutator();

            var expectedResults = new List<string>
            {
                "a{1,}",
            };

            // Act
            var mutations = target.ApplyMutations(quantifierNode, rootNode).ToList();

            // Assert
            var index = 0;
            var originalQuantifier = "a{0,}";
            foreach (var mutation in mutations)
            {
                mutation.OriginalNode.ShouldBe(quantifierNode);
                mutation.ReplacementNode.ToString().ShouldBe(expectedResults[index]);
                mutation.ReplacementPattern.ShouldBe(expectedResults[index]);
                mutation.DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");
                mutation.Description.ShouldBe($"Quantifier \"{originalQuantifier}\" was replaced with \"{expectedResults[index]}\" at offset 1.");
                index++;
            }
            mutations.Count.ShouldBe(1);
        }
    }
}
