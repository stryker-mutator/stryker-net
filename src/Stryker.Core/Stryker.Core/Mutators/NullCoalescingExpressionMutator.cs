using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Mutators;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators;

public class NullCoalescingExpressionMutator : MutatorBase<BinaryExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Basic;

    public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node, SemanticModel semanticModel)
    {
        if (node.Kind() != SyntaxKind.CoalesceExpression)
        {
            yield break;
        }

        var rightPartIsNullable = IsNullable(node.Right, semanticModel);
        // Do not create "left to right", or "remove right" mutants when the right
        // hand side is a throw expression, as they result in invalid code.
        if (!node.Right.IsKind(SyntaxKind.ThrowExpression))
        {
            // Only create a "left to right" mutant if both sides are nullable.
            if (rightPartIsNullable)
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = node.WithLeft(node.Right).WithRight(node.Left).WithCleanTrivia(),
                    DisplayName = "Null coalescing mutation (left to right)",
                    Type = Mutator.NullCoalescing
                };
            }

            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = node.Right.WithCleanTrivia(),
                DisplayName = "Null coalescing mutation (remove left)",
                Type = Mutator.NullCoalescing
            };
        }

        // Only create a "remove right" mutant if the right side is nullable.
        if (rightPartIsNullable || node.Right.IsKind(SyntaxKind.CollectionExpression))
        {
            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = node.Left.WithCleanTrivia(),
                DisplayName = $"Null coalescing mutation (remove right)",
                Type = Mutator.NullCoalescing
            };
        }

    }

    private static bool IsNullable(SyntaxNode node, SemanticModel semanticModel)
    {
        if (semanticModel == null)
        {
            // If the semantic model is not available, we cannot determine if the type is nullable,
            // so we should let it try to compile.
            return true;
        }

        var typeInfo = semanticModel.GetTypeInfo(node);
        // assume nullability if type resolution failed for some reason
        return (typeInfo.ConvertedType is not null && typeInfo.ConvertedType.TypeKind is TypeKind.Error or TypeKind.Unknown)
            || typeInfo.Nullability.FlowState == NullableFlowState.MaybeNull;
    }
}
