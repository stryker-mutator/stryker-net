using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public sealed class CharacterClassShorthandAnyCharMutatorTests
{
    [TestMethod]
    [CharacterClassShorthandAnyChar(@"\w\W\d\D\s\S", [
        @"[\w\W]\W\d\D\s\S",
        @"\w[\W\w]\d\D\s\S",
        @"\w\W[\d\D]\D\s\S",
        @"\w\W\d[\D\d]\s\S",
        @"\w\W\d\D[\s\S]\S",
        @"\w\W\d\D\s[\S\s]"
    ])]
    public void CharacterClassToAnyChar(string input, string[] expected)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(input, new CharacterClassShorthandAnyCharMutator());

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
        var result = TestHelpers.ParseAndMutate(pattern, new CharacterClassShorthandAnyCharMutator());

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateCharacterClassShorthand()
    {
        // Arrange
        var shorthandNode = new CharacterClassShorthandNode('w');
        var rootNode = new ConcatenationNode(shorthandNode);
        var target = new CharacterClassShorthandAnyCharMutator();

        // Act
        var result = target.ApplyMutations(shorthandNode, rootNode);

        // Assert
        var regexMutation = result.ShouldHaveSingleItem();
        regexMutation.OriginalNode.ShouldBe(shorthandNode);
        regexMutation.ReplacementNode.ToString().ShouldBe(@"[\w\W]");
        regexMutation.ReplacementPattern.ShouldBe(@"[\w\W]");
        regexMutation.DisplayName.ShouldBe("Regex predefined character class to character class with its negation change");
        regexMutation.Description.ShouldBe("""Character class shorthand "\w" was replaced with "[\w\W]" at offset 1.""");
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
file class CharacterClassShorthandAnyCharAttribute : DataRowAttribute
{
    /// <inheritdoc />
    public CharacterClassShorthandAnyCharAttribute(string pattern, string[] expected) : base(pattern, expected) =>
        DisplayName = $"""Changes Predefined Character Class to "[\w\W]" {pattern}""";
}
