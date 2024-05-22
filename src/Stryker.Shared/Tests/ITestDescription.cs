namespace Stryker.Shared.Tests;

public interface ITestDescription
{
    Identifier Id { get; }
    string Name { get; }
    string TestFilePath { get; }
}
