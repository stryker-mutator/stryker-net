using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class SyntaxNodeOrchestrator: NodeSpecificOrchestrator<SyntaxNode>
    {
        protected override SyntaxNode OrchestrateMutation(SyntaxNode node, MutationContext context)
        {
            var mutatedNode = context.MutateNodeAndChildren(node);
            return mutatedNode;
        }

        public SyntaxNodeOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
