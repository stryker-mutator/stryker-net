using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.CharacterClass;
using Stryker.Regex.Parser.Nodes.QuantifierNodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public sealed class QuantifierReluctantAdditionMutatorTests
{
    [TestMethod]
    [QuantifierReluctantAddition("ab+", ["ab+?"])]
    [QuantifierReluctantAddition("ab*", ["ab*?"])]
    [QuantifierReluctantAddition("ab?", ["ab??"])]
    [QuantifierReluctantAddition("ab{2,}", ["ab{2,}?"])]
    public void QuantifierReluctantAddition(string input, string[] expected)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(input, new QuantifierReluctantAdditionMutator());

        // Assert
        result.Select(static a => a.ReplacementPattern).ToArray().ShouldBeEquivalentTo(expected);
    }

    [TestMethod]
    [DataRow("ab+?")]
    [DataRow("ab*?")]
    [DataRow("ab??")]
    [DataRow("ab{2,}?")]
    public void DoesNotMutateLazyQuantifierNodes(string pattern)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(pattern, new QuantifierReluctantAdditionMutator());

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    [DataRow("(abc)")]
    [DataRow("(?:def)")]
    [DataRow("Alice")]
    [DataRow(@"\d\w")]
    public void DoesNotMutateNonQuantityNodes(string pattern)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(pattern, new QuantifierReluctantAdditionMutator());

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateQuantifier()
    {
        // Arrange
        var classNode = new CharacterClassNode(new CharacterClassCharacterSetNode([
            new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c')
        ]), false);
        var quantity = new QuantifierPlusNode(classNode);
        var rootNode = new ConcatenationNode(quantity);
        var target = new QuantifierReluctantAdditionMutator();

        // Act
        var result = target.ApplyMutations(quantity, rootNode);

        // Assert
        var regexMutation = result.ShouldHaveSingleItem();
        regexMutation.OriginalNode.ShouldBe(quantity);
        regexMutation.ReplacementNode.ToString().ShouldBe("[abc]+?");
        regexMutation.ReplacementPattern.ShouldBe("[abc]+?");
        regexMutation.DisplayName.ShouldBe("Regex greedy quantifier to reluctant quantifier modification");
        regexMutation.Description.ShouldBe("""Quantifier "[abc]+" was replace with "[abc]+?" at offset 5.""");
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
file class QuantifierReluctantAdditionAttribute : DataRowAttribute
{
    /// <inheritdoc />
    public QuantifierReluctantAdditionAttribute(string pattern, string[] expected) : base(pattern, expected) =>
        DisplayName = $"Greedy quantifier to reluctant quantifier modification {pattern}";
}
