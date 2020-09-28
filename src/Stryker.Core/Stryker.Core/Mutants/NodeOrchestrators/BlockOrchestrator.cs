using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class BlockOrchestrator: StatementSpecificOrchestrator<BlockSyntax>
    {
        public BlockOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        public override SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            var newContext = context.Clone();
            var mutated = base.Mutate(node, newContext);

            var blockLevelMutations = MutantPlacer.PlaceIfControlledMutations(mutated as StatementSyntax,
                newContext.BlockLevelControlledMutations.Select( m => (m.Id,  (node as StatementSyntax).InjectMutation(m.Mutation))));
            if (!(blockLevelMutations is BlockSyntax))
            {
                blockLevelMutations = SyntaxFactory.Block(blockLevelMutations);
            }
            return blockLevelMutations;
        }
    }
}
