using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators
{
    internal class ForStatementOrchestrator : BlockScopeOrchestrator<ForStatementSyntax>
    {
        /// <inheritdoc/>
        /// `<remarks>The sole benefit of this orchestrator is to provide code ordered mutations for now.</remarks>
        protected override StatementSyntax OrchestrateChildrenMutation(ForStatementSyntax node, MutationContext context)
        {
            // for needs special treatments for its incrementer(s)
            var originalFor = node;
            node = originalFor.ReplaceNodes(originalFor.Initializers.Union(originalFor.Incrementors),
                (syntax, expressionSyntax) => MutantOrchestrator.Mutate(syntax, context));
            if (node.Declaration != null)
            {
                node = node.ReplaceNode(node.Declaration,
                MutantOrchestrator.Mutate(originalFor.Declaration, context));
            }
            // mutate condition, if any
            if (originalFor.Condition != null)
            {
                node = node.ReplaceNode(node.Condition!,
                    MutantOrchestrator.Mutate(originalFor.Condition, context));
            }

            // mutate the statement/block
            node = node.ReplaceNode(node.Statement, MutantOrchestrator.Mutate(originalFor.Statement, context));
            return node;
        }

        public ForStatementOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
