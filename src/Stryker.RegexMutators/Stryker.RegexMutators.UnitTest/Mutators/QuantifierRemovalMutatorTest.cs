namespace Stryker.RegexMutators.UnitTest.Mutators;
using RegexParser.Nodes;
using RegexParser.Nodes.QuantifierNodes;
using Shouldly;
using Stryker.RegexMutators.Mutators;
using System.Collections.Generic;
using Xunit;

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
        var target = new QuantifierRemovalMutator();

        // Act
        var result = target.ApplyMutations(quantifierNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(quantifierNode);
        mutation.ReplacementNode.ToString().ShouldBe("X");
        mutation.ReplacementPattern.ShouldBe("abcX");
        mutation.DisplayName.ShouldBe("Regex quantifier removal mutation");
        mutation.Description.ShouldBe("Quantifier \"*\" was removed at offset 4.");
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
        var target = new QuantifierRemovalMutator();

        // Act
        var result = target.ApplyMutations(quantifierNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(quantifierNode);
        mutation.ReplacementNode.ToString().ShouldBe("X");
        mutation.ReplacementPattern.ShouldBe("abcX");
        mutation.DisplayName.ShouldBe("Regex quantifier removal mutation");
        mutation.Description.ShouldBe("Quantifier \"+\" was removed at offset 4.");
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
        var target = new QuantifierRemovalMutator();

        // Act
        var result = target.ApplyMutations(quantifierNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(quantifierNode);
        mutation.ReplacementNode.ToString().ShouldBe("X");
        mutation.ReplacementPattern.ShouldBe("abcX");
        mutation.DisplayName.ShouldBe("Regex quantifier removal mutation");
        mutation.Description.ShouldBe("Quantifier \"?\" was removed at offset 4.");
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
        var target = new QuantifierRemovalMutator();

        // Act
        var result = target.ApplyMutations(quantifierNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(quantifierNode);
        mutation.ReplacementNode.ToString().ShouldBe("X");
        mutation.ReplacementPattern.ShouldBe("abcX");
        mutation.DisplayName.ShouldBe("Regex quantifier removal mutation");
        mutation.Description.ShouldBe("Quantifier \"{5}\" was removed at offset 4.");
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
        var target = new QuantifierRemovalMutator();

        // Act
        var result = target.ApplyMutations(quantifierNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(quantifierNode);
        mutation.ReplacementNode.ToString().ShouldBe("X");
        mutation.ReplacementPattern.ShouldBe("abcX");
        mutation.DisplayName.ShouldBe("Regex quantifier removal mutation");
        mutation.Description.ShouldBe("Quantifier \"{5,}\" was removed at offset 4.");
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
        var target = new QuantifierRemovalMutator();

        // Act
        var result = target.ApplyMutations(quantifierNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(quantifierNode);
        mutation.ReplacementNode.ToString().ShouldBe("X");
        mutation.ReplacementPattern.ShouldBe("abcX");
        mutation.DisplayName.ShouldBe("Regex quantifier removal mutation");
        mutation.Description.ShouldBe("Quantifier \"{5,10}\" was removed at offset 4.");
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
        var target = new QuantifierRemovalMutator();

        // Act
        var result = target.ApplyMutations(quantifierNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(lazyNode);
        mutation.ReplacementNode.ToString().ShouldBe("X");
        mutation.ReplacementPattern.ShouldBe("abcX");
        mutation.DisplayName.ShouldBe("Regex quantifier removal mutation");
        mutation.Description.ShouldBe("Quantifier \"*?\" was removed at offset 4.");
    }

    [Fact]
    public void MutateShouldNotMutateNonQuantifierNode()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var rootNode = new ConcatenationNode(characterNode);
        var target = new QuantifierRemovalMutator();

        // Act
        var result = target.Mutate(characterNode, rootNode);

        // Assert
        result.ShouldBeEmpty();
    }
}
