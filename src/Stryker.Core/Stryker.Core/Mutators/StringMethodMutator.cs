using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        if (!IsOperationOnAString(member.Expression, model))
            yield break;

        var identifier = member.Name.Identifier.ValueText;
        var mutation = identifier switch
        {
            "Trim" or "Substring" or "ElementAt" or "ElementAtOrDefault" => ApplyReplaceWithEmptyStringMutation(node,
                identifier),
            "EndsWith" => ApplyEndsWithMutation(node, member),
            "StartsWith" => ApplyStartsWithMutation(node, member),
            "TrimStart" => ApplyTrimStartMutation(node, member),
            "TrimEnd" => ApplyTrimEndMutation(node, member),
            "ToUpper" => ApplyToUpperMutation(node, member),
            "ToLower" => ApplyToLowerMutation(node, member),
            "ToUpperInvariant" => ApplyToUpperInvariantMutation(node, member),
            "ToLowerInvariant" => ApplyToLowerInvariantMutation(node, member),
            "PadLeft" => ApplyPadLeftMutation(node, member),
            "PadRight" => ApplyPadRightMutation(node, member),
            _ => null
        };

        if (mutation == null)
            yield break;

        yield return mutation;
    }

    private static bool IsOperationOnAString(ExpressionSyntax expression, SemanticModel model)
    {
        var typeInfo = model.GetTypeInfo(expression);
        return typeInfo.Type?.SpecialType == SpecialType.System_String;
    }

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

    private Mutation ApplyEndsWithMutation(InvocationExpressionSyntax node, MemberAccessExpressionSyntax member) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = node.ReplaceNode(
                member.Name,
                SyntaxFactory.IdentifierName("StartsWith")
            ),
            DisplayName = "String Method Mutation (Replace EndsWith() with StartsWith())",
            Type = Mutator.String
        };

    private Mutation ApplyStartsWithMutation(InvocationExpressionSyntax node, MemberAccessExpressionSyntax member) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = node.ReplaceNode(
                member.Name,
                SyntaxFactory.IdentifierName("EndsWith")
            ),
            DisplayName = "String Method Mutation (Replace StartsWith() with EndsWith())",
            Type = Mutator.String
        };

    private Mutation ApplyTrimStartMutation(InvocationExpressionSyntax node, MemberAccessExpressionSyntax member) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = node.ReplaceNode(
                member.Name,
                SyntaxFactory.IdentifierName("TrimEnd")
            ),
            DisplayName = "String Method Mutation (Replace TrimStart() with TrimEnd())",
            Type = Mutator.String
        };

    private Mutation ApplyTrimEndMutation(InvocationExpressionSyntax node, MemberAccessExpressionSyntax member) => new()
    {
        OriginalNode = node,
        ReplacementNode = node.ReplaceNode(
            member.Name,
            SyntaxFactory.IdentifierName("TrimStart")
        ),
        DisplayName = "String Method Mutation (Replace TrimEnd() with TrimStart())",
        Type = Mutator.String
    };

    private Mutation ApplyToUpperMutation(InvocationExpressionSyntax node, MemberAccessExpressionSyntax member) => new()
    {
        OriginalNode = node,
        ReplacementNode = node.ReplaceNode(
            member.Name,
            SyntaxFactory.IdentifierName("ToLower")
        ),
        DisplayName = "String Method Mutation (Replace ToUpper() with ToLower())",
        Type = Mutator.String
    };

    private Mutation ApplyToLowerMutation(InvocationExpressionSyntax node, MemberAccessExpressionSyntax member) => new()
    {
        OriginalNode = node,
        ReplacementNode = node.ReplaceNode(
            member.Name,
            SyntaxFactory.IdentifierName("ToUpper")
        ),
        DisplayName = "String Method Mutation (Replace ToLower() with ToUpper())",
        Type = Mutator.String
    };

    private Mutation
        ApplyToUpperInvariantMutation(InvocationExpressionSyntax node, MemberAccessExpressionSyntax member) => new()
    {
        OriginalNode = node,
        ReplacementNode = node.ReplaceNode(
            member.Name,
            SyntaxFactory.IdentifierName("ToLowerInvariant")
        ),
        DisplayName = "String Method Mutation (Replace ToUpperInvariant() with ToLowerInvariant())",
        Type = Mutator.String
    };

    private Mutation
        ApplyToLowerInvariantMutation(InvocationExpressionSyntax node, MemberAccessExpressionSyntax member) => new()
    {
        OriginalNode = node,
        ReplacementNode = node.ReplaceNode(
            member.Name,
            SyntaxFactory.IdentifierName("ToUpperInvariant")
        ),
        DisplayName = "String Method Mutation (Replace ToLowerInvariant() with ToUpperInvariant())",
        Type = Mutator.String
    };

    private Mutation ApplyPadLeftMutation(InvocationExpressionSyntax node, MemberAccessExpressionSyntax member) => new()
    {
        OriginalNode = node,
        ReplacementNode = node.ReplaceNode(
            member.Name,
            SyntaxFactory.IdentifierName("PadRight")
        ),
        DisplayName = "String Method Mutation (Replace PadLeft() with PadRight())",
        Type = Mutator.String
    };

    private Mutation ApplyPadRightMutation(InvocationExpressionSyntax node, MemberAccessExpressionSyntax member) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = node.ReplaceNode(
                member.Name,
                SyntaxFactory.IdentifierName("PadLeft")
            ),
            DisplayName = "String Method Mutation (Replace PadRight() with PadLeft())",
            Type = Mutator.String
        };
}
