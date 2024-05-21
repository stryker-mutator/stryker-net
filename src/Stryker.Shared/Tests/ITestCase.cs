namespace Stryker.Shared.Tests;
public interface ITestCase
{
    Guid Id { get; }

    string Name { get; }

    string Source { get; }

    string CodeFilePath { get; }

    string FullyQualifiedName { get; }

    Uri Uri { get; }

    int LineNumber { get; }
}
