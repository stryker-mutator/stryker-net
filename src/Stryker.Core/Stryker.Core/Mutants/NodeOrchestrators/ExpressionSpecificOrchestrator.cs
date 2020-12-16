using System.Collections.Generic;
using System.Linq;
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
            var result = MutantPlacer.PlaceExpressionControlledMutations(
                targetNode,
                context.ExpressionLevelMutations.Select(m => (m.Id, (ExpressionSyntax) sourceNode.InjectMutation(m.Mutation)))) as T;
            context.ExpressionLevelMutations.Clear();
            return result;
        }

        protected override MutationContext StoreMutations(T node,
            IEnumerable<Mutant> mutations,
            MutationContext context)
        {
            // if the expression contains a declaration, it must be controlled at the block level.
            if (!node.ContainsDeclarations())
            {
                context.ExpressionLevelMutations.AddRange(mutations);
            }
            else
            {
                context.BlockLevelControlledMutations.AddRange(mutations);
            }
            return context;
        }

        public ExpressionSpecificOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
