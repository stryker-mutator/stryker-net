using System;

namespace Stryker.Abstractions.Testing;

public interface ITestResult
{
    TimeSpan Duration { get; }
}
