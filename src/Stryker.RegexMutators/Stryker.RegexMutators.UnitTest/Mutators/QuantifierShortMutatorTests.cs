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
public sealed class QuantifierShortMutatorTests
{
    [TestMethod]
    [QuantifierShort("abc?", [
        "abc{1,1}", "abc{0,0}", "abc{0,2}"
    ])]
    [QuantifierShort("abc*", [
        "abc{1,}"
    ])]
    [QuantifierShort("abc+", [
        "abc{0,}", "abc{2,}"
    ])]
    [QuantifierShort("a?b*c+", [
        "a{1,1}b*c+", "a{0,0}b*c+", "a{0,2}b*c+", "a?b{1,}c+", "a?b*c{0,}", "a?b*c{2,}"
    ])]
    public void QuantifierShort(string input, string[] expected)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(input, new QuantifierShortMutator());

        // Assert
        result.Select(static a => a.ReplacementPattern).ToArray().ShouldBeEquivalentTo(expected);
    }

    [TestMethod]
    [QuantifierShort("c+", [
        "c{0,}", "c{2,}"
    ])]
    public void QuantifierShort2(string input, string[] expected)
    {
        // Act
        var result = TestHelpers.ParseAndMutate(input, new QuantifierShortMutator());

        // Assert
        result.Select(static a => a.ReplacementPattern).ToArray().ShouldBeEquivalentTo(expected);
    }

    [TestMethod]
    public void ShouldMutatePlusQuantifier()
    {
        // Arrange
        var classNode = new CharacterClassNode(new CharacterClassCharacterSetNode([
            new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c')
        ]), false);
        var quantity = new QuantifierPlusNode(classNode);
        var rootNode = new ConcatenationNode(quantity);
        var target = new QuantifierShortMutator();

        // Act
        var result = target.ApplyMutations(quantity, rootNode);

        // Assert
        var regexMutations = result as RegexMutation[] ?? result.ToArray();
        regexMutations.Length.ShouldBe(2);
        regexMutations.ElementAt(0).OriginalNode.ShouldBe(quantity);
        regexMutations.ElementAt(0).ReplacementNode.ToString().ShouldBe("[abc]{0,}");
        regexMutations.ElementAt(0).ReplacementPattern.ShouldBe("[abc]{0,}");
        regexMutations.ElementAt(0).DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");

        regexMutations.ElementAt(0)
                   .Description.ShouldBe("""Quantifier "[abc]+" was replaced with "[abc]{0,}" at offset 5.""");

        regexMutations.ElementAt(1).OriginalNode.ShouldBe(quantity);
        regexMutations.ElementAt(1).ReplacementNode.ToString().ShouldBe("[abc]{2,}");
        regexMutations.ElementAt(1).ReplacementPattern.ShouldBe("[abc]{2,}");
        regexMutations.ElementAt(1).DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");

        regexMutations.ElementAt(1)
                   .Description.ShouldBe("""Quantifier "[abc]+" was replaced with "[abc]{2,}" at offset 5.""");
    }

    [TestMethod]
    public void ShouldMutateQuestionMarkQuantifier()
    {
        // Arrange
        var classNode = new CharacterClassNode(new CharacterClassCharacterSetNode([
            new CharacterNode('a'), new CharacterNode('b'), new CharacterNode('c')
        ]), false);
        var quantity = new QuantifierQuestionMarkNode(classNode);
        var rootNode = new ConcatenationNode(quantity);
        var target = new QuantifierShortMutator();

        // Act
        var result = target.ApplyMutations(quantity, rootNode);

        // Assert
        var regexMutations = result as RegexMutation[] ?? result.ToArray();
        regexMutations.Length.ShouldBe(3);
        regexMutations.ElementAt(0).OriginalNode.ShouldBe(quantity);
        regexMutations.ElementAt(0).ReplacementNode.ToString().ShouldBe("[abc]{1,1}");
        regexMutations.ElementAt(0).ReplacementPattern.ShouldBe("[abc]{1,1}");
        regexMutations.ElementAt(0).DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");

        regexMutations.ElementAt(0)
                   .Description.ShouldBe("""Quantifier "[abc]?" was replaced with "[abc]{1,1}" at offset 5.""");

        regexMutations.ElementAt(1).OriginalNode.ShouldBe(quantity);
        regexMutations.ElementAt(1).ReplacementNode.ToString().ShouldBe("[abc]{0,0}");
        regexMutations.ElementAt(1).ReplacementPattern.ShouldBe("[abc]{0,0}");
        regexMutations.ElementAt(1).DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");

        regexMutations.ElementAt(1)
                   .Description.ShouldBe("""Quantifier "[abc]?" was replaced with "[abc]{0,0}" at offset 5.""");

        regexMutations.ElementAt(2).OriginalNode.ShouldBe(quantity);
        regexMutations.ElementAt(2).ReplacementNode.ToString().ShouldBe("[abc]{0,2}");
        regexMutations.ElementAt(2).ReplacementPattern.ShouldBe("[abc]{0,2}");
        regexMutations.ElementAt(2).DisplayName.ShouldBe("Regex greedy quantifier quantity mutation");

        regexMutations.ElementAt(2)
                   .Description.ShouldBe("""Quantifier "[abc]?" was replaced with "[abc]{0,2}" at offset 5.""");
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
file class QuantifierShortAttribute : DataRowAttribute
{
    /// <inheritdoc />
    public QuantifierShortAttribute(string pattern, string[] expected) : base(pattern, expected) =>
        DisplayName = $"Modifies short quantifier {pattern}";
}
