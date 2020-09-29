using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StatementSpecificOrchestrator<T>: NodeSpecificOrchestrator<T> where T: StatementSyntax
    {
        public StatementSpecificOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override T InjectMutations(T originalNode, T mutatedNode, MutationContext context, IEnumerable<Mutant> mutations)
        {
            context.StatementLevelControlledMutations.AddRange(mutations);
            var mutated = MutantPlacer.PlaceIfControlledMutations(mutatedNode as StatementSyntax,
                context.StatementLevelControlledMutations.Select( m => (m.Id, (originalNode as StatementSyntax).InjectMutation(m.Mutation))));
            return mutated as T;
        }

        public override SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            var newContext = context.Clone();
            var mutated = base.Mutate(node, newContext);
            context.BlockLevelControlledMutations.AddRange(newContext.BlockLevelControlledMutations);
            return mutated;
        }
    }
}
