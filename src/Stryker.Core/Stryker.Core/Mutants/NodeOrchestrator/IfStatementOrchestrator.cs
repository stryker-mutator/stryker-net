using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    class IfStatementOrchestrator: NodeSpecificOrchestrator<IfStatementSyntax>
    {
        internal override IfStatementSyntax OrchestrateMutation(IfStatementSyntax nodeToParse, MutationContext context)
        {
            var mutatedIf = nodeToParse.Else != null
                ? nodeToParse.TrackNodes(nodeToParse.Condition, nodeToParse.Statement, nodeToParse.Else)
                : nodeToParse.TrackNodes(nodeToParse.Condition, nodeToParse.Statement);

            var mutated = false;

            if (!nodeToParse.Condition.ContainsDeclarations())
            {
                var currentCondition = mutatedIf.GetCurrentNode(nodeToParse.Condition);
                var mutatedCondition = context.Mutate(nodeToParse.Condition);
                if (mutatedCondition != currentCondition)
                {
                    mutatedIf = mutatedIf.ReplaceNode(currentCondition, mutatedCondition);
                    mutated = true;
                }
            }

            if (nodeToParse.Else != null)
            {
                var currentElse = mutatedIf.GetCurrentNode(nodeToParse.Else);
                var mutatedElse = context.Mutate(nodeToParse.Else);
                if (mutatedElse != currentElse)
                {
                    mutatedIf = mutatedIf.ReplaceNode(currentElse, mutatedElse);
                    mutated = true;
                }
            }

            var currentStatement = mutatedIf.GetCurrentNode(nodeToParse.Statement);
            var mutatedStatement = context.Mutate(nodeToParse.Statement);
            if (currentStatement != mutatedStatement)
            {
                mutatedIf = mutatedIf.ReplaceNode(currentStatement, mutatedStatement);
                mutated = true;
            }
            return mutated ? mutatedIf : nodeToParse;
        }
    }
}
    