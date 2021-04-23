using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Instrumentation
{
    /// <summary>
    /// Helper that converts a method (including operators) from expression body to statement body form (or arrow to curly braces).
    /// </summary>
    internal class ExpressionMethodToBodyEngine : BaseEngine<BaseMethodDeclarationSyntax>
    {
        public ExpressionMethodToBodyEngine(string markerId) : base(markerId)
        {
        }

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
                    statementLine = SyntaxFactory.ReturnStatement(method.ExpressionBody.Expression.WithLeadingTrivia(SyntaxFactory.Space));
                    break;

                case ConversionOperatorDeclarationSyntax _:
                case OperatorDeclarationSyntax _:
                    statementLine = SyntaxFactory.ReturnStatement(method.ExpressionBody.Expression.WithLeadingTrivia(SyntaxFactory.Space));
                    break;

                default:
                    statementLine = SyntaxFactory.ExpressionStatement(method.ExpressionBody.Expression);
                    break;
            }

            // do we need add return to the expression body?
            var statement = SyntaxFactory.Block(statementLine);

            BaseMethodDeclarationSyntax result = method switch
            {
                MethodDeclarationSyntax actualMethod => actualMethod.Update(actualMethod.AttributeLists,
                    actualMethod.Modifiers, actualMethod.ReturnType, actualMethod.ExplicitInterfaceSpecifier,
                    actualMethod.Identifier, actualMethod.TypeParameterList, actualMethod.ParameterList,
                    actualMethod.ConstraintClauses, statement, null, SyntaxFactory.Token(SyntaxKind.None)),
                OperatorDeclarationSyntax operatorDeclaration => operatorDeclaration.Update(
                    operatorDeclaration.AttributeLists, operatorDeclaration.Modifiers, operatorDeclaration.ReturnType,
                    operatorDeclaration.OperatorKeyword, operatorDeclaration.OperatorToken,
                    operatorDeclaration.ParameterList, statement, SyntaxFactory.Token(SyntaxKind.None)),
                ConversionOperatorDeclarationSyntax conversion => conversion.Update(conversion.AttributeLists,
                    conversion.Modifiers, conversion.ImplicitOrExplicitKeyword, conversion.OperatorKeyword,
                    conversion.Type, conversion.ParameterList, statement, null, SyntaxFactory.Token(SyntaxKind.None)),
                DestructorDeclarationSyntax destructor => destructor.Update(destructor.AttributeLists,
                    destructor.Modifiers, destructor.TildeToken, destructor.Identifier, destructor.ParameterList,
                    statement, SyntaxFactory.Token(SyntaxKind.None)),
                ConstructorDeclarationSyntax constructor => constructor.Update(constructor.AttributeLists,
                    constructor.Modifiers, constructor.Identifier, constructor.ParameterList, constructor.Initializer,
                    statement, SyntaxFactory.Token(SyntaxKind.None)),
                _ => method
            };

            return result.WithAdditionalAnnotations(Marker) as T;
        }

        protected override SyntaxNode Revert(BaseMethodDeclarationSyntax node)
        {
            // get expression
            var expression = SyntaxFactory.ArrowExpressionClause(node.Body?.Statements[0] switch
            {
                ReturnStatementSyntax returnStatement => returnStatement.Expression,
                ExpressionStatementSyntax expressionStatement => expressionStatement.Expression,
                _ => throw new InvalidOperationException($"Can't extract original expression from {node.Body}")
            });

            return node switch
            {
                MethodDeclarationSyntax actualMethod => actualMethod.Update(actualMethod.AttributeLists,
                    actualMethod.Modifiers, actualMethod.ReturnType, actualMethod.ExplicitInterfaceSpecifier,
                    actualMethod.Identifier, actualMethod.TypeParameterList, actualMethod.ParameterList,
                    actualMethod.ConstraintClauses, null, expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                OperatorDeclarationSyntax operatorDeclaration => operatorDeclaration.Update(
                    operatorDeclaration.AttributeLists, operatorDeclaration.Modifiers, operatorDeclaration.ReturnType,
                    operatorDeclaration.OperatorKeyword, operatorDeclaration.OperatorToken,
                    operatorDeclaration.ParameterList, null, expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                ConversionOperatorDeclarationSyntax conversion => conversion.Update(conversion.AttributeLists,
                    conversion.Modifiers, conversion.ImplicitOrExplicitKeyword, conversion.OperatorKeyword,
                    conversion.Type, conversion.ParameterList, null, expression,
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                DestructorDeclarationSyntax destructor => destructor.Update(destructor.AttributeLists,
                    destructor.Modifiers, destructor.TildeToken, destructor.Identifier, destructor.ParameterList,
                    null, expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                ConstructorDeclarationSyntax constructor => constructor.Update(constructor.AttributeLists,
                    constructor.Modifiers, constructor.Identifier, constructor.ParameterList, constructor.Initializer,
                    null, expression, SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                _ => node
            };
        }

    }
}
