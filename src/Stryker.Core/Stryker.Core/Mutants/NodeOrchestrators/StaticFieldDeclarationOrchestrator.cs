using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class StaticFieldDeclarationOrchestrator: MemberDeclarationOrchestrator<FieldDeclarationSyntax, BaseFieldDeclarationSyntax>
    {
        protected override bool CanHandle(FieldDeclarationSyntax t) => t.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        protected override MutationContext PrepareContext(MutationContext context) => context.EnterStatic();

        protected override BaseFieldDeclarationSyntax OrchestrateChildrenMutation(FieldDeclarationSyntax node, MutationContext context)
        {
            var newContext = context.EnterStatic();
            // we need to signal we are in a static field
            return base.OrchestrateChildrenMutation(node, newContext);
        }

        public StaticFieldDeclarationOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
