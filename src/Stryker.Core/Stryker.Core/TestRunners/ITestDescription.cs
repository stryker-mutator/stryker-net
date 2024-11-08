namespace Stryker.Core.TestRunners;

public interface ITestDescription
{
    Identifier Id { get; }
    string Name { get; }
    string TestFilePath { get; }
}
