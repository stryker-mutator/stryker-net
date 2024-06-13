using System.Collections.Generic;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.Initialisation;

public interface IProjectAndTests
{
    TestProjectsInfo TestProjectsInfo { get;}

    string HelperNamespace { get; }

    IReadOnlyList<string> GetTestAssemblies();
}
