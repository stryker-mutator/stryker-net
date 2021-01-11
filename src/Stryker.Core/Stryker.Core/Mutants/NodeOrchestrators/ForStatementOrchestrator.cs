using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class ForStatementOrchestrator: BlockScopeOrchestrator<ForStatementSyntax>
    {
        /// <inheritdoc/>
        /// `<remarks>The sole benefit of this orchestrator is to provide code ordered mutations for now.</remarks>
        protected override StatementSyntax OrchestrateChildrenMutation(ForStatementSyntax forStatement, MutationContext context)
        {
            // for needs special treatments for its incrementer(s)
            var originalFor = forStatement;
            forStatement = originalFor.ReplaceNodes(originalFor.Initializers.Union(originalFor.Incrementors),
                (syntax, expressionSyntax) => MutantOrchestrator.Mutate(syntax, context));
            if (forStatement.Declaration != null)
            {
                forStatement = forStatement.ReplaceNode(forStatement.Declaration,
                MutantOrchestrator.Mutate(originalFor.Declaration, context));
            }
            // mutate condition, if any
            if (originalFor.Condition != null)
            {
                forStatement = forStatement.ReplaceNode(forStatement.Condition!,
                    MutantOrchestrator.Mutate(originalFor.Condition, context));
            }

            // mutate the statement/block
            forStatement = forStatement.ReplaceNode(forStatement.Statement, MutantOrchestrator.Mutate(originalFor.Statement, context));
            return forStatement;
        }

        public ForStatementOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
