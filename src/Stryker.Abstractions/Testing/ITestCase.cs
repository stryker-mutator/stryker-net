using System;

namespace Stryker.Abstractions.Testing;

public interface ITestCase
{
    string Id { get; }

    Guid Guid { get; }

    string Name { get; }

    string Source { get; }

    string CodeFilePath { get; }

    string FullyQualifiedName { get; }

    Uri Uri { get; }

    int LineNumber { get; }
}
