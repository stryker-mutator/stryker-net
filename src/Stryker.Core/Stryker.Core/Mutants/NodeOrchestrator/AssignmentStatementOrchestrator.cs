﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal class AssignmentStatementOrchestrator : NodeSpecificOrchestrator<AssignmentExpressionSyntax>
    {
        internal override SyntaxNode OrchestrateMutation(AssignmentExpressionSyntax node, MutationContext context)
        {
            // mutate +=, *=, ...
            // those mutations must can"t be controlled in line, they can only be controlled as a full statement (i.e. using 'if's)
            context.GenerateStatementLevelControlledMutants(node);
            // mutate the part right to the equal sign
            return node.ReplaceNode(node.Right, context.Mutate(node.Right));
        }
    }
}