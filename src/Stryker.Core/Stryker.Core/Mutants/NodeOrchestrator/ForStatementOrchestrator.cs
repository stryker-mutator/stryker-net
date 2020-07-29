using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    class ForStatementOrchestrator: NodeSpecificOrchestrator<ForStatementSyntax>
    {
        internal override SyntaxNode OrchestrateMutation(ForStatementSyntax forStatement, MutationContext context)
        {
            // for needs special treatments for its incrementors
            StatementSyntax forWithMutantIncrementors = forStatement.TrackNodes(forStatement);

            foreach (var incrementor in forStatement.Incrementors)
            {
                forWithMutantIncrementors = context.MutateSubExpressionWithIfStatements(forStatement, forWithMutantIncrementors, incrementor);
            }

            var originalFor = forWithMutantIncrementors.GetCurrentNode(forStatement);

            // mutate condition, if any
            ForStatementSyntax mutatedFor;
            StatementSyntax statementPart;
            if (forStatement.Condition == null)
            {
                mutatedFor = forStatement;
                statementPart = forStatement.Statement;
            }
            else
            {
                mutatedFor = forStatement.TrackNodes(forStatement.Condition, forStatement.Statement);
                mutatedFor = mutatedFor.ReplaceNode(mutatedFor.GetCurrentNode(forStatement.Condition),
                    context.Mutate(forStatement.Condition));
                statementPart = mutatedFor.GetCurrentNode(forStatement.Statement);
            }

            // mutate the statement/block
            mutatedFor = mutatedFor.ReplaceNode(statementPart, context.Mutate(forStatement.Statement));
            // and now we replace it
            return  forWithMutantIncrementors.ReplaceNode(originalFor, mutatedFor);
        }
    }
}
