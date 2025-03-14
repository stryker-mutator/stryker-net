using System;
using System.Collections.Generic;

namespace Stryker.Abstractions.Testing;

public interface ITestRunResult
{
    TimeSpan Duration { get; }
    ITestIdentifiers ExecutedTests { get; }
    ITestIdentifiers FailingTests { get; }
    IEnumerable<string> Messages { get; }
    string ResultMessage { get; }
    bool SessionTimedOut { get; }
    ITestIdentifiers TimedOutTests { get; }
    IEnumerable<IFrameworkTestDescription> TestDescriptions { get; }
}
