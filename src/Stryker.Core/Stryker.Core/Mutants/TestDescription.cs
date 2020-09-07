﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Stryker.Core.Mutants
{
    public sealed class TestDescription
    {
        private static readonly TestDescription AllTestsDescription;
        private static readonly string AllTestsGuid = "-1";

        public TestDescription(string guid, string name, string testfilePath)
        {
            Guid = guid;
            Name = name;
            TestfilePath = testfilePath;
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

        public string Guid { get; }

        public string Name { get; }

        public bool IsAllTests => Guid == AllTestsGuid;

        public string TestfilePath { get; }



        private bool Equals(TestDescription other)
        {
            return string.Equals(Guid, other.Guid) || IsAllTests || other.IsAllTests;
        }

        public static implicit operator TestDescription(TestCase test)
        {
            return new TestDescription(test.Id.ToString(), test.FullyQualifiedName, test.CodeFilePath);
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
}
