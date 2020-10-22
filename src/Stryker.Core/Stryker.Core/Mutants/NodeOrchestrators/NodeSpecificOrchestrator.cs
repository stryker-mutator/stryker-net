using Microsoft.CodeAnalysis;
using System;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal abstract class NodeSpecificOrchestrator<T> : INodeMutator where T : SyntaxNode
    {
        public Type ManagedType => typeof(T);

        protected virtual bool CanHandle(T t) => true;

        public bool CanHandle(SyntaxNode t) => CanHandle(t as T);

        internal abstract SyntaxNode OrchestrateMutation(T node, MutationContext context);

        public SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            return OrchestrateMutation(node as T, context);
        }
    }
}