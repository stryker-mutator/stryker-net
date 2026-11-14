using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Tests;

namespace Stryker.Core.UnitTest.Testing;

[TestClass]
public class TestIdentifierListTests
{
    private static readonly string[][] Populations = [[], ["a"], ["b"], ["a", "b"], null];

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void ShouldPreserveMembershipAndSetOperations(bool leftWrapped, bool rightWrapped)
    {
        foreach (var left in Populations)
        foreach (var right in Populations)
        {
            var a = Create(left, leftWrapped);
            var b = Create(right, rightWrapped);
            foreach (var id in new[] { "a", "b", "c", "A" })
            {
                a.Contains(id).ShouldBe(left is null || left.Contains(id));
            }

            var intersection = left is null ? right : right is null ? left : left.Intersect(right).ToArray();
            var union = left is null || right is null ? null : left.Union(right).ToArray();
            AssertPopulation(a.Intersect(b), intersection);
            AssertPopulation(a.Merge(b), union);
            AssertPopulation(WrappedIdentifierEnumeration.MergeList(a, b), union);
            a.ContainsAny(b).ShouldBe(intersection is null || intersection.Length > 0);
            a.IsIncludedIn(b).ShouldBe(right is null || left is not null && !left.Except(right).Any());
        }
    }

    [TestMethod]
    public void ShouldExcludeRepresentableSets()
    {
        foreach (var left in Populations)
        foreach (var right in Populations)
        {
            var a = Create(left, false);
            var b = Create(right, false);
            if (left is null && right is { Length: > 0 })
            {
                Should.Throw<InvalidOperationException>(() => a.Excluding(b)).Message.ShouldBe("Can't exclude from EveryTest");
            }
            else
            {
                AssertPopulation(a.Excluding(b), right is null ? [] : left?.Except(right).ToArray());
            }
        }
    }

    private static ITestIdentifiers Create(string[] ids, bool wrapped) => ids is null
        ? TestIdentifierList.EveryTest()
        : wrapped ? new WrappedIdentifierEnumeration(ids) : new TestIdentifierList(ids);

    private static void AssertPopulation(ITestIdentifiers actual, string[] expected)
    {
        actual.IsEveryTest.ShouldBe(expected is null);
        actual.IsEmpty.ShouldBe(expected is { Length: 0 });
        actual.Count.ShouldBe(expected?.Length ?? 0);
        actual.GetIdentifiers().OrderBy(id => id).ShouldBe((expected ?? []).OrderBy(id => id));
    }
}
