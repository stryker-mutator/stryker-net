using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using Stryker.Core.Mutants;
using Microsoft.CodeAnalysis.CSharp;

namespace Stryker.Core.Mutators
{
    public class CheckedMutator : Mutator<CheckedExpressionSyntax>, IMutator
    {
        private Dictionary<SyntaxKind, SyntaxKind> _kindsToMutate { get; }

        public CheckedMutator()
        {
            _kindsToMutate = new Dictionary<SyntaxKind, SyntaxKind>
            {
                {SyntaxKind.CheckedExpression, SyntaxKind.UncheckedExpression },
                {SyntaxKind.UncheckedExpression, SyntaxKind.CheckedExpression }
            };
        }

        public override IEnumerable<Mutation> ApplyMutations(CheckedExpressionSyntax node)
        {
            if (node.Kind() == SyntaxKind.CheckedExpression)
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = node.Expression,
                    DisplayName = "Checked mutation",
                    Type = "CheckedMutator"
                };
            }
        }
    }
}
