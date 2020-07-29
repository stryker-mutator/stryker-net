
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class PostfixUnaryExpressionOrchestrator: NodeSpecificOrchestrator<ExpressionStatementSyntax>
    {
        protected override bool CanHandleThis(ExpressionStatementSyntax t)
        {
            return t.Expression is PostfixUnaryExpressionSyntax;
        }

        internal override SyntaxNode OrchestrateMutation(ExpressionStatementSyntax node, MutationContext context)
        {
            var expressionCopy = node.TrackNodes(node, node.Expression);
            return  context.MutateSubExpressionWithIfStatements(node, expressionCopy, node.Expression);
        }
    }
}