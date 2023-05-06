using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators;

public class CheckedMutatorTests : TestBase
{
    [Fact]
    public void ShouldBeMutationLevelComplete()
    {
        var target = new CheckedMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [Theory]
    [InlineData(SyntaxKind.CheckedExpression, "4 + 2", SyntaxKind.AddExpression)]
    public void ShouldMutate(SyntaxKind original, string expression, SyntaxKind expected)
    {
        var target = new CheckedMutator();

        ExpressionSyntax es = SyntaxFactory.ParseExpression(expression);
        var result = target.ApplyMutations(SyntaxFactory.CheckedExpression(original, es)).ToList();

        var mutation = result.ShouldHaveSingleItem();

        mutation.ReplacementNode.IsKind(expected).ShouldBeTrue();
        mutation.DisplayName.ShouldBe("Remove checked expression");
    }

    [Theory]
    [InlineData(SyntaxKind.UncheckedExpression)]
    public void ShouldNotMutate(SyntaxKind original)
    {
        var target = new CheckedMutator();

        ExpressionSyntax es = SyntaxFactory.ParseExpression("4 + 2");
        var result = target.ApplyMutations(SyntaxFactory.CheckedExpression(original, es)).ToList();

        result.ShouldBeEmpty();
    }
}
