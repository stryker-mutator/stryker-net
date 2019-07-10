using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Stryker.Core.Mutants
{
    internal class MutationContext
    {
        public bool InStaticValue { get; set; }

        public MutationContext UpdateContext(SyntaxNode node)
        {
            switch (node)
            {
                case FieldDeclarationSyntax fieldDeclaration:
                    if (fieldDeclaration.Modifiers.Any(x => CSharpExtensions.Kind((SyntaxToken) x) == SyntaxKind.StaticKeyword))
                    {
                        return new MutationContext {InStaticValue = true};
                    }
                    break;
                case ConstructorDeclarationSyntax constructorDeclaration:
                    if (constructorDeclaration.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword))
                    {
                        return new MutationContext {InStaticValue = true};
                    }
                    break;
            }

            return this;
        }
    }
}