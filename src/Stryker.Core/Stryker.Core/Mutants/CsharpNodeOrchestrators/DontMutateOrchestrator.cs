using System;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

/// <summary>
/// Generic class for node types (and their children) that must not be mutated
/// </summary>
/// <typeparam name="T">SyntaxNode subtype</typeparam>
internal class DontMutateOrchestrator<T> : NodeSpecificOrchestrator<T, T> where T : SyntaxNode
{
    private readonly Predicate<T> _validator;

    /// <inheritdoc />
    public override SyntaxNode Mutate(SyntaxNode node, MutationContext context) => node;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="validator">predicate that signals when this orchestrator applies. default: apply to all node of proper type.</param>
    public DontMutateOrchestrator(Predicate<T> validator = null)
    {
        _validator = validator ?? (_ => true);
    }

    /// <inheritdoc />
    protected override bool CanHandle(T t) => _validator(t);
}
