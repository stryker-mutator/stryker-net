using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Generic class to deal with syntax nodes which mutations must be injected at statement level 
/// </summary>
internal class MutateAtStatementLevelOrchestrator<T>: NodeSpecificOrchestrator<T, T> where T: SyntaxNode
{
    private readonly Predicate<T> _predicate;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="predicate">predicate that signals when this orchestrator applies. default: apply to all node of proper type.</param>
    public MutateAtStatementLevelOrchestrator(Predicate<T> predicate = null) => _predicate = predicate ?? (_ => true);

    /// <inheritdoc />
    protected override bool CanHandle(T t) => _predicate(t);

    protected override MutationContext StoreMutations(T node, IEnumerable<Mutant> mutations, MutationContext context) => context.AddMutations(mutations, MutationControl.Statement);
}
