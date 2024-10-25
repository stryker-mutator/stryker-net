using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using Stryker.Abstractions;
using Stryker.Abstractions.Reporting;
using System;

namespace Stryker.Core.Reporters.Json.SourceFiles
{
    public class JsonMutant : IJsonMutant
    {
        public string Id { get; init; }
        public string MutatorName { get; init; }
        public string Description { get; init; }

        public string Replacement { get; init; }
        public ILocation Location { get; init; }

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

    public class JsonMutantConverter : JsonConverter<IJsonMutant>
    {
        public override IJsonMutant Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Deserialize the JSON into the concrete type
            var sourceFile = JsonSerializer.Deserialize<JsonMutant>(ref reader, options);
            return sourceFile;
        }

        public override void Write(Utf8JsonWriter writer, IJsonMutant value, JsonSerializerOptions options)
        {
            // Serialize the concrete type
            JsonSerializer.Serialize(writer, (JsonMutant)value, options);
        }
    }
}
