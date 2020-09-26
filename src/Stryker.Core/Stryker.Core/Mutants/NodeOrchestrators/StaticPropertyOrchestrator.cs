using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticPropertyOrchestrator: NodeSpecificOrchestrator<PropertyDeclarationSyntax>
    {
        protected override bool CanHandle(PropertyDeclarationSyntax t)
        {
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword) &&
                t.AccessorList != null;
        }

        protected override SyntaxNode OrchestrateMutation(PropertyDeclarationSyntax node, MutationContext context)
        {
            return context.MutateNodeAndChildren(node);
        }

        public StaticPropertyOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
