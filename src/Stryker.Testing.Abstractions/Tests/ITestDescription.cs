namespace Stryker.Shared.Tests;

public interface ITestDescription
{
    Guid Id { get; }
    string Name { get; }
    string TestFilePath { get; }
}
