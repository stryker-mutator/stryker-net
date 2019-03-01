using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core
{
    internal static class SyntaxHelper
    {
        public static bool IsString(this ExpressionSyntax node)
        {
            return node.Kind() == SyntaxKind.StringLiteralExpression ||
                   node.Kind() == SyntaxKind.InterpolatedStringExpression;
        }
    }
}
