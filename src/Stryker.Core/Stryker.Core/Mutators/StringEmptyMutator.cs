using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators;

/// <summary>
/// Mutator that will mutate:
/// the access to <c>string.Empty</c> to a string that is not empty.
/// calls of <c>string.IsNullOrEmpty</c> and <c>string.IsNullOrWhiteSpace</c>
/// </summary>
/// <remarks>
/// Will only apply the mutation to the lowercase <c>string</c> since that is a reserved keyword in c# and can be distinguished from any variable or member access.
/// </remarks>
public class StringEmptyMutator : MutatorBase<ExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node, SemanticModel semanticModel) => node switch
    {
        MemberAccessExpressionSyntax memberAccess => ApplyMutations(memberAccess),
        InvocationExpressionSyntax invocation => ApplyMutations(invocation),
        _ => Enumerable.Empty<Mutation>()
    };

    private IEnumerable<Mutation> ApplyMutations(MemberAccessExpressionSyntax node)
    {
        if (IsAccessToStringPredefinedType(node.Expression) &&
            node.Name.Identifier.ValueText == nameof(string.Empty))
        {
            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode =
                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal("Stryker was here!")),
                DisplayName = "String mutation",
                Type = Mutator.String
            };
        }
    }

    private IEnumerable<Mutation> ApplyMutations(InvocationExpressionSyntax node)
    {
        if (node.Expression is MemberAccessExpressionSyntax memberAccessExpression &&
            IsAccessToStringPredefinedType(memberAccessExpression.Expression))
        {
            var identifier = memberAccessExpression.Name.Identifier.ValueText;

            if (identifier.StartsWith("IsNullOr"))
            {
                yield return ApplyIsNullMutation(node);
                yield return ApplyIsEmptyMutation(node);

                if (identifier == nameof(string.IsNullOrWhiteSpace))
                {
                    yield return ApplyIsWhiteSpaceMutation(node);
                }
            }
        }
    }

    private bool IsAccessToStringPredefinedType(ExpressionSyntax expression) =>
        expression is PredefinedTypeSyntax typeSyntax &&
        typeSyntax.Keyword.ValueText == "string";

    private Mutation ApplyIsNullMutation(InvocationExpressionSyntax node) => new()
    {
        OriginalNode = node,
        ReplacementNode =
            SyntaxFactory.ParenthesizedExpression(
                SyntaxFactory.BinaryExpression(
                    SyntaxKind.NotEqualsExpression,
                    node.ArgumentList.Arguments[0].Expression,
                    SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))),
        DisplayName = "String mutation",
        Type = Mutator.String
    };

    private Mutation ApplyIsEmptyMutation(InvocationExpressionSyntax node) => new()
    {
        OriginalNode = node,
        ReplacementNode = SyntaxFactory.ParenthesizedExpression(
            SyntaxFactory.BinaryExpression(
                SyntaxKind.NotEqualsExpression,
                node.ArgumentList.Arguments[0].Expression,
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("")))),
        DisplayName = "String mutation",
        Type = Mutator.String
    };

    private Mutation ApplyIsWhiteSpaceMutation(InvocationExpressionSyntax node) => new()
    {
        OriginalNode = node,
        ReplacementNode =
            SyntaxFactory.ParenthesizedExpression(
                SyntaxFactory.BinaryExpression(
                    SyntaxKind.NotEqualsExpression,
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            node.ArgumentList.Arguments[0].Expression,
                            SyntaxFactory.IdentifierName("Trim")),
                        SyntaxFactory.ArgumentList()),
                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("")))),
        DisplayName = "String mutation",
        Type = Mutator.String
    };
}
