using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    /// <summary>
    /// Checks if the mutants are part of ignored method calls.
    /// </summary>
    /// <seealso cref="IMutantFilter" />
    public sealed class IgnoredMethodMutantFilter : IMutantFilter
    {
        public MutantFilter Type => MutantFilter.IgnoreMethod;

        public string DisplayName => "method filter";

        private readonly SyntaxTriviaRemover _triviaRemover = new();

        public IEnumerable<IMutant> FilterMutants(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file, IStrykerOptions options) =>
            options.IgnoredMethods.Any() ?
                    mutants.Where(m => !IsPartOfIgnoredMethodCall(m.Mutation.OriginalNode, options)) :
                    mutants;

        private bool IsPartOfIgnoredMethodCall(SyntaxNode syntaxNode, IStrykerOptions options, bool canGoUp = true) =>
            syntaxNode switch
            {
                // Check if the current node is an invocation. This will also ignore invokable properties like `Func<bool> MyProp { get;}`
                // follow the invocation chain to see if it ends with a filtered one
                InvocationExpressionSyntax invocation => MatchesAnIgnoredMethod(_triviaRemover.Visit(invocation.Expression).ToString(), options)
                    || invocation.Parent is MemberAccessExpressionSyntax && invocation.Parent.Parent is InvocationExpressionSyntax &&
                    IsPartOfIgnoredMethodCall(invocation.Parent.Parent, options, false) || canGoUp && IsPartOfIgnoredMethodCall(invocation.Parent, options),

                // Check if the current node is an object creation syntax (constructor invocation).
                ObjectCreationExpressionSyntax creation => MatchesAnIgnoredMethod(_triviaRemover.Visit(creation.Type) + ".ctor", options),

                ConditionalAccessExpressionSyntax conditional => IsPartOfIgnoredMethodCall(conditional.WhenNotNull, options, false),

                ConditionalExpressionSyntax conditionalExpression => IsPartOfIgnoredMethodCall(conditionalExpression.WhenTrue, options, false) && IsPartOfIgnoredMethodCall(conditionalExpression.WhenFalse, options, false)
                                                                    || canGoUp && IsPartOfIgnoredMethodCall(conditionalExpression.Parent, options),

                ExpressionStatementSyntax expressionStatement => IsPartOfIgnoredMethodCall(expressionStatement.Expression, options, false),

                AssignmentExpressionSyntax assignmentExpression => IsPartOfIgnoredMethodCall(assignmentExpression.Right, options, false),

                LocalDeclarationStatementSyntax localDeclaration => localDeclaration.Declaration.Variables.All(v => IsPartOfIgnoredMethodCall(v.Initializer?.Value, options, false)),

                BlockSyntax { Statements.Count: > 0 } block => block.Statements.All(s => IsPartOfIgnoredMethodCall(s, options, false)),


                MemberDeclarationSyntax => false,

                // Traverse the tree upwards.
                { Parent: not null } => canGoUp && IsPartOfIgnoredMethodCall(syntaxNode.Parent, options),
                _ => false,
            };

        private static bool MatchesAnIgnoredMethod(string expressionString, IStrykerOptions options) => options.IgnoredMethods.Any(r => r.IsMatch(expressionString));

        /// <summary>
        /// Removes comments, whitespace, and other junk from a syntax tree.
        /// </summary>
        private sealed class SyntaxTriviaRemover : CSharpSyntaxRewriter
        {
            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia) => default;
        }
    }
}
