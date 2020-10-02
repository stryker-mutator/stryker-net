using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StatementSpecificOrchestrator<T>: NodeSpecificOrchestrator<T, StatementSyntax> where T: StatementSyntax
    {
        public StatementSpecificOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override bool NewContext => true;

        protected override StatementSyntax InjectMutations(T originalNode, StatementSyntax mutatedNode, MutationContext context)
        {
            var mutated = MutantPlacer.PlaceStatementControlledMutations(mutatedNode,
                context.StatementLevelControlledMutations.Select( m => (m.Id, (originalNode as StatementSyntax).InjectMutation(m.Mutation))));
            context.StatementLevelControlledMutations.Clear();
            return mutated;
        }

        protected override MutationContext StoreMutations(IEnumerable<Mutant> mutations, T node,
            MutationContext context)
        {
            context.StatementLevelControlledMutations.AddRange(mutations);
            return context;
        }

    }
}
