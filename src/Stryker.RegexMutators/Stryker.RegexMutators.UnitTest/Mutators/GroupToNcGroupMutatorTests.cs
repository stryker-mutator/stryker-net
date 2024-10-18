using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.Regex.Parser.Nodes.GroupNodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public sealed class GroupToNcGroupMutatorTests
{
    [TestMethod]
    [GroupToNcGroup("([abc])", ["(?:[abc])"])]
    [GroupToNcGroup("([abc][bcd])", ["(?:[abc][bcd])"])]
    [GroupToNcGroup(@"([\w\W])([bcd])", [@"(?:[\w\W])([bcd])", @"([\w\W])(?:[bcd])"])]
    [GroupToNcGroup("(([bcd])([bcd]))", ["((?:[bcd])([bcd]))", "(([bcd])(?:[bcd]))", "(?:([bcd])([bcd]))"])]
    public void GroupToNcGroup(string input, string[] expected)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(input, new GroupToNcGroupMutator());

        // Assert
        result.Select(static a => a.ReplacementPattern).ToArray().ShouldBeEquivalentTo(expected);
    }

    [TestMethod]
    [DataRow("(?:def)")]
    [DataRow("Alice")]
    [DataRow(@"\d+\w{2,}")]
    public void DoesNotMutateNonCaptureGroups(string pattern)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(pattern, new GroupToNcGroupMutator());

        // Assert
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public void FlipsCaptureGroupToNonCaptureGroup()
    {
        // Arrange
        var lookaroundGroupNode = new CaptureGroupNode([
            new CharacterNode('f'), new CharacterNode('o'), new CharacterNode('o')
        ]);
        var rootNode = new ConcatenationNode(lookaroundGroupNode);
        var target = new GroupToNcGroupMutator();

        // Act
        var result = target.ApplyMutations(lookaroundGroupNode, rootNode).ToList();

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(lookaroundGroupNode);
        mutation.ReplacementNode.ToString().ShouldBe("(?:foo)");
        mutation.ReplacementPattern.ShouldBe("(?:foo)");
        mutation.DisplayName.ShouldBe("Regex capturing group to non-capturing group modification");
        mutation.Description.ShouldBe("""Capturing group "(foo)" was replaced with "(?:foo)" at offset 0.""");
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
file class GroupToNcGroupAttribute : DataRowAttribute
{
    /// <inheritdoc />
    public GroupToNcGroupAttribute(string pattern, string[] expected) : base(pattern, expected) =>
        DisplayName = $"Capture group to non capture group {pattern}";
}
