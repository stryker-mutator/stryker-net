using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators;

public class StringMethodToConstantMutator : MutatorBase<InvocationExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Advanced;

    public override IEnumerable<Mutation> ApplyMutations(InvocationExpressionSyntax node, SemanticModel model)
    {
        if (node is not { Expression: MemberAccessExpressionSyntax member }
        || (model != null && !member.Expression.IsAStringExpression(model)))
        {
            yield break;
        }

        var identifier = member.Name.Identifier.ValueText;
        switch (identifier)
        {
            case "Trim" or "Substring":
                yield return ApplyReplaceWithEmptyStringMutation(node, identifier);
                break;
            case "ElementAt" or "ElementAtOrDefault":
                yield return ApplyReplaceWithCharMutation(node, identifier);
                break;
            default:
                break;
        }
    }

    private static Mutation ApplyReplaceWithEmptyStringMutation(SyntaxNode node, string identifier) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(string.Empty)
            ),
            DisplayName = $"String Method Mutation (Replace {identifier}() with Empty String)",
            Type = Mutator.StringMethod
        };

    private static Mutation ApplyReplaceWithCharMutation(SyntaxNode node, string identifier) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(char.MinValue)
            ),
            DisplayName = $"String Method Mutation (Replace {identifier}() with '\\0' char)",
            Type = Mutator.StringMethod
        };
}
