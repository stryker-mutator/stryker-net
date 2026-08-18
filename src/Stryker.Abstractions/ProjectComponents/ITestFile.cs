using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Stryker.Abstractions.Testing;

namespace Stryker.Abstractions.ProjectComponents;

public interface ITestFile
{
    string FilePath { get; init; }
    string Source { get; init; }
    SyntaxTree SyntaxTree { get; init; }
    IList<ITestCase> Tests { get; }

    /// <summary>
    /// Path relative to the root of the current analysis run (the solution directory when running in
    /// solution mode with multiple projects; otherwise the test project's own directory). Used as the
    /// report/baseline key, per the mutation-testing-report-schema.
    /// </summary>
    string RelativePath { get; init; }

    void AddTest(string id, string name, SyntaxNode node);
    bool Equals(object obj);
    bool Equals(ITestFile other);
    int GetHashCode();
}
