using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Instrumentation
{
    /// <summary>
    /// Helper that converts a property accessor from expression body to statement body form (or arrow to curly braces).
    /// </summary>
    internal class AccessorExpressionToBodyEngine : BaseEngine<AccessorDeclarationSyntax>
    {
        public AccessorExpressionToBodyEngine(string markerId) : base(markerId)
        {
        }

        /// <summary>
        /// Convert an expression accessor to the body form.
        /// </summary>
        /// <param name="accessor">Accessor to be converted</param>
        /// <returns>Accessor in body form</returns>
        /// <remarks>No conversion happens it is already in body form or if it is virtual.</remarks>
        public AccessorDeclarationSyntax ConvertExpressionToBody(AccessorDeclarationSyntax accessor)
        {
            if (accessor.Body != null || accessor.ExpressionBody == null)
            {
                return accessor;
            }
            var block = accessor.Keyword.Text switch
            {
                "get" => SyntaxFactory.Block(SyntaxFactory.ReturnStatement(accessor.ExpressionBody.Expression.WithLeadingTrivia(SyntaxFactory.Space))),
                _ => SyntaxFactory.Block( SyntaxFactory.ExpressionStatement(accessor.ExpressionBody.Expression))
            };
            return accessor.WithBody(block).WithExpressionBody(null).WithAdditionalAnnotations(Marker).WithSemicolonToken(SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken));
        }

        protected override SyntaxNode Revert(AccessorDeclarationSyntax node)
        {
            if (node.Body == null)
            {
                throw new InvalidOperationException($"Expected an accessor with a body: {node}.");
            }
            if (node.Body.Statements.Count != 1)
            {
                throw new InvalidOperationException($"Expected an accessor with body containing only one statement: {node.Body}.");
            }
            var expression = node.Keyword.Text switch
            {
                "get" => (node.Body.Statements[0] as ReturnStatementSyntax)?.Expression,
                _ => (node.Body.Statements[0] as ExpressionStatementSyntax)?.Expression
            };
            if (expression == null)
            {
                throw new InvalidOperationException($"Failed to extract original expression from {node.Body.Statements[0]}.");
            }

            return node.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expression)).WithBody(null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
    }
}
