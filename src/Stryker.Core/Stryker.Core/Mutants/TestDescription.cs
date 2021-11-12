using System;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// Provide a simple test description (for logging purpose)
    /// </summary>
    public sealed class TestDescription
    {
        public TestDescription(Guid id, string name, string testFilePath)
        {
            Id = id;
            Name = name;
            TestFilePath = testFilePath;
        }

        /// <summary>
        /// Gets the unique test identifier.
        /// </summary>
        /// <remarks>This ID is provided by VsTest and is based on the test method signature</remarks>
        public Guid Id { get; }

        /// <summary>
        /// Gets the display name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the test source file path.
        /// </summary>
        public string TestFilePath { get; }

        private bool Equals(TestDescription other) => Id == other.Id;

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((TestDescription) obj);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
