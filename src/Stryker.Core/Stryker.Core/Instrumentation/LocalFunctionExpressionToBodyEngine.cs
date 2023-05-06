using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation;

/// <summary>
/// Helper that converts a function from expression body to statement body form (or arrow to curly braces).
/// </summary>
internal class LocalFunctionExpressionToBodyEngine : BaseEngine<LocalFunctionStatementSyntax>
{
    /// <summary>
    /// Converts the given local function from expression to body form.
    /// </summary>
    /// <typeparam name="T">Specific node type</typeparam>
    /// <param name="function">local function to be converted.</param>
    /// <returns>the converted function</returns>
    /// <remarks>returns the original node if no conversion is needed/possible</remarks>
    public LocalFunctionStatementSyntax ConvertToBody(LocalFunctionStatementSyntax function)
    {
        if (function.ExpressionBody == null || function.Body != null)
        {
            // can't convert
            return function;
        }

        StatementSyntax statementLine;
        if (function.NeedsReturn())
        {
            statementLine = SyntaxFactory.ReturnStatement(function.ExpressionBody!.Expression.WithLeadingTrivia(SyntaxFactory.Space));
        }
        else
        {
            statementLine = SyntaxFactory.ExpressionStatement(function!.ExpressionBody!.Expression);
        }
        // do we need add return to the expression body?
        return function.WithBody(SyntaxFactory.Block(statementLine)).WithExpressionBody(null).WithAdditionalAnnotations(Marker);
    }

    protected override SyntaxNode Revert(LocalFunctionStatementSyntax node)
    {
        // get expression
        var expression = SyntaxFactory.ArrowExpressionClause(node.Body?.Statements[0] switch
        {
            ReturnStatementSyntax returnStatement => returnStatement.Expression,
            ExpressionStatementSyntax expressionStatement => expressionStatement.Expression,
            _ => throw new InvalidOperationException($"Can't extract original expression from {node.Body}")
        });

        return node.WithBody(null).WithExpressionBody(expression).WithoutAnnotations(Marker);
    }
}
