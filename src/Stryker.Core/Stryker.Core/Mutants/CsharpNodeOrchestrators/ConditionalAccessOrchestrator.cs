using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class ConditionalAccessOrchestrator: NodeSpecificOrchestrator<ConditionalAccessExpressionSyntax, ExpressionSyntax>
{
    /// <inheritdoc/>
    /// <remarks>Inject all pending mutations controlled with conditional operator(s).</remarks>
    protected override ExpressionSyntax InjectMutations(ConditionalAccessExpressionSyntax sourceNode, ExpressionSyntax targetNode, SemanticModel semanticModel, MutationContext context) => context.InjectExpressionLevel(targetNode, sourceNode);

    protected override ExpressionSyntax OrchestrateChildrenMutation(ConditionalAccessExpressionSyntax node, SemanticModel semanticModel, MutationContext context)
    {
        var pendingMutations = (ExpressionSyntax) MutateSingleNode(node.Expression, semanticModel, context);

        var resultingNode = node.WithExpression(pendingMutations).
            WithWhenNotNull((ExpressionSyntax)MutateSingleNode(node.WhenNotNull, semanticModel, context.EnterMemberAccess()));
        context.Leave(MutationControl.Expression);
        return resultingNode;
    }

 }
