using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class NegateConditionMutator : MutatorBase<ExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node)
        {
            SyntaxNode replacement = null;

            switch (node.Parent)
            {
                case IfStatementSyntax ifStatementSyntax:
                    replacement = NegateCondition(ifStatementSyntax.Condition);
                    break;
                case WhileStatementSyntax whileStatementSyntax:
                    replacement = NegateCondition(whileStatementSyntax.Condition);
                    break;
                case ConditionalExpressionSyntax conditionalExpressionSyntax:
                    if (conditionalExpressionSyntax.Condition == node)
                    {
                        replacement = NegateCondition(conditionalExpressionSyntax.Condition);
                    }
                    break;
            }

            if (replacement != null)
            {
                yield return new Mutation()
                {
                    OriginalNode = node,
                    ReplacementNode = replacement,
                    DisplayName = "Negate expression",
                    Type = Mutator.Boolean
                };
            }
        }

        private static PrefixUnaryExpressionSyntax NegateCondition(ExpressionSyntax expressionSyntax)
        {
            return SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, expressionSyntax);
        }
    }
}