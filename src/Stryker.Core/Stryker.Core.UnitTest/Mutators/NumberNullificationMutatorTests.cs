using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators;

public class NumberNullificationMutatorTests : TestBase
{
    [Fact]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new NumberNullificationMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [Theory]
    [InlineData(10, 0)]
    [InlineData(0, 0)]
    [InlineData(-10, 0)]
    public void ShouldMutate(int original, int expected)
    {
        var target = new NumberNullificationMutator();

        var result = target.ApplyMutations(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(original)), null).ToList();

        var mutation = result.ShouldHaveSingleItem();

        mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>()
            .Token.Value.ShouldBe(expected);
        mutation.DisplayName.ShouldBe("Number nullification mutation");
    }

    [Theory]
    [InlineData("array[1]")]
    public void ShouldNotMutate(string expression)
    {
        var target = new NumberNullificationMutator();

        var parent = SyntaxFactory.ParseExpression(expression);
        var child = parent.DescendantNodes(_ => true).OfType<LiteralExpressionSyntax>().FirstOrDefault();


        var result = target.ApplyMutations(child, null).ToList();

        Assert.Empty(result);
    }
}
