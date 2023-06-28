using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class ConditionalExpressionMutator : MutatorBase<ConditionalExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(ConditionalExpressionSyntax node)
        {
            yield return new Mutation()
            {
                Type = Mutator.Conditional,
                DisplayName = "Conditional (true) mutation",
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.ParenthesizedExpression(
                    SyntaxFactory.ConditionalExpression(
                        SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression),
                        node.WhenTrue,
                        node.WhenFalse
                    )
                )
            };

            yield return new Mutation()
            {
                Type = Mutator.Conditional,
                DisplayName = "Conditional (false) mutation",
                OriginalNode = node,
                ReplacementNode = SyntaxFactory.ParenthesizedExpression(
                    SyntaxFactory.ConditionalExpression(
                        SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
                        node.WhenTrue,
                        node.WhenFalse
                    )
                )
            };
        }
    }
}
