using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    /// <summary>
    /// Handles syntax blocks.
    /// </summary>
    internal class BlockOrchestrator : BlockScopeOrchestrator<BlockSyntax>
    {
        public BlockOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        /// <inheritdoc/>
        /// <remarks>Ensure we returns a block after mutants are injected.</remarks>
        protected override StatementSyntax InjectMutations(BlockSyntax sourceNode, StatementSyntax targetNode, MutationContext context)
        {
            var mutated= base.InjectMutations(sourceNode, targetNode, context);
            // ensure we still return a block!
            return mutated is BlockSyntax ? mutated : SyntaxFactory.Block(mutated);
        }
    }
}