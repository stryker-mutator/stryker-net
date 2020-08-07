using Microsoft.CodeAnalysis;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    interface INodeMutator: ITypeHandler<SyntaxNode>
    {
        SyntaxNode Mutate(SyntaxNode node, MutationContext context);
    }
}