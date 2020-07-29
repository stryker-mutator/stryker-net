using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class SyntaxNodeOrchestrator: NodeSpecificOrchestrator<SyntaxNode>
    {
        internal override SyntaxNode OrchestrateMutation(SyntaxNode node, MutationContext context)
        {
            return context.MutateChildren(node);
        }
    }
}
