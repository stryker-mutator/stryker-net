using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class BlockStatementOrchestrator : NodeSpecificOrchestrator<BlockSyntax>
    {
        internal override SyntaxNode OrchestrateMutation(BlockSyntax node, MutationContext context)
        {
            context = context.Clone();
            return context.InjectIfMutants(node, context.MutateChildren(node) as BlockSyntax);
        }
    }
}