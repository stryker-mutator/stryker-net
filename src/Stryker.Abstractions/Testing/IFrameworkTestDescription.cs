using System;

namespace Stryker.Abstractions.Testing;

public interface IFrameworkTestDescription
{
    TestFrameworks Framework { get; }

    ITestDescription Description { get; }

    TimeSpan InitialRunTime { get; }

    Identifier Id { get; }

    int NbSubCases { get; }

    ITestCase Case { get; }

    void RegisterInitialTestResult(ITestResult result);

    void AddSubCase();

    void ClearInitialResult();
}