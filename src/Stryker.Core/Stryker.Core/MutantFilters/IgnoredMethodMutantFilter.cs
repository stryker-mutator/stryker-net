using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
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
        public MutantFilter Type => MutantFilter.IgnoreMethod;
        public string DisplayName => "method filter";
        private readonly SyntaxTriviaRemover _triviaRemover = new();

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, IReadOnlyFileLeaf file, StrykerOptions options) =>
            options.IgnoredMethods.Any() ?
                    mutants.Where(m => !IsPartOfIgnoredMethodCall(m.Mutation.OriginalNode, options)) :
                    mutants;

        private bool IsPartOfIgnoredMethodCall(SyntaxNode syntaxNode, StrykerOptions options, bool canGoUp = true) =>
            syntaxNode switch
            {
                // Check if the current node is an invocation. This will also ignore invokable properties like `Func<bool> MyProp { get;}`
                // follow the invocation chain to see if it ends with a filtered one
                InvocationExpressionSyntax invocation => MatchesAnIgnoredMethod(_triviaRemover.Visit(invocation.Expression).ToString(), options)
                    || (invocation.Parent is MemberAccessExpressionSyntax && invocation.Parent.Parent is InvocationExpressionSyntax &&
                    IsPartOfIgnoredMethodCall(invocation.Parent.Parent, options, false)),

                // Check if the current node is an object creation syntax (constructor invocation).
                ObjectCreationExpressionSyntax creation => MatchesAnIgnoredMethod(_triviaRemover.Visit(creation.Type) + ".ctor", options),

                ConditionalAccessExpressionSyntax conditional => IsPartOfIgnoredMethodCall(conditional.WhenNotNull, options, false),

                ConditionalExpressionSyntax conditionalExpression => IsPartOfIgnoredMethodCall(conditionalExpression.WhenTrue, options, false) && IsPartOfIgnoredMethodCall(conditionalExpression.WhenFalse, options, false),

                ExpressionStatementSyntax expressionStatement => IsPartOfIgnoredMethodCall(expressionStatement.Expression, options, false),

                AssignmentExpressionSyntax assignmentExpression =>  IsPartOfIgnoredMethodCall(assignmentExpression.Right, options, false),

                LocalDeclarationStatementSyntax localDeclaration => localDeclaration.Declaration.Variables.All(v => IsPartOfIgnoredMethodCall(v.Initializer?.Value, options, false)),
  
                BlockSyntax { Statements.Count: >0 } block => block.Statements.All(s=> IsPartOfIgnoredMethodCall(s, options, false)),

                // Traverse the tree upwards.
                { Parent: { } }  => canGoUp && IsPartOfIgnoredMethodCall(syntaxNode.Parent, options),
                _ => false,
            };

        private static bool MatchesAnIgnoredMethod(string expressionString, StrykerOptions options) => options.IgnoredMethods.Any(r => r.IsMatch(expressionString));

        /// <summary>
        /// Removes comments, whitespace, and other junk from a syntax tree.
        /// </summary>
        private sealed class SyntaxTriviaRemover : CSharpSyntaxRewriter
        {
            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia) => default;
        }
    }
}
