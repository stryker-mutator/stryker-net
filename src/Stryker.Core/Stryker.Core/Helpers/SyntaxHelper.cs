using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Helpers;

// extension methods for SyntaxNode(s)
internal static class SyntaxHelper
{
    /// <summary>
    /// Check if an expression is a string.
    /// </summary>
    /// <param name="node">Expression to check</param>
    /// <returns>true if it is a string</returns>
    public static bool IsAStringExpression(this ExpressionSyntax node) =>
        node.Kind() == SyntaxKind.StringLiteralExpression ||
        node.Kind() == SyntaxKind.InterpolatedStringExpression;

    /// <summary>
    /// Check if an expression contains a declaration
    /// </summary>
    /// <param name="node">expression to check</param>
    /// <returns>true if it contains a declaration</returns>
    public static bool ContainsDeclarations(this ExpressionSyntax node) =>
        node.ContainsNodeThatVerifies(x =>
            x.IsKind(SyntaxKind.DeclarationExpression) || x.IsKind(SyntaxKind.DeclarationPattern));

    /// <summary>
    /// Scans recursively the node to find if a child node matches the predicated. Does not recurse into local functions or anonymous functions
    /// </summary>
    /// <param name="node">starting node</param>
    /// <param name="predicate">predicate to match</param>
    /// <param name="skipBlock">set to true to avoid scan into block statements</param>
    /// <returns></returns>
    public static bool ContainsNodeThatVerifies(this SyntaxNode node, Func<SyntaxNode, bool> predicate, bool skipBlock = true) =>
        // scan 
        node.DescendantNodes((child) =>
        {
            if (skipBlock && child is BlockSyntax)
            {
                return false;
            }
            // including anonymous/lambda declaration
            if ((child.Parent is AnonymousFunctionExpressionSyntax function && function.ExpressionBody == child)
            || (child.Parent is LocalFunctionStatementSyntax localFunction && localFunction.ExpressionBody == child))
            {
                return false;
            }
            return true;
        } ).Any(predicate);
}
