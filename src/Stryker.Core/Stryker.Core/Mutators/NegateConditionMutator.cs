using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class NegateConditionMutator : MutatorBase<ExpressionSyntax>, IMutator
    {
        private static IDictionary<SyntaxKind, SyntaxKind> _converters = new Dictionary<SyntaxKind, SyntaxKind>
        {
            [SyntaxKind.EqualsExpression] = SyntaxKind.NotEqualsExpression,
            [SyntaxKind.NotEqualsExpression] = SyntaxKind.EqualsExpression,
            [SyntaxKind.GreaterThanExpression] = SyntaxKind.LessThanOrEqualExpression,
            [SyntaxKind.GreaterThanOrEqualExpression] = SyntaxKind.LessThanExpression,
            [SyntaxKind.LessThanOrEqualExpression] = SyntaxKind.GreaterThanExpression,
            [SyntaxKind.LessThanExpression] = SyntaxKind.GreaterThanOrEqualExpression,
        };

        public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node)
        {
            SyntaxNode replacement = null;
            if (node is IsPatternExpressionSyntax)
            {
                // we can't mutate IsPatternExpression without breaking build
                yield break;
            }

            switch (node.Parent)
            {
                case IfStatementSyntax ifStatementSyntax:
                    if (ifStatementSyntax.Condition == node)
                    {
                        replacement = NegateCondition(ifStatementSyntax.Condition);
                    }
                    break;
                case WhileStatementSyntax whileStatementSyntax:
                    if (whileStatementSyntax.Condition == node)
                    {
                        replacement = NegateCondition(whileStatementSyntax.Condition);
                    }
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

        private static ExpressionSyntax NegateCondition(ExpressionSyntax expressionSyntax)
        {
            if (expressionSyntax.Kind() == SyntaxKind.LogicalNotExpression && expressionSyntax is PrefixUnaryExpressionSyntax prefixUnary)
            {
                return prefixUnary.Operand;
            }

            if (expressionSyntax is BinaryExpressionSyntax binaryExpression && _converters.ContainsKey(expressionSyntax.Kind()))
            {
                return SyntaxFactory.BinaryExpression(_converters[expressionSyntax.Kind()], binaryExpression.Left,
                    binaryExpression.Right);
            }
            return SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(expressionSyntax));
        }
    }
}