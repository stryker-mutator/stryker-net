using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class ExpressionSpecificOrchestrator<T>: NodeSpecificOrchestrator<T> where T: ExpressionSyntax
    {
        protected override SyntaxNode OrchestrateMutation(T node, MutationContext context)
        {
            var mutations = MutantOrchestrator.GenerateMutationsForNode(node, context);

            var mutatedNode1 = (ExpressionSyntax) node.ReplaceNodes(node.ChildNodes(), 
                (original, mutated) => MutantOrchestrator.Mutate(original, context));

            return InjectMutations(node, mutatedNode1, context, mutations);
        }

        protected virtual ExpressionSyntax InjectMutations(T originalNode, ExpressionSyntax mutatedNode, MutationContext context, IEnumerable<Mutant> mutations)
        {
            if (!originalNode.ContainsDeclarations())
            {
                return MutantPlacer.PlaceExpressionControlledMutations(
                    mutatedNode,
                    mutations.Select(m => (m.Id, (ExpressionSyntax) originalNode.InjectMutation(m.Mutation))));
            }
            context.BlockLevelControlledMutations.AddRange(mutations);
            return mutatedNode;

        }

        public ExpressionSpecificOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
