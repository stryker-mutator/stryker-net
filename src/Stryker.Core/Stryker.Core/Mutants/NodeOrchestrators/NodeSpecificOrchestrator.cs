using System;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal abstract class NodeSpecificOrchestrator<T>:INodeMutator where T: SyntaxNode
    {
        public Type ManagedType => typeof(T);

        protected virtual bool CanHandleThis(T t) => true;

        public bool CanHandle(SyntaxNode t) => CanHandleThis(t as T);

        internal abstract SyntaxNode OrchestrateMutation(T node, MutationContext context);

        public SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            return OrchestrateMutation(node as T, context);
        }
    }
}