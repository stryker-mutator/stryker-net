using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class ConditionalAccessOrchestrator: ExpressionSpecificOrchestrator<ConditionalAccessExpressionSyntax>
{
    /// <inheritdoc/>
    /// <remarks>Inject all pending mutations controlled with conditional operator(s).</remarks>
    protected override ExpressionSyntax OrchestrateChildrenMutation(ConditionalAccessExpressionSyntax node, SemanticModel semanticModel, MutationContext context)
    {
        var pendingMutations = (ExpressionSyntax)MutateSingleNode(node.Expression, semanticModel, context);

        var resultingNode = node.WithExpression(pendingMutations).
            WithWhenNotNull((ExpressionSyntax)MutateSingleNode(node.WhenNotNull, semanticModel, context.Enter(MutationControl.MemberAccess)));
        context.Leave(MutationControl.MemberAccess);
        return resultingNode;
    }

 }
