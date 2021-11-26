using System;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public class TestCase : IEquatable<TestCase>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Line { get; set; }
        public string Source { get; set; }

        public bool Equals(TestCase other) => other.Id.Equals(Id) && other.Name.Equals(Name) && other.Line.Equals(Line) && other.Source.Equals(Source);

        public override bool Equals(object other)
        {
            if (other is TestCase file)
            {
                return Equals(file);
            }
            return false;
        }

        public override int GetHashCode() => Id.GetHashCode() ^ Name.GetHashCode() ^ Line.GetHashCode() ^ Source?.GetHashCode() ?? 555;
    }
}
