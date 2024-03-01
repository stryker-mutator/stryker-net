using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Base class for node types (and their children) that are member definitions
/// </summary>
/// <typeparam name="T">Syntax node type. (not restricted to MemberDefinitionSyntax)</typeparam>
internal class MemberDefinitionOrchestrator<T>:NodeSpecificOrchestrator<T, T> where T : SyntaxNode
{
    protected override MutationContext PrepareContext(T node, MutationContext context) => base.PrepareContext(node, context.Enter(MutationControl.Member));

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());
}
