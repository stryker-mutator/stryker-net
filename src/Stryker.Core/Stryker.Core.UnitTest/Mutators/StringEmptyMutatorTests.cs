using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators;

public class StringEmptyMutatorTests : TestBase
{
    [Fact]
    public void ShouldBeMutationLevelStandard()
    {
        var target = new StringEmptyMutator();
        target.MutationLevel.ShouldBe(MutationLevel.Standard);
    }

    [Fact]
    public void ShouldMutateLowercaseString()
    {
        var node = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.PredefinedType(
                SyntaxFactory.Token(SyntaxKind.StringKeyword)),
            SyntaxFactory.IdentifierName("Empty"));
        var mutator = new StringEmptyMutator();

        var result = mutator.ApplyMutations(node).ToList();

        var mutation = result.ShouldHaveSingleItem();
        mutation.DisplayName.ShouldBe("String mutation");
        var replacement = mutation.ReplacementNode.ShouldBeOfType<LiteralExpressionSyntax>();
        replacement.Token.ValueText.ShouldBe("Stryker was here!");
    }

    [Fact]
    public void ShouldNotMutateUppercaseString()
    {
        var node = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName("String"),
            SyntaxFactory.IdentifierName("Empty"));
        var mutator = new StringEmptyMutator();

        var result = mutator.ApplyMutations(node).ToList();

        result.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("x")]
    [InlineData("string.Empty")]
    [InlineData("args[0].Substring(1)")]
    public void ShouldMutateIsNullOrEmpty(string argument)
    {
        var expression = (InvocationExpressionSyntax)SyntaxFactory.ParseExpression($"string.IsNullOrEmpty({argument})");
        var target = new StringEmptyMutator();
        var mutated = target.ApplyMutations(expression).ToList();

        mutated.Count.ShouldBe(2);
        ValidateMutationIsNullCheck(mutated[0], expression);
        ValidateMutationIsEmptyCheck(mutated[1], expression);
    }

    [Theory]
    [InlineData("x")]
    [InlineData("string.Empty")]
    [InlineData("args[0].Substring(1)")]
    public void ShouldMutateIsNullOrWhiteSpace(string argument)
    {
        var expression = (InvocationExpressionSyntax)SyntaxFactory.ParseExpression($"string.IsNullOrWhiteSpace({argument})");
        var target = new StringEmptyMutator();
        var mutated = target.ApplyMutations(expression).ToList();

        mutated.Count.ShouldBe(3);
        ValidateMutationIsNullCheck(mutated[0], expression);
        ValidateMutationIsEmptyCheck(mutated[1], expression);
        ValidateMutationIsWhiteSpaceCheck(mutated[2], expression);
    }

    [Theory]
    [InlineData("IsNormalized")]
    [InlineData("Test")]
    [InlineData("IsNotNullOrNotEmpty")]
    public void ShouldNotMutateOtherMethods(string method)
    {
        var expression = (InvocationExpressionSyntax)SyntaxFactory.ParseExpression($"string.{method}(x)");
        var target = new StringEmptyMutator();
        var mutated = target.ApplyMutations(expression).ToList();

        mutated.ShouldBeEmpty();
    }

    private void ValidateMutationIsNullCheck(Mutation mutation, InvocationExpressionSyntax original)
    {
        mutation.OriginalNode.ShouldBe(original);
        mutation.DisplayName.ShouldBe("String mutation");
        mutation.Type.ShouldBe(Mutator.String);

        var parenthesizedExpression = mutation.ReplacementNode.ShouldBeOfType<ParenthesizedExpressionSyntax>();
        var binaryExpression = parenthesizedExpression.Expression.ShouldBeOfType<BinaryExpressionSyntax>();

        binaryExpression.Kind().ShouldBe(SyntaxKind.NotEqualsExpression);
        binaryExpression.Left.ToString().ShouldBe(original.ArgumentList.Arguments[0].Expression.ToString());

        var nullLiteral = binaryExpression.Right.ShouldBeOfType<LiteralExpressionSyntax>();

        nullLiteral.Kind().ShouldBe(SyntaxKind.NullLiteralExpression);
    }

    private void ValidateMutationIsEmptyCheck(Mutation mutation, InvocationExpressionSyntax original)
    {
        mutation.OriginalNode.ShouldBe(original);
        mutation.DisplayName.ShouldBe("String mutation");
        mutation.Type.ShouldBe(Mutator.String);

        var parenthesizedExpression = mutation.ReplacementNode.ShouldBeOfType<ParenthesizedExpressionSyntax>();
        var binaryExpression = parenthesizedExpression.Expression.ShouldBeOfType<BinaryExpressionSyntax>();

        binaryExpression.Kind().ShouldBe(SyntaxKind.NotEqualsExpression);
        binaryExpression.Left.ToString().ShouldBe(original.ArgumentList.Arguments[0].Expression.ToString());

        var emptyLiteral = binaryExpression.Right.ShouldBeOfType<LiteralExpressionSyntax>();

        emptyLiteral.Kind().ShouldBe(SyntaxKind.StringLiteralExpression);
        emptyLiteral.Token.ToString().ShouldBe(@"""""");
    }

    private void ValidateMutationIsWhiteSpaceCheck(Mutation mutation, InvocationExpressionSyntax original)
    {
        mutation.OriginalNode.ShouldBe(original);
        mutation.DisplayName.ShouldBe("String mutation");
        mutation.Type.ShouldBe(Mutator.String);

        var parenthesizedExpression = mutation.ReplacementNode.ShouldBeOfType<ParenthesizedExpressionSyntax>();
        var binaryExpression = parenthesizedExpression.Expression.ShouldBeOfType<BinaryExpressionSyntax>();

        binaryExpression.Kind().ShouldBe(SyntaxKind.NotEqualsExpression);
        binaryExpression.Left.ToString().ShouldBe(original.ArgumentList.Arguments[0].Expression.ToString() + ".Trim()");

        var emptyLiteral = binaryExpression.Right.ShouldBeOfType<LiteralExpressionSyntax>();

        emptyLiteral.Kind().ShouldBe(SyntaxKind.StringLiteralExpression);
        emptyLiteral.Token.ToString().ShouldBe(@"""""");
    }
}
