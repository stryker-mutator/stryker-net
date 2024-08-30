using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;

namespace Stryker.Abstractions.Mutators
{
    public class ConditionalExpressionMutator : MutatorBase<ConditionalExpressionSyntax>
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(ConditionalExpressionSyntax node, SemanticModel semanticModel)
        {
            // if the condition contains variable declarations, we should not mutate it
            if (node.Condition.DescendantNodes().OfType<DeclarationPatternSyntax>().Any())
            {
                yield break;
            }

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
