using System;

namespace Stryker.Abstractions.Testing;

public interface ITestCase
{
    // Sentinel value indicating a test framework did not report a line number for a test case.
    public const int LineNumberNotFound = -1;

    string Id { get; }

    Guid Guid { get; }

    string Name { get; }

    string Source { get; }

    string CodeFilePath { get; }

    string FullyQualifiedName { get; }

    Uri Uri { get; }

    int LineNumber { get; }
}
