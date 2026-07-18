using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Core.DiffProviders;
using Location = Stryker.Core.Reporters.Json.Location;

namespace Stryker.Core.Baseline.Utils;

public class ContentTestMatcher : IContentTestMatcher
{
    public bool IsTestUnchanged(IJsonTest baselineTest, ITestCase currentTest, DiffResult diff)
    {
        if (baselineTest.Location?.End == null)
        {
            // Can't verify a test whose extent isn't known; be conservative and treat it as changed.
            return false;
        }

        if (!diff.TryMapLocation(baselineTest.Location, out var remappedLocation))
        {
            return false;
        }

        var currentLocation = new Location(currentTest.Node.GetLocation().GetMappedLineSpan());
        return remappedLocation.Equals(currentLocation);
    }
}
