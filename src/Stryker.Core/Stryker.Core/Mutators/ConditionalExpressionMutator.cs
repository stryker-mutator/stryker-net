using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class ConditionalExpressionMutator : MutatorBase<SyntaxNode>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Complete;

        public override IEnumerable<Mutation> ApplyMutations(SyntaxNode node)
        {
            if (node.Parent is not ConditionalExpressionSyntax parentExpression)
                yield break;

            if (parentExpression.Condition != node)
                yield break;

            yield return new Mutation()
            {
                Type = Mutator.Conditional,
                DisplayName = "Conditional (true) mutation",
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)
            };

            yield return new Mutation()
            {
                Type = Mutator.Conditional,
                DisplayName = "Conditional (false) mutation",
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)
            };
        }
    }
}
