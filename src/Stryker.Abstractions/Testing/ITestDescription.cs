namespace Stryker.Abstractions.Testing;

public interface ITestDescription
{
    string Id { get; }
    string Name { get; }
    string TestFilePath { get; }
}
