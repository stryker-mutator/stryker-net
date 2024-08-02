using System.Collections.Generic;
using Stryker.Configuration.ProjectComponents.TestProjects;

namespace Stryker.Configuration.Reporting;

public interface IJsonTestFile
{
    string Language { get; init; }
    string Source { get; init; }
    ISet<IJsonTest> Tests { get; set; }

    void AddTestFile(TestFile testFile);
}
