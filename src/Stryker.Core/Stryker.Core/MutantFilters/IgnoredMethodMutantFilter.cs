using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    /// <summary>
    /// Checks if the mutants are part of ignored method calls.
    /// </summary>
    /// <seealso cref="IMutantFilter" />
    public sealed class IgnoredMethodMutantFilter : IMutantFilter
    {
        public string DisplayName => "method filter";
        private readonly SyntaxTriviaRemover _triviaRemover = new SyntaxTriviaRemover();

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file, IStrykerOptions options)
        {
            if (!options.IgnoredMethods.Any())
            {
                return mutants;
            }

            return mutants.Where(m => !IsPartOfIgnoredMethodCall(m.Mutation.OriginalNode, options));
        }

        private bool IsPartOfIgnoredMethodCall(SyntaxNode syntaxNode, IStrykerOptions options) =>
            syntaxNode switch
            {
                // Check if the current node is an invocation
                // This will also ignore invokable properties like `Func<bool> MyProp { get;}`
                InvocationExpressionSyntax invocation => MatchesAnIgnoredMethod(_triviaRemover.Visit(invocation.Expression).ToString(), options),
                ObjectCreationExpressionSyntax creation => MatchesAnIgnoredMethod(_triviaRemover.Visit(creation.Type) + ".ctor", options),
                SyntaxNode node when node.Parent != null => IsPartOfIgnoredMethodCall(syntaxNode.Parent, options),
                _ => false,
            };

        private static bool MatchesAnIgnoredMethod(string expressionString, IStrykerOptions options) => options.IgnoredMethods.Any(r => r.IsMatch(expressionString));

        private sealed class SyntaxTriviaRemover : CSharpSyntaxRewriter
        {
            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia) => default;
        }
    }
}
