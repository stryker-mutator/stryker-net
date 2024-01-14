using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Mono.Cecil.Cil;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class InvocationExpressionOrchestrator: NodeSpecificOrchestrator<InvocationExpressionSyntax, ExpressionSyntax>
{

    protected override ExpressionSyntax InjectMutations(InvocationExpressionSyntax sourceNode, ExpressionSyntax targetNode, SemanticModel semanticModel, MutationContext context) => context.InjectExpressionLevel(targetNode, sourceNode);

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

    protected override MutationContext PrepareContext(InvocationExpressionSyntax node, MutationContext context)
    {
        // invocation mutations cannot be mutated within an invocation chain
        context = context.Enter(node?.Parent is InvocationExpressionSyntax or MemberAccessExpressionSyntax
            || (node?.Parent is ConditionalAccessExpressionSyntax cond && cond.WhenNotNull == node)
            || node?.Parent.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.SuppressNullableWarningExpression)==true ? MutationControl.MemberAccess : MutationControl.Expression);
        return base.PrepareContext(node, context);
    }

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());
}
