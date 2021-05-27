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
    /// <seealso cref="IMutantFilter" />
    public class IgnoredMethodMutantFilter : IMutantFilter
    {
        public string DisplayName => "method filter";

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file, IStrykerOptions options)
        {
            if (!options.IgnoredMethods.Any())
            {
                return mutants;
            }

            return mutants.Where(m => !IsPartOfIgnoredMethodCall(m.Mutation.OriginalNode, options));
        }

        private bool IsPartOfIgnoredMethodCall(SyntaxNode syntaxNode, IStrykerOptions options)
        {
            switch (syntaxNode)
            {
                // Check if the current node is an invocation and the expression is a member
                // This will also ignore invokable properties like `Func<bool> MyProp { get;}`
                case InvocationExpressionSyntax invocation when invocation.Expression is MemberAccessExpressionSyntax member:
                    return options.IgnoredMethods.Any(r => r.IsMatch(member.Name.ToString()));
                // Check when conditional access
                case InvocationExpressionSyntax invocation when invocation.Expression is MemberBindingExpressionSyntax member:
                    return options.IgnoredMethods.Any(r => r.IsMatch(member.Name.ToString()));
                // Check when direct identifier
                case InvocationExpressionSyntax invocation when invocation.Expression is IdentifierNameSyntax member:
                    return options.IgnoredMethods.Any(r => r.IsMatch(member.ToString()));
                // Check if the current node is an object creation syntax (constructor invocation).
                case ObjectCreationExpressionSyntax creation:
                {
                    var methodName = creation.Type + ".ctor";
                    return options.IgnoredMethods.Any(r => r.IsMatch(methodName));
                }
            }

            // Traverse the tree upwards
            if (syntaxNode.Parent != null)
            {
                return IsPartOfIgnoredMethodCall(syntaxNode.Parent, options);
            }

            return false;
        }
    }
}
