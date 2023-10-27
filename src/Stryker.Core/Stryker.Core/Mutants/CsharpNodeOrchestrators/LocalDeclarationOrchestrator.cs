using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    /// <summary>
    /// Handle const declarations.
    /// </summary>
    internal class LocalDeclarationOrchestrator : StatementSpecificOrchestrator<LocalDeclarationStatementSyntax>
    {
        /// <inheritdoc/>
        /// <remarks>We cannot mutate constants (for the time being)</remarks>
        protected override StatementSyntax OrchestrateChildrenMutation(LocalDeclarationStatementSyntax node, SemanticModel semanticModel, MutationContext context) =>
            node.IsConst ?
                // don't mutate const declaration statement
                node : base.OrchestrateChildrenMutation(node, semanticModel, context);

        // we don't inject mutations here, we want them promoted at block level
        protected override StatementSyntax InjectMutations(LocalDeclarationStatementSyntax sourceNode,
            StatementSyntax targetNode,
            SemanticModel semanticModel,
            MutationContext context) =>
            targetNode;
    }
}
