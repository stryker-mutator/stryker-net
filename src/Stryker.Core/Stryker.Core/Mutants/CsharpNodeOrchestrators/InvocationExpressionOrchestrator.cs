using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class InvocationExpressionOrchestrator: NodeSpecificOrchestrator<InvocationExpressionSyntax, ExpressionSyntax>
{

    protected override ExpressionSyntax InjectMutations(InvocationExpressionSyntax sourceNode, ExpressionSyntax targetNode, SemanticModel semanticModel, MutationContext context) => context.InjectMutations(targetNode, sourceNode);

    protected override MutationContext StoreMutations(InvocationExpressionSyntax node,
        IEnumerable<Mutant> mutations,
        MutationContext context) =>
        // if the expression contains a declaration, it must be controlled at the block level.
         context.AddMutations(mutations, node.ArgumentList.ContainsDeclarations() ? MutationControl.Block : MutationControl.Expression);

    protected override MutationContext PrepareContext(InvocationExpressionSyntax node, MutationContext context) =>
        // invocation with a member binding expression must be controlled at a higher expression level
        base.PrepareContext(node, context.Enter(node.HasAMemberBindingExpression() ? MutationControl.MemberAccess : MutationControl.Expression));

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());
}
