using System.Collections.Generic;
using Buildalyzer;

namespace Stryker.Abstractions.ProjectComponents;

public interface ITestProject
{
    IAnalyzerResult AnalyzerResult { get; }
    string ProjectFilePath { get; }
    IEnumerable<ITestFile> TestFiles { get; }

    bool Equals(object obj);
    bool Equals(ITestProject other);
    int GetHashCode();
}
