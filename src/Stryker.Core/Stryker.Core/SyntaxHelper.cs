using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core
{
    // extension methods for SyntaxNode(s)
    internal static class SyntaxHelper
    {
        public static bool IsAStringExpression(this ExpressionSyntax node)
        {
            return node.Kind() == SyntaxKind.StringLiteralExpression ||
                   node.Kind() == SyntaxKind.InterpolatedStringExpression;
        }
    }
}
