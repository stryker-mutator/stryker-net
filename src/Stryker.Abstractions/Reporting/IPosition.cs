namespace Stryker.Abstractions.Reporting;

public interface IPosition
{
    int Column { get; set; }
    int Line { get; set; }
}
