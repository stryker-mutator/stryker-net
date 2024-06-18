using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators;

public class NumberNegationMutatorTests : TestBase
{
    [Fact]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new NumberNegationMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [Theory]
    [InlineData(10, -10)]
    [InlineData(0, 0)]
    [InlineData(-10, 10)]
    public void ShouldMutate(int original, int expected)
    {
        var target = new NumberNegationMutator();

        var result = target.ApplyMutations(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(original)), null).ToList();

        var mutation = result.ShouldHaveSingleItem();

        mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>()
            .Token.Value.ShouldBe(expected);
        mutation.DisplayName.ShouldBe("Number negation mutation");
    }

    [Theory]
    [InlineData(SyntaxKind.StringLiteralExpression)]
    [InlineData(SyntaxKind.CharacterLiteralExpression)]
    [InlineData(SyntaxKind.NullLiteralExpression)]
    [InlineData(SyntaxKind.DefaultLiteralExpression)]
    [InlineData(SyntaxKind.TrueLiteralExpression)]
    [InlineData(SyntaxKind.FalseLiteralExpression)]
    public void ShouldNotMutate(SyntaxKind original)
    {
        var target = new NumberNegationMutator();

        var result = target.ApplyMutations(SyntaxFactory.LiteralExpression(original), null).ToList();

        Assert.Empty(result);
    }
}
