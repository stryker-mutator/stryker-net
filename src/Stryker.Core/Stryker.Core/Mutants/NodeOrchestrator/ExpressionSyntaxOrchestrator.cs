using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class ExpressionSyntaxOrchestrator: NodeSpecificOrchestrator<ExpressionSyntax>
    {
        internal override SyntaxNode OrchestrateMutation(ExpressionSyntax node, MutationContext context)
        {
            return context.MutateNodeAndChildren(node);
        }
    }    
}
