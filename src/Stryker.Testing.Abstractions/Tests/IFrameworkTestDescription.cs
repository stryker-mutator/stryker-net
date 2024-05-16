namespace Stryker.Shared.Tests;

public interface IFrameworkTestDescription
{
    TestFrameworks Framework { get; }

    ITestDescription Description { get; }

    TimeSpan InitialRunTime { get; }

    Guid Id { get; }

    int NbSubCases { get; }

    ITestCase Case { get; }

    void RegisterInitialTestResult(ITestResult result);

    void AddSubCase();

    void ClearInitialResult();
}
