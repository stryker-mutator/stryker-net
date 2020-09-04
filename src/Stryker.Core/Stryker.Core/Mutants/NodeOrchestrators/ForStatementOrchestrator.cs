using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class ForStatementOrchestrator: NodeSpecificOrchestrator<ForStatementSyntax>
    {
        internal override SyntaxNode OrchestrateMutation(ForStatementSyntax forStatement, MutationContext context)
        {
            // for needs special treatments for its incrementer
            var originalFor = forStatement;
            forStatement = originalFor.TrackNodes(forStatement.Incrementors);
            foreach (var incrementer in originalFor.Incrementors)
            {
                forStatement = forStatement.ReplaceNode(forStatement.GetCurrentNode(incrementer),
                    context.MutateNodeAndChildren(incrementer, true));
            }

            // mutate condition, if any
            if (originalFor.Condition != null)
            {
                forStatement = forStatement.ReplaceNode(forStatement.Condition,
                    context.MutateNodeAndChildren(originalFor.Condition));
            }

            // mutate the statement/block
            forStatement = forStatement.ReplaceNode(forStatement.Statement, context.MutateNodeAndChildren(forStatement.Statement));
            // and now we replace it
            return forStatement;
        }
    }
}
