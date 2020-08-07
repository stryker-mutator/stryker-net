using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Mutants
{
    public interface ITestListDescription
    {
        List<TestDescription> Tests { get; }
        int Count { get; }
        bool IsEmpty { get; }
        bool IsEveryTest { get; }

        void Add(TestDescription test);
        void AddTests(ITestListDescription otherFailingTests);
        bool Contains(string id);
        bool Contains(TestDescription test);
        bool ContainsAny(IReadOnlyList<TestDescription> usedTests);
        bool ContainsAny(ITestListDescription other);
        IReadOnlyList<TestDescription> GetList();
    }

    public class TestListDescription : ITestListDescription
    {

        public List<TestDescription> Tests { get; private set; }

        private static readonly ITestListDescription EveryTests;
        private static readonly ITestListDescription NoTestInstance;

        static TestListDescription()
        {
            EveryTests = new TestListDescription(new[] { TestDescription.AllTests() });
            NoTestInstance = new TestListDescription(null);
        }

        public TestListDescription()
        {
            Tests = null;
        }

        public TestListDescription(IEnumerable<TestDescription> testDescriptions)
        {
            Tests = testDescriptions == null ? new List<TestDescription>() : testDescriptions.ToList();
        }

        public bool IsEveryTest => Tests == null || (Tests.Count == 1 && Tests[0].IsAllTests);

        public bool IsEmpty => Tests != null && Tests.Count == 0;

        public int Count => Tests?.Count ?? 0;

        public void Add(TestDescription test)
        {
            if (Tests != null && IsEveryTest)
            {
                return;
            }
            if (Tests == null || test.IsAllTests)
            {
                Tests = new List<TestDescription>(1);
            }
            Tests.Add(test);
        }

        public bool Contains(string id)
        {
            return IsEveryTest || Tests.Any(t => t.Guid == id);
        }

        public bool Contains(TestDescription test)
        {
            return IsEveryTest || Tests.Any(t => t.Equals(test));
        }

        public static ITestListDescription EveryTest()
        {
            return EveryTests;
        }

        public static ITestListDescription NoTest()
        {
            return NoTestInstance;
        }

        public IReadOnlyList<TestDescription> GetList()
        {
            return Tests;
        }

        public void AddTests(ITestListDescription otherFailingTests)
        {
            if (otherFailingTests.IsEmpty)
                return;
            foreach (var testDescription in otherFailingTests.Tests)
            {
                Add(testDescription);
            }
        }

        public bool ContainsAny(ITestListDescription other)
        {
            return Tests?.Any(other.Contains) == true;
        }

        public bool ContainsAny(IReadOnlyList<TestDescription> usedTests)
        {
            return Tests?.Any(usedTests.Contains) == true;
        }
    }
}