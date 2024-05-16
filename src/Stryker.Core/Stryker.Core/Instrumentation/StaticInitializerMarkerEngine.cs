using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.Instrumentation;

/// <summary>
/// Injects static tracking logic in static fields/properties' initializers.
/// </summary>
internal class StaticInitializerMarkerEngine: BaseEngine<ExpressionSyntax>
{
    private const string MutantContextValueTrackName = "TrackValue";

    public ExpressionSyntax PlaceValueMarker(ExpressionSyntax node, CodeInjection codeInjection)
    {
        if (node is InitializerExpressionSyntax)
        {
            // we cannot track array initializer with this construction
            return node;
        }
        // inject a MutantContext construction with the expression as a lambda expression (used the parameter)
        return SyntaxFactory.InvocationExpression(
                codeInjection.GetContextClassAccessExpression(MutantContextValueTrackName),
                SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(SyntaxFactory.ParenthesizedLambdaExpression(node)))))
            .WithAdditionalAnnotations(Marker);
    }

    protected override SyntaxNode Revert(ExpressionSyntax node)
    {
        // extract the original expression for the parameter list
        if (node is InvocationExpressionSyntax invocation
            && CodeInjection.IsContextAccessExpression(invocation.Expression, MutantContextValueTrackName)
            && invocation.ArgumentList.Arguments.First().Expression is ParenthesizedLambdaExpressionSyntax parenthesized)
        {
            return parenthesized.ExpressionBody;
        }
        throw new InvalidOperationException($"Can't extract original expression from {node}");
    }
}
