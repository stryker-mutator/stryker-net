using System;
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

        protected virtual SyntaxNode OrchestrateMutation(T node, MutationContext context)
        {
            return context.MutateNodeAndChildren(node);
        }

        protected virtual MutationContext PrepareContext(T node, MutationContext context)
        {
            return context;
        }

        public virtual SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            return OrchestrateMutation(node as T, PrepareContext(node as T, context));
        }
    }
}