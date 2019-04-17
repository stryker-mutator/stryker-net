using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class NegateConditionMutator : MutatorBase<ExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node)
        {
            if (ContainsArguments(node))
            {
                yield break;
            }
            
            if (node.Parent is IfStatementSyntax ifStatementSyntax)
            {
                if (!IsInvocationExpression(ifStatementSyntax.Condition))
                {
                    yield break;
                }
                
                var replacement = PrepareReplacement(ifStatementSyntax.Condition);

                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = replacement,
                    DisplayName = "If statement mutation",
                    Type = Mutator.Boolean
                };
            }

            if (node.Parent is WhileStatementSyntax whileStatementSyntax)
            {
                if (!IsInvocationExpression(whileStatementSyntax.Condition))
                {
                    yield break;
                }
                
                var replacement = PrepareReplacement(whileStatementSyntax.Condition);

                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = replacement,
                    DisplayName = "While statement mutation",
                    Type = Mutator.Boolean
                };
            }
        }

        private bool IsInvocationExpression(ExpressionSyntax condition)
            => condition is InvocationExpressionSyntax;

        private static PrefixUnaryExpressionSyntax PrepareReplacement(ExpressionSyntax expressionSyntax)
        {
            return SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression,
                expressionSyntax);
        }

        private static bool ContainsArguments(ExpressionSyntax node)
        {
            return node is InvocationExpressionSyntax invocationExpressionSyntax &&
                   invocationExpressionSyntax.ArgumentList.Arguments.Any();
        }
    }
}