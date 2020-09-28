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

        public override SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            var newContext = context.Clone();
            var mutated = base.Mutate(node, newContext);
            mutated = MutantPlacer.PlaceIfControlledMutations(mutated as StatementSyntax,
                newContext.StatementLevelControlledMutations.Select( m => (m.Id, (node as StatementSyntax).InjectMutation(m.Mutation))));
            context.BlockLevelControlledMutations.AddRange(newContext.BlockLevelControlledMutations);
            return mutated;
        }
    }
}
