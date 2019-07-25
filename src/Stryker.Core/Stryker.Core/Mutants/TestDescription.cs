using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Stryker.Core.Mutants
{
    public class TestDescription
    {
        protected bool Equals(TestDescription other)
        {
            return string.Equals(Guid, other.Guid);
        }

        public static implicit operator TestDescription(TestCase test)
        {
            return new TestDescription(test.Id.ToString(), test.FullyQualifiedName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestDescription) obj);
        }

        public override int GetHashCode()
        {
            return (Guid != null ? Guid.GetHashCode() : 0);
        }

        public TestDescription(string guid, string name)
        {
            Guid = guid;
            Name = name;
        }

        public string Guid { get; set; }

        public string Name { get; set; }
    }
}
