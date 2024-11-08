using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.TestRunners.MsTest.Testing.Tests;

internal class MsTestDescription : IFrameworkTestDescription
{
    private readonly ICollection<ITestResult> _initialResults = [];

    private int _subCases;

    public MsTestDescription(ITestCase testCase)
    {
        Case = testCase;
        Description = new TestDescription(testCase.Id.ToString(), testCase.Name, testCase.CodeFilePath);
    }

    public TestFrameworks Framework =>
        Case.Uri.AbsolutePath.Contains("xunit") ? TestFrameworks.xUnit :
        Case.Uri.AbsolutePath.Contains("mstest") ? TestFrameworks.MsTest :
        TestFrameworks.NUnit;

    public ITestDescription Description { get; }

    public TimeSpan InitialRunTime => TimeSpan.FromTicks(_initialResults.Sum(t => t.Duration.Ticks));

    public Identifier Id => Case.Id;

    public int NbSubCases => Math.Max(_subCases, _initialResults.Count);
    public ITestCase Case { get; }

    public void AddSubCase() => _subCases++;

    public void ClearInitialResult() => _initialResults.Clear();

    public void RegisterInitialTestResult(ITestResult result) => _initialResults.Add(result);
}
