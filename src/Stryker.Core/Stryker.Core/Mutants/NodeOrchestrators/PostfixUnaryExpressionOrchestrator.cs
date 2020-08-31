using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class PostfixUnaryExpressionOrchestrator: NodeSpecificOrchestrator<PostfixUnaryExpressionSyntax>
    {
        protected override bool CanHandle(PostfixUnaryExpressionSyntax t)
        {
            return t.Parent is ExpressionStatementSyntax;
        }

        internal override SyntaxNode OrchestrateMutation(PostfixUnaryExpressionSyntax node, MutationContext context)
        {
            // incrementer/decrementer as statement must be mutated as statements
            return context.MutateNodeAndChildren(node, true);
        }
    }
}