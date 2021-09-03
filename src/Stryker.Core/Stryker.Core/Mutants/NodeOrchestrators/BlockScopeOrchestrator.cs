using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    /// <summary>
    /// Handles statements that have block scope (e.g. curly braces blocks, for/while statements...)
    /// </summary>
    /// <typeparam name="T">Precise type of the statement</typeparam>
    internal class BlockScopeOrchestrator<T>: StatementSpecificOrchestrator<T> where T: StatementSyntax
    {
        protected BlockScopeOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {}

        protected override bool NewContext => true;

        /// <inheritdoc/>
        /// <remarks>Inject all pending mutations and control them with if statements.</remarks>
        protected override StatementSyntax InjectMutations(T sourceNode, StatementSyntax targetNode, MutationContext context) => context.InjectBlockLevel(targetNode, sourceNode);

        protected override MutationContext StoreMutations(T node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            context.AddBlockLevel(mutations);
            return context;
        }
    }
}
