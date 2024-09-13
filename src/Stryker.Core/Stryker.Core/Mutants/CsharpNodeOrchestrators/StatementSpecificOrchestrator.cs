using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// General handler for Statements. Remember to inherit from this class when you wand to create a statement specific logic.
/// </summary>
/// <typeparam name="T">Statement syntax type. Must inherit from <see cref="StatementSyntax"/></typeparam>
internal class StatementSpecificOrchestrator<T> : NodeSpecificOrchestrator<T, StatementSyntax> where T : StatementSyntax
{
    protected override MutationContext PrepareContext(T node, MutationContext context) => base.PrepareContext(node, context).Enter(MutationControl.Statement);

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());

    /// <inheritdoc/>
    /// <remarks>Inject pending mutations that are controlled with 'if' statements.</remarks>
    protected override StatementSyntax InjectMutations(T sourceNode, StatementSyntax targetNode, SemanticModel semanticModel, MutationContext context) =>
        context.InjectMutations(targetNode, sourceNode);

}
