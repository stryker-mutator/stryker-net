using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticFieldDeclarationOrchestrator: NodeSpecificOrchestrator<FieldDeclarationSyntax>
    {
        protected override MutationContext PrepareContext(FieldDeclarationSyntax node, MutationContext context)
        {
            return context.EnterStatic();
        }

        protected override bool CanHandle(FieldDeclarationSyntax t)
        {
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        }

        protected override SyntaxNode OrchestrateMutation(FieldDeclarationSyntax node, MutationContext context)
        {
            return context.MutateNodeAndChildren(node);
        }

        public StaticFieldDeclarationOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}