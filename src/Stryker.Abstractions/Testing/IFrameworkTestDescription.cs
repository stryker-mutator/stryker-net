using System;

namespace Stryker.Abstractions.Testing;

public interface IFrameworkTestDescription
{
    TestFrameworks Framework { get; }

    ITestDescription Description { get; }

    TimeSpan InitialRunTime { get; }

    string Id { get; }

    int NbSubCases { get; }

    ITestCase Case { get; }

    void RegisterInitialTestResult(ITestResult result);

    void AddSubCase();

    void ClearInitialResult();
}
