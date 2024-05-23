namespace Stryker.Shared.Options;
public interface IThresholds
{
    int High { get; init; }
    int Low { get; init; }
    int Break { get; init; }
}
