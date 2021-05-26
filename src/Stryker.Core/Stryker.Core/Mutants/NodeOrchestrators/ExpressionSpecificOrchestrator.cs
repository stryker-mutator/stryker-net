using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    /// <summary>
    /// Handles expressions and sub expressions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ExpressionSpecificOrchestrator<T>: NodeSpecificOrchestrator<T, ExpressionSyntax> where T: ExpressionSyntax
    {
        /// <inheritdoc/>
        /// <remarks>Inject all pending mutations controlled with conditional operator(s).</remarks>
        protected override ExpressionSyntax InjectMutations(T sourceNode, ExpressionSyntax targetNode, MutationContext context)
        {
            return context.Store.PlaceExpressionMutations(targetNode, m => (ExpressionSyntax) sourceNode.InjectMutation(m));
        }

        protected override MutationContext StoreMutations(T node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            // if the expression contains a declaration, it must be controlled at the block level.
            context.Store.StoreMutations(mutations,
                !node.ContainsDeclarations() ? MutationControl.Expression : MutationControl.Block);
            return context;
        }

        public ExpressionSpecificOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
