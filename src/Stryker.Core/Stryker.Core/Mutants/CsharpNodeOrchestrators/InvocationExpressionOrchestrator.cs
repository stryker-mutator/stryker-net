using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class InvocationExpressionOrchestrator : MemberAccessExpressionOrchestrator<InvocationExpressionSyntax>
{
    protected override ExpressionSyntax InjectMutations(InvocationExpressionSyntax sourceNode, ExpressionSyntax targetNode, SemanticModel semanticModel, MutationContext context) =>
        sourceNode.Parent is ExpressionStatementSyntax || semanticModel?.GetTypeInfo(sourceNode).Type?.SpecialType == SpecialType.System_Void
            ? targetNode
            : base.InjectMutations(sourceNode, targetNode, semanticModel, context);

    protected override MutationContext StoreMutations(InvocationExpressionSyntax node,
        IEnumerable<Mutant> mutations,
        MutationContext context) =>
         // if the invocation contains a declaration, it must be controlled at the block level.
         context.AddMutations(mutations, node.ArgumentList.ContainsDeclarations() ? MutationControl.Block : MutationControl.Expression);

    protected override ExpressionSyntax OrchestrateChildrenMutation(InvocationExpressionSyntax node, SemanticModel semanticModel,
        MutationContext context)
    {
        var mutated = node.ReplaceNodes(node.ChildNodes(), (original, _) =>
        {
            if (original == node.Expression)
            {
                // we cannot mutate only the invoked method name, mutations must be controlled at the expression level
                var subContext = context.Enter(MutationControl.MemberAccess);
                var result = subContext.Mutate(original, semanticModel);
                subContext.Leave();
                return result;
            }
            else
            {
                //The argument list can be freely mutated,
                var subContext = context.Enter(MutationControl.Member);
                var result = subContext.Mutate(original, semanticModel);
                subContext.Leave();
                return result;
            }
        });
        return mutated;
    }
}
