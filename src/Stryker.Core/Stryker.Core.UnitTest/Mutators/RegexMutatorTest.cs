using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Mutators;

namespace Stryker.Core.UnitTest.Mutators;

[TestClass]
public class RegexMutatorTest : TestBase
{
    [TestMethod]
    public void ShouldBeMutationLevelAdvanced()
    {
        var target = new StringMutator();
        target.RegexMutationLevel.ShouldBe(MutationLevel.Advanced);
    }

    [TestMethod]
    public void ShouldMutateStringLiteralInRegexConstructor()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex(@\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new StringMutator();

        var result = target.ApplyMutations(objectCreationExpression.DescendantNodesAndSelf().OfType<LiteralExpressionSyntax>().First(), null, MutationLevel.Advanced);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("abc");
    }

    [TestMethod]
    public void ShouldMutateStringLiteralInRegexConstructorWithFullName()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new System.Text.RegularExpressions.Regex(@\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new StringMutator();

        var result = target.ApplyMutations(objectCreationExpression.DescendantNodesAndSelf().OfType<LiteralExpressionSyntax>().First(), null, MutationLevel.Advanced);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("abc");
    }

    [TestMethod]
    public void ShouldNotMutateStringLiteralInOtherConstructor()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Other(@\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new StringMutator();
        var result = target.ApplyMutations(objectCreationExpression.DescendantNodesAndSelf().OfType<LiteralExpressionSyntax>().First(), null, MutationLevel.Advanced);

        result.Where(a=>a.Type == Mutator.Regex).ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldMutateStringLiteralMultipleTimes()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex(@\"^abc$\")") as ObjectCreationExpressionSyntax;
        var target = new StringMutator();

        var result = target.ApplyMutations(objectCreationExpression.DescendantNodesAndSelf().OfType<LiteralExpressionSyntax>().First(), null, MutationLevel.Advanced);

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
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex(options: RegexOptions.None, pattern: @\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new StringMutator();

        var result = target.ApplyMutations(objectCreationExpression.DescendantNodesAndSelf().OfType<LiteralExpressionSyntax>().First(), null, MutationLevel.Advanced);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("abc");
    }
}
