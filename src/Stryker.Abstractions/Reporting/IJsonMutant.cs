using System.Collections.Generic;

namespace Stryker.Abstractions.Reporting;

public interface IJsonMutant
{
    IEnumerable<string> CoveredBy { get; set; }
    string Description { get; init; }
    int? Duration { get; set; }
    string Id { get; init; }
    IEnumerable<string> KilledBy { get; set; }
    ILocation Location { get; init; }
    string MutatorName { get; init; }
    string Replacement { get; init; }
    bool? Static { get; init; }
    string Status { get; init; }
    string StatusReason { get; init; }
    int? TestsCompleted { get; set; }
}
