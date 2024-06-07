using System;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Generic class for node types (and their children) that must not be mutated
/// </summary>
/// <typeparam name="T">SyntaxNode subtype</typeparam>
internal class DoNotMutateOrchestrator<T> : NodeSpecificOrchestrator<T, T> where T : SyntaxNode
{
    private readonly Predicate<T> _predicate;

    /// <inheritdoc />
    public override SyntaxNode Mutate(SyntaxNode node, SemanticModel semanticModel, MutationContext context) => node;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="predicate">predicate that signals when this orchestrator applies. default: apply to all node of proper type.</param>
    public DoNotMutateOrchestrator(Predicate<T> predicate = null) => _predicate = predicate ?? (_ => true);

    /// <inheritdoc />
    protected override bool CanHandle(T t) => _predicate(t);
}
