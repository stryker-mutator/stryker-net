using System;

namespace Stryker.Core.Mutants
{
    public sealed class TestDescription
    {
        public TestDescription(Guid id, string name, string testFilePath)
        {
            Id = id;
            Name = name;
            TestFilePath = testFilePath;
        }

        public Guid Id { get; }

        public string Name { get; }

        public string TestFilePath { get; }

        private bool Equals(TestDescription other)
        {
            return Id == other.Id;
        }
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((TestDescription) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
