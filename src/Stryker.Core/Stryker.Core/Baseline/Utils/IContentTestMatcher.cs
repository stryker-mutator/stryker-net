using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.DiffProviders;

namespace Stryker.Core.Baseline.Utils;

public interface IContentTestMatcher
{
    /// <summary>
    /// Determines whether <paramref name="baselineTest"/> (a test from the baseline report) is still
    /// unchanged: its remapped location (through <paramref name="diff"/>) must match exactly the
    /// current location of <paramref name="currentTest"/> (the same test, found by id).
    /// </summary>
    bool IsTestUnchanged(IJsonTest baselineTest, ITestCase currentTest, DiffResult diff);
}
