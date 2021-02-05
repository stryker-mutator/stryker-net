using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation
{
    internal class PropertyExpressionToBodyEngine : BaseEngine<PropertyDeclarationSyntax>
    {
        public PropertyExpressionToBodyEngine(string markerId) : base(markerId)
        {
        }

        public PropertyDeclarationSyntax ConvertExpressionToBody(PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration.ExpressionBody == null)
            {
                return propertyDeclaration;
            }

            var block = SyntaxFactory.Block(
                SyntaxFactory.ReturnStatement(
                    propertyDeclaration.ExpressionBody.Expression.WithLeadingTrivia(SyntaxFactory.Space)));

            return propertyDeclaration.WithExpressionBody(null).WithAccessorList(
                SyntaxFactory.AccessorList(SyntaxFactory.List(new []{
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, block)}))).
                WithSemicolonToken(SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken))
                .WithAdditionalAnnotations(Marker);
        }

        protected override SyntaxNode Revert(PropertyDeclarationSyntax node)
        {
            if (node.AccessorList == null)
            {
                throw new InvalidOperationException($"Expected a property with a get propertyDeclaration {node}.");
            }

            var getter = node.GetAccessor();
            if (getter == null)
            {
                throw new InvalidOperationException($"Expected a get accessor {node}.");
            }
            var ReturnStatement = getter?.Body?.Statements.FirstOrDefault() as ReturnStatementSyntax;
            if (ReturnStatement == null)
            {
                throw new InvalidOperationException($"Expected a return statement here {getter.Body}.");
            }

            return node.WithAccessorList(null)
                .WithExpressionBody(SyntaxFactory.ArrowExpressionClause(ReturnStatement.Expression))
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }
    }
}
