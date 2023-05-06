using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Orchestrate mutations for syntax block
/// </summary>
internal class BlockOrchestrator : StatementSpecificOrchestrator<BlockSyntax>
{
    /// <inheritdoc/>
    /// <remarks>Ensure we return a block after mutants are injected.</remarks>
    protected override StatementSyntax InjectMutations(BlockSyntax sourceNode, StatementSyntax targetNode, MutationContext context)
    {
        var mutated = context.InjectBlockLevel(targetNode, sourceNode);
        // ensure we still return a block!
        return mutated is BlockSyntax ? mutated : SyntaxFactory.Block(mutated);
    }

    protected override MutationContext PrepareContext(BlockSyntax node, MutationContext context) => base.PrepareContext(node, context.Enter(MutationControl.Block));

    protected override void RestoreContext(MutationContext context)
    {
        context.Leave(MutationControl.Block);
        base.RestoreContext(context);
    }

    protected override MutationContext StoreMutations(BlockSyntax node,
        IEnumerable<Mutant> mutations,
        MutationContext context)
    {
        context.AddBlockLevel(mutations);
        return context;
    }
}
