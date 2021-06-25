using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticFieldDeclarationOrchestrator: NodeSpecificOrchestrator<FieldDeclarationSyntax, BaseFieldDeclarationSyntax>
    {
        protected override bool CanHandle(FieldDeclarationSyntax t) => t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        protected override MutationContext PrepareContext(FieldDeclarationSyntax _, MutationContext context) => context.EnterStatic();
    }
}
