using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class BooleanMutator : MutatorBase<LiteralExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        private static readonly Dictionary<SyntaxKind, SyntaxKind> KindsToMutate = new Dictionary<SyntaxKind, SyntaxKind>
        {
            {SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression },
            {SyntaxKind.FalseLiteralExpression, SyntaxKind.TrueLiteralExpression }
        };


        public override IEnumerable<Mutation> ApplyMutations(LiteralExpressionSyntax node)
        {
            if (KindsToMutate.ContainsKey(node.Kind()))
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = SyntaxFactory.LiteralExpression(KindsToMutate[node.Kind()]),
                    DisplayName = "Boolean mutation",
                    Type = Mutator.Boolean
                };
            }
        }
    }
}
