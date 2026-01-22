using System;
using System.Collections.Generic;
using System.Linq;
using MsTestRunnerDemo.Models;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

public sealed class MtpTestDescription : IFrameworkTestDescription
{
    private readonly ICollection<ITestResult> _initialResults = [];
    private readonly TestNode _testNode;

    public MtpTestDescription(TestNode testNode)
    {
        _testNode = testNode;
        Description = new TestDescription(testNode.Uid, testNode.DisplayName, string.Empty);
        Case = new MtpTestCase(testNode);
    }

    public TestFrameworks Framework => TestFrameworks.MsTest;

    public ITestDescription Description { get; }

    public TimeSpan InitialRunTime => TimeSpan.FromTicks(_initialResults.Sum(t => t.Duration.Ticks));

    public string Id => _testNode.Uid;

    public ITestCase Case { get; }

    public int NbSubCases => Math.Max(1, _initialResults.Count);

    public void RegisterInitialTestResult(ITestResult result) => _initialResults.Add(result);

    public void AddSubCase()
    {
    }

    public void ClearInitialResult() => _initialResults.Clear();
}

