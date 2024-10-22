using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Options;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.TestRunners;
using Stryker.Core.TestRunners.VsTest;
using VsTest = Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Stryker.Core.UnitTest.TestRunners;

/// <summary>
/// This class hosts the VsTestRunner related tests. The design of VsTest implies the creation of many mocking objects, so the tests may be hard to read.
/// This is sad but expected. Please use caution when changing/creating tests.
/// </summary>
[TestClass]
public class VsTestRunnerPoolTests : VsTestMockingHelper
{
    [TestMethod]
    public void InitializeProperly()
    {
        BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
        runner.GetTests(SourceProjectInfo).Count.ShouldBe(2);
    }

    [TestMethod]
    public void RunInitialTestsWithOneFailingTest()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
        SetupMockTestRun(mockVsTest, new[] { ("T0", false), ("T1", true) });
        var result = runner.InitialTest(SourceProjectInfo);
        // one test is failing
        result.FailingTests.Count.ShouldBe(1);
    }

    [TestMethod]
    public void ShouldCaptureErrorMessages()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
        var testResult = new VsTest.TestResult(TestCases[0])
        {
            Outcome = VsTest.TestOutcome.Passed,
            ErrorMessage = "Test"
        };
        SetupMockTestRun(mockVsTest, new[] { testResult });
        var result = runner.InitialTest(SourceProjectInfo);
        result.ResultMessage.ShouldEndWith("Test");
    }

    [TestMethod]
    public void ShouldComputeTimeoutProperly()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
        var now = DateTimeOffset.Now;
        var duration = TimeSpan.FromMilliseconds(2);
        var testResult = new VsTest.TestResult(TestCases[0])
        {
            Outcome = VsTest.TestOutcome.Passed,
            // ensure the test exhibit a long run time: Stryker should only use duration.
            StartTime = now,
            EndTime = DateTimeOffset.Now + TimeSpan.FromSeconds(1),
            Duration = duration
        };
        SetupMockTestRun(mockVsTest, new[] { testResult });
        runner.InitialTest(SourceProjectInfo);
        runner.Context.VsTests[TestCases[0].Id].InitialRunTime.ShouldBe(duration);
    }

    [TestMethod]
    public void ShouldComputeTimeoutProperlyForMultipleResults()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
        var now = DateTimeOffset.Now;
        var duration = TimeSpan.FromMilliseconds(2);
        var testResult = new VsTest.TestResult(TestCases[0])
        {
            Outcome = VsTest.TestOutcome.Passed,
            // ensure the test exhibit a long run time: Stryker should only use duration.
            StartTime = now,
            EndTime = DateTimeOffset.Now + TimeSpan.FromSeconds(1),
            Duration = duration
        };
        var otherTestResult = new VsTest.TestResult(TestCases[0])
        {
            Outcome = VsTest.TestOutcome.Passed,
            StartTime = testResult.StartTime,
            EndTime = testResult.EndTime,
            Duration = duration
        };
        SetupMockTestRun(mockVsTest, new[] { testResult, otherTestResult });
        runner.InitialTest(SourceProjectInfo);
        runner.Context.VsTests[TestCases[0].Id].InitialRunTime.ShouldBe(duration);
    }

    [TestMethod]
    public void HandleVsTestCreationFailure()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
        SetupFailingTestRun(mockVsTest);

        var result = runner.InitialTest(SourceProjectInfo);
        // legacy behavior was to crash on VsTest crash as an fail fast strategy
        // now We have fixed Stryker issues related to VsTest and issues are rare, so this should be a minor event

        result.SessionTimedOut.ShouldBeTrue();
    }

    [TestMethod]
    public void RunTests()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
        SetupMockTestRun(mockVsTest, true, TestCases);
        var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        // tests are successful => run should be successful
        result.FailingTests.IsEmpty.ShouldBeTrue();
    }

    [TestMethod]
    public void DoNotTestWhenNoTestPresent()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner, testCases: new List<VsTest.TestCase>());
        SetupMockTestRun(mockVsTest, true, new List<VsTest.TestCase>());
        var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        // tests are successful => run should be successful
        result.ExecutedTests.IsEmpty.ShouldBeTrue();
    }

    // If no tests are present in the assembly, VsTest raises no event at all
    // previous versions of Stryker froze when this happened
    [TestMethod]
    public void HandleWhenNoTestAreFound()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner, TestCases);
        SetupMockTestRun(mockVsTest, true, new List<VsTest.TestCase>());
        var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        // tests are successful => run should be successful
        result.ExecutedTests.IsEmpty.ShouldBeTrue();
    }

    [TestMethod]
    public void RecycleRunnerOnError()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
        SetupFailingTestRun(mockVsTest);
        runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        // the test will always end in a crash, VsTestRunner should retry at least a few times
        mockVsTest.Verify(m => m.RunTestsWithCustomTestHost(It.IsAny<IEnumerable<string>>(),
            It.IsAny<string>(), It.IsAny<TestPlatformOptions>(),
            It.IsAny<ITestRunEventsHandler>(),
            It.IsAny<IStrykerTestHostLauncher>()), Times.AtLeast(3));
    }

    [TestMethod]
    public void DetectTestsErrors()
    {
        var options = new StrykerOptions();
        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        SetupMockTestRun(mockVsTest, false, TestCases);
        var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        // run is failed
        result.FailingTests.IsEmpty.ShouldBeFalse();
    }

    [TestMethod]
    public void DetectTimeout()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;", ["T1"] = "1;" });
        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());
        SetupMockTimeOutTestRun(mockVsTest, new Dictionary<string, string> { ["0"] = "T0=S;T1=S" }, "T0");

        var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        result.TimedOutTests.IsEmpty.ShouldBeFalse();
    }

    [TestMethod]
    public void ShouldRetryFrozenSession()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
        var defaultTimeOut = VsTestRunner.VsTestExtraTimeOutInMs;
        VsTestRunner.VsTestExtraTimeOutInMs = 100;
        // the test session will freeze twice
        SetupFrozenTestRun(mockVsTest, 2);
        runner.TestMultipleMutants(SourceProjectInfo, new TimeoutValueCalculator(0, 10, 9), new[] { Mutant }, null);
        VsTestRunner.VsTestExtraTimeOutInMs = defaultTimeOut;
        mockVsTest.Verify(m => m.RunTestsWithCustomTestHost(It.IsAny<IEnumerable<string>>(),
            It.IsAny<string>(), It.IsAny<TestPlatformOptions>(),
            It.IsAny<ITestRunEventsHandler>(),
            It.IsAny<IStrykerTestHostLauncher>()), Times.Exactly(3));
    }

    [TestMethod]
    public void ShouldNotRetryFrozenVsTest()
    {
        var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
        var defaultTimeOut = VsTestRunner.VsTestExtraTimeOutInMs;
        // the test session will end properly, but VsTest will hang
        // it will be recycled
        SetupFrozenVsTest(mockVsTest, 3);
        VsTestRunner.VsTestExtraTimeOutInMs = 100;
        runner.TestMultipleMutants(SourceProjectInfo, new TimeoutValueCalculator(0, 10, 9), new[] { Mutant }, null);
        VsTestRunner.VsTestExtraTimeOutInMs = defaultTimeOut;
        mockVsTest.Verify(m => m.EndSession(), Times.Exactly(2));
    }

    [TestMethod]
    public void AbortOnError()
    {
        var options = new StrykerOptions();

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        mockVsTest.Setup(x => x.CancelTestRun()).Verifiable();
        SetupMockTestRun(mockVsTest, false, TestCases);

        var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, (_, _, _, _) => false);
        // verify Abort has been called
        Mock.Verify(mockVsTest);
        // and test run is failed
        result.FailingTests.IsEmpty.ShouldBeFalse();
    }

    [TestMethod]
    public void IdentifyNonCoveredMutants()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.SkipUncoveredMutants
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        // test 0 and 1 cover mutant 1
        SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;", ["T1"] = "0;" });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());
        // one mutant is covered by tests 0 and 1
        Mutant.CoveringTests.IsEmpty.ShouldBe(false);
        OtherMutant.CoveringTests.IsEmpty.ShouldBe(true);
        OtherMutant.ResultStatus.ShouldBe(MutantStatus.NoCoverage);
    }

    [TestMethod]
    public void WorksWhenAllMutantsAreIgnoredPool()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.SkipUncoveredMutants
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        // test 0 and 1 cover mutant 1
        SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = ";", ["T1"] = ";" });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());
        Mutant.CoveringTests.Count.ShouldBe(0);
    }

    [TestMethod]
    public void RunOnlyUsefulTest()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        // only first test covers one mutant
        SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;", ["T1"] = ";" });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());

        SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string> { ["0"] = "T0=S" });

        // process coverage information
        var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        // verify Abort has been called
        Mock.Verify(mockVsTest);
        // verify only one test has been run
        result.ExecutedTests.Count.ShouldBe(1);
    }

    [TestMethod]
    public void NotRunTestWhenNotCovered()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        // only first test covers one mutant
        SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;0", ["T1"] = ";" });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());

        SetupMockTestRun(mockVsTest, false, TestCases);
        // mutant 0 is covered
        Mutant.IsStaticValue.ShouldBeTrue();
        var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        // mutant is killed
        result.FailingTests.IsEmpty.ShouldBeFalse();
        // mutant 1 is not covered
        result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { OtherMutant }, null);
        // tests are ok
        result.ExecutedTests.IsEmpty.ShouldBeTrue();
    }

    [TestMethod]
    public void RunTestsSimultaneouslyWhenPossible()
    {
        var options = new StrykerOptions()
        {
            OptimizationMode = OptimizationModes.DisableBail | OptimizationModes.CoverageBasedTest,
            Concurrency = Math.Max(Environment.ProcessorCount / 2, 1)
        };

        var project = BuildSourceProjectInfo(new[] { Mutant, OtherMutant, new Mutant { Id = 2 }, new Mutant { Id = 4 } });
        // make sure we have 4 mutants
        var myTestCases = TestCases.ToList();
        myTestCases.Add(BuildCase("T2"));
        myTestCases.Add(BuildCase("T3"));
        var mockVsTest = BuildVsTestRunnerPool(options, out var runner, myTestCases);

        var tester = BuildMutationTestProcess(runner, options, sourceProject: project);
        SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;", ["T1"] = "1;" });
        tester.GetCoverage();
        SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string> { ["0,1"] = "T0=S,T1=F" });
        tester.Test(project.ProjectContents.Mutants.Where(x => !x.CoveringTests.IsEmpty));

        Mutant.ResultStatus.ShouldBe(MutantStatus.Survived);
        OtherMutant.ResultStatus.ShouldBe(MutantStatus.Killed);
    }

    [TestMethod]
    public void ShouldThrowWhenTestingMultipleMutantsWithoutCoverageAnalysis()
    {
        var options = new StrykerOptions()
        {
            OptimizationMode = OptimizationModes.None,
            Concurrency = Math.Max(Environment.ProcessorCount / 2, 1)
        };

        var mutants = new[] { Mutant, OtherMutant };
        // make sure we have 4 mutants
        var myTestCases = TestCases.ToList();
        myTestCases.Add(BuildCase("T2"));
        myTestCases.Add(BuildCase("T3"));
        BuildVsTestRunnerPool(options, out var runner, myTestCases);

        var testFunc = () => runner.TestMultipleMutants(SourceProjectInfo, new TimeoutValueCalculator(0), mutants, null);

        testFunc.ShouldThrow(typeof(GeneralStrykerException));
    }

    [TestMethod]
    public void RunRelevantTestsOnStaticWhenPerTestCoverage()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest | OptimizationModes.CaptureCoveragePerTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        SetupMockCoveragePerTestRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0,1;1", ["T1"] = ";" });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());

        SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string> { ["0"] = "T0=F", ["1"] = "T0=S" });
        var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { OtherMutant }, null);
        result.FailingTests.IsEmpty.ShouldBeTrue();
        result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        result.FailingTests.IsEmpty.ShouldBeFalse();
    }

    [TestMethod]
    public void HandleMultipleTestResults()
    {
        var options = new StrykerOptions();
        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        // assume 2 results for T0
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) });
        var result = runner.InitialTest(SourceProjectInfo);
        // initial test is fine
        result.FailingTests.IsEmpty.ShouldBeTrue();
        // test session will fail
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T0", true), ("T1", true) });
        result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        result.FailingTests.IsEmpty.ShouldBeTrue();
        result.ExecutedTests.IsEveryTest.ShouldBeTrue();
    }

    [TestMethod]
    public void HandleMultipleTestResultsForXUnit()
    {
        var options = new StrykerOptions();
        var tests = new List<VsTest.TestCase>
        {
            BuildCase("X0"),
            BuildCase("X1")
        };
        var mockVsTest = BuildVsTestRunnerPool(options, out var runner, tests);
        // assume 3 results for X0
        SetupMockTestRun(mockVsTest, new[] { ("X0", true), ("X1", true), ("X0", true), ("X0", true) }, tests);
        var result = runner.InitialTest(SourceProjectInfo);
        // the duration should be less than 3 times the (test) default duration
        result.Duration.ShouldBeLessThan(TestDefaultDuration.Duration() * 3);
    }

    [TestMethod]
    public void HandleFailureWithinMultipleTestResults()
    {
        var options = new StrykerOptions();
        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        // assume 2 results for T0
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) });
        var result = runner.InitialTest(SourceProjectInfo);
        // initial test is fine
        result.FailingTests.IsEmpty.ShouldBeTrue();
        // test session will fail on test 1
        SetupMockTestRun(mockVsTest, new[] { ("T0", false), ("T0", true), ("T1", true) });

        result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        result.ExecutedTests.IsEveryTest.ShouldBeTrue();
        result.FailingTests.IsEmpty.ShouldBeFalse();
        result.FailingTests.GetGuids().ShouldContain(TestCases[0].Id);
        // test session will fail on the other test result
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T0", false), ("T1", true) });
        result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        result.ExecutedTests.IsEveryTest.ShouldBeTrue();
        result.FailingTests.IsEmpty.ShouldBeFalse();
        result.FailingTests.GetGuids().ShouldContain(TestCases[0].Id);
    }

    [TestMethod]
    public void HandleTimeOutWithMultipleTestResults()
    {
        var options = new StrykerOptions();
        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        // assume 2 results for T0
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) });
        var result = runner.InitialTest(SourceProjectInfo);
        // initial test is fine
        result.FailingTests.IsEmpty.ShouldBeTrue();
        // test session will fail
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true) });
        result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);

        result.FailingTests.IsEmpty.ShouldBeTrue();
        result.TimedOutTests.Count.ShouldBe(1);
        result.TimedOutTests.GetGuids().ShouldContain(TestCases[0].Id);
        result.ExecutedTests.IsEveryTest.ShouldBeFalse();
    }

    [TestMethod]
    public void HandleFailureWhenExtraMultipleTestResults()
    {
        var options = new StrykerOptions();
        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        // assume 2 results for T0
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) });
        var result = runner.InitialTest(SourceProjectInfo);
        // initial test is fine
        result.FailingTests.IsEmpty.ShouldBeTrue();
        // test session will fail
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T0", true), ("T0", false), ("T1", true) });
        result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        result.ExecutedTests.IsEveryTest.ShouldBeTrue();
        result.FailingTests.IsEmpty.ShouldBeFalse();
        result.FailingTests.GetGuids().ShouldContain(TestCases[0].Id);
    }

    [TestMethod]
    public void HandleUnexpectedTestResult()
    {
        var options = new StrykerOptions();
        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        // assume 2 results for T0
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) });
        var result = runner.InitialTest(SourceProjectInfo);
        // initial test is fine
        result.FailingTests.IsEmpty.ShouldBeTrue();
        // test session will fail
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T2", true), ("T1", true), ("T0", true) });
        result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
        result.ExecutedTests.IsEveryTest.ShouldBeTrue();
        result.FailingTests.IsEmpty.ShouldBeTrue();
    }

    [TestMethod]
    public void HandleUnexpectedTestCase()
    {
        var options = new StrykerOptions();
        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        // assume 2 results for T0
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T2", true) });
        runner.InitialTest(SourceProjectInfo);
        runner.Context.Tests.Count.ShouldBe(3);
    }

    // this verifies that mutant that are covered outside any tests are
    // flagged as to be tested against all tests
    [TestMethod]
    public void MarkSuspiciousCoverage()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;|1", ["T1"] = ";" });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());
        // the suspicious mutant should be tested against all tests
        OtherMutant.CoveringTests.IsEveryTest.ShouldBe(true);
    }

    // this verifies that static mutants are flagged as to be tested against all tests
    [TestMethod]
    public void StaticMutantsShouldBeTestedAgainstAllTests()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };
        var staticMutant = new Mutant() { Id = 14, IsStaticValue = true };
        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;", ["T1"] = "1;" });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant, staticMutant }, TestGuidsList.NoTest());
        // the suspicious mutant should be tested against all tests
        staticMutant.CoveringTests.IsEveryTest.ShouldBe(true);
    }

    // this verifies that mutant that are covered outside any tests are
    // flagged as to be tested against all tests (except failed ones)
    [TestMethod]
    public void MarkSuspiciousCoverageInPresenceOfFailedTests()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
        SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", false), ("T2", true) });
        runner.InitialTest(SourceProjectInfo);

        SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "0;|1", ["T1"] = ";" });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, new TestGuidsList(TestCases[1].Id));
        // the suspicious mutant should be tested against all tests except the failing one
        OtherMutant.AssessingTests.IsEveryTest.ShouldBe(false);
    }

    // this verifies that tests missing any coverage information are
    // flagged as to be tested used against every mutant
    [TestMethod]
    public void MarkSuspiciousTests()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        SetupMockCoverageRun(mockVsTest, new Dictionary<string, string> { ["T0"] = "1;", ["T1"] = null });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());
        // the suspicious mutant should be tested against all tests
        OtherMutant.CoveringTests.Count.ShouldBe(2);
        Mutant.CoveringTests.Count.ShouldBe(1);
    }

    // this verifies that tests covering no mutants
    // are properly handled
    [TestMethod]
    public void HandleNonCoveringTests()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        var testResult = BuildCoverageTestResult("T0", new[] { "0;", "" });
        var other = BuildCoverageTestResult("T1", new[] { "", "" });
        SetupMockCoverageRun(mockVsTest, new[] { testResult, other });


        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());

        OtherMutant.CoveringTests.Count.ShouldBe(0);
        Mutant.CoveringTests.Count.ShouldBe(1);
    }

    // this verifies extra test results (without any coverage info) are properly handled
    // are properly handled
    [TestMethod]
    public void HandleExtraTestResult()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        var testResult = BuildCoverageTestResult("T0", new[] { "0;", "" });
        var other = new VsTest.TestResult(FindOrBuildCase("T0"))
        {
            DisplayName = "T0",
            Outcome = VsTest.TestOutcome.Passed,
            ComputerName = "."
        };
        SetupMockCoverageRun(mockVsTest, new[] { testResult, other });


        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());

        OtherMutant.CoveringTests.Count.ShouldBe(0);
        Mutant.CoveringTests.Count.ShouldBe(1);
    }

    // this verifies that unexpected test case (i.e. unseen during test discovery and without coverage info)
    // are assumed to cover every mutant
    [TestMethod]
    public void DetectUnexpectedCase()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        var buildCase = BuildCase("unexpected", TestFrameworks.NUnit);
        SetupMockCoverageRun(mockVsTest, new[] { new VsTest.TestResult(buildCase) { Outcome = VsTest.TestOutcome.Passed } });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());
        // the suspicious tests should be used for every mutant
        OtherMutant.CoveringTests.GetGuids().ShouldContain(buildCase.Id);
        Mutant.CoveringTests.GetGuids().ShouldContain(buildCase.Id);
    }

    // this verifies that Stryker disregard skipped tests
    [TestMethod]
    public void IgnoreSkippedTestResults()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        var testResult = BuildCoverageTestResult("T0", new[] { "0;", "" });
        testResult.Outcome = VsTest.TestOutcome.Skipped;
        var other = BuildCoverageTestResult("T1", new[] { "0;", "" });
        SetupMockCoverageRun(mockVsTest, new[] { testResult, other });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());
        // the suspicious tests should be used for every mutant
        Mutant.CoveringTests.Count.ShouldBe(1);
    }

    // this verifies that Stryker aggregates multiple coverage results
    [TestMethod]
    public void HandlesMultipleResultsForCoverage()
    {
        var options = new StrykerOptions
        {
            OptimizationMode = OptimizationModes.CoverageBasedTest
        };

        var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

        var testResult = BuildCoverageTestResult("T0", new[] { "0;", "" });
        var other = BuildCoverageTestResult("T0", new[] { "1;0", "" });
        SetupMockCoverageRun(mockVsTest, new[] { testResult, other });

        var analyzer = new CoverageAnalyser(options);
        analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());
        // the suspicious tests should be used for every mutant
        Mutant.CoveringTests.IsEveryTest.ShouldBe(true);
        Mutant.IsStaticValue.ShouldBe(true);
        OtherMutant.CoveringTests.Count.ShouldBe(1);
    }
}
