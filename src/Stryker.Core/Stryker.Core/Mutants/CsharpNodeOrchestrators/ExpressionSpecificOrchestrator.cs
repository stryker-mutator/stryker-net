using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    /// <summary>
    /// Orchestrate mutations for expressions (and sub expressions).
    /// </summary>
    /// <typeparam name="T">Node specific type, must inherit <see cref="ExpressionSyntax"/>.</typeparam>
    internal class ExpressionSpecificOrchestrator<T> : NodeSpecificOrchestrator<T, ExpressionSyntax> where T : ExpressionSyntax
    {
        /// <inheritdoc/>
        /// <remarks>Inject all pending mutations controlled with conditional operator(s).</remarks>
        protected override ExpressionSyntax InjectMutations(T sourceNode, ExpressionSyntax targetNode, MutationContext context) => context.InjectExpressionLevel(targetNode, sourceNode);

        protected override MutationContext StoreMutations(T node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            // if the expression contains a declaration, it must be controlled at the block level.
            if (node.ContainsDeclarations())
            {
                context.AddBlockLevel(mutations);
            }
            else
            {
                context.AddExpressionLevel(mutations);
            }
            return context;
        }
    }
}
