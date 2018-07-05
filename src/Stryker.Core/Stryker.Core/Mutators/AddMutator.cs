using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    /// <summary>
    /// A simple mutator that changes AddExpressions to SubtractExpressions, useful for UnitTesting
    /// </summary>
    public class AddMutator : Mutator<BinaryExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(BinaryExpressionSyntax node)
        {
            if (node.IsKind(SyntaxKind.AddExpression))
            {
                yield return new Mutation() {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, node.Left, node.Right),
                    DisplayName = "Add to Subtract mutation",
                    Type = "AddMutator"
                };
            }
        }

    }
}
