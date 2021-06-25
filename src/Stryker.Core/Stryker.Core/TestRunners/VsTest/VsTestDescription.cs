using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners.VsTest
{

    public class VsTestDescription
    {
        private readonly ICollection<TestResult> _initialResults = new List<TestResult>();
        private int _subCases = 0;

        public VsTestDescription(TestCase testCase)
        {
            Case = testCase;
            Description = new TestDescription(testCase.Id, testCase.DisplayName, testCase.CodeFilePath);
        }

        public TestFramework Framework
        {
            get
            {
                if (Case.ExecutorUri.AbsoluteUri.Contains("nunit"))
                {
                    return TestFramework.nUnit;
                }
                if (Case.Properties.Any(p => p.Id == "XunitTestCase"))
                {
                    return TestFramework.xUnit;
                }

                return TestFramework.msTest;
            }
        }

        public TestDescription Description { get; }

        public TimeSpan InitialRunTime => _initialResults.Aggregate(new TimeSpan(), (current, result) => current + result.Duration);

        public Guid Id => Case.Id;

        public TestCase Case { get; }

        public int NbSubCases => Math.Max(_subCases, _initialResults.Count);

        public void RegisterInitialTestResult(TestResult result)
        {
            _initialResults.Add(result);
        }

        public void AddSubCase()
        {
            _subCases++;
        }

        protected bool Equals(VsTestDescription other) => Equals(Case.Id, other.Case.Id);

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((VsTestDescription) obj);
        }

        public override int GetHashCode() => (Case != null ? Case.Id.GetHashCode() : 0);

        public override string ToString() => Case.FullyQualifiedName;
    }
}
