using Microsoft.CodeAnalysis;
using Stryker.Utilities.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal interface INodeOrchestrator : ITypeHandler<SyntaxNode>
{
    SyntaxNode Mutate(SyntaxNode node, SemanticModel semanticModel, MutationContext context);
}
