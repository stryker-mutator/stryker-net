using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Regex.Parser.Nodes;
using Stryker.RegexMutators.Mutators;

namespace Stryker.RegexMutators.UnitTest.Mutators;

[TestClass]
public sealed class UnicodeCharClassNegationMutatorTests
{
    [TestMethod]
    public void NegatesUnicodeCharacterClassWithLoneProperty()
    {
        // Act
        var result =
            TestHelpers.ParseAndMutate(@"\p{IsBasicLatin}\P{IsBasicLatin}", new UnicodeCharClassNegationMutator());

        // Assert
        result.Select(static a => a.ReplacementPattern)
           .ToArray()
           .ShouldBeEquivalentTo((string[])
               [
                   @"\P{IsBasicLatin}\P{IsBasicLatin}", @"\p{IsBasicLatin}\p{IsBasicLatin}"
               ]);
    }

    [TestMethod]
    [Ignore("Dotnet does not support this regex expression")]
    public void NegatesUnicodeCharacterClassWithPropertyAndValue()
    {
        // Act
        var result = TestHelpers.ParseAndMutate(@"\p{Script_Extensions=Latin}\P{Script_Extensions=Latin}",
                                                new UnicodeCharClassNegationMutator());

        // Assert
        result.Select(static a => a.ReplacementPattern)
           .ToArray()
           .ShouldBeEquivalentTo((string[])
               [
                   @"\P{Script_Extensions=Latin}\P{Script_Extensions=Latin}",
                   @"\p{Script_Extensions=Latin}\p{Script_Extensions=Latin}"
               ]);
    }

    [TestMethod]
    public void ShouldNegateUnicodeCharacterClassAtStart()
    {
        // Arrange
        var unicodeNode = new UnicodeCategoryNode("IsBasicLatin", false);

        var childNodes = new List<RegexNode>
        {
            unicodeNode, new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c')
        };
        var rootNode = new ConcatenationNode(childNodes);
        var target = new UnicodeCharClassNegationMutator();

        // Act
        var result = target.ApplyMutations(unicodeNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(unicodeNode);
        mutation.ReplacementNode.ToString().ShouldBe(@"\P{IsBasicLatin}");
        mutation.ReplacementPattern.ShouldBe(@"\P{IsBasicLatin}abc");
        mutation.DisplayName.ShouldBe("Regex Unicode character class negation mutation");

        mutation.Description
             .ShouldBe("""Unicode category "\p{IsBasicLatin}" was replaced with "\P{IsBasicLatin}" at offset 0.""");
    }

    [TestMethod]
    public void ShouldNegateUnicodeCharacterClassInMiddle()
    {
        // Arrange
        var unicodeNode = new UnicodeCategoryNode("IsBasicLatin", false);

        var childNodes = new List<RegexNode>
        {
            new CharacterNode('a'), unicodeNode, new CharacterNode('b'), new CharacterNode('c')
        };
        var rootNode = new ConcatenationNode(childNodes);
        var target = new UnicodeCharClassNegationMutator();

        // Act
        var result = target.ApplyMutations(unicodeNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(unicodeNode);
        mutation.ReplacementNode.ToString().ShouldBe(@"\P{IsBasicLatin}");
        mutation.ReplacementPattern.ShouldBe(@"a\P{IsBasicLatin}bc");
        mutation.DisplayName.ShouldBe("Regex Unicode character class negation mutation");

        mutation.Description
             .ShouldBe("""Unicode category "\p{IsBasicLatin}" was replaced with "\P{IsBasicLatin}" at offset 1.""");
    }

    [TestMethod]
    public void ShouldNegateUnicodeCharacterClassAtEnd()
    {
        // Arrange
        var unicodeNode = new UnicodeCategoryNode("IsBasicLatin", false);

        var childNodes = new List<RegexNode>
        {
            new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c'), unicodeNode
        };
        var rootNode = new ConcatenationNode(childNodes);
        var target = new UnicodeCharClassNegationMutator();

        // Act
        var result = target.ApplyMutations(unicodeNode, rootNode);

        // Assert
        var mutation = result.ShouldHaveSingleItem();
        mutation.OriginalNode.ShouldBe(unicodeNode);
        mutation.ReplacementNode.ToString().ShouldBe(@"\P{IsBasicLatin}");
        mutation.ReplacementPattern.ShouldBe(@"abc\P{IsBasicLatin}");
        mutation.DisplayName.ShouldBe("Regex Unicode character class negation mutation");

        mutation.Description
             .ShouldBe("""Unicode category "\p{IsBasicLatin}" was replaced with "\P{IsBasicLatin}" at offset 3.""");
    }

    [TestMethod]
    public void MutateShouldNotMutateNonUnicodeCharacterNode()
    {
        // Arrange
        var characterNode = new CharacterNode('a');
        var rootNode = new ConcatenationNode(characterNode);
        var target = new UnicodeCharClassNegationMutator();

        // Act
        var result = target.Mutate(characterNode, rootNode);

        // Assert
        result.ShouldBeEmpty();
    }
}
