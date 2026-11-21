using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.VsTest;

public sealed class VsTestDescription : IFrameworkTestDescription
{
    private readonly ICollection<ITestResult> _initialResults = [];
    private readonly VsTestCase _case;
    private int _subCases;

    public VsTestDescription(VsTestCase testCase)
    {
        _case = testCase;
        Description = new TestDescription(testCase.Id, testCase.Name, testCase.CodeFilePath);
    }

    public TestFrameworks Framework
    {
        get
        {
            if (Case.Uri.AbsoluteUri.Contains("nunit"))
            {
                return TestFrameworks.NUnit;
            }
            return Case.Uri.AbsoluteUri.Contains("xunit") ? TestFrameworks.xUnit : TestFrameworks.MsTest;
        }
    }

    public ITestDescription Description { get; }

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

    public string Id => Case.Id;

    public ITestCase Case => _case;
    public int NbSubCases => Math.Max(_subCases, _initialResults.Count);
    public bool IsExplicit => _case.OriginalTestCase.Properties.Any(p => p.Id == "NUnit.Explicit" &&  _case.OriginalTestCase.GetPropertyValue(p) is true);
    public bool IsIgnored => _initialResults.Count == 0;
    public void RegisterInitialTestResult(ITestResult result) => _initialResults.Add(result);

    public void AddSubCase() => _subCases++;

    public void ClearInitialResult() => _initialResults.Clear();
}
