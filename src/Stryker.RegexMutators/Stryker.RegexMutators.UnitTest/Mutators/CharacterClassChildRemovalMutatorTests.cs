using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.CharacterClass;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public sealed class CharacterClassChildRemovalMutatorTests
{
    [DataTestMethod("Character Class Remove Node")]
    [CharacterClassRemoveChild("[A-Z]", [])]
    [CharacterClassRemoveChild("[ab0-9A-Zcd]",
    [
        "[b0-9A-Zcd]", "[a0-9A-Zcd]", "[abA-Zcd]", "[ab0-9cd]", "[ab0-9A-Zd]", "[ab0-9A-Zc]"
    ])]
    [CharacterClassRemoveChild("[A-Z-[CD]]", ["[A-Z-[D]]", "[A-Z-[C]]", "[A-Z]", "[CD]"])]
    [CharacterClassRemoveChild("[a-zA-Z-[CD]]",
                                  ["[a-zA-Z-[D]]", "[a-zA-Z-[C]]", "[a-zA-Z]", "[A-Z-[CD]]", "[a-z-[CD]]"])]
    [CharacterClassRemoveChild("[a-zA-Z-[CDE-[D-F]]]",
    [
        "[a-zA-Z-[CDE]]", "[a-zA-Z-[DE-[D-F]]]", "[a-zA-Z-[CE-[D-F]]]", "[a-zA-Z-[CD-[D-F]]]", "[a-zA-Z]",
        "[A-Z-[CDE-[D-F]]]", "[a-z-[CDE-[D-F]]]"
    ])]
    public void CharacterClassRemoveNode(string input, string[] expected)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(input, new CharacterClassChildRemovalMutator());

        // Assert
        result.Select(static a => a.ReplacementPattern).ToArray().ShouldBeEquivalentTo(expected);
    }

    [TestMethod]
    [DataRow("[A-Z]")]
    [DataRow("[A]")]
    [DataRow(@"[\u1234]")]
    public void DoesNotRemoveSingleItemClasses(string pattern)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(pattern, new CharacterClassChildRemovalMutator());

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldRemoveEachChildOfTheCharacterClass()
    {
        // Arrange
        RegexNode a;
        RegexNode b;
        RegexNode c;

        var characterClassNode = new CharacterClassNode(new CharacterClassCharacterSetNode([
            a = new CharacterNode('a'), b = new CharacterNode('b'), c = new CharacterNode('c')
        ]), false);

        var childNodes = new List<RegexNode> { characterClassNode, new CharacterNode('a') };
        var rootNode = new ConcatenationNode(childNodes);
        var target = new CharacterClassChildRemovalMutator();

        // Act
        var result = target.ApplyMutations(characterClassNode, rootNode);

        // Assert
        var regexMutations = result as RegexMutation[] ?? result.ToArray();
        regexMutations.Length.ShouldBe(3);
        regexMutations.ElementAt(0).OriginalNode.ShouldBe(a);
        regexMutations.ElementAt(0).ReplacementNode.ShouldBeNull();
        regexMutations.ElementAt(0).ReplacementPattern.ShouldBe("[bc]a");
        regexMutations.ElementAt(0).DisplayName.ShouldBe("Regex character class child removal");

        regexMutations.ElementAt(0)
                   .Description.ShouldBe("""Removed child "a" from character class "[abc]" at offset 0.""");

        regexMutations.ElementAt(1).OriginalNode.ShouldBe(b);
        regexMutations.ElementAt(1).ReplacementNode.ShouldBeNull();
        regexMutations.ElementAt(1).ReplacementPattern.ShouldBe("[ac]a");
        regexMutations.ElementAt(1).DisplayName.ShouldBe("Regex character class child removal");

        regexMutations.ElementAt(1)
                   .Description.ShouldBe("""Removed child "b" from character class "[abc]" at offset 0.""");

        regexMutations.ElementAt(2).OriginalNode.ShouldBe(c);
        regexMutations.ElementAt(2).ReplacementNode.ShouldBeNull();
        regexMutations.ElementAt(2).ReplacementPattern.ShouldBe("[ab]a");
        regexMutations.ElementAt(2).DisplayName.ShouldBe("Regex character class child removal");

        regexMutations.ElementAt(2)
                   .Description.ShouldBe("""Removed child "c" from character class "[abc]" at offset 0.""");
    }

    [TestMethod]
    public void ShouldRemoveEachChildOfTheCharacterClassAndTheSubstitution()
    {
        // Arrange
        RegexNode a;
        RegexNode b;
        RegexNode c;
        CharacterClassNode sub;

        var characterClassNode = new CharacterClassNode(new CharacterClassCharacterSetNode([
            a = new CharacterNode('a'), b = new CharacterNode('b'), c = new CharacterNode('c')
        ]), sub = new CharacterClassNode(new CharacterClassCharacterSetNode([new CharacterNode('b')]), false), false);

        var childNodes = new List<RegexNode> { characterClassNode, new CharacterNode('a') };
        var rootNode = new ConcatenationNode(childNodes);
        var target = new CharacterClassChildRemovalMutator();

        // Act
        var result = target.ApplyMutations(characterClassNode, rootNode);

        // Assert
        var regexMutations = result as RegexMutation[] ?? result.ToArray();
        regexMutations.Length.ShouldBe(4);
        regexMutations.ElementAt(0).OriginalNode.ShouldBe(sub);
        regexMutations.ElementAt(0).ReplacementNode.ShouldBeNull();
        regexMutations.ElementAt(0).ReplacementPattern.ShouldBe("[abc]a");
        regexMutations.ElementAt(0).DisplayName.ShouldBe("Regex character class subtraction removal");

        regexMutations.ElementAt(0)
                   .Description.ShouldBe("""Character Class Subtraction "-[b]" was removed at offset 4.""");

        regexMutations.ElementAt(1).OriginalNode.ShouldBe(a);
        regexMutations.ElementAt(1).ReplacementNode.ShouldBeNull();
        regexMutations.ElementAt(1).ReplacementPattern.ShouldBe("[bc-[b]]a");
        regexMutations.ElementAt(1).DisplayName.ShouldBe("Regex character class child removal");

        regexMutations.ElementAt(1)
                   .Description.ShouldBe("""Removed child "a" from character class "[abc-[b]]" at offset 0.""");

        regexMutations.ElementAt(2).OriginalNode.ShouldBe(b);
        regexMutations.ElementAt(2).ReplacementNode.ShouldBeNull();
        regexMutations.ElementAt(2).ReplacementPattern.ShouldBe("[ac-[b]]a");
        regexMutations.ElementAt(2).DisplayName.ShouldBe("Regex character class child removal");

        regexMutations.ElementAt(2)
                   .Description.ShouldBe("""Removed child "b" from character class "[abc-[b]]" at offset 0.""");

        regexMutations.ElementAt(3).OriginalNode.ShouldBe(c);
        regexMutations.ElementAt(3).ReplacementNode.ShouldBeNull();
        regexMutations.ElementAt(3).ReplacementPattern.ShouldBe("[ab-[b]]a");
        regexMutations.ElementAt(3).DisplayName.ShouldBe("Regex character class child removal");

        regexMutations.ElementAt(3)
                   .Description.ShouldBe("""Removed child "c" from character class "[abc-[b]]" at offset 0.""");
    }

    [TestMethod]
    public void ShouldRemoveOnlyChildOfTheCharacterClassAndTheSubstitution()
    {
        // Arrange
        var sub = new CharacterClassNode(new CharacterClassCharacterSetNode([new CharacterNode('b')]), false);

        var characterClassNode =
            new CharacterClassNode(new CharacterClassCharacterSetNode([new CharacterNode('a')]), sub, false);

        var childNodes = new List<RegexNode> { characterClassNode, new CharacterNode('a') };
        var rootNode = new ConcatenationNode(childNodes);
        var target = new CharacterClassChildRemovalMutator();

        // Act
        var result = target.ApplyMutations(characterClassNode, rootNode);

        // Assert
        var regexMutations = result as RegexMutation[] ?? result.ToArray();
        regexMutations.Length.ShouldBe(2);
        regexMutations.ElementAt(0).OriginalNode.ShouldBe(sub);
        regexMutations.ElementAt(0).ReplacementNode.ShouldBeNull();
        regexMutations.ElementAt(0).ReplacementPattern.ShouldBe("[a]a");
        regexMutations.ElementAt(0).DisplayName.ShouldBe("Regex character class subtraction removal");

        regexMutations.ElementAt(0)
                   .Description.ShouldBe("""Character Class Subtraction "-[b]" was removed at offset 2.""");

        regexMutations.ElementAt(1).OriginalNode.ShouldBe(characterClassNode);
        regexMutations.ElementAt(1).ReplacementNode.ToString().ShouldBe(sub.ToString());
        regexMutations.ElementAt(1).ReplacementPattern.ShouldBe("[b]a");
        regexMutations.ElementAt(1).DisplayName.ShouldBe("Regex character class subtraction replacement");

        regexMutations.ElementAt(1)
                   .Description
                   .ShouldBe("""Character Class "[a-[b]]" was replace with its subtraction "[b]" at offset 0.""");
    }

    [TestMethod]
    public void ShouldNotMutateNonCharacterClassNode()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var rootNode = new ConcatenationNode(characterNode);
        var target = new CharacterClassChildRemovalMutator();

        // Act
        var result = target.Mutate(characterNode, rootNode);

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateCharacterClassWithSingleChild()
    {
        // Arrange
        var characterClassNode =
            new CharacterClassNode(new CharacterClassCharacterSetNode([new CharacterNode('a')]), false);
        var rootNode = new ConcatenationNode(characterClassNode);
        var target = new CharacterClassChildRemovalMutator();

        // Act
        var result = target.Mutate(characterClassNode, rootNode);

        // Assert
        result.ShouldBeEmpty();
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
file class CharacterClassRemoveChildAttribute : DataRowAttribute
{
    /// <inheritdoc />
    public CharacterClassRemoveChildAttribute(string pattern, string[] expected) : base(pattern, expected) =>
        DisplayName = $"Removes children of Character Classes {pattern}";
}
