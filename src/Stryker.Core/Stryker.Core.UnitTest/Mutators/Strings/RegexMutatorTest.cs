using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators.Strings;

[TestClass]
public class RegexMutatorTest : TestBase
{

    private static LiteralExpressionSyntax ParseExpression(string text)
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression(text) as ObjectCreationExpressionSyntax;
        return objectCreationExpression?.DescendantNodesAndSelf().OfType<LiteralExpressionSyntax>().FirstOrDefault();
    }

    [TestMethod]
    public void ShouldBeMutationLevelAdvanced()
    {
        var target = new StringMutator();
        target.RegexMutationLevel.ShouldBe(MutationLevel.Advanced);
    }

    [TestMethod]
    public void ShouldMutateStringLiteralInRegexConstructor()
    {
        var literalExpression = ParseExpression("new Regex(@\"^abc\")");
        var target = new StringMutator();

        var result = target.ApplyMutations(literalExpression, null, MutationLevel.Advanced);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("abc");
    }

    [TestMethod]
    public void ShouldMutateStringLiteralInRegexConstructorWithFullName()
    {
        var literalExpression = ParseExpression("new System.Text.RegularExpressions.Regex(@\"^abc\")");
        var target = new StringMutator();

        var result = target.ApplyMutations(literalExpression, null, MutationLevel.Advanced);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("abc");
    }

    [TestMethod]
    public void ShouldNotMutateStringLiteralInOtherConstructor()
    {
        var literalExpression = ParseExpression("new Other(@\"^abc\")");
        var target = new StringMutator();
        var result = target.ApplyMutations(literalExpression, null, MutationLevel.Advanced);

        result.Where(a => a.Type == Mutator.Regex).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldNotMutateAtLowerMutationLevel()
    {
        var literalExpression = ParseExpression("new Other(@\"^abc\")");
        var target = new StringMutator();
        var result = target.ApplyMutations(literalExpression, null, MutationLevel.Standard);

        result.Where(a => a.Type == Mutator.Regex).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateStringLiteralMultipleTimes()
    {
        var literalExpression = ParseExpression("new Regex(@\"^abc$\")");
        var target = new StringMutator();

        var result = target.ApplyMutations(literalExpression, null, MutationLevel.Advanced);

        result.Count().ShouldBe(2);
        result.ShouldAllBe(mutant => mutant.DisplayName == "Regex anchor removal mutation");
        var first = result.First().ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        var last = result.Last().ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        first.Token.ValueText.ShouldBe("abc$");
        last.Token.ValueText.ShouldBe("^abc");
    }

    [TestMethod]
    public void ShouldMutateStringLiteralAsNamedArgumentPatternInRegexConstructor()
    {
        var literalExpression = ParseExpression("new Regex(options: RegexOptions.None, pattern: @\"^abc\")");
        var target = new StringMutator();

        var result = target.ApplyMutations(literalExpression, null, MutationLevel.Advanced);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("abc");
    }
}
