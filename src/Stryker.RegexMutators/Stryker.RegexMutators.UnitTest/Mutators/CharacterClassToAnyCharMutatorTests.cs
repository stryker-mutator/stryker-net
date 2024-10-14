using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.CharacterClass;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public sealed class CharacterClassToAnyCharMutatorTests
{
    [TestMethod]
    [CharacterClassToAnyChar("[abc]", [@"[\w\W]"])]
    [CharacterClassToAnyChar("[abc][bcd]", [@"[\w\W][bcd]", @"[abc][\w\W]"])]
    [CharacterClassToAnyChar(@"[\w\W][bcd]", [@"[\w\W][\w\W]"])]
    public void CharacterClassToAnyChar(string input, string[] expected)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(input, new CharacterClassToAnyCharMutator());

        // Assert
        result.Select(static a => a.ReplacementPattern).ToArray().ShouldBeEquivalentTo(expected);
    }

    [TestMethod]
    [DataRow(@"[\w\W]")]
    [DataRow(@"[\W\w]")]
    public void DoesNotMutateToItself(string pattern)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(pattern, new CharacterClassToAnyCharMutator());

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    [DataRow("(abc)")]
    [DataRow("(?:def)")]
    [DataRow("Alice")]
    [DataRow(@"\d+\w{2,}")]
    public void DoesNotMutateNonCharacterClasses(string pattern)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(pattern, new CharacterClassToAnyCharMutator());

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateCharacterClass()
    {
        // Arrange
        var classNode = new CharacterClassNode(new CharacterClassCharacterSetNode([
            new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c')
        ]), false);
        var rootNode = new ConcatenationNode(classNode);
        var target = new CharacterClassToAnyCharMutator();

        // Act
        var result = target.ApplyMutations(classNode, rootNode);

        // Assert
        var regexMutation = result.ShouldHaveSingleItem();
        regexMutation.OriginalNode.ShouldBe(classNode);
        regexMutation.ReplacementNode.ToString().ShouldBe(@"[\w\W]");
        regexMutation.ReplacementPattern.ShouldBe(@"[\w\W]");
        regexMutation.DisplayName.ShouldBe("""Regex character class to "[\w\W]" change""");
        regexMutation.Description.ShouldBe("""Replaced regex node "[abc]" with "[\w\W]" at offset 0.""");
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
file class CharacterClassToAnyCharAttribute : DataRowAttribute
{
    /// <inheritdoc />
    public CharacterClassToAnyCharAttribute(string pattern, string[] expected) : base(pattern, expected) =>
        DisplayName = $"Character Class to any char {pattern}";
}
