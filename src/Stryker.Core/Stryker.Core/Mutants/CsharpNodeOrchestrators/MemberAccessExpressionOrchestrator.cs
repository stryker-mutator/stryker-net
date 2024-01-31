using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal class MemberAccessExpressionOrchestrator<T> : NodeSpecificOrchestrator<T, T> where T : ExpressionSyntax
{
    protected override bool CanHandle(T t) => t.Parent is MemberAccessExpressionSyntax or InvocationExpressionSyntax;

    protected override MutationContext PrepareContext(T node, MutationContext context) => base.PrepareContext(node, context.Enter(MutationControl.MemberAccess));

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());
}
