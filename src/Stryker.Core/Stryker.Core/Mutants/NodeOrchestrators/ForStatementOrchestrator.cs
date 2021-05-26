using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class ForStatementOrchestrator: StatementSpecificOrchestrator<ForStatementSyntax>
    {
        /// <inheritdoc/>
        /// `<remarks>The sole benefit of this orchestrator is to provide code ordered mutations for now.</remarks>
        protected override StatementSyntax OrchestrateChildrenMutation(ForStatementSyntax forStatement, MutationContext context)
        {
            // for needs special treatments for its incrementer(s)
            var originalFor = forStatement;
            forStatement = originalFor.ReplaceNodes(originalFor.Initializers.Union(originalFor.Incrementors),
                (syntax, _) => MutateSingleNode(syntax, context));
            if (forStatement.Declaration != null)
            {
                forStatement = forStatement.ReplaceNode(forStatement.Declaration,
                MutateSingleNode(originalFor.Declaration, context));
            }
            // mutate condition, if any
            if (originalFor.Condition != null)
            {
                forStatement = forStatement.ReplaceNode(forStatement.Condition!,
                    MutateSingleNode(originalFor.Condition, context));
            }

            // mutate the statement/block
            forStatement = forStatement.ReplaceNode(forStatement.Statement, MutateSingleNode(originalFor.Statement, context));
            return forStatement;
        }

        public ForStatementOrchestrator(CsharpMutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
