using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Stryker.Core.Mutants
{
    public sealed class TestDescription
    {
        private static readonly TestDescription AllTestsDescription;
        private const string AllTestsGuid = "-1";

        public TestDescription(string guid, string name)
        {
            Guid = guid;
            Name = name;
        }

        static TestDescription()
        {
            AllTestsDescription = new TestDescription(AllTestsGuid, "All Tests");
        }

        /// <summary>
        /// Returns an 'all tests' description for test runners that does not support test selection.
        /// </summary>
        public static TestDescription AllTests()
        {
            return AllTestsDescription;
        }

        public string Guid { get; }

        public string Name { get; }

        public bool IsAllTests => Guid == AllTestsGuid;

        private bool Equals(TestDescription other)
        {
            return string.Equals(Guid, other.Guid) || IsAllTests || other.IsAllTests;
        }

        public static implicit operator TestDescription(TestCase test)
        {
            return new TestDescription(test.Id.ToString(), test.FullyQualifiedName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((TestDescription) obj);
        }

        public override int GetHashCode()
        {
            return (Guid != null ? Guid.GetHashCode() : 0);
        }
    }

    public class TestListDescription
    {
        private List<TestDescription> _tests;
        private static readonly TestListDescription EveryTests;
        private static readonly TestListDescription NoTestInstance;

        static TestListDescription()
        {
            EveryTests = new TestListDescription(new []{TestDescription.AllTests()});
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
            if (_tests!=null && IsEveryTest)
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

        public static TestListDescription EveryTest()
        {
            return EveryTests;
        }

        public static TestListDescription NoTest()
        {
            return NoTestInstance;
        }

        public IReadOnlyList<TestDescription> GetList()
        {
            return _tests;
        }

        public void AddTests(TestListDescription otherFailingTests)
        {
            if (otherFailingTests.IsEmpty)
                return;
            foreach (var testDescription in otherFailingTests._tests)
            {
                Add(testDescription);
            }
        }

        public bool ContainsAny(TestListDescription other)
        {
            return _tests?.Any(other.Contains) == true;
        }

        public bool ContainsAny(IReadOnlyList<TestDescription> usedTests)
        {   
            return _tests?.Any(usedTests.Contains) == true;
        }
    }
}
