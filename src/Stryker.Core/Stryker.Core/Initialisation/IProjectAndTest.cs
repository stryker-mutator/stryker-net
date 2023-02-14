using System.Collections.Generic;

namespace Stryker.Core.Initialisation;

public interface IProjectAndTest
{
    bool IsFullFramework { get; }

    string HelperNamespace { get; }

    IReadOnlyList<string> TestAssemblies { get; }
}
