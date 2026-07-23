using System;

namespace Stryker.Abstractions.Reporting;

public interface ILocation : IEquatable<ILocation>
{
    IPosition End { get; init; }
    IPosition Start { get; init; }
}
