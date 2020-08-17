using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class ForStatementOrchestrator: NodeSpecificOrchestrator<ForStatementSyntax>
    {
        internal override SyntaxNode OrchestrateMutation(ForStatementSyntax forStatement, MutationContext context)
        {
            // for needs special treatments for its incrementer
            var originalFor = forStatement;
            foreach (var incrementer in forStatement.Incrementors)
            {
                context.GenerateStatementLevelControlledMutants(incrementer);
            }

            // mutate condition, if any
            if (forStatement.Condition != null)
            {
                forStatement = forStatement.ReplaceNode(forStatement.Condition,
                    context.Mutate(forStatement.Condition));
            }

            // mutate the statement/block
            forStatement = forStatement.ReplaceNode(forStatement.Statement, context.Mutate(originalFor.Statement));
            // and now we replace it
            return forStatement;
        }
    }
}
