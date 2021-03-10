using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Stryker.Core.Mutants
{
    public sealed class TestDescription
    {
        private static readonly TestDescription AllTestsDescription;
        private static readonly Guid AllTestsGuid = Guid.NewGuid();
        private readonly Guid _guid;

        public TestDescription(Guid guid, string name, string testFilePath)
        {
            _guid = guid;
            Name = name;
            TestFilePath = testFilePath;
        }

        static TestDescription()
        {
            AllTestsDescription = new TestDescription(AllTestsGuid, "All Tests", "");
        }

        /// <summary>
        /// Returns an 'all tests' description for test runners that does not support test selection.
        /// </summary>
        public static TestDescription AllTests()
        {
            return AllTestsDescription;
        }

        public Guid Guid => _guid;

        public string Name { get; }

        public bool IsAllTests => _guid == AllTestsGuid;

        public string TestFilePath { get; }

        private bool Equals(TestDescription other)
        {
            return _guid == other._guid || IsAllTests || other.IsAllTests;
        }

        public static implicit operator TestDescription(TestCase test)
        {
            return new TestDescription(test.Id, test.FullyQualifiedName, test.CodeFilePath);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((TestDescription) obj);
        }

        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
    }
}
