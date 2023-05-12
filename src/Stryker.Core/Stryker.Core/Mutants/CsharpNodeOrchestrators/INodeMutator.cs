namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;
using Microsoft.CodeAnalysis;
using Stryker.Core.Helpers;

internal interface INodeMutator : ITypeHandler<SyntaxNode>
{
    SyntaxNode Mutate(SyntaxNode node, MutationContext context);
}
