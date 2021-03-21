using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{
    public interface ITestListDescription
    {
        HashSet<TestDescription> Tests { get; }
        int Count { get; }
        bool IsEmpty { get; }
        bool IsEveryTest { get; }
        ITestListDescription Add(TestDescription test);
        public TestListDescription Merge(ITestListDescription other);
        bool Contains(TestDescription test);
        bool IsIncluded(ITestListDescription test);
        bool ContainsAny(IReadOnlyList<TestDescription> usedTests);
        bool ContainsAny(ITestListDescription other);
        IReadOnlyList<TestDescription> GetList();
        public IEnumerable<Guid> GetGuids();
    }

    public class TestListDescription : ITestListDescription
    {
        public HashSet<TestDescription> Tests { get; }

        private static readonly ITestListDescription EveryTests;
        private static readonly ITestListDescription NoTestAtAll;

        static TestListDescription()
        {
            EveryTests = new TestListDescription(new[] { TestDescription.AllTests() });
            NoTestAtAll = new TestListDescription((IEnumerable<TestDescription>)null);
        }

        public TestListDescription()
        {
            Tests = null;
        }

        public TestListDescription(IEnumerable<TestDescription> testDescriptions)
        {
            Tests = testDescriptions == null ? new HashSet<TestDescription>() : testDescriptions.ToHashSet();
        }

        public bool IsEveryTest => Tests == null || (Tests.Count == 1 && Tests.First().IsAllTests);

        public bool IsEmpty => Tests != null && Tests.Count == 0;

        public int Count => Tests?.Count ?? 0;

        public ITestListDescription Add(TestDescription test)
        {
            if (Tests != null && IsEveryTest)
            {
                return this;
            }

            return new TestListDescription((Tests ?? new HashSet<TestDescription>()).Append(test));
        }

        public TestListDescription Merge(ITestListDescription other)
        {
            return new TestListDescription(Tests.Union(Tests));
        }

        public bool Contains(TestDescription test)
        {
            return IsEveryTest || Tests.Contains(test);
        }

        public bool IsIncluded(ITestListDescription test)
        {
            return test.IsEveryTest || Tests.All(test.Contains);
        }

        public static ITestListDescription EveryTest()
        {
            return EveryTests;
        }

        public static ITestListDescription NoTest()
        {
            return NoTestAtAll;
        }

        public IReadOnlyList<TestDescription> GetList()
        {
            return Tests.ToList();
        }

        public IEnumerable<Guid> GetGuids()
        {
            return Tests.Select(t => t.Guid);
        }

        public bool ContainsAny(ITestListDescription other)
        {
            return (IsEveryTest && other.Count>0) || Tests?.Any(other.Contains) == true;
        }

        public bool ContainsAny(IReadOnlyList<TestDescription> usedTests)
        {
            return Tests != null && usedTests.Any(Tests.Contains);
        }
    }
}
