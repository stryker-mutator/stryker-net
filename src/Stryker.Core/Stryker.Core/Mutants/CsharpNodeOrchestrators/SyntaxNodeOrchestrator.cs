namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

internal class SyntaxNodeOrchestrator : NodeSpecificOrchestrator<SyntaxNode, SyntaxNode>
{
    // we don't mutate this node
    protected override IEnumerable<Mutant> GenerateMutationForNode(SyntaxNode node, MutationContext context) => Enumerable.Empty<Mutant>();
}
