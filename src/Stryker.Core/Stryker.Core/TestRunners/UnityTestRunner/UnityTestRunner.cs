using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Reporters.Json.SourceFiles;
using Stryker.Core.Testing;

namespace Stryker.Core.TestRunners.UnityTestRunner;

public class UnityTestRunner : ITestRunner
{
    private const int TestFailedExitCode = 2;
    private readonly IProcessExecutor _processExecutor;
    private readonly StrykerOptions _strykerOptions;
    private readonly ILogger _logger;
    private readonly TestSet _testSet = new();
    private TestRunResult _initialRunTestResult;


    public UnityTestRunner(IProcessExecutor processExecutor, StrykerOptions strykerOptions, ILogger logger)
    {
        _processExecutor = processExecutor;
        _strykerOptions = strykerOptions;
        _logger = logger;
    }

    public TestSet DiscoverTests()
    {
        var testResultsXml = RunTests(out var duration);

        //todo add valid test file path. It used for checking diff by git and ut of the box dont provides in xml
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
        throw new System.NotImplementedException();
    }

    public TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants,
        TestUpdateHandler update)
    {
        Environment.SetEnvironmentVariable("ActiveMutation", mutants.Single().Id.ToString());

        var testResultsXml = RunTests(out var duration);

        var passedTests = GetPassedTests(testResultsXml);
        var failedTests = GetFailedTests(testResultsXml);
        var remainingMutants =
            update?.Invoke(mutants, failedTests, TestGuidsList.EveryTest(), GetTimeoutTestGuidsList());

        if (remainingMutants == false)
        {
            // all mutants status have been resolved, we can stop
            _logger.LogDebug($"Each mutant's fate has been established, we can stop.");
        }


        return new TestRunResult(passedTests, failedTests, GetTimeoutTestGuidsList(), string.Empty, duration);
    }

    public void Dispose()
    {
        //todo add stop job during run
        //todo remove all modifications
        //todo remove installed package
    }

    private XDocument RunTests(out TimeSpan duration)
    {
        var pathToUnity = GetPathToUnity();
        var pathToTestResultXml =
            Path.Combine(_strykerOptions.OutputPath, $"test_results_{DateTime.Now.ToFileTime()}.xml");
        var pathToProject = _strykerOptions.WorkingDirectory;
        var pathToUnityLogFile =
            Path.Combine(_strykerOptions.OutputPath, "unity_" + DateTime.Now.ToFileTime() + ".log");
        var startTime = DateTime.UtcNow;

        var processResult = _processExecutor.Start(pathToProject, pathToUnity,
            @$" -batchmode -projectPath='.\' -logFile {pathToUnityLogFile} -runTests -testResults {pathToTestResultXml} -testPlatform='editmode'");
        duration = DateTime.UtcNow - startTime;

        if (processResult.ExitCode != 0 && processResult.ExitCode != TestFailedExitCode)
        {
            throw new UnityExecuteException(processResult.ExitCode, processResult.Output);
        }

        return XDocument.Load(pathToTestResultXml);
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

    private string GetPathToUnity()
    {
        var pathToProject = _strykerOptions.WorkingDirectory;
        _logger.LogDebug("Unity pathToProject " + pathToProject);
        //todo add discovery of unity version
        //todo add platform specific unity path
        return @"/Applications/Unity/Hub/Editor/2022.2.0f1/Unity.app/Contents/MacOS/Unity";
    }

    private static Guid ToGuid(int value)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(value).CopyTo(bytes, 0);
        return new Guid(bytes);
    }
}
