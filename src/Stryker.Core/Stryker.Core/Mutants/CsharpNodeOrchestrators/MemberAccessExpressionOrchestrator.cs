using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;
internal class MemberAccessExpressionOrchestrator<T>: NodeSpecificOrchestrator<T, T> where T : ExpressionSyntax
{
    protected override MutationContext PrepareContext(T node, MutationContext context) => context.EnterMemberAccess();
    protected override void RestoreContext(MutationContext context) => context.LeaveMemberAccess();

    protected override T InjectMutations(T sourceNode, T targetNode, SemanticModel semanticModel, MutationContext context) => (T) context.InjectExpressionLevel(targetNode, sourceNode);

    protected override MutationContext StoreMutations(T node, IEnumerable<Mutant> mutations, MutationContext context)
    {
        context.AddExpressionLevel(mutations);  
        return context;
    }   
}
