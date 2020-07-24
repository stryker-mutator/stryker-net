using System;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    interface INodeMutator: ITypeHandler<SyntaxNode>
    {
        SyntaxNode Mutate(SyntaxNode node, MutationContext context);
    }

    internal abstract class NodeSpecificOrchestrator<T>:INodeMutator where T: SyntaxNode
    {
        public Type ManagedType => typeof(T);

        public virtual bool CanHandle(SyntaxNode t) => true;

        internal abstract T OrchestrateMutation(T node, MutationContext context);

        public SyntaxNode Mutate(SyntaxNode node, MutationContext context)
        {
            return OrchestrateMutation(node as T, context);
        }
    }
}