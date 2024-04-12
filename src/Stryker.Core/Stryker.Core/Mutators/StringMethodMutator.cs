using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RegexParser.Nodes.QuantifierNodes;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators;

/// <summary> Mutator Implementation for String method Mutations </summary>
public class StringMethodMutator : MutatorBase<ExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Advanced;

    public override IEnumerable<Mutation> ApplyMutations(
        ExpressionSyntax node,
        SemanticModel semanticModel
    ) =>
        node switch
        {
            InvocationExpressionSyntax invocation => ApplyInvocationMutations(invocation, semanticModel),
            _ => Enumerable.Empty<Mutation>()
        };

    private IEnumerable<Mutation> ApplyInvocationMutations(InvocationExpressionSyntax node, SemanticModel model)
    {
        if (node is not { Expression: MemberAccessExpressionSyntax member })
            yield break;

        if (!IsOperationOnAString(member, model))
            yield break;

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
                var replacement = GetReplacement(identifier);
                if (replacement == null)
                    yield break;

                yield return ApplyReplaceMutation(member, identifier, replacement);
                break;
        }
    }

    private static string GetReplacement(string identifier) =>
        identifier switch
        {
            "EndsWith" => "StartsWith",
            "StartsWith" => "EndsWith",
            "TrimStart" => "TrimEnd",
            "TrimEnd" => "TrimStart",
            "ToUpper" => "ToLower",
            "ToLower" => "ToUpper",
            "ToUpperInvariant" => "ToLowerInvariant",
            "ToLowerInvariant" => "ToUpperInvariant",
            "PadLeft" => "PadRight",
            "PadRight" => "PadLeft",
            _ => null
        };

    private static bool IsOperationOnAString(ExpressionSyntax expression, SemanticModel model)
    {
        var typeInfo = model.GetTypeInfo(expression);
        return typeInfo.Type?.SpecialType == SpecialType.System_String;
    }

    private Mutation ApplyReplaceMutation(MemberAccessExpressionSyntax node, string original, string replacement) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = node.WithName(SyntaxFactory.IdentifierName(replacement)),
            DisplayName = $"String Method Mutation (Replace {original}() with {replacement}())",
            Type = Mutator.String
        };

    private Mutation ApplyReplaceWithEmptyStringMutation(SyntaxNode node, string identifier) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(string.Empty)
            ),
            DisplayName = $"String Method Mutation (Replace {identifier}() with Empty String)",
            Type = Mutator.String
        };

    private Mutation ApplyReplaceWithCharMutation(SyntaxNode node, string identifier) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(char.MinValue)
            ),
            DisplayName = $"String Method Mutation (Replace {identifier}() with '\\0' char)",
            Type = Mutator.String
        };
}
