using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutators;

public class NegateConditionMutator : MutatorBase<ExpressionSyntax>
{
    public override MutationLevel MutationLevel => MutationLevel.Standard;

    public override IEnumerable<Mutation> ApplyMutations(ExpressionSyntax node, SemanticModel semanticModel)
    {
        SyntaxNode replacement = null;
        if (node is IsPatternExpressionSyntax)
        {
            // we can't mutate IsPatternExpression without breaking build
            yield break;
        }

        // we don't mutate expressions that can be mutated by binary expression mutator
        if (WillBeMutatedByOtherMutators(node))
        {
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
        }

        if (replacement != null)
        {
            yield return new Mutation()
            {
                OriginalNode = node,
                ReplacementNode = replacement.WithCleanTriviaFrom(node),
                DisplayName = "Negate expression",
                Type = Mutator.Boolean
            };
        }
    }

    private static bool WillBeMutatedByOtherMutators(ExpressionSyntax node)
    {
        if (node.Kind() == SyntaxKind.LogicalNotExpression)
        {
            return true;
        }
        if (node is ParenthesizedExpressionSyntax parenthesized)
        {
            return WillBeMutatedByOtherMutators(parenthesized.Expression);
        }
        if (node is BinaryExpressionSyntax binary)
        {
            return binary.Kind() == SyntaxKind.EqualsExpression || binary.Kind() == SyntaxKind.NotEqualsExpression;
        }
        return false;
    }

    private static PrefixUnaryExpressionSyntax NegateCondition(ExpressionSyntax expressionSyntax)
        => SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, SyntaxFactory.ParenthesizedExpression(expressionSyntax.WithCleanTrivia()));
}
