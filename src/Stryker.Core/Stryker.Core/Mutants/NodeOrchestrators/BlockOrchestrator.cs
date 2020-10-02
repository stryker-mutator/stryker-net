using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class BlockOrchestrator : BlockScopeOrchestrator<BlockSyntax>
    {
        public BlockOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override StatementSyntax InjectMutations(BlockSyntax originalNode, StatementSyntax mutatedNode, MutationContext context)
        {
            var mutated= base.InjectMutations(originalNode, mutatedNode, context);
            // ensure we still return a block!
            return mutated is BlockSyntax ? mutated : SyntaxFactory.Block(mutated);
        }
    }
}