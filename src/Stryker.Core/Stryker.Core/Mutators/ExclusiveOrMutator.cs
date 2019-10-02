using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class ExclusiveOrMutator : MutatorBase<BinaryExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node)
        {
            if (node.Kind() != SyntaxKind.ExclusiveOrExpression)
            {
                yield break;
            }

            yield return GetLogicalMutator(node);
            yield return GetIntegralMutator(node);

        }

        private Mutation GetLogicalMutator(BinaryExpressionSyntax node)
        {
            var replacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, node.Left, node.Right);
            replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));

            return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                DisplayName = "Binary expression mutation",
                Type = Mutator.Logical
            };
        }

        private Mutation GetIntegralMutator(BinaryExpressionSyntax node)
        {
            var replacementNode = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.BitwiseNotExpression, SyntaxFactory.ParenthesizedExpression(node));

            return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                DisplayName = "Binary expression mutation",
                Type = Mutator.Bitwise
            };
        }
    }
}
