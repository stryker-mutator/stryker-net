using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions;
using Stryker.Abstractions.Reporting;
using Stryker.Core.DiffProviders;
using Location = Stryker.Core.Reporters.Json.Location;

namespace Stryker.Core.Baseline.Utils;

public class ContentMutantMatcher : IContentMutantMatcher
{
    public IEnumerable<IMutant> MatchByLocation(IEnumerable<IMutant> currentMutants, IJsonMutant baselineMutant, DiffResult diff)
    {
        if (!diff.TryMapLocation(baselineMutant.Location, out var newLocation))
        {
            // The baseline mutant's code changed (or was removed); its previous result cannot be reused.
            return [];
        }

        return currentMutants.Where(mutant =>
            mutant.Mutation.DisplayName == baselineMutant.MutatorName &&
            mutant.Mutation.ReplacementNode.ToString() == baselineMutant.Replacement &&
            newLocation.Equals(GetCurrentLocation(mutant)));
    }

    private static ILocation GetCurrentLocation(IMutant mutant) =>
        new Location(mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan());
}
