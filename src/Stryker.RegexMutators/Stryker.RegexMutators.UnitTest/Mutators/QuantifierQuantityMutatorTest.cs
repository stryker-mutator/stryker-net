using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.QuantifierNodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public class QuantifierQuantityMutatorTest
{
    [TestMethod]
    public void ShouldApplyVariationsOnRegularInput()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var quantifierNode = new QuantifierNMNode(5, 8, characterNode);
        var rootNode = new ConcatenationNode(quantifierNode);
        var target = new QuantifierQuantityMutator();

        var expectedResults = new List<string>
        {
            "a{4,8}",
            "a{6,8}",
            "a{5,7}",
            "a{5,9}",
        };

        // Act
        var mutations = target.ApplyMutations(quantifierNode, rootNode).ToList();

        // Assert
        var index = 0;
        var originalQuantifier = "a{5,8}";
        foreach (var mutation in mutations)
        {
            mutation.OriginalNode.ShouldBe(quantifierNode);
            mutation.ReplacementNode.ToString().ShouldBe(expectedResults[index]);
            mutation.ReplacementPattern.ShouldBe(expectedResults[index]);
            mutation.DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");
            mutation.Description.ShouldBe($"Quantifier \"{originalQuantifier}\" was replaced with \"{expectedResults[index]}\" at offset 1.");
            index++;
        }
        mutations.Count.ShouldBe(4);
    }

    [TestMethod]
    public void ShouldSkipDecrementOnZeroStartValue()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var quantifierNode = new QuantifierNMNode(0, 8, characterNode);
        var rootNode = new ConcatenationNode(quantifierNode);
        var target = new QuantifierQuantityMutator();

        var expectedResults = new List<string>
        {
            "a{1,8}",
            "a{0,7}",
            "a{0,9}",
        };

        // Act
        var mutations = target.ApplyMutations(quantifierNode, rootNode).ToList();

        // Assert
        var index = 0;
        var originalQuantifier = "a{0,8}";
        foreach (var mutation in mutations)
        {
            mutation.OriginalNode.ShouldBe(quantifierNode);
            mutation.ReplacementNode.ToString().ShouldBe(expectedResults[index]);
            mutation.ReplacementPattern.ShouldBe(expectedResults[index]);
            mutation.DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");
            mutation.Description.ShouldBe($"Quantifier \"{originalQuantifier}\" was replaced with \"{expectedResults[index]}\" at offset 1.");
            index++;
        }
        mutations.Count.ShouldBe(3);
    }

    [TestMethod]
    public void ShouldSkipDecrementOnZeroEndValue()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var quantifierNode = new QuantifierNMNode(0, 0, characterNode);
        var rootNode = new ConcatenationNode(quantifierNode);
        var target = new QuantifierQuantityMutator();

        var expectedResults = new List<string>
        {
            "a{0,1}",
        };

        // Act
        var mutations = target.ApplyMutations(quantifierNode, rootNode).ToList();

        // Assert
        var index = 0;
        var originalQuantifier = "a{0,0}";
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

    [TestMethod]
    public void ShouldSkipStartValueHigherThanEndValue()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var quantifierNode = new QuantifierNMNode(8, 8, characterNode);
        var rootNode = new ConcatenationNode(quantifierNode);
        var target = new QuantifierQuantityMutator();

        var expectedResults = new List<string>
        {
            "a{7,8}",
            "a{8,9}",
        };

        // Act
        var mutations = target.ApplyMutations(quantifierNode, rootNode).ToList();

        // Assert
        var index = 0;
        var originalQuantifier = "a{8,8}";
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


    [TestMethod]
    public void ShouldAcceptInputWithLeadingZeros()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var quantifierNode = new QuantifierNMNode("008", "008", characterNode);
        var rootNode = new ConcatenationNode(quantifierNode);
        var target = new QuantifierQuantityMutator();

        var expectedResults = new List<string>
        {
            "a{7,8}",
            "a{8,9}",
        };

        // Act
        var mutations = target.ApplyMutations(quantifierNode, rootNode).ToList();

        // Assert
        var index = 0;
        var originalQuantifier = "a{008,008}";
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
}
