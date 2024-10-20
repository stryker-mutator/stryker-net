using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes.CharacterClass;
using Stryker.Regex.Parser.Nodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public sealed class CharacterClassRangeMutatorTests
{
    [DataTestMethod("Character Class Modify Range")]
    [CharacterClassModifyRange("[b-y][B-Y][1-8]", [
        // [b-y] -> [a-y] or [c-y] or [b-z] or [b-x]
        "[a-y][B-Y][1-8]", "[c-y][B-Y][1-8]", "[b-x][B-Y][1-8]", "[b-z][B-Y][1-8]",
        // [B-Y] -> [A-Y] OR [C-Y] OR [B-Z] OR [B-X]
        "[b-y][A-Y][1-8]", "[b-y][C-Y][1-8]", "[b-y][B-X][1-8]", "[b-y][B-Z][1-8]",
        // [1-8] -> [0-8] OR [2-8] OR [1-9] OR [1-7]
        "[b-y][B-Y][0-8]", "[b-y][B-Y][2-8]", "[b-y][B-Y][1-7]", "[b-y][B-Y][1-9]"
    ])]
    [CharacterClassModifyRange("[a-y][A-Y][0-8]", [
        // [a-y] -> [b-y] or [a-z] or [a-x]
        "[b-y][A-Y][0-8]", "[a-x][A-Y][0-8]", "[a-z][A-Y][0-8]",
        // [A-Y] -> [B-Y] OR [A-Z] OR [A-X]
        "[a-y][B-Y][0-8]", "[a-y][A-X][0-8]", "[a-y][A-Z][0-8]",
        // [0-8] -> [1-8] OR [0-9] OR [0-7]
        "[a-y][A-Y][1-8]", "[a-y][A-Y][0-7]", "[a-y][A-Y][0-9]"
    ])]
    [CharacterClassModifyRange("[b-z][B-Z][1-9]", [
        // [b-z] -> [a-z] or [c-z] or [b-y]
        "[a-z][B-Z][1-9]", "[c-z][B-Z][1-9]", "[b-y][B-Z][1-9]",
        // [B-Z] -> [A-Z] OR [C-Z] OR [B-Y]
        "[b-z][A-Z][1-9]", "[b-z][C-Z][1-9]", "[b-z][B-Y][1-9]",
        // [1-9] -> [0-9] OR [2-9] OR [1-8]
        "[b-z][B-Z][0-9]", "[b-z][B-Z][2-9]", "[b-z][B-Z][1-8]"
    ])]
    [CharacterClassModifyRange("[a-z][A-Z][0-9]", [
        // [a-z] -> [b-z] or [a-y]
        "[b-z][A-Z][0-9]", "[a-y][A-Z][0-9]",
        // [A-Z] -> [B-Z] OR [A-Y]
        "[a-z][B-Z][0-9]", "[a-z][A-Y][0-9]",
        // [0-9] -> [1-9] OR [0-8]
        "[a-z][A-Z][1-9]", "[a-z][A-Z][0-8]"
    ])]
    [CharacterClassModifyRange("[b-b][B-B][1-1]", [
        // [b-b] -> [a-b] or [b-c]
        "[a-b][B-B][1-1]", "[b-c][B-B][1-1]",
        // [B-B] -> [A-B] OR [B-C]
        "[b-b][A-B][1-1]", "[b-b][B-C][1-1]",
        // [1-1] -> [0-1] OR [1-2]
        "[b-b][B-B][0-1]", "[b-b][B-B][1-2]"
    ])]
    [CharacterClassModifyRange("[a-a][A-A][0-0]", [
        // [a-a] -> [a-b]
        "[a-b][A-A][0-0]",
        // [A-A] -> [A-B]
        "[a-a][A-B][0-0]",
        // [0-0] -> [0-1]
        "[a-a][A-A][0-1]"
    ])]
    [CharacterClassModifyRange("[z-z][Z-Z][9-9]", [
        // [z-z] -> [y-z]
        "[y-z][Z-Z][9-9]",
        // [Z-Z] -> [Y-Z]
        "[z-z][Y-Z][9-9]",
        // [9-9] -> [8-9]
        "[z-z][Z-Z][8-9]"
    ])]
    [CharacterClassModifyRange(@"[\u0600-\u06ff][\x34-\x37][\123-\347]", [
        @"[\u05ff-\u06ff][\x34-\x37][\123-\347]",
        @"[\u0601-\u06ff][\x34-\x37][\123-\347]",
        @"[\u0600-\u06fe][\x34-\x37][\123-\347]",
        @"[\u0600-\u0700][\x34-\x37][\123-\347]",

        @"[\u0600-\u06ff][\x33-\x37][\123-\347]",
        @"[\u0600-\u06ff][\x35-\x37][\123-\347]",
        @"[\u0600-\u06ff][\x34-\x36][\123-\347]",
        @"[\u0600-\u06ff][\x34-\x38][\123-\347]",

        @"[\u0600-\u06ff][\x34-\x37][\122-\347]",
        @"[\u0600-\u06ff][\x34-\x37][\124-\347]",
        @"[\u0600-\u06ff][\x34-\x37][\123-\346]",
        @"[\u0600-\u06ff][\x34-\x37][\123-\350]"
    ])]
    [CharacterClassModifyRange(@"[a-\u0600][\u0040-z]", [
        @"[b-\u0600][\u0040-z]",
        @"[a-\u05ff][\u0040-z]",
        @"[a-\u0601][\u0040-z]",
        @"[a-\u0600][\u003f-z]",
        @"[a-\u0600][\u0041-z]",
        @"[a-\u0600][\u0040-y]"
    ])]
    [CharacterClassModifyRange("[!-#][#-a][!-z][#-A][!-Z][#-1][!-8]", [
        "[!-#][#-b][!-z][#-A][!-Z][#-1][!-8]",
        "[!-#][#-a][!-y][#-A][!-Z][#-1][!-8]",
        "[!-#][#-a][!-z][#-B][!-Z][#-1][!-8]",
        "[!-#][#-a][!-z][#-A][!-Y][#-1][!-8]",
        "[!-#][#-a][!-z][#-A][!-Z][#-0][!-8]",
        "[!-#][#-a][!-z][#-A][!-Z][#-2][!-8]",
        "[!-#][#-a][!-z][#-A][!-Z][#-1][!-7]",
        "[!-#][#-a][!-z][#-A][!-Z][#-1][!-9]"
    ])]
    [CharacterClassModifyRange(@"[\u0031-\u0031][1-\u0031][\u0031-1]", [
        @"[\u0030-\u0031][1-\u0031][\u0031-1]",
        @"[\u0031-\u0032][1-\u0031][\u0031-1]",
        @"[\u0031-\u0031][0-\u0031][\u0031-1]",
        @"[\u0031-\u0031][1-\u0032][\u0031-1]",
        @"[\u0031-\u0031][1-\u0031][\u0030-1]",
        @"[\u0031-\u0031][1-\u0031][\u0031-2]",
    ])]
    [CharacterClassModifyRange(@"[\u0031-\u0032][1-\u0032][\u0031-2][1-2]", [
        @"[\u0030-\u0032][1-\u0032][\u0031-2][1-2]",
        @"[\u0032-\u0032][1-\u0032][\u0031-2][1-2]",
        @"[\u0031-\u0031][1-\u0032][\u0031-2][1-2]",
        @"[\u0031-\u0033][1-\u0032][\u0031-2][1-2]",
        @"[\u0031-\u0032][0-\u0032][\u0031-2][1-2]",
        @"[\u0031-\u0032][2-\u0032][\u0031-2][1-2]",
        @"[\u0031-\u0032][1-\u0031][\u0031-2][1-2]",
        @"[\u0031-\u0032][1-\u0033][\u0031-2][1-2]",
        @"[\u0031-\u0032][1-\u0032][\u0030-2][1-2]",
        @"[\u0031-\u0032][1-\u0032][\u0032-2][1-2]",
        @"[\u0031-\u0032][1-\u0032][\u0031-1][1-2]",
        @"[\u0031-\u0032][1-\u0032][\u0031-3][1-2]",
        @"[\u0031-\u0032][1-\u0032][\u0031-2][0-2]",
        @"[\u0031-\u0032][1-\u0032][\u0031-2][2-2]",
        @"[\u0031-\u0032][1-\u0032][\u0031-2][1-1]",
        @"[\u0031-\u0032][1-\u0032][\u0031-2][1-3]"
    ])]
    public void CharacterClassModifyRange(string input, string[] expected)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(input, new CharacterClassRangeMutator());

        // Assert
        result.Select(static a => a.ReplacementPattern).ToArray().ShouldBeEquivalentTo(expected);
    }
    
    [TestMethod]
    [DataRow(@"[\a-\f]")]
    [DataRow(@"[\ca-\cc]")]
    [DataRow(@"[\t-\n]")]
    public void DoesNotModifyNonAlphaNumericRanges(string pattern)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(pattern, new CharacterClassRangeMutator());

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateCharacterClassRange()
    {
        // Arrange
        var leftNode = new CharacterNode('A');
        var rightNode = new CharacterNode('Z');
        var rangeNode = new CharacterClassRangeNode(leftNode, rightNode);
        var rootNode = new CharacterClassNode(new CharacterClassCharacterSetNode(rangeNode), false);
        var target = new CharacterClassRangeMutator();

        // Act
        var result = target.ApplyMutations(rangeNode, rootNode);

        // Assert
        var regexMutations = result as RegexMutation[] ?? result.ToArray();
        regexMutations.Length.ShouldBe(2);
        regexMutations.ElementAt(0).OriginalNode.ShouldBe(leftNode);
        regexMutations.ElementAt(0).ReplacementNode.ToString().ShouldBe("B");
        regexMutations.ElementAt(0).ReplacementPattern.ShouldBe("[B-Z]");
        regexMutations.ElementAt(0).DisplayName.ShouldBe("Regex character class range modification");
        regexMutations.ElementAt(0).Description.ShouldBe("""Replaced character "A" with "B" at offset 1.""");

        regexMutations.ElementAt(1).OriginalNode.ShouldBe(rightNode);
        regexMutations.ElementAt(1).ReplacementNode.ToString().ShouldBe("Y");
        regexMutations.ElementAt(1).ReplacementPattern.ShouldBe("[A-Y]");
        regexMutations.ElementAt(1).DisplayName.ShouldBe("Regex character class range modification");
        regexMutations.ElementAt(1).Description.ShouldBe("""Replaced character "Z" with "Y" at offset 3.""");
    }

    [TestMethod]
    public void ShouldNotMutateInvalidCharacterClassRange()
    {
        // Arrange
        var rangeNode = new CharacterClassRangeNode(new AnyCharacterNode(), new AnyCharacterNode());
        var childNodes = new List<RegexNode>
        {
            rangeNode
        };
        var rootNode = new ConcatenationNode(childNodes);
        var target = new CharacterClassRangeMutator();

        // Act
        var result = target.ApplyMutations(rangeNode, rootNode);

        // Assert
        result.ShouldBeEmpty();
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
file class CharacterClassModifyRangeAttribute : DataRowAttribute
{
    /// <inheritdoc />
    public CharacterClassModifyRangeAttribute(string pattern, string[] expected) : base(pattern, expected) =>
        DisplayName = $"Character Class Modify Range {pattern}";
}
