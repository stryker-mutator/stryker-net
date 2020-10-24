using RegexParser.Nodes;
using Shouldly;
using Stryker.RegexMutators.Mutators;
using System.Collections.Generic;
using System.Linq;
using RegexParser.Nodes.GroupNodes;
using Xunit;

namespace Stryker.RegexMutators.UnitTest.Mutators
{
    public class LookAroundMutatorTest
    {
        [Fact]
        public void FlipsPositiveLookAheadToNegativeLookAhead()
        {
            // Arrange
            var foo = new List<RegexNode>
            {
                new CharacterNode('f'), 
                new CharacterNode('o'), 
                new CharacterNode('o')
            };
            var lookaroundGroupNode = new LookaroundGroupNode(true, true, foo);
            var rootNode = new ConcatenationNode(lookaroundGroupNode);
            var target = new LookAroundMutator();

            var expectedResults = new List<string>
            {
                "(?!foo)",
                "(?<=foo)"
            };

            // Act
            var mutations = target.ApplyMutations(lookaroundGroupNode, rootNode).ToList();

            // Assert
            var index = 0;
            var originalQuantifier = "(?=foo)";
            foreach (var mutation in mutations)
            {
                mutation.OriginalNode.ShouldBe(lookaroundGroupNode);
                mutation.ReplacementNode.ToString().ShouldBe(expectedResults[index]);
                mutation.ReplacementPattern.ShouldBe(expectedResults[index]);
                mutation.DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");
                mutation.Description.ShouldBe($"Quantifier \"{originalQuantifier}\" was replaced with \"{expectedResults[index]}\" at offset 0.");
                index++;
            }
            mutations.Count.ShouldBe(2);
        }
        
        [Fact]
        public void FlipsNegativeLookAheadToPositiveLookAhead()
        {
            // Arrange
            var foo = new List<RegexNode>
            {
                new CharacterNode('f'), 
                new CharacterNode('o'), 
                new CharacterNode('o')
            };
            var lookaroundGroupNode = new LookaroundGroupNode(true, false, foo);
            var rootNode = new ConcatenationNode(lookaroundGroupNode);
            var target = new LookAroundMutator();

            var expectedResults = new List<string>
            {
                "(?=foo)",
                "(?<!foo)"
            };

            // Act
            var mutations = target.ApplyMutations(lookaroundGroupNode, rootNode).ToList();

            // Assert
            var index = 0;
            var originalQuantifier = "(?!foo)";
            foreach (var mutation in mutations)
            {
                mutation.OriginalNode.ShouldBe(lookaroundGroupNode);
                mutation.ReplacementNode.ToString().ShouldBe(expectedResults[index]);
                mutation.ReplacementPattern.ShouldBe(expectedResults[index]);
                mutation.DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");
                mutation.Description.ShouldBe($"Quantifier \"{originalQuantifier}\" was replaced with \"{expectedResults[index]}\" at offset 0.");
                index++;
            }
            mutations.Count.ShouldBe(2);
        }
    }
}
