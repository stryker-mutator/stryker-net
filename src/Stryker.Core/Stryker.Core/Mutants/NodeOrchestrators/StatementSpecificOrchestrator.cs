using System.Collections.Generic;
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

        protected override MutationContext PrepareContext(T node, MutationContext context) => base.PrepareContext(node, context).Enter(MutationControl.Statement);

        protected override void RestoreContext(MutationContext context) => context.Leave(MutationControl.Statement);

        /// <inheritdoc/>
        /// <remarks>Inject pending mutations that are controlled with 'if' statements.</remarks>
        protected override StatementSyntax InjectMutations(T sourceNode, StatementSyntax targetNode, MutationContext context) =>
            context.Store.PlaceStatementMutations(targetNode,
                m => (sourceNode as StatementSyntax).InjectMutation(m));

        /// <inheritdoc/>
        /// <remarks>Mutations are stored ath statement level.</remarks>
        protected override MutationContext StoreMutations(T node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            context.Store.StoreMutations(mutations, MutationControl.Statement);
            return context;
        }
    }
}
