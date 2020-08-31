using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrators
{
    internal class AssignmentStatementOrchestrator : NodeSpecificOrchestrator<AssignmentExpressionSyntax>
    {
        internal override SyntaxNode OrchestrateMutation(AssignmentExpressionSyntax node, MutationContext context)
        {
            // mutate +=, *=, ...
            // those mutations can"t be controlled in line, they can only be controlled as a full statement (i.e. using 'if's)
            // mutate the part right to the equal sign
            return context.MutateNodeAndChildren(node, true);
        }
    }
}