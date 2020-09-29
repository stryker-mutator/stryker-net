using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class SyntaxNodeOrchestrator: NodeSpecificOrchestrator<SyntaxNode>
    {
        protected override SyntaxNode OrchestrateMutation(SyntaxNode node, MutationContext context)
        {
            // we don't know (yet?) how to control mutations outside of expression, statement or block. So need to mutate other syntax node.
            return node.ReplaceNodes(node.ChildNodes(), (original, mutated) => MutantOrchestrator.Mutate(original, context));
        }

        public SyntaxNodeOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
