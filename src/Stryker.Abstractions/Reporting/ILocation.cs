namespace Stryker.Configuration.Reporting;

public interface ILocation
{
    IPosition End { get; init; }
    IPosition Start { get; init; }
}
