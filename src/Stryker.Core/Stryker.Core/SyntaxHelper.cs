using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core
{
    // extension methods for SyntaxNode(s)
    internal static class SyntaxHelper
    {
        // is the expression some king of string ?
 
        /// <summary>
        /// Returns true if the node is a string literal
        /// </summary>
        /// <param name="node">node to evaluate</param>
        /// <returns></returns>
        public static bool IsStringLiteral(this CSharpSyntaxNode node) =>
            node.Kind() == SyntaxKind.StringLiteralExpression ||
            node.Kind() == SyntaxKind.InterpolatedStringExpression;


        // does the expression contain declaration?
        public static bool ContainsDeclarations(this ExpressionSyntax node) =>
            node.ContainsNodeThatVerifies(x =>
                x.IsKind(SyntaxKind.DeclarationExpression) || x.IsKind(SyntaxKind.DeclarationPattern));

        public static bool ContainsNodeThatVerifies(this SyntaxNode node, Func<SyntaxNode, bool> predicate, bool skipBlock = true) =>
            // check if there is a variable declaration at this scope level
            node.DescendantNodes((child) =>
            {
                if (skipBlock && child is BlockSyntax)
                {
                    return false;
                }

                // including anonymous/lambda declaration
                return child.Parent is not AnonymousFunctionExpressionSyntax function || function.ExpressionBody != child;
            } ).Any(predicate);
    }
}
