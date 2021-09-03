using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        protected override ExpressionSyntax InjectMutations(T sourceNode, ExpressionSyntax targetNode, MutationContext context) => context.InjectExpressionLevel(targetNode, sourceNode);

        protected override MutationContext StoreMutations(T node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            // if the expression contains a declaration, it must be controlled at the block level.
            if (!node.ContainsDeclarations())
            {
                context.AddExpressionLevel(mutations);
            }
            else
            {
                context.AddBlockLevel(mutations);
            }
            return context;
        }

        public ExpressionSpecificOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
