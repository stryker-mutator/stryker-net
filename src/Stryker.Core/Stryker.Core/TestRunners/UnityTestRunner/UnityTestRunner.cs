using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Testing;
using Stryker.Core.TestRunners.UnityTestRunner.RunUnity;

namespace Stryker.Core.TestRunners.UnityTestRunner;

public class UnityTestRunner : ITestRunner
{
    private const int TestFailedExitCode = 2;
    private readonly IProcessExecutor _processExecutor;
    private readonly StrykerOptions _strykerOptions;
    private readonly ILogger _logger;
    private readonly IRunUnity _runUnity;
    private TestSet _testSet = null;
    private TestRunResult _initialRunTestResult;
    private bool _firstMutationTestStarted;
    private Task _unityProcessTask;
    private string PathToUnityListenFile => Path.Combine(_strykerOptions.OutputPath, "UnityListens.txt");

    private string PathToActiveMutantsListenFile =>
        Path.Combine(_strykerOptions.OutputPath, "ActiveMutantsListens.txt");


    public UnityTestRunner(IProcessExecutor processExecutor, StrykerOptions strykerOptions, ILogger logger,
        IRunUnity runUnity)
    {
        _processExecutor = processExecutor;
        _strykerOptions = strykerOptions;
        _logger = logger;
        _runUnity = runUnity;
    }

    public TestSet DiscoverTests()
    {
        if (_testSet != null)
            return _testSet;

        var pathToTestResultXml =
            Path.Combine(_strykerOptions.OutputPath, $"test_results_{DateTime.Now.ToFileTime()}.xml");
        RunTests(pathToTestResultXml, out var duration);

        var testResultsXml = XDocument.Load(pathToTestResultXml);

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
        throw new System.NotImplementedException();
    }

    public TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants,
        TestUpdateHandler update)
    {
        if (!_firstMutationTestStarted)
        {
            //rerun unity to apply modifications and reload domain
            FinishUnityProcess();
        }

        ApplyMutantIdToTestRun(mutants);

        var pathToTestResultXml =
            Path.Combine(_strykerOptions.OutputPath, $"test_results_{DateTime.Now.ToFileTime()}.xml");
        RunTests(pathToTestResultXml, out var duration);

        var testResultsXml = XDocument.Load(pathToTestResultXml);

        var passedTests = GetPassedTests(testResultsXml);
        var failedTests = GetFailedTests(testResultsXml);
        var remainingMutants =
            update?.Invoke(mutants, failedTests, TestGuidsList.EveryTest(), GetTimeoutTestGuidsList());

        if (remainingMutants == false)
        {
            // all mutants status have been resolved, we can stop
            _logger.LogDebug($"Each mutant's fate has been established, we can stop.");
        }

        _firstMutationTestStarted = true;

        return new TestRunResult(passedTests, failedTests, GetTimeoutTestGuidsList(), string.Empty, duration);
    }

    private void ApplyMutantIdToTestRun(IEnumerable<Mutant> mutants)
    {
        Environment.SetEnvironmentVariable("ActiveMutationPath", PathToActiveMutantsListenFile);
        File.WriteAllText(PathToActiveMutantsListenFile, mutants.Single().Id.ToString());
    }

    public void Dispose()
    {
        FinishUnityProcess();
        //todo remove all modifications
        //todo remove installed package
    }

    private void StartUnityProcess()
    {
        var pathToProject = _strykerOptions.WorkingDirectory;

        var processResult = _runUnity.RunUnityUntilFinish(_strykerOptions, pathToProject,
            "-executeMethod Stryker.UnitySDK.RunTests.Run");
        if (processResult.ExitCode != 0 && processResult.ExitCode != TestFailedExitCode)
        {
            throw new UnityExecuteException(processResult.ExitCode, processResult.Output);
        }
    }

    private void RunTests(string pathToSave, out TimeSpan duration)
    {
        _logger.LogDebug("Request to run tests and save at " + pathToSave);
        Environment.SetEnvironmentVariable("Stryker.Unity.PathToListen", PathToUnityListenFile);
        File.WriteAllText(PathToUnityListenFile, pathToSave);

        if (_unityProcessTask == null || _unityProcessTask.Status != TaskStatus.Running)
        {
            _unityProcessTask = Task.Run(StartUnityProcess);
        }

        var startTime = DateTime.UtcNow;
        while (!File.Exists(pathToSave) && _unityProcessTask.Status != TaskStatus.Canceled &&
               _unityProcessTask.Status != TaskStatus.Faulted && _unityProcessTask.Status != TaskStatus.RanToCompletion)
        {
        }

        if (_unityProcessTask.Exception != null) throw _unityProcessTask.Exception;


        if (File.Exists(pathToSave))
        {
            _logger.LogDebug("Test run accomplished and save at " + pathToSave);
        }


        duration = DateTime.UtcNow - startTime;
        //hotfix to wait that all xml content was wrote fully
        Thread.Sleep(100);
    }

    private void FinishUnityProcess()
    {
        _logger.LogDebug("StartUnityProcess FinishUnityProcess");

        File.WriteAllText(PathToUnityListenFile, "exit");
        _unityProcessTask.GetAwaiter().GetResult();
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
