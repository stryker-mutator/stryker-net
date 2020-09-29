using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class ForStatementOrchestrator: NodeSpecificOrchestrator<ForStatementSyntax>
    {
        protected override SyntaxNode OrchestrateMutation(ForStatementSyntax forStatement, MutationContext context)
        {
            // for needs special treatments for its incrementer
            var originalFor = forStatement;
            forStatement = originalFor.TrackNodes(forStatement.Incrementors);
            foreach (var incrementer in originalFor.Incrementors)
            {
                forStatement = forStatement.ReplaceNode(forStatement.GetCurrentNode(incrementer),
                    MutantOrchestrator.Mutate(incrementer, context));
            }

            // mutate condition, if any
            if (originalFor.Condition != null)
            {
                forStatement = forStatement.ReplaceNode(forStatement.Condition!,
                    MutantOrchestrator.Mutate(forStatement.Condition, context));
            }

            // mutate the statement/block
            forStatement = forStatement.ReplaceNode(forStatement.Statement, MutantOrchestrator.Mutate(forStatement.Statement, context));
            // and now we replace it
            return forStatement;
        }

        public ForStatementOrchestrator(MutantOrchestrator mutantOrchestrator) : base(mutantOrchestrator)
        {
        }
    }
}
