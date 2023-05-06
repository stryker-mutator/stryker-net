using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Instrumentation;

/// <summary>
/// Helper that converts a method (including operators) from expression body to statement body form (or arrow to curly braces).
/// </summary>
internal class AnonymousFunctionExpressionToBodyEngine : BaseEngine<AnonymousFunctionExpressionSyntax>
{
    /// <summary>
    /// Converts the given method (or operator) from expression to body form.
    /// </summary>
    /// <typeparam name="T">Specific node type</typeparam>
    /// <param name="method">Method/operator to be converted.</param>
    /// <returns>the converted method/operator</returns>
    /// <remarks>returns the original node if no conversion is needed/possible</remarks>
    public T ConvertToBody<T>(T method) where T : AnonymousFunctionExpressionSyntax
    {
        if (method.ExpressionBody == null || method.Block != null)
        {
            // no need to convert
            return method;
        }
        var statementLine = SyntaxFactory.ReturnStatement(method.ExpressionBody.WithLeadingTrivia(SyntaxFactory.Space));

        // do we need add return to the expression body?
        return method.WithBody(SyntaxFactory.Block(statementLine)).WithAdditionalAnnotations(Marker) as T;
    }

    protected override SyntaxNode Revert(AnonymousFunctionExpressionSyntax node)
    {
        // get expression
        var expression = node.Block?.Statements[0] switch
        {
            ReturnStatementSyntax returnStatement => returnStatement.Expression!,
            ExpressionStatementSyntax expressionStatement => expressionStatement.Expression!,
            _ => throw new InvalidOperationException($"Can't extract original expression from {node.Body}")
        };

        return node.WithBody(expression!)
            .WithoutAnnotations(Marker);
    }

}
