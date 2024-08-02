namespace Stryker.Configuration.Options;

public interface IThresholds
{
    int Break { get; init; }
    int High { get; init; }
    int Low { get; init; }
}
