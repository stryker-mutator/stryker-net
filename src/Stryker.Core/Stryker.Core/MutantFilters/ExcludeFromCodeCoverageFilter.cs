using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.MutantFilters;

public class ExcludeFromCodeCoverageFilter : IMutantFilter
{
    public MutantFilter Type => MutantFilter.ExcludeFromCodeCoverage;
    public string DisplayName => "exclude from code coverage filter";

    public IEnumerable<IMutant> FilterMutants(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file, IStrykerOptions options)
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
            .Any(attr => Regex.IsMatch(attr.Name.ToString(),
                @"^(?:System\.Diagnostics\.CodeAnalysis\.)?ExcludeFromCodeCoverage(?:Attribute)?$"));
    }
}
