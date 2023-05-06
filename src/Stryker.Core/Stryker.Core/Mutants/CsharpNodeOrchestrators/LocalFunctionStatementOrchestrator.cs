using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class LocalFunctionStatementOrchestrator : NodeSpecificOrchestrator<LocalFunctionStatementSyntax, LocalFunctionStatementSyntax>
{
    /// <summary>
    /// Mutate the children, except the arrow expression body that may require conversion.
    /// </summary>
    protected override LocalFunctionStatementSyntax OrchestrateChildrenMutation(LocalFunctionStatementSyntax node,
        MutationContext context) =>
        node.ReplaceNodes(node.ChildNodes().Where(child => child != node.ExpressionBody),
            (original, _) => MutateSingleNode(original, context));

    protected override LocalFunctionStatementSyntax InjectMutations(LocalFunctionStatementSyntax sourceNode, LocalFunctionStatementSyntax targetNode,
        MutationContext context)
    {
        targetNode = base.InjectMutations(sourceNode, targetNode, context);

        var fullTargetBody = targetNode.Body;
        var sourceNodeParameterList = sourceNode.ParameterList;

        if (fullTargetBody == null)
        {
            // we will now mutate the expression body
            var localContext = context.Enter(MutationControl.Block);
            targetNode = targetNode.ReplaceNode(targetNode.ExpressionBody!,
                MutateSingleNode(sourceNode.ExpressionBody, localContext));
            if (localContext.HasStatementLevelMutant)
            {
                // this is an expression body method
                // we need to convert it to expression body form
                targetNode = MutantPlacer.ConvertExpressionToBody(targetNode);
                // we need to inject pending block (and statement) level mutations
                targetNode = targetNode.WithBody(SyntaxFactory.Block(
                    localContext.InjectBlockLevelExpressionMutation(targetNode.Body,
                        sourceNode.ExpressionBody!.Expression, true)));
            }
            localContext.Leave(MutationControl.Block);
            if (targetNode.Body == null)
            {
                // we did not perform any conversion
                return targetNode;
            }

        }
        
        // the function is in the body form
        // inject initialization to default for all out parameters
        targetNode = targetNode.WithBody(MutantPlacer.AddDefaultInitializers(targetNode.Body,
            sourceNodeParameterList.Parameters.Where(p =>
                p.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)))));
        // add a return in case we changed the control flow
        return MutantPlacer.AddEndingReturn(targetNode);
    }
}
