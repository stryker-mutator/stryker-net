using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class InterpolatedStringExpressionOrchestrator : ExpressionSpecificOrchestrator<InterpolatedStringExpressionSyntax>
{
    public override SyntaxNode Mutate(SyntaxNode node, SemanticModel semanticModel, MutationContext context)
    {
        if (node is InterpolatedStringExpressionSyntax interpolatedString && RequiresHigherLevelControl(interpolatedString, semanticModel))
        {
            context = PrepareContext(interpolatedString, context);
            var mutations = GenerateMutationForNode(interpolatedString, semanticModel, context);
            var targetNode = OrchestrateChildrenMutation(interpolatedString, semanticModel, context);
            context.AddMutations(mutations, interpolatedString.ContainsDeclarations() ? MutationControl.Block : MutationControl.Statement);
            RestoreContext(context);
            return targetNode;
        }

        return base.Mutate(node, semanticModel, context);
    }

    private static bool RequiresHigherLevelControl(InterpolatedStringExpressionSyntax node, SemanticModel semanticModel)
    {
        if (semanticModel is null)
        {
            return false;
        }

        var conversion = semanticModel.GetConversion(node);
        return conversion.IsInterpolatedStringHandler || conversion.IsInterpolatedString;
    }
}
