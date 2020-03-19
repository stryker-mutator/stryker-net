using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class BooleanMutator : MutatorBase<LiteralExpressionSyntax>, IMutator
    {
        private Dictionary<SyntaxKind, SyntaxKind> _kindsToMutate { get; }

        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public BooleanMutator()
        {
            _kindsToMutate = new Dictionary<SyntaxKind, SyntaxKind>
            {
                {SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression },
                {SyntaxKind.FalseLiteralExpression, SyntaxKind.TrueLiteralExpression }
            };
        }

        public override IEnumerable<Mutation> ApplyMutations(LiteralExpressionSyntax node)
        {
            if (_kindsToMutate.ContainsKey(node.Kind()))
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.LiteralExpression(_kindsToMutate[node.Kind()]),
                    DisplayName = "Boolean mutation",
                    Type = Mutator.Boolean
                };
            }
        }
    }
}
