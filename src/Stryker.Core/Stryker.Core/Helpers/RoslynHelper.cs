using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;

namespace Stryker.Core.Helpers
{
    internal static class RoslynHelper
    {
        // gets the return type of a method (incl. constructor, destructor...)
        public static TypeSyntax ReturnType(this BaseMethodDeclarationSyntax baseMethod)
        {
            return baseMethod switch
            {
                ConstructorDeclarationSyntax _ => null,
                ConversionOperatorDeclarationSyntax _ => null,
                DestructorDeclarationSyntax _ => null,
                MethodDeclarationSyntax method => method.ReturnType,
                OperatorDeclarationSyntax oper => oper.ReturnType,
                _ => null
            };
        }

        public static bool IsVoidReturningMethod(this MethodDeclarationSyntax baseMethod)
        {
            if (baseMethod.ReturnType is PredefinedTypeSyntax predefinedType &&
                predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword))
            {
                return true;
            }

            return false;
        }

        public static T InjectMutation<T>(this T original, Mutation mutation) where T:SyntaxNode
        {
            if (!original.Contains(mutation.OriginalNode))
            {
                throw new InvalidOperationException($"Cannot inject mutation '{mutation.ReplacementNode}' as id does not contains the reference node.");
            }
            return original.ReplaceNode(mutation.OriginalNode, mutation.ReplacementNode);
        }
    }
}
