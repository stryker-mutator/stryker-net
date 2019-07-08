using System.Collections.Generic;
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

        // enumerate subexpressions of interest of a node
        public static IEnumerable<ExpressionSyntax> EnumerateSubExpressions(SyntaxNode node)
        {
            switch (node)
            {
                case AssignmentExpressionSyntax assignmentExpression:
                    yield return assignmentExpression.Right;
                    yield break;
                case LocalDeclarationStatementSyntax localDeclarationStatement:
                    foreach(var vars in localDeclarationStatement.Declaration.Variables.Select(x => x.Initializer?.Value))
                    {
                        yield return vars;
                    }
                    yield break;
                case ReturnStatementSyntax returnStatement:
                    yield return returnStatement.Expression;
                    yield break;
                case LocalFunctionStatementSyntax localFunction:
                    yield return localFunction.ExpressionBody?.Expression;
                    yield break;
                case InvocationExpressionSyntax invocationExpression:
                    yield return invocationExpression.Expression;
                    foreach (var arg in invocationExpression.ArgumentList.Arguments.Select(x => x.Expression))
                    {
                        yield return arg;
                    }
                    yield break;
            }

            if (node is ExpressionSyntax expression)
            {
                yield return expression;
            }
        }

        public static bool CanBeMutated(SyntaxNode node)
        {
            switch (node)
            {
                // don't mutate attributes or their arguments
                case AttributeListSyntax _:
                // don't mutate parameters
                case ParameterSyntax _:
                // don't mutate constant fields
                case FieldDeclarationSyntax field when field.Modifiers.Any(x => x.Kind() == SyntaxKind.ConstKeyword):
                    return false;
                default:
                    return true;
            }
        }
    }
}
