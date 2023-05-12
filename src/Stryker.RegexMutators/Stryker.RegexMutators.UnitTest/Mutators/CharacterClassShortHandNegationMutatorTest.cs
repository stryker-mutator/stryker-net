namespace Stryker.RegexMutators.UnitTest.Mutators;
using RegexParser.Nodes;
using Shouldly;
using Stryker.RegexMutators.Mutators;
using System.Collections.Generic;
using Xunit;

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
        var target = new CharacterClassShorthandNegationMutator();

        // Act
        var result = target.ApplyMutations(shorthandNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(shorthandNode);
        mutation.ReplacementNode.ToString().ShouldBe("\\D");
        mutation.ReplacementPattern.ShouldBe("\\Dabc");
        mutation.DisplayName.ShouldBe("Regex character class shorthand negation mutation");
        mutation.Description.ShouldBe("Character class shorthand \"\\d\" was replaced with \"\\D\" at offset 0.");
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
        var target = new CharacterClassShorthandNegationMutator();

        // Act
        var result = target.ApplyMutations(shorthandNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(shorthandNode);
        mutation.ReplacementNode.ToString().ShouldBe("\\d");
        mutation.ReplacementPattern.ShouldBe("\\dabc");
        mutation.DisplayName.ShouldBe("Regex character class shorthand negation mutation");
        mutation.Description.ShouldBe("Character class shorthand \"\\D\" was replaced with \"\\d\" at offset 0.");
    }

    [Fact]
    public void MutateShouldNotMutateNonCharacterClassShorthandNode()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var rootNode = new ConcatenationNode(characterNode);
        var target = new CharacterClassShorthandNegationMutator();

        // Act
        var result = target.Mutate(characterNode, rootNode);

        // Assert
        result.ShouldBeEmpty();
    }
}
