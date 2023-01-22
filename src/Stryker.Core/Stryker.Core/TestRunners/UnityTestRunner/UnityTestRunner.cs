using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.TestRunners.UnityTestRunner.RunUnity;

namespace Stryker.Core.TestRunners.UnityTestRunner;

public class UnityTestRunner : ITestRunner
{
    private readonly StrykerOptions _strykerOptions;
    private readonly ILogger _logger;
    private readonly IRunUnity _runUnity;
    private TestSet _testSet = null;
    private TestRunResult _initialRunTestResult;
    private bool _firstMutationTestStarted;

    public UnityTestRunner(StrykerOptions strykerOptions, ILogger logger,
        IRunUnity runUnity)
    {
        _strykerOptions = strykerOptions;
        _logger = logger;
        _runUnity = runUnity;
    }

    public TestSet DiscoverTests()
    {
        if (_testSet != null)
            return _testSet;

        var testResultsXml = RunTests(out var duration);

        //todo add valid test file path. It used for checking diff by git and ut of the box dont provides in xml
        _testSet = new TestSet();
        _testSet.RegisterTests(testResultsXml
            .Descendants("test-case")
            .Where(element => element.Attribute("result").Value is "Passed" or "Failed")
            .Select(element => new TestDescription(ToGuid(int.Parse(element.Attribute("id").Value)),
                element.Attribute("name").Value, element.Attribute("fullname").Value)));

        _initialRunTestResult = new TestRunResult(GetPassedTests(testResultsXml), GetFailedTests(testResultsXml),
            GetTimeoutTestGuidsList(), string.Empty, duration);

        return _testSet;
    }

    public TestRunResult InitialTest() => _initialRunTestResult;


    public IEnumerable<CoverageRunResult> CaptureCoverage()
    {
        throw new NotImplementedException();
    }

    public TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants,
        TestUpdateHandler update)
    {
        if (!_firstMutationTestStarted)
        {
            //rerun unity to apply modifications and reload domain
            _runUnity.ReloadDomain(_strykerOptions, _strykerOptions.WorkingDirectory);
        }

        var testResultsXml = RunTests(out var duration, mutants.Single().Id.ToString());

        var passedTests = GetPassedTests(testResultsXml);
        var failedTests = GetFailedTests(testResultsXml);
        var remainingMutants =
            update?.Invoke(mutants, failedTests, TestGuidsList.EveryTest(), GetTimeoutTestGuidsList());

        if (remainingMutants == false)
        {
            // all mutants status have been resolved, we can stop
            _logger.LogDebug("Each mutant's fate has been established, we can stop.");
        }

        _firstMutationTestStarted = true;

        return new TestRunResult(passedTests, failedTests, GetTimeoutTestGuidsList(), string.Empty, duration);
    }


    public void Dispose()
    {
        _runUnity.Dispose();
        //todo remove all modifications
        //todo remove installed package
    }

    private XDocument RunTests(out TimeSpan duration, string activeMutantId = null)
    {
        var startTime = DateTime.UtcNow;

        var xmlTestResults = _runUnity.RunTests(_strykerOptions, _strykerOptions.WorkingDirectory,
            activeMutantId: activeMutantId);

        duration = DateTime.UtcNow - startTime;
        return xmlTestResults;
    }

    private static TestGuidsList GetTimeoutTestGuidsList() =>
        //NUnit result has no result of time-out https://docs.nunit.org/articles/nunit/technical-notes/usage/Test-Result-XML-Format.html#test-case
        TestGuidsList.NoTest();

    private TestGuidsList GetPassedTests(XContainer testResultsXml)
    {
        var ids = testResultsXml.Descendants("test-case")
            .Where(element => element.Attribute("result").Value == "Passed")
            .Select(element => ToGuid(int.Parse(element.Attribute("id").Value)));
        var passedTests = ids.Count() == _testSet.Count
            ? TestGuidsList.EveryTest()
            : new TestGuidsList(_testSet.Extract(ids));

        return passedTests;
    }

    private TestGuidsList GetFailedTests(XContainer testResultsXml)
    {
        var ids = testResultsXml.Descendants("test-case")
            .Where(element => element.Attribute("result").Value == "Failed")
            .Select(element => ToGuid(int.Parse(element.Attribute("id").Value)));
        var failedTests = ids.Count() == _testSet.Count
            ? TestGuidsList.EveryTest()
            : new TestGuidsList(_testSet.Extract(ids));

        return failedTests;
    }

    private static Guid ToGuid(int value)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(value).CopyTo(bytes, 0);
        return new Guid(bytes);
    }
}
