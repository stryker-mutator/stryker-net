using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.QuantifierNodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public class QuantifierRemovalMutatorTest
{
    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
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
