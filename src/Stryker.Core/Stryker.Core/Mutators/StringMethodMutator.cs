using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators;

/// <summary> Mutator Implementation for String method Mutations </summary>
public class StringMethodMutator : MutatorBase<MemberAccessExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Advanced;

    public override IEnumerable<Mutation> ApplyMutations(MemberAccessExpressionSyntax member, SemanticModel semanticModel)
    {

        var identifier = member.Name.Identifier.ValueText;
        var replacement = GetReplacement(identifier);

        if (replacement == null || (semanticModel!= null && !member.Expression.IsAStringExpression(semanticModel)))
        {
            yield break;
        }

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

    private static Mutation ApplyReplaceMutation(MemberAccessExpressionSyntax node, string original, string replacement) =>
        new()
        {
            OriginalNode = node,
            ReplacementNode = node.WithName(SyntaxFactory.IdentifierName(replacement)).WithCleanTrivia(),
            DisplayName = $"String Method Mutation (Replace {original}() with {replacement}())",
            Type = Mutator.StringMethod
        };
}
