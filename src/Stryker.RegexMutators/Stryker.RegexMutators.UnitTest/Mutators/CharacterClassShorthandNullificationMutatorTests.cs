using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public sealed class CharacterClassShorthandNullificationMutatorTests
{
    [TestMethod]
    [CharacterClassShorthandNullification(@"\w\W\d\D\s\S", [
        @"w\W\d\D\s\S",
        @"\wW\d\D\s\S",
        @"\w\Wd\D\s\S",
        @"\w\W\dD\s\S",
        @"\w\W\d\Ds\S",
        @"\w\W\d\D\sS"
    ])]
    public void CharacterClassToAnyChar(string input, string[] expected)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(input, new CharacterClassShorthandNullificationMutator());

        // Assert
        result.Select(static a => a.ReplacementPattern).ToArray().ShouldBeEquivalentTo(expected);
    }

    [TestMethod]
    [DataRow("(abc)")]
    [DataRow("(?:def)")]
    [DataRow("Alice")]
    [DataRow(@"\n+\t{2,}")]
    public void DoesNotMutateNonCharacterClasses(string pattern)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(pattern, new CharacterClassShorthandNullificationMutator());

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateCharacterClassShorthand()
    {
        // Arrange
        var shorthandNode = new CharacterClassShorthandNode('w');
        var rootNode = new ConcatenationNode(shorthandNode);
        var target = new CharacterClassShorthandNullificationMutator();

        // Act
        var result = target.ApplyMutations(shorthandNode, rootNode);

        // Assert
        var regexMutation = result.ShouldHaveSingleItem();
        regexMutation.OriginalNode.ShouldBe(shorthandNode);
        regexMutation.ReplacementNode.ToString().ShouldBe("w");
        regexMutation.ReplacementPattern.ShouldBe("w");
        regexMutation.DisplayName.ShouldBe("Regex predefined character class nullification");
        regexMutation.Description.ShouldBe("""Character class shorthand "\w" was replaced with "w" at offset 0.""");
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
file class CharacterClassShorthandNullificationAttribute : DataRowAttribute
{
    /// <inheritdoc />
    public CharacterClassShorthandNullificationAttribute(string pattern, string[] expected) : base(pattern, expected) =>
        DisplayName = $"Nullifies Predefined Character Class {pattern}";
}
