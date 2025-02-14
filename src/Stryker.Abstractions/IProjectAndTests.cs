using System.Collections.Generic;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Abstractions;

public interface IProjectAndTests
{
    ITestProjectsInfo TestProjectsInfo { get; }

    string HelperNamespace { get; }

    IReadOnlyList<string> GetTestAssemblies();
}
