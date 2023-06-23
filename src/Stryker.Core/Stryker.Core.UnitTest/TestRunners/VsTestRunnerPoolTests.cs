using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using Shouldly;
using Stryker.Core.CoverageAnalysis;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.TestRunners.VsTest;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    /// <summary>
    /// This class hosts the VsTestRunner related tests. The design of VsTest implies the creation of many mocking objects, so the tests may be hard to read.
    /// This is sad but expected. Please use caution when changing/creating tests.
    /// </summary>
    public class VsTestRunnerPoolTests : VsTestMockingHelper
    {
        [Fact]
        public void InitializeProperly()
        {
            BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            runner.GetTests(SourceProjectInfo).Count.ShouldBe(2);
        }

        [Fact]
        public void RunInitialTestsWithOneFailingTest()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            SetupMockTestRun(mockVsTest, new[] { ("T0", false), ("T1", true) });
            var result = runner.InitialTest(SourceProjectInfo);
            // one test is failing
            result.FailingTests.Count.ShouldBe(1);
        }

        [Fact]
        public void ShouldCaptureErrorMessages()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            var testResult = new TestResult(TestCases[0])
            {
                Outcome = TestOutcome.Passed,
                ErrorMessage = "Test"
            };
            SetupMockTestRun(mockVsTest, new[] { testResult });
            var result = runner.InitialTest(SourceProjectInfo);
            result.ResultMessage.ShouldEndWith("Test");
        }

        [Fact]
        public void ShouldComputeTimeoutProperly()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            var now = DateTimeOffset.Now;
            var duration = TimeSpan.FromMilliseconds(2);
            var testResult = new TestResult(TestCases[0])
            {
                Outcome = TestOutcome.Passed,
                // ensure the test exhibit a long run time: Stryker should only use duration.
                StartTime = now,
                EndTime = DateTimeOffset.Now + TimeSpan.FromSeconds(1),
                Duration = duration
            };
            SetupMockTestRun(mockVsTest, new[] { testResult });
            runner.InitialTest(SourceProjectInfo);
            runner.Context.VsTests[TestCases[0].Id].InitialRunTime.ShouldBe(duration);
        }

        [Fact]
        public void ShouldComputeTimeoutProperlyForMultipleResults()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            var now = DateTimeOffset.Now;
            var duration = TimeSpan.FromMilliseconds(2);
            var testResult = new TestResult(TestCases[0])
            {
                Outcome = TestOutcome.Passed,
                // ensure the test exhibit a long run time: Stryker should only use duration.
                StartTime = now,
                EndTime = DateTimeOffset.Now + TimeSpan.FromSeconds(1),
                Duration = duration
            };
            var otherTestResult = new TestResult(TestCases[0])
            {
                Outcome = TestOutcome.Passed,
                StartTime = testResult.StartTime,
                EndTime = testResult.EndTime,
                Duration = duration
            };
            SetupMockTestRun(mockVsTest, new[] { testResult, otherTestResult });
            runner.InitialTest(SourceProjectInfo);
            runner.Context.VsTests[TestCases[0].Id].InitialRunTime.ShouldBe(duration);
        }

        [Fact]
        public void HandleVsTestCreationFailure()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            SetupFailingTestRun(mockVsTest);

            Action action = () => runner.InitialTest(SourceProjectInfo);
            action.ShouldThrow<GeneralStrykerException>();
        }

        [Fact]
        public void RunTests()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            SetupMockTestRun(mockVsTest, true, TestCases);
            var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
            // tests are successful => run should be successful
            result.FailingTests.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void DoNotTestWhenNoTestPresent()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner, testCases: new List<TestCase>());
            SetupMockTestRun(mockVsTest, true, new List<TestCase>());
            var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
            // tests are successful => run should be successful
            result.ExecutedTests.IsEmpty.ShouldBeTrue();
        }
        
        // If no tests are present in the assembly, VsTest raises no event at all
        // previous versions of Stryker froze when this happened
        [Fact]
        public void HandleWhenNoTestAreFound()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner, TestCases);
            SetupMockTestRun(mockVsTest, true, new List<TestCase>());
            var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
            // tests are successful => run should be successful
            result.ExecutedTests.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void RecycleRunnerOnError()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            SetupFailingTestRun(mockVsTest);
            var action = () =>  runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
            action.ShouldThrow<GeneralStrykerException>();
            // the test will always end in a crash, VsTestRunner should retry at least a few times
            mockVsTest.Verify(m => m.EndSession(), Times.AtLeast(4));
        }

        [Fact]
        public void DetectTestsErrors()
        {
            var options = new StrykerOptions();
            var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
            SetupMockTestRun(mockVsTest, false, TestCases);
            var result = runner.TestMultipleMutants(SourceProjectInfo, null, new[] { Mutant }, null);
            // run is failed
            result.FailingTests.IsEmpty.ShouldBeFalse();
        }

        [Fact]
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

        [Fact]
        public void ShouldRetryFrozenSession()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            // the test session will hung twice
            SetupFrozenTestRun(mockVsTest, 2);
            runner.TestMultipleMutants(SourceProjectInfo, new TimeoutValueCalculator(0, 10,9), new[] { Mutant }, null);
            mockVsTest.Verify(m => m.EndSession(), Times.Exactly(3));
        }

        [Fact]
        public void ShouldNotRetryFrozenVsTest()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            var defaultTimeOut = VsTestRunner.VsTestExtraTimeOutInMs;
            // the test session will end properly, but VsTest will hang
            SetupFrozenVsTest(mockVsTest, 3);
            VsTestRunner.VsTestExtraTimeOutInMs = 500;
            runner.TestMultipleMutants(SourceProjectInfo, new TimeoutValueCalculator(0, 10,9), new[] { Mutant }, null);
            VsTestRunner.VsTestExtraTimeOutInMs = defaultTimeOut;
            mockVsTest.Verify(m => m.EndSession(), Times.Exactly(2));
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void HandleMultipleTestResultsForXUnit()
        {
            var options = new StrykerOptions();
            var tests = new List<TestCase>
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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
        [Fact]
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
        [Fact]
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
        [Fact]
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
        // flagged as to be tested used against every mutants
        [Fact]
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
        [Fact]
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

        // this verifies that unexpected test case (i.e. unseen during test discovery and without coverage info)
        // are assumed to cover every mutant
        [Fact]
        public void DetectUnexpectedCase()
        {
            var options = new StrykerOptions
            {
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

            var buildCase = BuildCase("unexpected", Core.TestRunners.TestFrameworks.NUnit);
            SetupMockCoverageRun(mockVsTest, new[] { new TestResult(buildCase) { Outcome = TestOutcome.Passed } });

            var analyzer = new CoverageAnalyser(options);
            analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());
            // the suspicious tests should be used for every mutant
            OtherMutant.CoveringTests.GetGuids().ShouldContain(buildCase.Id);
            Mutant.CoveringTests.GetGuids().ShouldContain(buildCase.Id);
        }

        // this verifies that Stryker disregard skipped tests
        [Fact]
        public void IgnoreSkippedTestResults()
        {
            var options = new StrykerOptions
            {
                OptimizationMode = OptimizationModes.CoverageBasedTest
            };

            var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

            var testResult = BuildCoverageTestResult("T0", new[] { "0;", "" });
            testResult.Outcome = TestOutcome.Skipped;
            var other = BuildCoverageTestResult("T1", new[] { "0;", "" });
            SetupMockCoverageRun(mockVsTest, new[] { testResult, other });

            var analyzer = new CoverageAnalyser(options);
            analyzer.DetermineTestCoverage(SourceProjectInfo, runner, new[] { Mutant, OtherMutant }, TestGuidsList.NoTest());
            // the suspicious tests should be used for every mutant
            Mutant.CoveringTests.Count.ShouldBe(1);
        }

        // this verifies that Stryker aggregates multiple coverage results
        [Fact]
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
}
