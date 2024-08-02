using Microsoft.CodeAnalysis;
using Stryker.Configuration.Helpers;

namespace Stryker.Configuration.Mutants.CsharpNodeOrchestrators;

internal interface INodeOrchestrator : ITypeHandler<SyntaxNode>
{
    SyntaxNode Mutate(SyntaxNode node, SemanticModel semanticModel, MutationContext context);
}
