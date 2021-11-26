
using Stryker.Core.Mutants;

namespace Stryker.Core.Reporters.Json.SourceFiles
{
    public class JsonMutant
    {
        public string Id { get; init; }
        public string MutatorName { get; init; }
        public string Replacement { get; init; }
        public Location Location { get; init; }
        public string Status { get; init; }
        public string Description { get; init; }

        public JsonMutant() { }

        public JsonMutant(IReadOnlyMutant mutant)
        {
            Id = mutant.Id.ToString();
            MutatorName = mutant.Mutation.DisplayName;
            Replacement = mutant.Mutation.ReplacementNode.ToFullString();
            Location = new Location(mutant.Mutation.OriginalNode.GetLocation().GetMappedLineSpan());
            Status = mutant.ResultStatus.ToString();
            Description = mutant.Mutation.Description;
        }
    }
}
