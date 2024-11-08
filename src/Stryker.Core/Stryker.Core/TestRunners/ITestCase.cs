using System;

namespace Stryker.Core.TestRunners;

public interface ITestCase
{
    Identifier Id { get; }

    string Name { get; }

    string Source { get; }

    string CodeFilePath { get; }

    string FullyQualifiedName { get; }

    Uri Uri { get; }

    int LineNumber { get; }
}
