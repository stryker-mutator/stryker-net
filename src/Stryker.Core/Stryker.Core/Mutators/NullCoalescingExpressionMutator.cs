using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions.Mutators
{
    public class NullCoalescingExpressionMutator : MutatorBase<BinaryExpressionSyntax>
    {
        public override MutationLevel MutationLevel => MutationLevel.Basic;

        public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node, SemanticModel semanticModel)
        {
            if (node.Kind() != SyntaxKind.CoalesceExpression)
            {
                yield break;
            }

            // Flip left and right
            var replacementNode = SyntaxFactory
                .BinaryExpression(SyntaxKind.CoalesceExpression, node.Right, node.Left);
            replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken
                .WithTriviaFrom(node.OperatorToken)
                .WithLeadingTrivia(node.Left.GetTrailingTrivia())
            );

            // Do not create "left to right", or "remove right" mutants when the right
            // hand side is a throw expression, as they result in invalid code.
            if (!node.Right.IsKind(SyntaxKind.ThrowExpression))
            {
                // Only create a "left to right" mutant if both sides are nullable.
                if (IsNullable(node.Right, semanticModel))
                {
                    yield return new Mutation
                    {
                        OriginalNode = node,
                        ReplacementNode = replacementNode,
                        DisplayName = "Null coalescing mutation (left to right)",
                        Type = Mutator.NullCoalescing
                    };
                }

                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = replacementNode.Left,
                    DisplayName = "Null coalescing mutation (remove right)",
                    Type = Mutator.NullCoalescing
                };
            }

            // Only create a "remove left" mutant if the right side is nullable.
            if (IsNullable(node.Right, semanticModel))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = replacementNode.Right,
                    DisplayName = $"Null coalescing mutation (remove left)",
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
            return typeInfo.Nullability.FlowState == NullableFlowState.MaybeNull;
        }
    }
}
