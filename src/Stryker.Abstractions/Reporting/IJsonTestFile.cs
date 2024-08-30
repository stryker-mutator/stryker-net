using System.Collections.Generic;
using Stryker.Abstractions.ProjectComponents.TestProjects;

namespace Stryker.Abstractions.Reporting;

public interface IJsonTestFile
{
    string Language { get; init; }
    string Source { get; init; }
    ISet<IJsonTest> Tests { get; set; }

    void AddTestFile(TestFile testFile);
}
