using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class InvocationExpressionOrchestrator: NodeSpecificOrchestrator<InvocationExpressionSyntax, ExpressionSyntax>
{
    /// <inheritdoc/>
    /// <remarks>Inject all pending mutations controlled with conditional operator(s).</remarks>
    protected override ExpressionSyntax InjectMutations(InvocationExpressionSyntax sourceNode, ExpressionSyntax targetNode, SemanticModel semanticModel, MutationContext context) => context.InjectExpressionLevel(targetNode, sourceNode);

    protected override ExpressionSyntax OrchestrateChildrenMutation(InvocationExpressionSyntax node, SemanticModel semanticModel, MutationContext context)
    {
        var expressions = (ExpressionSyntax) MutateSingleNode(node.Expression, semanticModel, context.Enter(MutationControl.MemberAccess));
        context.Leave(MutationControl.MemberAccess);
        context.Enter(MutationControl.Member);
        var argumentListSyntax = (ArgumentListSyntax)MutateSingleNode(node.ArgumentList, semanticModel, context);
        var result = node.WithExpression(expressions).WithArgumentList(argumentListSyntax);
        context.Leave(MutationControl.Member);
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
