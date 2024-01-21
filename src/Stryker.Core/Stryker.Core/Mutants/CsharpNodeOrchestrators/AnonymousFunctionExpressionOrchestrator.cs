using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class AnonymousFunctionExpressionOrchestrator : BaseFunctionOrchestrator<AnonymousFunctionExpressionSyntax>
{
/*
    protected override AnonymousFunctionExpressionSyntax InjectMutations(AnonymousFunctionExpressionSyntax sourceNode,
        AnonymousFunctionExpressionSyntax targetNode, SemanticModel semanticModel, MutationContext context)
    {
        var target = targetNode;
        if (target.Block == null)
        {
            if (context.HasLeftOverMutations)
            {
                // this is an expression body method
                // we need to convert it to expression body form
                target = MutantPlacer.ConvertExpressionToBody(target);
                // we need to inject pending block (and statement) level mutations
                target = target.WithBody(
                    context.InjectMutations(target.Block,
                        sourceNode.ExpressionBody, true));
            }
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
*/
protected override (BlockSyntax block, ExpressionSyntax expression) GetBodies(AnonymousFunctionExpressionSyntax node) => (node.Block, node.ExpressionBody);

protected override ParameterListSyntax Parameters(AnonymousFunctionExpressionSyntax node) => node switch {ParenthesizedLambdaExpressionSyntax parenthesizedLambda => parenthesizedLambda.ParameterList,
    SimpleLambdaExpressionSyntax simpleLambda => SyntaxFactory.ParameterList(SyntaxFactory.SingletonSeparatedList(simpleLambda.Parameter)),
    _ => throw new ArgumentOutOfRangeException(nameof(node), node, null)
};

protected override TypeSyntax ReturnType(AnonymousFunctionExpressionSyntax node) => null;

protected override AnonymousFunctionExpressionSyntax SwitchToThisBodies(AnonymousFunctionExpressionSyntax node, BlockSyntax blockBody,
    ExpressionSyntax expressionBody) =>
    node.WithBody(blockBody).WithExpressionBody(expressionBody);
}
