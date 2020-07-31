using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class AssignmentStatementOrchestrator : NodeSpecificOrchestrator<ExpressionStatementSyntax>
    {
        protected override bool CanHandleThis(ExpressionStatementSyntax t)
        {
            return t.Expression is AssignmentExpressionSyntax;
        }

        internal override SyntaxNode OrchestrateMutation(ExpressionStatementSyntax node, MutationContext context)
        {
            var assign = node.Expression as AssignmentExpressionSyntax;
            var expressionCopy = node.TrackNodes(node, assign, assign.Right);
            // mutate +=, *=, ...
            var result =  context.MutateSubExpressionWithIfStatements(node, expressionCopy, assign);
            // mutate the part right to the equal sign
            return result.ReplaceNode(result.GetCurrentNode(assign.Right), context.Mutate(assign.Right));
        }
    }
}