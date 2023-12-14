using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class InvocationExpressionOrchestrator: ExpressionSpecificOrchestrator<InvocationExpressionSyntax>
{
    protected override ExpressionSyntax OrchestrateChildrenMutation(InvocationExpressionSyntax node, SemanticModel semanticModel, MutationContext context)
    {
        var subContext = context.Enter(MutationControl.MemberAccess);
        var expressions = (ExpressionSyntax)MutateSingleNode(node.Expression, semanticModel, subContext);
        subContext.Leave(MutationControl.MemberAccess);
        var argumentListSyntax = (ArgumentListSyntax)MutateSingleNode(node.ArgumentList, semanticModel, subContext);
        var result = node.WithExpression(expressions).WithArgumentList(argumentListSyntax);
        return result;
    }

    protected override MutationContext StoreMutations(InvocationExpressionSyntax node,
        IEnumerable<Mutant> mutations,
        MutationContext context)
    {
        // if the expression contains a declaration, it must be controlled at the block level.
        if (node.ContainsDeclarations())
        {
            context.AddBlockLevel(mutations);
        }
        else
        {
            context.AddExpressionLevel(mutations);
        }
        return context;
    }
}
