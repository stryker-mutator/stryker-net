using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class BlockScopeOrchestrator<T>: StatementSpecificOrchestrator<T> where T: StatementSyntax
    {
        public BlockScopeOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {}

        protected override bool NewContext => true;

        protected override StatementSyntax InjectMutations(T originalNode, StatementSyntax mutatedNode, MutationContext context)
        {
            // we inject all pending mutations
            var mutationsToInject = context.StatementLevelControlledMutations
                .Union(context.BlockLevelControlledMutations);
            // mutations are controlled by 'if's
            var blockLevelMutations = MutantPlacer.PlaceStatementControlledMutations(mutatedNode,
                mutationsToInject.Select(m =>
                    (m.Id, (originalNode as StatementSyntax).InjectMutation(m.Mutation))));
            context.BlockLevelControlledMutations.Clear();
            context.StatementLevelControlledMutations.Clear();
            // ensure we have a block at the end
            return blockLevelMutations;
        }

        protected override MutationContext StoreMutations(IEnumerable<Mutant> mutations, T node,
            MutationContext context)
        {
            context.BlockLevelControlledMutations.AddRange(mutations);
            return context;
        }
    }
}
