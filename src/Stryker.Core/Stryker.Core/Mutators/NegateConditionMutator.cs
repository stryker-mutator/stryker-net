using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Mutators
{
    public class NegateConditionMutator : MutatorBase<InvocationExpressionSyntax>, IMutator
    {
        public override IEnumerable<Mutation> ApplyMutations(InvocationExpressionSyntax node)
        {
            if (node.ArgumentList.Arguments.Any())
            {
                yield break;
            }

            SyntaxNode replacement = null;

            switch (node.Parent)
            {
                case IfStatementSyntax ifStatementSyntax:
                    replacement = NegateCondition(ifStatementSyntax?.Condition);
                    break;
                case WhileStatementSyntax whileStatementSyntax:
                    replacement = NegateCondition(whileStatementSyntax.Condition);
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