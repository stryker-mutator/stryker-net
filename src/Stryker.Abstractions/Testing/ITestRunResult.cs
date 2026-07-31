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

    /// <summary>
    /// True when the test run could not be completed because the test host process crashed
    /// (e.g. a mutation caused a fatal fault such as a stack overflow). The affected mutants
    /// cannot be conclusively tested and are reported as <see cref="MutantStatus.RuntimeError"/>.
    /// </summary>
    bool SessionHadRuntimeIssue { get; }
    ITestIdentifiers TimedOutTests { get; }
    IEnumerable<IFrameworkTestDescription> TestDescriptions { get; }
}
