using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    internal class LocalFunctionStatementOrchestrator : NodeSpecificOrchestrator<LocalFunctionStatementSyntax, LocalFunctionStatementSyntax>
    {
        protected override LocalFunctionStatementSyntax InjectMutations(LocalFunctionStatementSyntax sourceNode, LocalFunctionStatementSyntax targetNode,
            MutationContext context)
        {
            // find out parameters
            targetNode = base.InjectMutations(sourceNode, targetNode, context);

            var fullTargetBody = targetNode.Body;
            var sourceNodeParameterList = sourceNode.ParameterList;

            if (fullTargetBody != null)
            {
                // the function is in the body form
                // inject initialization to default for all out parameters
                targetNode = sourceNode.WithBody(MutantPlacer.AddDefaultInitializers(fullTargetBody,
                    sourceNodeParameterList.Parameters.Where(p =>
                        p.Modifiers.Any(m => m.Kind() == SyntaxKind.OutKeyword))));
                // add a return in case we changed the control flow
                return MutantPlacer.AddEndingReturn(targetNode);
            }
            // nothing to do if there is now pending statement mutations
            if (!context.HasStatementLevelMutant)
            {
                return targetNode;
            }

            // we need to move to a body version of the function to inject pending mutations
            targetNode = MutantPlacer.ConvertExpressionToBody(targetNode);
            return targetNode.WithBody(SyntaxFactory.Block(context.InjectBlockLevelExpressionMutation(targetNode.Body, sourceNode.ExpressionBody.Expression, sourceNode.NeedsReturn())));
        }
    }
}
