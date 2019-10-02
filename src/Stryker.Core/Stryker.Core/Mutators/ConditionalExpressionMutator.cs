using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class ConditionalExpressionMutator : MutatorBase<ConditionalExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(ConditionalExpressionSyntax node)
        {
            if (node.Kind() != SyntaxKind.ConditionalExpression)
            {
                yield break;
            }

            var replacementNode = node.Update(
                condition: node.Condition.WithTriviaFrom(node.Condition),
                questionToken: node.QuestionToken,
                whenTrue: node.WhenFalse.WithTriviaFrom(node.WhenTrue),
                colonToken: node.ColonToken,
                whenFalse: node.WhenTrue.WithTriviaFrom(node.WhenFalse));

            yield return new Mutation
            {
                OriginalNode = node,
                ReplacementNode = replacementNode,
                Type = Mutator.Conditional,
                DisplayName = "Conditional expression mutation"
            };
        }
    }
}
