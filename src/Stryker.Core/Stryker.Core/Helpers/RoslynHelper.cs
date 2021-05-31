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

        /// <summary>
        /// Gets the return type of a method (incl. constructor, destructor...)
        /// </summary>
        /// <param name="baseMethod">method to analyze</param>
        /// <returns>method returns type. Null if this not relevant</returns>
        public static TypeSyntax ReturnType(this BaseMethodDeclarationSyntax baseMethod) =>
            baseMethod switch
            {
                ConstructorDeclarationSyntax _ => null,
                ConversionOperatorDeclarationSyntax _ => null,
                DestructorDeclarationSyntax _ => null,
                MethodDeclarationSyntax method => method.ReturnType,
                OperatorDeclarationSyntax operatorSyntax => operatorSyntax.ReturnType,
                _ => null
            };

        /// <summary>
        /// Checks if a method ends with return.
        /// </summary>
        /// <param name="baseMethod">method to analyze.</param>
        /// <returns>true if <paramref name="baseMethod"/>is non void method, an operator or a conversion operator.</returns>
        public static bool NeedsReturn(this BaseMethodDeclarationSyntax baseMethod) =>
            baseMethod switch
            {
                MethodDeclarationSyntax method => !(method.ReturnType is PredefinedTypeSyntax predefinedType &&
                                                    predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword)),
                OperatorDeclarationSyntax _ => true,
                ConversionOperatorDeclarationSyntax _=> true,
                _ => false
            };

        /// <summary>
        /// Checks if a member is static
        /// </summary>
        /// <param name="node">Member to analyze</param>
        /// <returns>true if it is a static method, properties, fields...</returns>
        public static bool IsStatic(this MemberDeclarationSyntax node) => node.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);

        /// <summary>
        /// Checks if an accessor is a getter and needs to end with a return
        /// </summary>
        /// <param name="baseMethod"></param>
        /// <returns>true if this is a getter</returns>
        public static bool NeedsReturn(this AccessorDeclarationSyntax baseMethod) =>
            baseMethod.Keyword.Text == "get";

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <param name="mutation"></param>
        /// <returns></returns>
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

        public static ExpressionSyntax BuildDefaultExpression(this TypeSyntax type) => SyntaxFactory.DefaultExpression(type.WithoutTrailingTrivia()).WithLeadingTrivia(SyntaxFactory.Space);
    }
}
