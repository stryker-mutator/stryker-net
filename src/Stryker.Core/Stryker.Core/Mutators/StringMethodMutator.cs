using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;

namespace Stryker.Core.Mutators;

public class StringMethodToConstantMutator : MutatorBase<InvocationExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Advanced;

    public override IEnumerable<Mutation> ApplyMutations(InvocationExpressionSyntax node, SemanticModel model)
    {
        if (node is not { Expression: MemberAccessExpressionSyntax member }
        || !IsOperationOnAString(member.Expression, model))
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

    private static bool IsOperationOnAString(ExpressionSyntax expression, SemanticModel model)
    {
        if (model == null)
        {
            switch (expression)
            {
                case LiteralExpressionSyntax literal:
                    return literal.IsKind(SyntaxKind.StringLiteralExpression);
                case InvocationExpressionSyntax:
                    return true;
                case IdentifierNameSyntax:
                {
                    return true;
                    var canBeValidSemanticModel = expression.Ancestors().Last() is CompilationUnitSyntax;
                    if (!canBeValidSemanticModel)
                        return false;

                    var compilation = CSharpCompilation.Create(null)
                        .AddSyntaxTrees(expression.SyntaxTree);
                    model = compilation.GetSemanticModel(expression.SyntaxTree);
                    break;
                }
            }
        }

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
            Type = Mutator.StringMethod
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
            Type = Mutator.StringMethod
        };
}

/// <summary> Mutator Implementation for String method Mutations </summary>
public class StringMethodMutator : MutatorBase<MemberAccessExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Advanced;

    public override IEnumerable<Mutation> ApplyMutations(MemberAccessExpressionSyntax member, SemanticModel model)
    {

        if (!IsOperationOnAString(member.Expression, model))
            yield break;

        var identifier = member.Name.Identifier.ValueText;
 
        var replacement = GetReplacement(identifier);
        if (replacement == null)
            yield break;

        yield return ApplyReplaceMutation(member, identifier, replacement);
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
            "IndexOf" => "LastIndexOf",
            "LastIndexOf" => "IndexOf",
            _ => null
        };

    private static bool IsOperationOnAString(ExpressionSyntax expression, SemanticModel model)
    {
        if (model == null)
        {
            switch (expression)
            {
                case LiteralExpressionSyntax literal:
                {
                    return literal.IsKind(SyntaxKind.StringLiteralExpression);
                }
                case InvocationExpressionSyntax:
                    return true;
                case IdentifierNameSyntax:
                {
                    var canBeValidSemanticModel = expression.Ancestors().Last() is CompilationUnitSyntax;
                    if (!canBeValidSemanticModel)
                        return false;

                    var compilation = CSharpCompilation.Create(null)
                        .AddSyntaxTrees(expression.SyntaxTree);
                    model = compilation.GetSemanticModel(expression.SyntaxTree);
                    break;
                }
            }
        }

        var typeInfo = model.GetTypeInfo(expression);
        return typeInfo.Type?.SpecialType == SpecialType.System_String;
    }

    private Mutation ApplyReplaceMutation(MemberAccessExpressionSyntax node, string original, string replacement) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = node.WithName(SyntaxFactory.IdentifierName(replacement)),
            DisplayName = $"String Method Mutation (Replace {original}() with {replacement}())",
            Type = Mutator.StringMethod
        };
}
