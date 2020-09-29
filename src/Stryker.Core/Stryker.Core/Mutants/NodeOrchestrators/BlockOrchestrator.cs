using System.Collections.Generic;
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

        protected override BlockSyntax InjectMutations(BlockSyntax originalNode, BlockSyntax mutatedNode, MutationContext context,
            IEnumerable<Mutant> mutations)
        {
            context.BlockLevelControlledMutations.AddRange(context.StatementLevelControlledMutations);
            return InjectBlockLevelMutations(originalNode, mutatedNode, context);
        }

        internal static BlockSyntax InjectBlockLevelMutations(BlockSyntax originalNode, BlockSyntax mutatedNode,
            MutationContext context)
        {
            var blockLevelMutations = MutantPlacer.PlaceIfControlledMutations(mutatedNode,
                context.BlockLevelControlledMutations.Select(m =>
                    (m.Id, (originalNode as StatementSyntax).InjectMutation(m.Mutation))));
            return !(blockLevelMutations is BlockSyntax block) ? SyntaxFactory.Block(blockLevelMutations) : block;
        }

        public override SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            var newContext = context.Clone();
            return base.Mutate(node, newContext);
        }
    }
}
