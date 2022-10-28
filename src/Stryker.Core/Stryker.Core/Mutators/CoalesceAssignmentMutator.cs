using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    internal class CoalesceAssignmentMutator : MutatorBase<AssignmentExpressionSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        public override IEnumerable<Mutation> ApplyMutations(AssignmentExpressionSyntax node)
        {
            var assignmentKind = node.Kind();

            if (assignmentKind == SyntaxKind.CoalesceAssignmentExpression)
            {
                var replacementNode = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, node.Left, node.Right);
                replacementNode = replacementNode.WithOperatorToken(replacementNode.OperatorToken.WithTriviaFrom(node.OperatorToken));
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode = replacementNode,
                    DisplayName = "Coalesce assignment mutation",
                    Type = Mutator.Assignment
                };
            }
        }
    }
}
