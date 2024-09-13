using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Stryker.Abstractions.ProjectComponents;

public interface ITestFile
{
    string FilePath { get; init; }
    string Source { get; init; }
    SyntaxTree SyntaxTree { get; init; }
    IList<ITestCase> Tests { get; }

    void AddTest(Guid id, string name, SyntaxNode node);
    bool Equals(object obj);
    bool Equals(ITestFile other);
    int GetHashCode();
}
