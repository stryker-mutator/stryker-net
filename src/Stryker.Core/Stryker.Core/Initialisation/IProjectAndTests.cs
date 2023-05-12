namespace Stryker.Core.Initialisation;
using System.Collections.Generic;

public interface IProjectAndTests
{
    bool IsFullFramework { get; }

    string HelperNamespace { get; }

    IReadOnlyList<string> GetTestAssemblies();
}
