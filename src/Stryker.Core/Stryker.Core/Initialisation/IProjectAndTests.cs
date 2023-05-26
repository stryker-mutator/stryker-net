using System.Collections.Generic;

namespace Stryker.Core.Initialisation;

public interface IProjectAndTests
{
    bool IsFullFramework { get; }

    string HelperNamespace { get; }

    IReadOnlyList<string> GetTestAssemblies();
}
