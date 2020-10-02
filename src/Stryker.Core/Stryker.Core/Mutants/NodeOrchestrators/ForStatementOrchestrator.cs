using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class ForStatementOrchestrator: BlockScopeOrchestrator<ForStatementSyntax>
    {
        protected override StatementSyntax OrchestrateChildrenMutation(ForStatementSyntax forStatement, MutationContext context)
        {
            // for needs special treatments for its incrementer
            var originalFor = forStatement;
            forStatement = originalFor.ReplaceNodes(originalFor.Incrementors,
                (syntax, expressionSyntax) => MutantOrchestrator.Mutate(syntax, context));

            // mutate condition, if any
            if (originalFor.Condition != null)
            {
                forStatement = forStatement.ReplaceNode(forStatement.Condition!,
                    MutantOrchestrator.Mutate(forStatement.Condition, context));
            }

            // mutate the statement/block
            forStatement = forStatement.ReplaceNode(forStatement.Statement, MutantOrchestrator.Mutate(forStatement.Statement, context));
            return forStatement;
        }

        public ForStatementOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
