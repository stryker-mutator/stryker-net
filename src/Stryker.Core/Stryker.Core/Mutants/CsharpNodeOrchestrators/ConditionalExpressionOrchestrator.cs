using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;
internal class ConditionalExpressionOrchestrator :MemberAccessExpressionOrchestrator<ConditionalAccessExpressionSyntax>
{
    
    protected override ExpressionSyntax OrchestrateChildrenMutation(ConditionalAccessExpressionSyntax node, SemanticModel semanticModel,
        MutationContext context)
    {
        var mutated = node.ReplaceNodes(node.ChildNodes(), (original, _) =>
        {
            if (original != node.WhenNotNull)
            {
                return context.Mutate(original, semanticModel);
            }
            // there must be some MemberBindingAccess in the chain, so we assume we are in a member access chain (no mutation injection)
            var subContext = context.Enter(MutationControl.MemberAccess);
            var result = subContext.Mutate(original, semanticModel);
            subContext.Leave();
            return result;

        });
        return mutated;
    }
}
