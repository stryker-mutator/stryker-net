using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;

namespace Stryker.Core.Reporters.Json.SourceFiles
{
    public class JsonMutant
    {
        public string Id { get; init; }
        public string MutatorName { get; init; }
        public string Description { get; init; }

        public string Replacement { get; init; }
        public Location Location { get; init; }

        public string Status { get; init; }
        public string StatusReason { get; init; }

        public bool? Static { get; init; }

        public IEnumerable<string> CoveredBy { get; set; }
        public IEnumerable<string> KilledBy { get; set; }
        public int? TestsCompleted { get; set; }
        public int? Duration { get; set; }

        public JsonMutant() { }

        public JsonMutant(IReadOnlyMutant mutant)
        {
            Id = mutant.Id.ToString();
            MutatorName = mutant.Mutation.DisplayName;
            Description = mutant.Mutation.Description;

            Replacement = mutant.Mutation.ReplacementNode.ToString();
            Location = new Location(mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan());

            Status = mutant.ResultStatus.ToString();
            StatusReason = mutant.ResultStatusReason;

            Static = mutant.IsStaticValue;

            CoveredBy = mutant.CoveringTests.GetGuids()?.Select(g => g.ToString());
            KilledBy = mutant.KillingTests.GetGuids()?.Select(g => g.ToString());
        }
    }
}
