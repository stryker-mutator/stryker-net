using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.Testing;

namespace Stryker.Core.TestRunners.UnityTestRunner;

public class UnityTestRunner : ITestRunner
{
    private readonly ProcessExecutor _processExecutor;
    private readonly StrykerOptions _strykerOptions;
    private readonly ILogger _logger;
    private readonly TestSet _testSet = new();


    public UnityTestRunner(ProcessExecutor processExecutor, StrykerOptions strykerOptions, ILogger logger)
    {
        _processExecutor = processExecutor;
        _strykerOptions = strykerOptions;
        _logger = logger;
    }

    public TestSet DiscoverTests()
    {
        //todo read file generated from unity build https://docs.unity3d.com/Packages/com.unity.test-framework@1.1/manual/extension-retrieve-test-list.html
        TestDescription[] testsDescription =
        {
            new TestDescription(Guid.NewGuid(), "RunPoint_Sync_NormalRun_MultipleInstance",
                "/Users/andrewv/Desktop/DummyProject/Assets/ExtensionPoints/Tests/SyncNoPayloadExtensionPointTests.cs"),
            new TestDescription(Guid.NewGuid(), "RunPoint_Sync_NormalRun_OneInstance",
                "/Users/andrewv/Desktop/DummyProject/Assets/ExtensionPoints/Tests/SyncNoPayloadExtensionPointTests.cs")
        };
        _testSet.RegisterTests(testsDescription);
        return _testSet;
    }

    public TestRunResult InitialTest()
    {
        var pathToProject = _strykerOptions.WorkingDirectory;
        //todo add discovery of unity version
        var pathToUnity = @"/Applications/Unity/Hub/Editor/2022.2.0f1/Unity.app/Contents/MacOS/Unity";
        var processResult = _processExecutor.Start(pathToProject, pathToUnity,
            @" -batchmode -projectPath='.\' -logFile - -runTests -testResults ./test_resultseditmode.xml -testPlatform='editmode'");

        if (processResult.ExitCode != 0)
        {
            throw new Exception("Unity failed with return code " + processResult.ExitCode);
        }

        _logger.LogDebug(processResult.Output);
        _logger.LogDebug("Exit code: " + processResult.ExitCode);
        var pathToTestResultXml = Path.Combine(pathToProject, "test_resultseditmode.xml");

        var testResultsXml = XDocument.Load(pathToTestResultXml);
        //todo add full test path
        _testSet.RegisterTests(testResultsXml.Descendants("test-case")
            .Select(element => new TestDescription(ToGuid(Int32.Parse(element.Attribute("id").Value)),
                element.Attribute("fullname").Value, element.Attribute("fullname").Value)));
        var passedTests = _testSet.Extract(testResultsXml.Descendants("test-case")
            .Where(element => element.Attribute("result").Value == "Passed")
            .Select(element => ToGuid(Int32.Parse(element.Attribute("id").Value))));
        IEnumerable<TestDescription> failedTests = _testSet.Extract(testResultsXml.Descendants("test-case")
            .Where(element => element.Attribute("result").Value == "Failed")
            .Select(element => ToGuid(Int32.Parse(element.Attribute("id").Value))));
        var timeSpan = new TimeSpan();
        return new TestRunResult(new TestGuidsList(passedTests), new TestGuidsList(failedTests), TestGuidsList.NoTest(),
            "Unity was finished with code " + processResult.ExitCode, timeSpan);
    }


    public IEnumerable<CoverageRunResult> CaptureCoverage()
    {
        throw new System.NotImplementedException();
    }

    public TestRunResult TestMultipleMutants(ITimeoutValueCalculator timeoutCalc, IReadOnlyList<Mutant> mutants,
        TestUpdateHandler update)
    {
        var pathToProject = _strykerOptions.WorkingDirectory;
        _logger.LogDebug("Unity pathToProject " + pathToProject);
        //todo add discovery of unity version
        var pathToUnity = @"/Applications/Unity/Hub/Editor/2022.2.0f1/Unity.app/Contents/MacOS/Unity";
        var pathToTestResultXml = Path.Combine(pathToProject, "test_resultseditmode.xml");
        if (File.Exists(pathToTestResultXml))
        {
            File.Delete(pathToTestResultXml);
        }

        Environment.SetEnvironmentVariable("ActiveMutation", mutants.Single().Id.ToString());
        var processResult = _processExecutor.Start(pathToProject, pathToUnity,
            @$" -batchmode -projectPath='.\' -logFile {Path.Combine(pathToProject, DateTime.Now.ToFileTime() + ".log")} -runTests -testResults ./test_resultseditmode.xml -testPlatform='editmode'");

        if (processResult.ExitCode != 0 && processResult.ExitCode != 2)
        {
            throw new Exception("Unity failed with return code " + processResult.ExitCode);
        }


        var testResultsXml = XDocument.Load(pathToTestResultXml);

        var passedTests = new TestGuidsList(_testSet.Extract(testResultsXml.Descendants("test-case")
            .Where(element => element.Attribute("result").Value == "Passed")
            .Select(element => ToGuid(Int32.Parse(element.Attribute("id").Value)))));
        var failedTests = new TestGuidsList(_testSet.Extract(testResultsXml.Descendants("test-case")
            .Where(element => element.Attribute("result").Value == "Failed")
            .Select(element => ToGuid(Int32.Parse(element.Attribute("id").Value)))));
        var timeSpan = new TimeSpan();
        _logger.LogDebug("Amount of passedTests " + passedTests.Count);
        _logger.LogDebug("Amount of failedTests " + failedTests.Count);


        //NUnit result has no result of time-out https://docs.nunit.org/articles/nunit/technical-notes/usage/Test-Result-XML-Format.html#test-case
        var remainingMutants = update?.Invoke(mutants, failedTests, TestGuidsList.EveryTest(), TestGuidsList.NoTest());

        if (remainingMutants == false)
        {
            // all mutants status have been resolved, we can stop
            _logger.LogDebug($"Each mutant's fate has been established, we can stop.");
        }

        if (passedTests.Count == _testSet.Count)
        {
            passedTests = TestGuidsList.EveryTest();
        }


        return new TestRunResult(passedTests, failedTests, TestGuidsList.NoTest(),
            "Unity was finished with code " + processResult.ExitCode, timeSpan);
    }

    public void Dispose()
    {
        //todo add stop job during run
    }

    private static Guid ToGuid(int value)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(value).CopyTo(bytes, 0);
        return new Guid(bytes);
    }
}
