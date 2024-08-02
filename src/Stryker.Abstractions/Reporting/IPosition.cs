namespace Stryker.Configuration.Reporting;

public interface IPosition
{
    int Column { get; set; }
    int Line { get; set; }
}
