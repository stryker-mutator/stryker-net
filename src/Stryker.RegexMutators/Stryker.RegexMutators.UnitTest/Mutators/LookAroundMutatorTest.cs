using RegexParser.Nodes;
using Shouldly;
using Stryker.RegexMutators.Mutators;
using System.Collections.Generic;
using System.Linq;
using RegexParser.Nodes.GroupNodes;
using Xunit;

namespace Stryker.RegexMutators.UnitTest.Mutators;

public class LookAroundMutatorTest
{
    [Fact]
    public void FlipsPositiveLookBehindToNegativeLookBehind()
    {
        // Arrange
        var foo = new List<RegexNode>
        {
            new CharacterNode('f'),
            new CharacterNode('o'),
            new CharacterNode('o')
        };
        var lookaroundGroupNode = new LookaroundGroupNode(false, true, foo);
        var rootNode = new ConcatenationNode(lookaroundGroupNode);
        var target = new LookAroundMutator();

        // Act
        var result = target.ApplyMutations(lookaroundGroupNode, rootNode).ToList();

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(lookaroundGroupNode);
        mutation.ReplacementNode.ToString().ShouldBe("(?<!foo)");
        mutation.ReplacementPattern.ShouldBe("(?<!foo)");
        mutation.DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");
        mutation.Description.ShouldBe("Quantifier \"(?<=foo)\" was replaced with \"(?<!foo)\" at offset 0.");
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

        // Act
        var result = target.ApplyMutations(lookaroundGroupNode, rootNode).ToList();

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(lookaroundGroupNode);
        mutation.ReplacementNode.ToString().ShouldBe("(?=foo)");
        mutation.ReplacementPattern.ShouldBe("(?=foo)");
        mutation.DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");
        mutation.Description.ShouldBe("Quantifier \"(?!foo)\" was replaced with \"(?=foo)\" at offset 0.");
    }
}
