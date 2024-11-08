using System.Collections.Generic;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Stryker.Core.TestRunners.MsTest.Testing.Tests;

internal class TestCoverageInfo
{
    private readonly IList<int> _coveredMutations;
    private readonly IList<int> _coveredStaticMutations;
    private readonly IList<int> _leakedMutationsFromPreviousTest;

    public TestCoverageInfo(TestNode testNode, IList<int> coveredMutations, IList<int> coveredStaticMutations, IList<int> leakedMutationsFromPreviousTest)
    {
        TestNode = testNode;
        _coveredMutations = coveredMutations;
        _coveredStaticMutations = coveredStaticMutations;
        _leakedMutationsFromPreviousTest = leakedMutationsFromPreviousTest;
    }

    public TestNode TestNode { get; }

    public IList<int> CoveredMutations => [.. _coveredMutations];
    public IList<int> CoveredStaticMutations => [.. _coveredStaticMutations];
    public IList<int> LeakedMutations => [.. _leakedMutationsFromPreviousTest];

    public bool HasLeakedMutations => LeakedMutations.Count > 0;
}
