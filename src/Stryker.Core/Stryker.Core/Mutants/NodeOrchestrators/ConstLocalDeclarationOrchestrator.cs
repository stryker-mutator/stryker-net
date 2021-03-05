using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    /// <summary>
    /// Handle const declarations.
    /// </summary>
    internal class ConstLocalDeclarationOrchestrator : StatementSpecificOrchestrator<LocalDeclarationStatementSyntax>
    {
        protected override bool CanHandle(LocalDeclarationStatementSyntax t)
        {
            return t.IsConst;
        }

        /// <inheritdoc/>
        /// <remarks>We cannot mutate constants (for the time being)</remarks>
        protected override StatementSyntax OrchestrateChildrenMutation(LocalDeclarationStatementSyntax node, MutationContext context)
        {
            // don't mutate const declaration statement
            return node;
        }

        public ConstLocalDeclarationOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
