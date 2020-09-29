using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal abstract class NodeSpecificOrchestrator<T>:INodeMutator where T: SyntaxNode
    {
        protected MutantOrchestrator MutantOrchestrator;

        protected NodeSpecificOrchestrator(MutantOrchestrator mutantOrchestrator)
        {
            MutantOrchestrator = mutantOrchestrator;
        }

        public Type ManagedType => typeof(T);

        protected virtual bool CanHandle(T t) => true;

        public bool CanHandle(SyntaxNode t) => CanHandle(t as T);

        protected virtual T InjectMutations(T originalNode, T mutatedNode, MutationContext context, IEnumerable<Mutant> mutations)
        {
            return mutatedNode;
        }

        protected virtual SyntaxNode OrchestrateMutation(T node, MutationContext context)
        {
            var mutations = MutantOrchestrator.GenerateMutationsForNode(node, context);

            var mutatedNode1 = node.ReplaceNodes(node.ChildNodes(), 
                (original, mutated) => MutantOrchestrator.Mutate(original, context));

            return InjectMutations(node, mutatedNode1, context, mutations);
        }

        public virtual SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            return OrchestrateMutation(node as T, context);
        }
    }
}