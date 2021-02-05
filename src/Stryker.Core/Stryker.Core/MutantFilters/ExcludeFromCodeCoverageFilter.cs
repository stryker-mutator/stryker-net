using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.Core.MutantFilters
{
    public class ExcludeFromCodeCoverageFilter : IMutantFilter
    {
        string IMutantFilter.DisplayName { get; } = "exclude from code coverage filter";

        public IEnumerable<Mutant> FilterMutants(IEnumerable<Mutant> mutants, ReadOnlyFileLeaf file, IStrykerOptions options)
        {
            return mutants.Where(m => !Exclude(m.Mutation.OriginalNode));
        }

        private static bool Exclude(SyntaxNode node)
        {
            return node != null && (HasExcludeFromCodeCoverageAttribute(node) || Exclude(node.Parent));
        }

        private static bool HasExcludeFromCodeCoverageAttribute(SyntaxNode node)
        {
            switch (node)
            {
                case MemberDeclarationSyntax m:
                    return HasExcludeFromCodeCoverageAttribute(m);
                default:
                    return false;
            }
        }

        private static bool HasExcludeFromCodeCoverageAttribute(MemberDeclarationSyntax m)
        {
            return m.AttributeLists
                .SelectMany(attr => attr.Attributes)
                .Any(attr => Regex.IsMatch(attr.ToString(),
                    @"^(?:System\.Diagnostics\.CodeAnalysis\.)?ExcludeFromCodeCoverage(?:Attribute)?$"));
        }
    }
}