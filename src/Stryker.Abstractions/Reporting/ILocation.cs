namespace Stryker.Abstractions.Reporting;

public interface ILocation
{
    IPosition End { get; init; }
    IPosition Start { get; init; }
}
