using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Orchestrate mutations for syntax block
/// </summary>
internal class BlockOrchestrator : NodeSpecificOrchestrator<BlockSyntax, BlockSyntax>
{
    /// <inheritdoc/>
    /// <remarks>Ensure we return a block after mutants are injected.</remarks>
    protected override BlockSyntax InjectMutations(BlockSyntax sourceNode, BlockSyntax targetNode, SemanticModel semanticModel, MutationContext context) =>
        context.InjectMutations(targetNode, sourceNode);

    protected override MutationContext PrepareContext(BlockSyntax node, MutationContext context) => base.PrepareContext(node, context.Enter(MutationControl.Block));

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());
}
