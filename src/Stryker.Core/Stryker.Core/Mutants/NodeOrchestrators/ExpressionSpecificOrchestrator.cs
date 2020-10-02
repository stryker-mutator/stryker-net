using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class ExpressionSpecificOrchestrator<T>: NodeSpecificOrchestrator<T, ExpressionSyntax> where T: ExpressionSyntax
    {

        protected override ExpressionSyntax InjectMutations(T originalNode, ExpressionSyntax mutatedNode, MutationContext context)
        {
            var result = MutantPlacer.PlaceExpressionControlledMutations(
                mutatedNode,
                context.ExpressionLevelMutations.Select(m => (m.Id, (ExpressionSyntax) originalNode.InjectMutation(m.Mutation)))) as T;
            context.ExpressionLevelMutations.Clear();
            return result;
        }

        protected override MutationContext StoreMutations(IEnumerable<Mutant> mutations,
            T node,
            MutationContext context)
        {
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

        public ExpressionSpecificOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
