using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.Mutants;
using Stryker.Abstractions.Options;

namespace Stryker.Core.MutantFilters;

public class IgnoreBlockMutantFilter : IMutantFilter
{
    private readonly HashSet<MutantStatus> _inactiveStatuses;

    public string DisplayName => "block already covered filter";

    public MutantFilter Type => MutantFilter.IgnoreBlockRemoval;

    public IgnoreBlockMutantFilter()
    {
        _inactiveStatuses = new HashSet<MutantStatus>
        {
            MutantStatus.Ignored,
            MutantStatus.CompileError,
        };
    }

    public IEnumerable<IMutant> FilterMutants(IEnumerable<IMutant> mutants, IReadOnlyFileLeaf file, IStrykerOptions options)
    {
        var blockMutants = mutants.Where(m => m.Mutation.Type == Mutator.Block);
        var mutantsToIgnore = blockMutants.Where(blockMutant => mutants.Any(
            m => m != blockMutant
            && !_inactiveStatuses.Contains(m.ResultStatus)
            && Contains(blockMutant.Mutation.Location, m.Mutation.Location)
        ));

        return mutants.Except(mutantsToIgnore);
    }

    private static bool Contains(MutationLocation outer, MutationLocation inner) =>
        outer.StartOffset <= inner.StartOffset && inner.EndOffset <= outer.EndOffset;
}
