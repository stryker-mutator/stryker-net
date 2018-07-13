using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Stryker.Core.Mutants;
using Microsoft.CodeAnalysis.CSharp;

namespace Stryker.Core.Mutators
{
    public class CheckedMutator : Mutator<CheckedExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(CheckedExpressionSyntax node)
        {
            if (node.Kind() == SyntaxKind.CheckedExpression)
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = node.Expression,
                    DisplayName = "Remove checked expression",
                    Type = nameof(CheckedMutator)
                };
            }
        }
    }
}
