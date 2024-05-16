using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Shared.Mutants;
using Stryker.Shared.Mutators;
using Stryker.Shared.Options;

namespace Stryker.Core.MutantFilters;

/// <summary>
/// Checks if the linq expression of the mutant should be excluded.
/// </summary>
/// <seealso cref="IMutantFilter" />
public class ExcludeLinqExpressionFilter : IMutantFilter
{
    public MutantFilter Type => MutantFilter.IgnoreLinqMutation;
    public string DisplayName => "linq expression filter";
    private SyntaxTriviaRemover _triviaRemover { get; init; } = new SyntaxTriviaRemover();

    public IEnumerable<IMutant> FilterMutants(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file, IStrykerOptions options) => options.ExcludedLinqExpressions.Any() ?
                mutants.Where(m => m.Mutation.Type != Mutator.Linq || !IsIgnoreExpression(m.Mutation.OriginalNode, options)) :
                mutants;

    private bool IsIgnoreExpression(SyntaxNode syntaxNode, IStrykerOptions options) =>
        syntaxNode switch
        {
            // Check if the current node is an invocation. This will also ignore invokable properties like `Func<bool> MyProp { get;}`
            InvocationExpressionSyntax invocation => MatchesAnIgnoredExpression(_triviaRemover.Visit(invocation.Expression).ToString(), options),

            // Check if the current node is an object creation syntax (constructor invocation).
            ObjectCreationExpressionSyntax creation => MatchesAnIgnoredExpression(_triviaRemover.Visit(creation.Type) + ".ctor", options),

            // Traverse the tree upwards.
            SyntaxNode node when node.Parent != null => IsIgnoreExpression(syntaxNode.Parent, options),
            _ => false,
        };


    private static bool MatchesAnIgnoredExpression(string expressionString, IStrykerOptions options) => options.ExcludedLinqExpressions.Any(r => expressionString.EndsWith(Enum.GetName(r)));

    /// <summary>
    /// Removes comments, whitespace, and other junk from a syntax tree.
    /// </summary>
    private sealed class SyntaxTriviaRemover : CSharpSyntaxRewriter
    {
        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia) => default;
    }
}
