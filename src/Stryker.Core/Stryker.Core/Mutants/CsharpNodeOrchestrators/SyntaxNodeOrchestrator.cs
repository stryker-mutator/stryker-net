using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// This is the default orchestrator: it does not mutate the node, but it will orchestrate the mutations of its children.
/// </summary>
internal class SyntaxNodeOrchestrator : NodeSpecificOrchestrator<SyntaxNode, SyntaxNode>
{
    // we don't mutate this node
    protected override IEnumerable<Mutant> GenerateMutationForNode(SyntaxNode node, SemanticModel semanticModel, MutationContext context) => Enumerable.Empty<Mutant>();
}
