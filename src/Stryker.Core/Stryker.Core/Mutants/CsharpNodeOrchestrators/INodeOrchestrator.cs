using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Helpers;

namespace Stryker.Abstractions.Mutants.CsharpNodeOrchestrators;

internal interface INodeOrchestrator : ITypeHandler<SyntaxNode>
{
    SyntaxNode Mutate(SyntaxNode node, SemanticModel semanticModel, MutationContext context);
}
