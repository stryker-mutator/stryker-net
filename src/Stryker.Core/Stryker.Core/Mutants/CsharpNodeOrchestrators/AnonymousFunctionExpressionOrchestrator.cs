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
        ExpressionSyntax targetNode, SemanticModel semanticModel, MutationContext context)
    {

        var target = (AnonymousFunctionExpressionSyntax)base.InjectMutations(sourceNode, targetNode, semanticModel, context);
            
        if (target.Block == null)
        {
            // we will now mutate the expression body
            var localContext = context.Enter(MutationControl.Member);
            target = target.ReplaceNode(target.ExpressionBody!,
                MutateSingleNode(sourceNode.ExpressionBody, semanticModel, localContext));
            if (localContext.HasLeftOverMutations)
            {
                // this is an expression body method
                // we need to convert it to expression body form
                target = MutantPlacer.ConvertExpressionToBody(target);
                // we need to inject pending block (and statement) level mutations
                target = target.WithBody(
                    localContext.InjectMutations(target.Block,
                        sourceNode.ExpressionBody, true));
            }
            context.Leave();
            if (target.Block == null)
            {
                // we did not perform any conversion
                return target;
            }
        }
        else
        {
            // we add an ending return, just in case
            target = MutantPlacer.AddEndingReturn(target);
        }

        switch (target)
        {
            case SimpleLambdaExpressionSyntax lambdaExpression when lambdaExpression.Parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)):
                target = target.WithBody(MutantPlacer.AddDefaultInitializers(target.Block, new List<ParameterSyntax> { lambdaExpression.Parameter }));
                break;
            // inject initialization to default for all out parameters
            case ParenthesizedLambdaExpressionSyntax parenthesizedLambda:
                target = target.WithBody(MutantPlacer.AddDefaultInitializers(target.Block, parenthesizedLambda.ParameterList.Parameters.Where(p =>
                    p.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)))));
                break;
        }
        return target;
    }
}
