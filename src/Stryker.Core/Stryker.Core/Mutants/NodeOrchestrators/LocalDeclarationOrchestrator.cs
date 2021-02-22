using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    /// <summary>
    /// Handle const declarations.
    /// </summary>
    internal class LocalDeclarationOrchestrator : StatementSpecificOrchestrator<LocalDeclarationStatementSyntax>
    {

        /// <inheritdoc/>
        /// <remarks>We cannot mutate constants (for the time being)</remarks>
        protected override StatementSyntax OrchestrateChildrenMutation(LocalDeclarationStatementSyntax node, MutationContext context)
        {
            if (node.IsConst)
            {
                // don't mutate const declaration statement
                return node;
            }

            var result = base.OrchestrateChildrenMutation(node, context);
            // statement level mutations need to be changed to block level
            context.BlockLevelControlledMutations.AddRange(context.StatementLevelControlledMutations);
            context.StatementLevelControlledMutations.Clear();
            return result;
        }

        public LocalDeclarationOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
