using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Collections.Generic;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators;

public class InitializerMutatorTests : TestBase
{
    [Fact]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new InitializerMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [Fact]
    public void ShouldRemoveValuesFromArrayInitializer()
    {
        var initializerExpression = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression,
            SyntaxFactory.SeparatedList(new List<ExpressionSyntax> {
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(5))
            }));
        var target = new InitializerMutator();

        var result = target.ApplyMutations(initializerExpression);

        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("Array initializer mutation");

        var replacement = mutation.ReplacementNode.ShouldBeOfType<InitializerExpressionSyntax>();
        replacement.Expressions.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldNotMutateEmptyInitializer()
    {
        var emptyInitializerExpression = SyntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression,
            SyntaxFactory.SeparatedList<ExpressionSyntax>());
        var target = new InitializerMutator();

        var result = target.ApplyMutations(emptyInitializerExpression);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldNotMutateStackallocArrayCreationExpressionSyntax()
    {
        var arrayCreationExpression = SyntaxFactory.ParseExpression("stackalloc int[] { 0 }") as StackAllocArrayCreationExpressionSyntax;

        var target = new InitializerMutator();

        var result = target.ApplyMutations(arrayCreationExpression.Initializer);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldNotMutateArrayCreationExpressionSyntax()
    {
        var arrayCreationExpression = SyntaxFactory.ParseExpression("new int[] { 0 }") as ArrayCreationExpressionSyntax;

        var target = new InitializerMutator();

        var result = target.ApplyMutations(arrayCreationExpression.Initializer);

        result.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldNotMutateImplicitArrayCreationExpressionSyntax()
    {
        var arrayCreationExpression = SyntaxFactory.ParseExpression("new [] { 0 }") as ImplicitArrayCreationExpressionSyntax;

        var target = new InitializerMutator();

        var result = target.ApplyMutations(arrayCreationExpression.Initializer);

        result.ShouldBeEmpty();
    }
}
