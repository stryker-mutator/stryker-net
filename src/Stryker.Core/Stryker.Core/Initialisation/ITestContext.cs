using System.Collections.Generic;
using Stryker.Core.Mutants;

namespace Stryker.Core.Initialisation;

public interface ITestContext
{
    /// <summary>
    ///     Test assemblies
    /// </summary>
    IEnumerable<string> TestSources { get; }

    /// <summary>
    ///     Tests (Stryker format)
    /// </summary>
    TestSet Tests { get; }
}
