using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Helpers
{
    // extension methods for SyntaxNode(s)
    internal static class SyntaxHelper
    {
        // is the expression some king of string ?
        public static bool IsAStringExpression(this ExpressionSyntax node)
        {
            return node.Kind() == SyntaxKind.StringLiteralExpression ||
                   node.Kind() == SyntaxKind.InterpolatedStringExpression;
        }

        // does the expression contain declaration?
        public static bool ContainsDeclarations(this ExpressionSyntax node)
        {
            return node.ContainsNodeThatVerifies(x =>
                x.IsKind(SyntaxKind.DeclarationExpression) || x.IsKind(SyntaxKind.DeclarationPattern));
        }

        public static bool ContainsNodeThatVerifies(this SyntaxNode node, Func<SyntaxNode, bool> predicate, bool skipBlock = true)
        {
            // check if there is a variable declaration at this scope level
            return node.DescendantNodes((child) =>
            {
                if (skipBlock && child is BlockSyntax)
                {
                    return false;
                }
                // including anonymous/lambda declaration
                if (child.Parent is AnonymousFunctionExpressionSyntax function && function.ExpressionBody == child)
                {
                    return false;
                }
                return true;
            } ).Any(predicate);
        }
    }
}
