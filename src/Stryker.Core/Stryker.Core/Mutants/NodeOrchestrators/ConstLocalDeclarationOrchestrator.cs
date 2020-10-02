using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    class ConstLocalDeclarationOrchestrator : NodeSpecificOrchestrator<LocalDeclarationStatementSyntax>
    {
        protected override bool CanHandle(LocalDeclarationStatementSyntax t)
        {
            return t.IsConst;
        }

        internal override SyntaxNode OrchestrateMutation(LocalDeclarationStatementSyntax node, MutationContext context)
        {
            // don't mutate const declaration statement
            return node;
        }
    }
}
