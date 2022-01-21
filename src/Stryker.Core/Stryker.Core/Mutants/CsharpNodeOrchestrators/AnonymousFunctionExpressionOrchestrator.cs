using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    internal class AnonymousFunctionExpressionOrchestrator : NodeSpecificOrchestrator<AnonymousFunctionExpressionSyntax, AnonymousFunctionExpressionSyntax>
    {
        protected override AnonymousFunctionExpressionSyntax InjectMutations(AnonymousFunctionExpressionSyntax sourceNode,
            AnonymousFunctionExpressionSyntax targetNode, MutationContext context)
        {
            targetNode = base.InjectMutations(sourceNode, targetNode, context);

            if (targetNode.Block == null)
            {
                if (targetNode.ExpressionBody == null)
                {
                    // only a definition (eg interface)
                    return targetNode;
                }

                // this is an expression body method
                if (!context.HasStatementLevelMutant)
                {
                    // there is no statement or block level mutant, so the method control flow is not changed by mutations
                    // there is no need to change the method in any may
                    return targetNode;
                }

                // we need to convert it to expression body form
                targetNode = MutantPlacer.ConvertExpressionToBody(targetNode);

                // we need to inject pending block (and statement) level mutations
                targetNode = targetNode.WithBody(
                    SyntaxFactory.Block(context.InjectBlockLevelExpressionMutation(targetNode.Block, sourceNode.ExpressionBody, true)));
            }
            else
            {
                // we add an ending return, just in case
                targetNode = MutantPlacer.AddEndingReturn(targetNode);
            }

            if (targetNode is SimpleLambdaExpressionSyntax lambdaExpression && lambdaExpression.Parameter.Modifiers.Any(m => m.Kind() == SyntaxKind.OutKeyword))
            {
                targetNode = targetNode.WithBody(MutantPlacer.AddDefaultInitializers(targetNode.Block, new List<ParameterSyntax> { lambdaExpression.Parameter }));
            }
            else if (targetNode is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
            // inject initialization to default for all out parameters
            {
                targetNode = targetNode.WithBody(MutantPlacer.AddDefaultInitializers(targetNode.Block, parenthesizedLambda.ParameterList.Parameters.Where(p =>
                p.Modifiers.Any(m => m.Kind() == SyntaxKind.OutKeyword))));
            }
            return targetNode;
        }

        protected override MutationContext PrepareContext(AnonymousFunctionExpressionSyntax node, MutationContext context)
        {
            context.Enter(MutationControl.Block);
            return base.PrepareContext(node, context);
        }
    }
}
