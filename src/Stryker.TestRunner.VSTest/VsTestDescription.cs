using Stryker.Shared.Tests;

namespace Stryker.TestRunner.VSTest;

public sealed class VsTestDescription : IFrameworkTestDescription
{
    private readonly ICollection<ITestResult> _initialResults = new List<ITestResult>();

    private int _subCases;

    public VsTestDescription(ITestCase testCase)
    {
        Case = testCase;
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

    public Identifier Id => Case.Id;

    public ITestCase Case { get; }

    public int NbSubCases => Math.Max(_subCases, _initialResults.Count);

    public void RegisterInitialTestResult(ITestResult result) => _initialResults.Add(result);

    public void AddSubCase() => _subCases++;

    public void ClearInitialResult() => _initialResults.Clear();
}
