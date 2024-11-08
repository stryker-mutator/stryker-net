using System;

namespace Stryker.Core.TestRunners;

public interface ITestResult
{
    TimeSpan Duration { get; }
}
