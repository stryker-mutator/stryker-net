using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutators
{
    public class NullCoalescingExpressionMutator : MutatorBase<BinaryExpressionSyntax>
    {
        public override MutationLevel MutationLevel => MutationLevel.Basic;

        public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node, SemanticModel semanticModel)
        {
            if (node.Kind() == SyntaxKind.CoalesceExpression)
            {
                var replacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.CoalesceExpression, node.Right, node.Left); // Flip left and right
                replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken).WithLeadingTrivia(node.Left.GetTrailingTrivia()));

                // Do not create "left to right", or "remove right" mutants when the right
                // hand side is a throw expression, as they result in invalid code.
                if (!node.Right.IsKind(SyntaxKind.ThrowExpression))
                {
                    yield return new Mutation
                    {
                        OriginalNode = node,
                        ReplacementNode = replacementNode,
                        DisplayName = $"Null coalescing mutation (left to right)",
                        Type = Mutator.NullCoalescing,
                    };

                    yield return new Mutation
                    {
                        OriginalNode = node,
                        ReplacementNode = replacementNode.Left,
                        DisplayName = $"Null coalescing mutation (remove right)",
                        Type = Mutator.NullCoalescing,
                    };
                }

                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = replacementNode.Right,
                    DisplayName = $"Null coalescing mutation (remove left)",
                    Type = Mutator.NullCoalescing,
                };
            }
        }
    }
}
