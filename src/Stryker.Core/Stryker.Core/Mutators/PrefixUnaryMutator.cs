using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class PrefixUnaryMutator : Mutator<PrefixUnaryExpressionSyntax>, IMutator
    {
        private readonly Dictionary<SyntaxKind, SyntaxKind> _unaryWithOpposite = new Dictionary<SyntaxKind, SyntaxKind>()
        {
            {SyntaxKind.UnaryMinusExpression, SyntaxKind.UnaryPlusExpression},
            {SyntaxKind.UnaryPlusExpression, SyntaxKind.UnaryMinusExpression},
            {SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression},
            {SyntaxKind.PreDecrementExpression, SyntaxKind.PreIncrementExpression},
        };

        public override IEnumerable<Mutation> ApplyMutations(PrefixUnaryExpressionSyntax node)
        {
            var unaryKind = node.Kind();
            if (_unaryWithOpposite.TryGetValue(unaryKind, out var oppositeKind))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.PrefixUnaryExpression(oppositeKind, node.Operand),
                    DisplayName = unaryKind + " to " + oppositeKind + " mutation",
                    Type = nameof(PrefixUnaryMutator)
                };
            }
            else if (node.IsKind(SyntaxKind.BitwiseNotExpression))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = node.Operand,
                    DisplayName = "Bitwise Not to un-Bitwise",
                    Type = nameof(PrefixUnaryMutator)
                };
            }
            else if (node.IsKind(SyntaxKind.LogicalNotExpression))
            {
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = node.Operand,
                    DisplayName = "Bitwise Not to un-Bitwise",
                    Type = nameof(PrefixUnaryMutator)
                };
            }
        }
    }
}