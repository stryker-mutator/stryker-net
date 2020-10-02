using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    class ConstLocalDeclarationOrchestrator : StatementSpecificOrchestrator<LocalDeclarationStatementSyntax>
    {
        protected override bool CanHandle(LocalDeclarationStatementSyntax t)
        {
            return t.IsConst;
        }

        protected override StatementSyntax OrchestrateChildrenMutation(LocalDeclarationStatementSyntax node, MutationContext context)
        {
            // don't mutate const declaration statement
            return node;
        }

        public ConstLocalDeclarationOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
