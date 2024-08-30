using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions.ProjectComponents;

public interface ITestFile
{
    string FilePath { get; init; }
    string Source { get; init; }
    SyntaxTree SyntaxTree { get; init; }
    IEnumerable<ITestCase> Tests { get; }

    void AddTest(Guid id, string name, SyntaxNode node);
    bool Equals(object obj);
    bool Equals(ITestFile other);
    int GetHashCode();
}
