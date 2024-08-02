using System.Collections.Generic;
using Stryker.Configuration.ProjectComponents.TestProjects;

namespace Stryker.Configuration.Initialisation;

public interface IProjectAndTests
{
    TestProjectsInfo TestProjectsInfo { get;}

    string HelperNamespace { get; }

    IReadOnlyList<string> GetTestAssemblies();
}
