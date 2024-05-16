using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Core.Mutants;

namespace Stryker.Core.TestRunners.VsTest
{
    public sealed class VsTestDescription
    {
        private readonly ICollection<TestResult> _initialResults = new List<TestResult>();
        private int _subCases;

        public VsTestDescription(TestCase testCase)
        {
            Case = testCase;
            Description = new TestDescription(testCase.Id, testCase.DisplayName, testCase.CodeFilePath);
        }

        public TestFrameworks Framework
        {
            get
            {
                if (Case.ExecutorUri.AbsoluteUri.Contains("nunit"))
                {
                    return TestFrameworks.NUnit;
                }
                return Case.ExecutorUri.AbsoluteUri.Contains("xunit") ? TestFrameworks.xUnit : TestFrameworks.MsTest;
            }
        }

        public TestDescription Description { get; }

        public TimeSpan InitialRunTime
        {
            get
            {
                if (Framework == TestFrameworks.xUnit)
                {
                    // xUnit returns the run time for the case within each result, so the first one is sufficient
                    return _initialResults.FirstOrDefault()?.Duration ?? TimeSpan.Zero;
                }

                return TimeSpan.FromTicks(_initialResults.Sum(t => t.Duration.Ticks));
            }
        }

        public Guid Id => Case.Id;

        public TestCase Case { get; }

        public int NbSubCases => Math.Max(_subCases, _initialResults.Count);

        public void RegisterInitialTestResult(TestResult result) => _initialResults.Add(result);

        public void AddSubCase() => _subCases++;

        public void ClearInitialResult() => _initialResults.Clear();
    }
}
