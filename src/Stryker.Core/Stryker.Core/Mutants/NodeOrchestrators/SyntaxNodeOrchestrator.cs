using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class SyntaxNodeOrchestrator: NodeSpecificOrchestrator<SyntaxNode>
    {
        internal override SyntaxNode OrchestrateMutation(SyntaxNode node, MutationContext context)
        {
            return context.MutateNodeAndChildren(node);
        }
    }
}
