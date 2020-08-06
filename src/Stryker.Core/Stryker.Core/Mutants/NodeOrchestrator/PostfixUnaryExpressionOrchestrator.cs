using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class PostfixUnaryExpressionOrchestrator: NodeSpecificOrchestrator<PostfixUnaryExpressionSyntax>
    {
        protected override bool CanHandleThis(PostfixUnaryExpressionSyntax t)
        {
            return t.Parent is ExpressionStatementSyntax;
        }

        internal override SyntaxNode OrchestrateMutation(PostfixUnaryExpressionSyntax node, MutationContext context)
        {
            // incrementor/decrementor as statement must be mutated as statements
            context.StoreMutants(node);
            return context.MutateChildren(node);
        }
    }
}