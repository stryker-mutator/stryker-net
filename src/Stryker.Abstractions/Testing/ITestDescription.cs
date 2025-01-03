namespace Stryker.Abstractions.Testing;

public interface ITestDescription
{
    Identifier Id { get; }
    string Name { get; }
    string TestFilePath { get; }
}
