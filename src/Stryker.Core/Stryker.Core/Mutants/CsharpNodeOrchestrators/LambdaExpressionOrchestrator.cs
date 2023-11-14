using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class LambdaExpressionOrchestrator: NodeSpecificOrchestrator<LambdaExpressionSyntax, LambdaExpressionSyntax> 
{
    protected override MutationContext PrepareContext(LambdaExpressionSyntax node, MutationContext context)
        => base.PrepareContext(node, context.Enter(MutationControl.Member));

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave(MutationControl.Member));

    /// <inheritdoc/>
    /// Inject mutations and convert expression body to block body if required.
    protected override LambdaExpressionSyntax InjectMutations(LambdaExpressionSyntax sourceNode, LambdaExpressionSyntax targetNode,
        SemanticModel semanticModel, MutationContext context)
    {
        targetNode = base.InjectMutations(sourceNode, targetNode, semanticModel, context);

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
                SyntaxFactory.Block(context.InjectBlockLevelExpressionMutation(targetNode.Block, sourceNode.ExpressionBody,
                true)));
        }
        else
        {
            // we add an ending return, just in case
            targetNode = MutantPlacer.AddEndingReturn(targetNode);
        }

        // inject initialization to default for all out parameters
        if (sourceNode is SimpleLambdaExpressionSyntax simpleLambda)
        {
            if (simpleLambda.Parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)))
            {
                targetNode = targetNode.WithBody(MutantPlacer.AddDefaultInitializers(targetNode.Block,  new []{ simpleLambda.Parameter}));
            }
        }
        else if (sourceNode is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
        {
            targetNode = targetNode.WithBody(MutantPlacer.AddDefaultInitializers(targetNode.Block, parenthesizedLambda.ParameterList.Parameters.Where(p =>
                p.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)))));
        }

        return targetNode;
    }
}
