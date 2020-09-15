using Microsoft.Build.ObjectModelRemoting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    }
}
