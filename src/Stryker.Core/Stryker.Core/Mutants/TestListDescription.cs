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

        public List<TestDescription> Tests { get => _tests }
        private List<TestDescription> _tests;
        private static readonly ITestListDescription EveryTests;
        private static readonly ITestListDescription NoTestInstance;

        static TestListDescription()
        {
            EveryTests = new TestListDescription(new[] { TestDescription.AllTests() });
            NoTestInstance = new TestListDescription(null);
        }

        public TestListDescription()
        {
            _tests = null;
        }

        public TestListDescription(IEnumerable<TestDescription> testDescriptions)
        {
            _tests = testDescriptions == null ? new List<TestDescription>() : testDescriptions.ToList();
        }

        public bool IsEveryTest => _tests == null || (_tests.Count == 1 && _tests[0].IsAllTests);

        public bool IsEmpty => _tests != null && _tests.Count == 0;

        public int Count => _tests?.Count ?? 0;

        public void Add(TestDescription test)
        {
            if (_tests != null && IsEveryTest)
            {
                return;
            }
            if (_tests == null || test.IsAllTests)
            {
                _tests = new List<TestDescription>(1);
            }
            _tests.Add(test);
        }

        public bool Contains(string id)
        {
            return IsEveryTest || _tests.Any(t => t.Guid == id);
        }

        public bool Contains(TestDescription test)
        {
            return IsEveryTest || _tests.Any(t => t.Equals(test));
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
            return _tests;
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
            return _tests?.Any(other.Contains) == true;
        }

        public bool ContainsAny(IReadOnlyList<TestDescription> usedTests)
        {
            return _tests?.Any(usedTests.Contains) == true;
        }
    }
}