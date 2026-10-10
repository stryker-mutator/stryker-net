using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Results;

namespace Stryker.Core.UnitTest.MutationTest;

[TestClass]
public class CoverageRunResultTests
{
    [TestMethod]
    public void Merge_ShouldSumMutationHitCounts()
    {
        var first = CoverageRunResult.Create("test", CoverageConfidence.Normal, [1], [], [],
            new Dictionary<int, int> { [1] = 4, [2] = 3 });
        var second = CoverageRunResult.Create("test", CoverageConfidence.Normal, [1], [], [],
            new Dictionary<int, int> { [1] = 6, [3] = 2 });

        first.Merge(second);

        first.MutationHitCounts.ShouldBe(new Dictionary<int, int> { [1] = 10, [2] = 3, [3] = 2 });
    }
}