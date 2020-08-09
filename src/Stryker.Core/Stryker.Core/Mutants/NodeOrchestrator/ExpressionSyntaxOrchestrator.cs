using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class ExpressionSyntaxOrchestrator: NodeSpecificOrchestrator<ExpressionSyntax>
    {
        internal override SyntaxNode OrchestrateMutation(ExpressionSyntax node, MutationContext context)
        {
            if (!node.ContainsDeclarations())
            {
                return context.MutateWithConditionals(node, context.MutateChildren(node) as ExpressionSyntax);
            }
            // we have variable declaration(s) as part of the expression, mutation need to be controlled at the block level
            context.GenerateStatementLevelControlledMutants(node);
            return context.MutateChildren(node);
        }
    }    
}
