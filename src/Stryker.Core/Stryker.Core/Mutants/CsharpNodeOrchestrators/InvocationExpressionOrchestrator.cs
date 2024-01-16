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
        MutationContext context)
    {
        // if the expression contains a declaration, it must be controlled at the block level.
        if (node.ContainsDeclarations())
        {
            context.AddBlockLevel(mutations);
        }
        else
        {
            context.AddMutations(mutations);
        }
        return context;
    }

    // sadly, we need to parse recursively to find out if there is a member binding expression which cannot be preceded by a ternary operator
    private bool HasAMemberBindingExpression(MemberAccessExpressionSyntax node) =>
        node.Expression switch
        {
            MemberBindingExpressionSyntax => true,
            MemberAccessExpressionSyntax memberAccess => HasAMemberBindingExpression(memberAccess),
            InvocationExpressionSyntax invocation => HasAMemberBindingExpression(invocation),
            _ => false
        };

    private bool HasAMemberBindingExpression(InvocationExpressionSyntax node) =>
        node.Expression switch
        {
            MemberBindingExpressionSyntax => true,
            MemberAccessExpressionSyntax memberAccess => HasAMemberBindingExpression(memberAccess),
            InvocationExpressionSyntax invocation => HasAMemberBindingExpression(invocation),
            _ => false
        };

    protected override MutationContext PrepareContext(InvocationExpressionSyntax node, MutationContext context)
    {
        // invocation mutations cannot be mutated within an invocation chain
        context = context.Enter(HasAMemberBindingExpression(node) ? MutationControl.MemberAccess : MutationControl.Expression);
        return base.PrepareContext(node, context);
    }

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());
}
