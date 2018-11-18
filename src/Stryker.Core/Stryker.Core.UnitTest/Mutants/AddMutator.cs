using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using System.Collections.Generic;

namespace Stryker.Core.UnitTest.Mutants
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
                    Type = MutatorType.Binary
                };
            }
        }

    }
}
