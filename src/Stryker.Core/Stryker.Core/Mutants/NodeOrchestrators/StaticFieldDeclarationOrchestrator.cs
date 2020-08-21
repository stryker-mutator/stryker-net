using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticFieldDeclarationOrchestrator: NodeSpecificOrchestrator<FieldDeclarationSyntax>
    {
        protected override bool CanHandleThis(FieldDeclarationSyntax t)
        {
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        }

        internal override SyntaxNode OrchestrateMutation(FieldDeclarationSyntax node, MutationContext context)
        {
            return context.EnterStatic().MutateNodeAndChildren(node);
        }
    }
}