using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.MutantFilters;

/// <summary>
/// Ignores the mutants inside the arguments of the calls named by ignore-method-arguments, and nothing else:
/// not the receiver of the call, the statement that holds it, nor the await around it.
/// </summary>
/// <remarks>
/// <see cref="IgnoredMethodMutantFilter"/> also ignores those, which suits sinks such as <c>Log*</c>,
/// but not a call such as <c>ConfigureAwait</c> whose receiver is the call that does the work.
/// </remarks>
/// <seealso cref="IMutantFilter" />
public sealed class IgnoredMethodArgumentMutantFilter : IMutantFilter
{
    private readonly SyntaxTriviaRemover _triviaRemover = new();

    public MutantFilter Type => MutantFilter.IgnoreMethodArgument;

    public string DisplayName => "method argument filter";

    public IEnumerable<IMutant> FilterMutants(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file, IStrykerOptions options) =>
        mutants.Where(m => !IsInIgnoredArgumentList(m.Mutation.OriginalNode, options));

    private bool IsInIgnoredArgumentList(SyntaxNode node, IStrykerOptions options) =>
        node.AncestorsAndSelf()
            .TakeWhile(n => n is not MemberDeclarationSyntax)
            .OfType<ArgumentListSyntax>()
            .Any(list => list.Parent is InvocationExpressionSyntax invocation
                         && options.IgnoredMethodArguments.Any(r =>
                             r.IsMatch(_triviaRemover.Visit(invocation.Expression).ToString())));

    /// <summary>
    /// Removes comments, whitespace, and other junk from a syntax tree.
    /// </summary>
    private sealed class SyntaxTriviaRemover : CSharpSyntaxRewriter
    {
        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia) => default;
    }
}
