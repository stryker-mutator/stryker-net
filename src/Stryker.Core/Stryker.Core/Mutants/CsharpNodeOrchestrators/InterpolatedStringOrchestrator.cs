using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class InterpolatedStringOrchestrator : ExpressionSpecificOrchestrator<InterpolatedStringExpressionSyntax>
{
    protected override ExpressionSyntax InjectMutations(InterpolatedStringExpressionSyntax sourceNode, ExpressionSyntax targetNode,
        SemanticModel semanticModel, MutationContext context)
    {
        // if the interpolated string is used as an argument, we need to inject the mutations in a way that preserves the argument list
        var conversion = semanticModel?.GetConversion(sourceNode) ?? default;
        return conversion.IsInterpolatedString || conversion.IsInterpolatedStringHandler
            ? targetNode : base.InjectMutations(sourceNode, targetNode, semanticModel, context);
    }
}
