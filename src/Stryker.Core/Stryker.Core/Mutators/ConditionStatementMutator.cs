using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Mutators
{
    public class ConditionStatementMutator : MutatorBase<ExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node)
        {
            if (ContainsArguments(node)) yield break;
            
            if (node.Parent is IfStatementSyntax ifStatementSyntax)
            {
                var replacement = PrepareReplacement(ifStatementSyntax.Condition);

                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = replacement,
                    DisplayName = "If statement mutation",
                    Type = Mutator.Unary
                };
            }

            if (node.Parent is WhileStatementSyntax whileStatementSyntax)
            {
                var replacement = PrepareReplacement(whileStatementSyntax.Condition);

                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = replacement,
                    DisplayName = "While statement mutation",
                    Type = Mutator.Unary
                };
            }
        }

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