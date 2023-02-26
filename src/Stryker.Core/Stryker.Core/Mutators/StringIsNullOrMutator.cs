using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    /// <summary>
    /// Mutator that will mutate calls of <c>string.IsNullOrEmpty</c> and <c>string.IsNullOrWhiteSpace</c>.
    /// </summary>
    /// <remarks>
    /// Will only apply the mutation to the lowercase <c>string</c> since that is a reserved keyword in C# and can be distinguished from any variable or member access.
    /// </remarks>
    public class StringIsNullOrMutator : MutatorBase<InvocationExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax memberAccessExpression &&
                memberAccessExpression.Expression is PredefinedTypeSyntax typeSyntax &&
                typeSyntax.Keyword.ValueText == "string")
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

        private Mutation ApplyIsNullMutation(InvocationExpressionSyntax node) => new()
        {
            OriginalNode = node,
            ReplacementNode = SyntaxFactory.BinaryExpression(
                SyntaxKind.NotEqualsExpression,
                node.ArgumentList.Arguments[0].Expression,
                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)),
            DisplayName = "String mutation",
            Type = Mutator.String
        };

        private Mutation ApplyIsEmptyMutation(InvocationExpressionSyntax node) => new()
        {
            OriginalNode = node,
            ReplacementNode = SyntaxFactory.BinaryExpression(
                SyntaxKind.NotEqualsExpression,
                node.ArgumentList.Arguments[0].Expression,
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(""))),
            DisplayName = "String mutation",
            Type = Mutator.String
        };

        private Mutation ApplyIsWhiteSpaceMutation(InvocationExpressionSyntax node) => new()
        {
            OriginalNode = node,
            ReplacementNode = SyntaxFactory.BinaryExpression(
                SyntaxKind.NotEqualsExpression,
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            node.ArgumentList.Arguments[0].Expression,
                            SyntaxFactory.IdentifierName("Trim")),
                        SyntaxFactory.ArgumentList()),
                    SyntaxFactory.IdentifierName("Length")),
                SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(0))),
            DisplayName = "String mutation",
            Type = Mutator.String
        };
    }
}
