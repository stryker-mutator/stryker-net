using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticFieldDeclarationDeclarationOrchestrator: MemberDeclarationOrchestrator<FieldDeclarationSyntax, BaseFieldDeclarationSyntax>
    {
        protected override bool CanHandle(FieldDeclarationSyntax t)
        {
            return t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        }

        protected override BaseFieldDeclarationSyntax OrchestrateChildrenMutation(FieldDeclarationSyntax node, MutationContext context)
        {
            using var newContext = context.EnterStatic();
            // we need to signal we are in a static field
            return base.OrchestrateChildrenMutation(node, newContext);
        }

        public StaticFieldDeclarationDeclarationOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
