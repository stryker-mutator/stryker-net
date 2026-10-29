using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.MutantFilters;

/// <summary>
/// Removes mutants whose only effect is on whether, or with what arguments, an ignored method is called.
/// Kept apart from <see cref="IgnoredMethodMutantFilter"/> so the report shows which mutants this heuristic removed.
/// </summary>
/// <seealso cref="IMutantFilter" />
public sealed class IgnoreMethodsEffectsMutantFilter : IMutantFilter
{
    public MutantFilter Type => MutantFilter.IgnoreMethodsEffects;

    public string DisplayName => "ignore-methods-effects filter";

    public IEnumerable<IMutant> FilterMutants(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file, IStrykerOptions options)
    {
        if (!options.IgnoreMethodsEffects || !options.IgnoredMethods.Any())
        {
            return mutants;
        }

        var onlyEffect = new IgnoredCallOnlyEffect(node => IsIgnoredCall(node, options));
        return mutants.Where(m => !onlyEffect.IsOnlyEffect(m.Mutation));
    }

    // Only the call itself: unlike IgnoredMethodMutantFilter, a receiver of a fluent chain that
    // ends in an ignored call is not treated as ignored, because it still runs.
    private static bool IsIgnoredCall(SyntaxNode syntaxNode, IStrykerOptions options) => syntaxNode switch
    {
        InvocationExpressionSyntax invocation => options.IgnoredMethods.Any(r => r.IsMatch(WithoutTrivia(invocation.Expression))),
        ConditionalAccessExpressionSyntax conditional => IsIgnoredCall(conditional.WhenNotNull, options),
        _ => false,
    };

    private static string WithoutTrivia(SyntaxNode node) => string.Concat(node.DescendantTokens().Select(token => token.Text));
}
