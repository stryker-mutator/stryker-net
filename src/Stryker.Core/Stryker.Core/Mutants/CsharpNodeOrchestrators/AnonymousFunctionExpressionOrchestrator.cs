using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class AnonymousFunctionExpressionOrchestrator : NodeSpecificOrchestrator<AnonymousFunctionExpressionSyntax, AnonymousFunctionExpressionSyntax>
{
    /// <summary>
    /// Mutate the children, except the arrow expression body that may require conversion.
    /// </summary>
    protected override AnonymousFunctionExpressionSyntax OrchestrateChildrenMutation(AnonymousFunctionExpressionSyntax node,
        MutationContext context) =>
        node.ReplaceNodes(node.ChildNodes().Where(child => child != node.ExpressionBody),
            (original, _) => MutateSingleNode(original, context));

    protected override AnonymousFunctionExpressionSyntax InjectMutations(AnonymousFunctionExpressionSyntax sourceNode,
        AnonymousFunctionExpressionSyntax targetNode, MutationContext context)
    {
        targetNode = base.InjectMutations(sourceNode, targetNode, context);
        
        if (targetNode.Block == null)
        {
            // we will now mutate the expression body
            var localContext = context.Enter(MutationControl.Block);
            targetNode = targetNode.ReplaceNode(targetNode.ExpressionBody,
                MutateSingleNode(sourceNode.ExpressionBody, localContext));
            if (localContext.HasStatementLevelMutant)
            {
                // this is an expression body method
                // we need to convert it to expression body form
                targetNode = MutantPlacer.ConvertExpressionToBody(targetNode);
                // we need to inject pending block (and statement) level mutations
                targetNode = targetNode.WithBody(SyntaxFactory.Block(
                    localContext.InjectBlockLevelExpressionMutation(targetNode.Block,
                        sourceNode.ExpressionBody, true)));
            }
            localContext.Leave(MutationControl.Block);
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
