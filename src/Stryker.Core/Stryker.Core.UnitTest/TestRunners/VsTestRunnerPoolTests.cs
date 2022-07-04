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
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            runner.DiscoverTests().Count.ShouldBe(2);
        }

        [Fact]
        public void RunInitialTestsWithOneFailingTest()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            SetupMockTestRun(mockVsTest, new[] { ("T0", false), ("T1", true) });
            var result = runner.InitialTest();
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
            var result = runner.InitialTest();
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
            var result = runner.InitialTest();
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
            var result = runner.InitialTest();
            runner.Context.VsTests[TestCases[0].Id].InitialRunTime.ShouldBe(duration);
        }

        [Fact]
        public void HandleVsTestCreationFailure()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner, succeed: false);
            SetupMockTestRun(mockVsTest, new[] { ("T0", false), ("T1", true) });

            Action action = () => runner.InitialTest();
            action.ShouldThrow<GeneralStrykerException>();
        }

        [Fact]
        public void RunTests()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            SetupMockTestRun(mockVsTest, true, TestCases);
            var result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
            // tests are successful => run should be successful
            result.FailingTests.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void RecycleRunnerOnError()
        {
            var mockVsTest = BuildVsTestRunnerPool(new StrykerOptions(), out var runner);
            SetupFailingTestRun(mockVsTest);
            runner.TestMultipleMutants(null, new[] { Mutant }, null);
            // the test will always end in a crash, VsTestRunner should retry at least once
            mockVsTest.Verify(m => m.EndSession(), Times.AtLeastOnce());
        }

        [Fact]
        public void DetectTestsErrors()
        {
            var options = new StrykerOptions();
            var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
            SetupMockTestRun(mockVsTest, false, TestCases);
            var result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
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
            analyzer.DetermineTestCoverage(runner, new[] { Mutant, OtherMutant }, TestsGuidList.NoTest());
            SetupMockTimeOutTestRun(mockVsTest, new Dictionary<string, string> { ["0"] = "T0=S;T1=S" }, "T0");

            var result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
            result.TimedOutTests.IsEmpty.ShouldBeFalse();
        }

        [Fact]
        public void AbortOnError()
        {
            var options = new StrykerOptions();

            var mockVsTest = BuildVsTestRunnerPool(options, out var runner);

            mockVsTest.Setup(x => x.CancelTestRun()).Verifiable();
            SetupMockTestRun(mockVsTest, false, TestCases);

            var result = runner.TestMultipleMutants(null, new[] { Mutant }, ((_, _, _, _) => false));
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
            analyzer.DetermineTestCoverage(runner, new[] { Mutant, OtherMutant }, TestsGuidList.NoTest());
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
            analyzer.DetermineTestCoverage(runner, new[] { Mutant, OtherMutant }, TestsGuidList.NoTest());
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
            analyzer.DetermineTestCoverage(runner, new[] { Mutant, OtherMutant }, TestsGuidList.NoTest());

            SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string> { ["0"] = "T0=S" });

            // process coverage information
            var result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
            // verify Abort has been called
            Mock.Verify(mockVsTest);
            // verify only one test has been run
            result.RanTests.Count.ShouldBe(1);
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
            analyzer.DetermineTestCoverage(runner, new[] { Mutant, OtherMutant }, TestsGuidList.NoTest());

            SetupMockTestRun(mockVsTest, false, TestCases);
            // mutant 0 is covered
            Mutant.IsStaticValue.ShouldBeTrue();
            var result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
            // mutant is killed
            result.FailingTests.IsEmpty.ShouldBeFalse();
            // mutant 1 is not covered
            result = runner.TestMultipleMutants(null, new[] { OtherMutant }, null);
            // tests are ok
            result.RanTests.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void RunTestsSimultaneouslyWhenPossible()
        {
            var options = new StrykerOptions()
            {
                OptimizationMode = OptimizationModes.DisableBail | OptimizationModes.CoverageBasedTest,
                Concurrency = Math.Max(Environment.ProcessorCount / 2, 1)
            };

            var project = BuildTestProject(new[] { Mutant, OtherMutant, new Mutant { Id = 2 }, new Mutant { Id = 4 } });
            // make sure we have 4 mutants
            var myTestCases = TestCases.ToList();
            myTestCases.Add(BuildCase("T2"));
            myTestCases.Add(BuildCase("T3"));
            var mockVsTest = BuildVsTestRunnerPool(options, out var runner, myTestCases);

            var tester = BuildMutationTestProcess(runner, options, targetProject: project);
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

            var testFunc = () => runner.TestMultipleMutants(new TimeoutValueCalculator(0), mutants, null);

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
            analyzer.DetermineTestCoverage(runner, new[] { Mutant, OtherMutant }, TestsGuidList.NoTest());

            SetupMockPartialTestRun(mockVsTest, new Dictionary<string, string> { ["0"] = "T0=F", ["1"] = "T0=S" });
            var result = runner.TestMultipleMutants(null, new[] { OtherMutant }, null);
            result.FailingTests.IsEmpty.ShouldBeTrue();
            result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
            result.FailingTests.IsEmpty.ShouldBeFalse();
        }

        [Fact]
        public void HandleMultipleTestResults()
        {
            var options = new StrykerOptions();
            var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
            // assume 2 results for T0
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) });
            var result = runner.InitialTest();
            // initial test is fine
            result.FailingTests.IsEmpty.ShouldBeTrue();
            // test session will fail
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T0", true), ("T1", true) });
            result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
            result.FailingTests.IsEmpty.ShouldBeTrue();
            result.RanTests.IsEveryTest.ShouldBeTrue();
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
            var result = runner.InitialTest();
            // the duration should be less than 3 times the (test) default duration
            result.Duration.ShouldBeLessThan(_testDefaultDuration.Duration() * 3);
        }

        [Fact]
        public void HandleFailureWithinMultipleTestResults()
        {
            var options = new StrykerOptions();
            var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
            // assume 2 results for T0
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) });
            var result = runner.InitialTest();
            // initial test is fine
            result.FailingTests.IsEmpty.ShouldBeTrue();
            // test session will fail on test 1
            SetupMockTestRun(mockVsTest, new[] { ("T0", false), ("T0", true), ("T1", true) });

            result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
            result.RanTests.IsEveryTest.ShouldBeTrue();
            result.FailingTests.IsEmpty.ShouldBeFalse();
            result.FailingTests.GetGuids().ShouldContain(TestCases[0].Id);
            // test session will fail on the other test result
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T0", false), ("T1", true) });
            result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
            result.RanTests.IsEveryTest.ShouldBeTrue();
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
            var result = runner.InitialTest();
            // initial test is fine
            result.FailingTests.IsEmpty.ShouldBeTrue();
            // test session will fail
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true) });
            result = runner.TestMultipleMutants(null, new[] { Mutant }, null);

            result.FailingTests.IsEmpty.ShouldBeTrue();
            result.TimedOutTests.Count.ShouldBe(1);
            result.TimedOutTests.GetGuids().ShouldContain(TestCases[0].Id);
            result.RanTests.IsEveryTest.ShouldBeFalse();
        }

        [Fact]
        public void HandleFailureWhenExtraMultipleTestResults()
        {
            var options = new StrykerOptions();
            var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
            // assume 2 results for T0
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T0", true) });
            var result = runner.InitialTest();
            // initial test is fine
            result.FailingTests.IsEmpty.ShouldBeTrue();
            // test session will fail
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T0", true), ("T0", false), ("T1", true) });
            result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
            result.RanTests.IsEveryTest.ShouldBeTrue();
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
            var result = runner.InitialTest();
            // initial test is fine
            result.FailingTests.IsEmpty.ShouldBeTrue();
            // test session will fail
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T2", true), ("T1", true), ("T0", true) });
            result = runner.TestMultipleMutants(null, new[] { Mutant }, null);
            result.RanTests.IsEveryTest.ShouldBeTrue();
            result.FailingTests.IsEmpty.ShouldBeTrue();
        }

        [Fact]
        public void HandleUnexpectedTestCase()
        {
            var options = new StrykerOptions();
            var mockVsTest = BuildVsTestRunnerPool(options, out var runner);
            // assume 2 results for T0
            SetupMockTestRun(mockVsTest, new[] { ("T0", true), ("T1", true), ("T2", true) });
            var result = runner.InitialTest();
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
            analyzer.DetermineTestCoverage(runner, new[] { Mutant, OtherMutant }, TestsGuidList.NoTest());
            // the suspicious mutant should be tested against all tests
            OtherMutant.CoveringTests.IsEveryTest.ShouldBe(true);
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
            analyzer.DetermineTestCoverage(runner, new[] { Mutant, OtherMutant }, TestsGuidList.NoTest());
            // the suspicious mutant should be tested against all tests
            OtherMutant.CoveringTests.Count.ShouldBe(2);
            Mutant.CoveringTests.Count.ShouldBe(1);
        }
    }
}