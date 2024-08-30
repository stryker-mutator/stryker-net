namespace Stryker.Abstractions.Reporting;

public interface IJsonReport
{
    IDictionary<string, ISourceFile> Files { get; init; }
    string ProjectRoot { get; init; }
    string SchemaVersion { get; init; }
    IDictionary<string, IJsonTestFile> TestFiles { get; set; }
    IDictionary<string, int> Thresholds { get; init; }
}
