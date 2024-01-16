using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class AnonymousFunctionExpressionOrchestrator : ExpressionSpecificOrchestrator<AnonymousFunctionExpressionSyntax>
{
    /// <summary>
    /// Mutate the children, except the arrow expression body that may require conversion.
    /// </summary>
    protected override AnonymousFunctionExpressionSyntax OrchestrateChildrenMutation(AnonymousFunctionExpressionSyntax node,
        SemanticModel semanticModel,
        MutationContext context) =>
        node.ReplaceNodes(node.ChildNodes().Where(child => child != node.ExpressionBody),
            (original, _) => MutateSingleNode(original, semanticModel, context));

    protected override ExpressionSyntax InjectMutations(AnonymousFunctionExpressionSyntax sourceNode,
        ExpressionSyntax target, SemanticModel semanticModel, MutationContext context)
    {

        var targetNode = (AnonymousFunctionExpressionSyntax)base.InjectMutations(sourceNode, target, semanticModel, context);
            
        if (targetNode.Block == null)
        {
            // we will now mutate the expression body
            var localContext = context.Enter(MutationControl.Member);
            targetNode = targetNode.ReplaceNode(targetNode.ExpressionBody!,
                MutateSingleNode(sourceNode.ExpressionBody, semanticModel, localContext));
            if (localContext.HasLeftOverMutations)
            {
                // this is an expression body method
                // we need to convert it to expression body form
                targetNode = MutantPlacer.ConvertExpressionToBody(targetNode);
                // we need to inject pending block (and statement) level mutations
                targetNode = targetNode.WithBody(
                    localContext.InjectMutations(targetNode.Block,
                        sourceNode.ExpressionBody, true));
            }
            context.Leave();
            if (targetNode.Block == null)
            {
                // we did not perform any conversion
                return targetNode;
            }
        }
        else
        {
            // we add an ending return, just in case
            targetNode = MutantPlacer.AddEndingReturn(targetNode);
        }

        switch (targetNode)
        {
            case SimpleLambdaExpressionSyntax lambdaExpression when lambdaExpression.Parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)):
                targetNode = targetNode.WithBody(MutantPlacer.AddDefaultInitializers(targetNode.Block, new List<ParameterSyntax> { lambdaExpression.Parameter }));
                break;
            // inject initialization to default for all out parameters
            case ParenthesizedLambdaExpressionSyntax parenthesizedLambda:
                targetNode = targetNode.WithBody(MutantPlacer.AddDefaultInitializers(targetNode.Block, parenthesizedLambda.ParameterList.Parameters.Where(p =>
                    p.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)))));
                break;
        }
        return targetNode;
    }
}
