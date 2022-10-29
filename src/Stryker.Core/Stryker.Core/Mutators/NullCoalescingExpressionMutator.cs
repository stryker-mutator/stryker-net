using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class NullCoalescingExpressionMutator : MutatorBase<BinaryExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Basic;

        public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node)
        {
            if (node.Kind() == SyntaxKind.CoalesceExpression)
            {
                var replacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.CoalesceExpression, node.Right, node.Left); // Flip left and right
                replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken).WithLeadingTrivia(node.Left.GetTrailingTrivia()));
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = replacementNode,
                    DisplayName = $"Flip left and right hand side",
                    Type = Mutator.Assignment,
                };

                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = replacementNode.Left,
                    DisplayName = $"Replace by the left hand side",
                    Type = Mutator.Assignment,
                };

                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = replacementNode.Right,
                    DisplayName = $"Replace by the right hand side",
                    Type = Mutator.Assignment,
                };
            }
        }
    }
}
// TODO type toevoegen en ook in docs toevoegen met sterretje dat dit alleen voor .net geldt met aparte PR
