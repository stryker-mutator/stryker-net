using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Stryker.Abstractions.Reporting;

public interface ISourceFile
{
    string Language { get; init; }
    ISet<IJsonMutant> Mutants { get; init; }
    string Source { get; init; }
}
