using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.QuantifierNodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public class QuantifierUnlimitedQuantityMutatorTest
{
    [TestMethod]
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

    [TestMethod]
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
