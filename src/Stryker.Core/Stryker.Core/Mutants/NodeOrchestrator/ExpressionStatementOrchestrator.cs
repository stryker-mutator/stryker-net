
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class ExpressionStatementOrchestrator : NodeSpecificOrchestrator<ExpressionStatementSyntax>
    {
        internal override SyntaxNode OrchestrateMutation(ExpressionStatementSyntax node, MutationContext context)
        {
            return node.ReplaceNode(node.Expression, context.Mutate(node.Expression));
        }
    }
}