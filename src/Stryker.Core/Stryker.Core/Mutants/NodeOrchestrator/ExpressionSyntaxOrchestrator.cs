using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class ExpressionSyntaxOrchestrator: NodeSpecificOrchestrator<ExpressionSyntax>
    {
        protected override bool CanHandleThis(ExpressionSyntax t)
        {
            return !t.ContainsDeclarations();
        }

        internal override SyntaxNode OrchestrateMutation(ExpressionSyntax node, MutationContext context)
        {
            return context.MutateWithConditionals(node, context.MutateChildren(node) as ExpressionSyntax);
        }
    }
}
