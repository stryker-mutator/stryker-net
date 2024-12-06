using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions.ProjectComponents;

public interface ITestCase
{
    Testing.Identifier Id { get; init; }
    string Name { get; init; }
    SyntaxNode Node { get; init; }

    bool Equals(object obj);
    bool Equals(ITestCase other);
    int GetHashCode();
}
