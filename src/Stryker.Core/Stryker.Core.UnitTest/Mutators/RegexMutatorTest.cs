using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators;

public class RegexMutatorTest : TestBase
{
    [Fact]
    public void ShouldBeMutationLevelAdvanced()
    {
        var target = new RegexMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Advanced);
    }

    [Fact]
    public void ShouldMutateStringLiteralInRegexConstructor()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex(@\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();

        var result = target.ApplyMutations(objectCreationExpression);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("abc");
    }

    [Fact]
    public void ShouldMutateStringLiteralInRegexConstructorWithFullName()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new System.Text.RegularExpressions.Regex(@\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();

        var result = target.ApplyMutations(objectCreationExpression);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("abc");
    }


    [Fact]
    public void ShouldNotMutateRegexWithoutParameters()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex()") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();
        var result = target.ApplyMutations(objectCreationExpression);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldNotMutateStringLiteralInOtherConstructor()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Other(@\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();
        var result = target.ApplyMutations(objectCreationExpression);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldMutateStringLiteralMultipleTimes()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex(@\"^abc$\")") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();

        var result = target.ApplyMutations(objectCreationExpression);

        result.Count().ShouldBe(2);
        result.ShouldAllBe(mutant => mutant.DisplayName == "Regex anchor removal mutation");
        var first = result.First().ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        var last = result.Last().ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        first.Token.ValueText.ShouldBe("abc$");
        last.Token.ValueText.ShouldBe("^abc");
    }

    [Fact]
    public void ShouldMutateStringLiteralAsNamedArgumentPatternInRegexConstructor()
    {
        var objectCreationExpression = SyntaxFactory.ParseExpression("new Regex(options: RegexOptions.None, pattern: @\"^abc\")") as ObjectCreationExpressionSyntax;
        var target = new RegexMutator();

        var result = target.ApplyMutations(objectCreationExpression);

        var mutation = result.ShouldHaveSingleItem();

        mutation.DisplayName.ShouldBe("Regex anchor removal mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("abc");
    }
}
