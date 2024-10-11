using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.AnchorNodes;
using Stryker.Regex.Parser.Nodes.GroupNodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public class AnchorRemovalMutatorTest
{
    [TestMethod]
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
        var target = new AnchorRemovalMutator();

        // Act
        var result = target.ApplyMutations(startOfLineNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(startOfLineNode);
        mutation.ReplacementNode.ShouldBeNull();
        mutation.ReplacementPattern.ShouldBe("abc");
        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        mutation.Description.ShouldBe("Anchor \"^\" was removed at offset 0.");
    }

    [TestMethod]
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
        var target = new AnchorRemovalMutator();

        // Act
        var result = target.ApplyMutations(endOfLineNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(endOfLineNode);
        mutation.ReplacementNode.ShouldBeNull();
        mutation.ReplacementPattern.ShouldBe("abc");
        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        mutation.Description.ShouldBe("Anchor \"$\" was removed at offset 3.");
    }

    [TestMethod]
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
        var target = new AnchorRemovalMutator();

        // Act
        var result = target.ApplyMutations(startOfStringNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(startOfStringNode);
        mutation.ReplacementNode.ShouldBeNull();
        mutation.ReplacementPattern.ShouldBe("abc");
        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        mutation.Description.ShouldBe("Anchor \"\\A\" was removed at offset 0.");
    }

    [TestMethod]
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
        var target = new AnchorRemovalMutator();

        // Act
        var result = target.ApplyMutations(endOfStringNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(endOfStringNode);
        mutation.ReplacementNode.ShouldBeNull();
        mutation.ReplacementPattern.ShouldBe("abc");
        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        mutation.Description.ShouldBe("Anchor \"\\z\" was removed at offset 3.");
    }

    [TestMethod]
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
        var target = new AnchorRemovalMutator();

        // Act
        var result = target.ApplyMutations(endOfStringZNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(endOfStringZNode);
        mutation.ReplacementNode.ShouldBeNull();
        mutation.ReplacementPattern.ShouldBe("abc");
        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        mutation.Description.ShouldBe("Anchor \"\\Z\" was removed at offset 3.");
    }

    [TestMethod]
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
        var target = new AnchorRemovalMutator();

        // Act
        var result = target.ApplyMutations(wordBoundaryNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(wordBoundaryNode);
        mutation.ReplacementNode.ShouldBeNull();
        mutation.ReplacementPattern.ShouldBe("abc");
        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        mutation.Description.ShouldBe("Anchor \"\\b\" was removed at offset 0.");
    }

    [TestMethod]
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
        var target = new AnchorRemovalMutator();

        // Act
        var result = target.ApplyMutations(nonWordBoundaryNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(nonWordBoundaryNode);
        mutation.ReplacementNode.ShouldBeNull();
        mutation.ReplacementPattern.ShouldBe("abc");
        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        mutation.Description.ShouldBe("Anchor \"\\B\" was removed at offset 0.");
    }

    [TestMethod]
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
        var target = new AnchorRemovalMutator();

        // Act
        var result = target.ApplyMutations(contiguousMatchNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(contiguousMatchNode);
        mutation.ReplacementNode.ShouldBeNull();
        mutation.ReplacementPattern.ShouldBe("abc");
        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        mutation.Description.ShouldBe("Anchor \"\\G\" was removed at offset 0.");
    }

    [TestMethod]
    public void MutateShouldNotMutateNonAnchorNode()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var rootNode = new ConcatenationNode(characterNode);
        var target = new AnchorRemovalMutator();

        // Act
        var result = target.Mutate(characterNode, rootNode);

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void MutationShouldNotContainOriginalNodesPrefixInDescription()
    {
        // Arrange
        var prefix = new CommentGroupNode("This is a comment.");
        var startOfLineNode = new StartOfLineNode() { Prefix = prefix };
        var childNodes = new List<RegexNode>
        {
            startOfLineNode,
            new CharacterNode('a'),
            new CharacterNode('b'),
            new CharacterNode('c')
        };
        var rootNode = new ConcatenationNode(childNodes);
        var target = new AnchorRemovalMutator();

        // Act
        var result = target.ApplyMutations(startOfLineNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(startOfLineNode);
        mutation.ReplacementNode.ShouldBeNull();
        mutation.ReplacementPattern.ShouldBe("(?#This is a comment.)abc");
        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        mutation.Description.ShouldBe("Anchor \"^\" was removed at offset 22.");
    }
}
