namespace Stryker.Shared.Tests;
public interface ITestCase : IFormattable
{
    Guid Id { get; }

    string Name { get; }

    string CodeFilePath { get; }

    string FullyQualifiedName { get; }

    Uri Uri { get; }

    int LineNumber { get; }
}
