using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Helpers
{
    internal static class RoslynHelper
    {
        // gets the return type of a method (incl. constructor, destructor...)
        public static TypeSyntax ReturnType(this BaseMethodDeclarationSyntax baseMethod) =>
            baseMethod switch
            {
                ConstructorDeclarationSyntax _ => null,
                ConversionOperatorDeclarationSyntax conversion => conversion.Type,
                DestructorDeclarationSyntax _ => null,
                MethodDeclarationSyntax method => method.ReturnType,
                OperatorDeclarationSyntax operatorSyntax => operatorSyntax.ReturnType,
                _ => null
            };

        public static bool NeedsReturn(this BaseMethodDeclarationSyntax baseMethod) =>
            baseMethod switch
            {
                MethodDeclarationSyntax method => !(method.ReturnType is PredefinedTypeSyntax predefinedType &&
                                                    predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword)),
                OperatorDeclarationSyntax _ => true,
                ConversionOperatorDeclarationSyntax _=> true,
                _ => false
            };

        public static bool NeedsReturn(this AccessorDeclarationSyntax baseMethod) =>
            baseMethod.Keyword.Text switch
            {
                "get" => true,
                _ => false
            };

        public static bool NeedsReturn(this LocalFunctionStatementSyntax localFunction) =>
            !localFunction.ReturnType.IsVoid();

        public static bool IsVoid(this TypeSyntax type) => type is PredefinedTypeSyntax predefinedType &&
                                                      predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword);
        public static bool IsStatic(this MemberDeclarationSyntax node) => node.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);

        public static T InjectMutation<T>(this T original, Mutation mutation) where T:SyntaxNode
        {
            if (!original.Contains(mutation.OriginalNode))
            {
                // if this happens, there is a probably a bug in some orchestrator
                throw new InvalidOperationException($"Cannot inject mutation '{mutation.ReplacementNode}' in '{original}' because we cannot find the original code.");
            }
            return original.ReplaceNode(mutation.OriginalNode, mutation.ReplacementNode);
        }

        public static AccessorDeclarationSyntax GetAccessor(this PropertyDeclarationSyntax propertyDeclaration)
            => propertyDeclaration?.AccessorList?.Accessors.FirstOrDefault(a => a.Keyword.Text == "get");

        public static ExpressionSyntax BuildDefaultExpression(this TypeSyntax type) => SyntaxFactory.DefaultExpression(type.WithoutTrivia()).WithLeadingTrivia(SyntaxFactory.Space);
    }
}
