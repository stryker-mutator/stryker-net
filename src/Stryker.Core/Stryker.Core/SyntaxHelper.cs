﻿using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core
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
            return node.DescendantNodes().Any(x =>
                x.IsKind(SyntaxKind.DeclarationExpression) || x.IsKind(SyntaxKind.DeclarationPattern));
        }
    }
}
