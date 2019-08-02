using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    /// <summary>
    /// Checks if the mutants are part of ignored method calls.
    /// </summary>
    /// <seealso cref="Stryker.Core.MutantFilters.IMutantFilter" />
    public class IgnoredMethodMutantFilter : IMutantFilter
    {
        /// <inheritdoc />
        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            if (!options.IgnoredMethods.Any())
                return mutants;

            return mutants.Where(m => !IsPartOfIgnoredMethodCall(m.Mutation.OriginalNode, options));
        }

        /// <inheritdoc />
        public string DisplayName => "method filter";

        private bool IsPartOfIgnoredMethodCall(SyntaxNode syntaxNode, StrykerOptions options)
        {
            // Check if the current node is an invocation and the expression is a member
            // This will also ignore invokable properties like `Func<bool> MyProp { get;}`
            if (syntaxNode is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax member)
            {
                return options.IgnoredMethods.Any(r => r.IsMatch(member.Name.ToString()));
            }

            // Check if the current node is an object creation syntax (constructor invocation).
            if (syntaxNode is ObjectCreationExpressionSyntax creation)
            {
                var methodName = creation.Type + ".ctor";
                return options.IgnoredMethods.Any(r => r.IsMatch(methodName));
            }

            // Traverse the tree upwards
            if (syntaxNode.Parent != null)
                return IsPartOfIgnoredMethodCall(syntaxNode.Parent, options);

            return false;
        }
    }
}