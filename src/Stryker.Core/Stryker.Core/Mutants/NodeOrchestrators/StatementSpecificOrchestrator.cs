using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    /// <summary>
    /// General handler for Statements. Remember to inherit from this class when you wand to create a statement specific logic.
    /// </summary>
    /// <typeparam name="T">Statement syntax type. Must inherit from <see cref="StatementSyntax"/></typeparam>
    internal class StatementSpecificOrchestrator<T>: NodeSpecificOrchestrator<T, StatementSyntax> where T: StatementSyntax
    {
        public StatementSpecificOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }

        protected override bool NewContext => true;

        /// <inheritdoc/>
        /// <remarks>Inject pending mutations that are controlled with 'if' statements.</remarks>
        protected override StatementSyntax InjectMutations(T sourceNode, StatementSyntax targetNode, MutationContext context)
        {
            var mutated = MutantPlacer.PlaceStatementControlledMutations(targetNode,
                context.StatementLevelControlledMutations.Select( m => (m.Id, (sourceNode as StatementSyntax).InjectMutation(m.Mutation))));
            context.StatementLevelControlledMutations.Clear();
            return mutated;
        }

        /// <inheritdoc/>
        /// <remarks>Mutations are stored ath statement level.</remarks>
        protected override MutationContext StoreMutations(T node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            context.StatementLevelControlledMutations.AddRange(mutations);
            return context;
        }
    }
}
