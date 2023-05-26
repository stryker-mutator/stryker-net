
using System;

namespace Stryker.Core.Reporters.Json.TestFiles
{
    public sealed class Test : IEquatable<Test>
    {
        public string Id { get; }
        public string Name { get; set; }
        public Location Location { get; set; }

        public Test(string id) => Id = id;

        public bool Equals(Test other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Test)obj);
        }

        public override int GetHashCode() => Id != null ? Id.GetHashCode() : 0;

        public static bool operator ==(Test left, Test right) => Equals(left, right);

        public static bool operator !=(Test left, Test right) => !Equals(left, right);

    }
}
