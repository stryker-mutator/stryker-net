using System;
using System.Collections.Generic;
using Stryker.Abstractions.TestRunners;
using Stryker.Abstractions.TestRunners.VsTest;

namespace Stryker.Abstractions.TestRunners;

public interface ITestRunResult
{
    TimeSpan Duration { get; }
    ITestGuids ExecutedTests { get; }
    ITestGuids FailingTests { get; }
    IEnumerable<string> Messages { get; }
    string ResultMessage { get; }
    bool SessionTimedOut { get; }
    ITestGuids TimedOutTests { get; }
    IEnumerable<VsTestDescription> VsTestDescriptions { get; }
}
