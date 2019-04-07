using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class IfStatementMutator : MutatorBase<IfStatementSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(IfStatementSyntax node)
        {
            if (node.IsKind(SyntaxKind.IfStatement))
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = node.WithCondition(SyntaxFactory.ParseExpression($"!{node.Condition}")),
                    DisplayName = "If statement mutation",
                    Type = Mutator.If
                };
            }
        }
    }
}