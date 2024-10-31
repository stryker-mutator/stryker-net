using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Helpers;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
///     Orchestrate mutations for arguments (and sub expressions).
/// </summary>
internal class ArgumentSpecificOrchestrator : NodeSpecificOrchestrator<ArgumentSyntax, ArgumentSyntax>
{
    /// <inheritdoc />
    /// <remarks>Inject all pending mutations controlled with conditional operator(s).</remarks>
    protected override ArgumentSyntax InjectMutations(ArgumentSyntax sourceNode,    ArgumentSyntax  targetNode,
                                                      SemanticModel  semanticModel, MutationContext context) =>
        targetNode.WithExpression(context.InjectMutations(targetNode.Expression, sourceNode.Expression));


    protected override MutationContext StoreMutations(ArgumentSyntax      node,
                                                      IEnumerable<Mutant> mutations,
                                                      MutationContext     context) =>
        // if the expression contains a declaration, it must be controlled at the block level.
        context.AddMutations(mutations,
                             node.ContainsDeclarations() ? MutationControl.Block : MutationControl.Expression);

    protected override MutationContext PrepareContext(ArgumentSyntax node, MutationContext context) =>
        base.PrepareContext(node, context.Enter(MutationControl.Expression));

    protected override void RestoreContext(MutationContext context) => base.RestoreContext(context.Leave());
}
