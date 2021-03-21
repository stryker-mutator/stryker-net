using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners.VsTest
{

    public class VsTestDescription
    {
        private readonly TestCase _case;
        private int _subCasesCount = 0;
        private readonly ICollection<TestResult> _initialResults = new List<TestResult>();

        static public implicit operator TestDescription(VsTestDescription test)
        {
            return test._case;
        }

        public VsTestDescription(TestCase @case) => _case = @case;

        public TestFramework Framework
        {
            get
            {
                if (_case.ExecutorUri.AbsoluteUri.Contains("nunit"))
                {
                    return TestFramework.nUnit;
                }
                if (_case.Properties.Any(p => p.Id == "XunitTestCase"))
                {
                    return TestFramework.xUnit;
                }

                return TestFramework.msTest;
            }
        }

        public TimeSpan InitialRunTime
        {
            get
            {
                return _initialResults.Aggregate(new TimeSpan(), (current, result) => current + result.Duration);
            }
        }

        public Guid Id => _case.Id;

        public TestCase Case => _case;

        public void AddSubCase()
        {
            _subCasesCount++;
        }

        public void RegisterTestResult(TestResult result)
        {
            _initialResults.Add(result);
        }

        protected bool Equals(VsTestDescription other) => Equals(_case.Id, other._case.Id);

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((VsTestDescription) obj);
        }

        public override int GetHashCode() => (_case != null ? _case.Id.GetHashCode() : 0);

        public override string ToString() => _case.FullyQualifiedName;
    }
}
