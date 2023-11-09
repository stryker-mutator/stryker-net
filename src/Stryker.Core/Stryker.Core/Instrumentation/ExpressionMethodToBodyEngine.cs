using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation;

/// <summary>
/// Helper that converts a method (including operators) from expression body to statement body form (or arrow to curly braces).
/// </summary>
internal class ExpressionMethodToBodyEngine : BaseEngine<BaseMethodDeclarationSyntax>
{
    /// <summary>
    /// Converts the given method (or operator) from expression to body form.
    /// </summary>
    /// <typeparam name="T">Specific node type</typeparam>
    /// <param name="method">Method/operator to be converted.</param>
    /// <returns>the converted method/operator</returns>
    /// <remarks>returns the original node if no conversion is needed/possible</remarks>
    public T ConvertToBody<T>(T method) where T: BaseMethodDeclarationSyntax
    {
        if (method.ExpressionBody == null || method.Body != null)
        {
            // can't convert
            return method;
        }

        StatementSyntax statementLine;
        switch (method)
        {
            case MethodDeclarationSyntax actualMethod when actualMethod.NeedsReturn():
            case ConversionOperatorDeclarationSyntax _:
            case OperatorDeclarationSyntax _:
                statementLine = SyntaxFactory.ReturnStatement(method.ExpressionBody.Expression.WithLeadingTrivia(SyntaxFactory.Space));
                break;

            default:
                statementLine = SyntaxFactory.ExpressionStatement(method.ExpressionBody.Expression);
                break;
        }

        // do we need add return to the expression body?

        return method.WithBody(SyntaxFactory.Block(statementLine)).WithExpressionBody(null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None)).WithAdditionalAnnotations(Marker) as T;
    }

    protected override SyntaxNode Revert(BaseMethodDeclarationSyntax node)
    {
        // get expression
        var expression = SyntaxFactory.ArrowExpressionClause(node.Body?.Statements[0] switch
        {
            ReturnStatementSyntax returnStatement => returnStatement.Expression!,
            ExpressionStatementSyntax expressionStatement => expressionStatement.Expression!,
            _ => throw new InvalidOperationException($"Can't extract original expression from {node.Body}")
        });

        return node.WithBody(null).WithExpressionBody(expression)
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)).WithoutAnnotations(Marker);
    }

}
