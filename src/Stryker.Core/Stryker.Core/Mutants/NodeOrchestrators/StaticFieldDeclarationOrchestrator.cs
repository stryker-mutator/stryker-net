using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticFieldDeclarationOrchestrator : NodeSpecificOrchestrator<FieldDeclarationSyntax>
    {
        protected override bool CanHandle(FieldDeclarationSyntax t)
        {
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        }

        internal override SyntaxNode OrchestrateMutation(FieldDeclarationSyntax node, MutationContext context)
        {
            return context.EnterStatic().MutateNodeAndChildren(node);
        }
    }
}