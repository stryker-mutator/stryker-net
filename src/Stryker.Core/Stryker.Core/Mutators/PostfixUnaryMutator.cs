using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class PostfixUnaryMutator : MutatorBase<PostfixUnaryExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        private static readonly Dictionary<SyntaxKind, SyntaxKind> UnaryWithOpposite = new Dictionary<SyntaxKind, SyntaxKind>
        {
            {SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression},
            {SyntaxKind.PostDecrementExpression, SyntaxKind.PostIncrementExpression},
        };

        private static readonly Dictionary<SyntaxKind, SyntaxKind> UnaryWithInverse = new Dictionary<SyntaxKind, SyntaxKind>
        {
            {SyntaxKind.PostIncrementExpression, SyntaxKind.PreIncrementExpression},
            {SyntaxKind.PostDecrementExpression, SyntaxKind.PreDecrementExpression},
        };

        public override IEnumerable<Mutation> ApplyMutations(PostfixUnaryExpressionSyntax node)
        {
            var unaryKind = node.Kind();

            if (UnaryWithOpposite.TryGetValue(unaryKind, out var oppositeKind))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.PostfixUnaryExpression(oppositeKind, node.Operand),
                    DisplayName = $"{unaryKind} to {oppositeKind} mutation",
                    Type = Mutator.Update
                };
            }

            if (UnaryWithInverse.TryGetValue(unaryKind, out var inverseKind))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.PrefixUnaryExpression(inverseKind, node.Operand),
                    DisplayName = $"{unaryKind} to {inverseKind} mutation",
                    Type = Mutator.Update
                };
            }
        }
    }
}
