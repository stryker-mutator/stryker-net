using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    public class IgnoredMethodMutantFilter : IMutantFilter
    {
        /// <inheritdoc />
        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, FileLeaf file, StrykerOptions options)
        {
            foreach (var mutant in mutants)
            {
                if (!IsPartOfIgnoredMethodCall(mutant.Mutation.OriginalNode, options))
                    yield return mutant;
            }
        }

        /// <inheritdoc />
        public string DisplayName => "method filter";

        private bool IsPartOfIgnoredMethodCall(SyntaxNode syntaxNode, StrykerOptions options)
        {
            // Check if the current node is an invocation and the expression is a member
            // This will also ignore invokable properties like `Func<bool> MyProp { get;}`
            if (syntaxNode is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax member)
            {
                return options.IgnoredMethods.Contains(member.Name.ToString());
            }

            // Traverse the tree upwards
            if (syntaxNode.Parent != null)
                return IsPartOfIgnoredMethodCall(syntaxNode.Parent, options);

            return false;
        }
    }
}