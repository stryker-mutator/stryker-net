using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.Core.CoverageAnalysis;
using Stryker.Fsharp.UnitTest.Fixtures;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;

namespace Stryker.Fsharp.UnitTest;

/// <summary>Coverage contract between F# mutants and the runners: the compiled helper type, no runner involvement.</summary>
[TestClass]
public class FsharpCoverageTests
{
    [TestMethod]
    public void FsharpCoverage_PerTest_UncoveredMutantIsNoCoverage()
    {
        using var project = new MutatedFsharpProject("module Lib\nlet a () = true\nlet b () = true\n");
        var mutants = project.Leaf.Mutants.OrderBy(m => m.Id).ToList();
        mutants.Count.ShouldBe(2);
        var (mutantA, mutantB) = (mutants[0], mutants[1]);
        var loaded = project.Load();

        // what the VsTest data collector does around a test: capture on, run, read the data
        loaded.Set("CaptureCoverage", true);
        loaded.Call("Lib", "a");
        var data = loaded.CoverageData();

        data[0].ShouldBe([mutantA.Id]);
        data[1].ShouldBeEmpty("a function body is not a static initialiser");

        // the runner-agnostic analyser turns that per-test coverage into statuses
        var coverage = CoverageRunResult.Create("TestA", CoverageConfidence.Exact, data[0], data[1], []);
        var runner = new Mock<ITestRunner>();
        runner.Setup(r => r.CaptureCoverage(It.IsAny<IProjectAndTests>())).Returns([coverage]);
        var options = new Mock<IStrykerOptions>();
        options.Setup(o => o.OptimizationMode).Returns(OptimizationModes.CoverageBasedTest | OptimizationModes.SkipUncoveredMutants);

        new CoverageAnalyser(NullLogger<CoverageAnalyser>.Instance)
            .DetermineTestCoverage(options.Object, Mock.Of<IProjectAndTests>(), runner.Object, mutants, TestIdentifierList.NoTest());

        mutantB.ResultStatus.ShouldBe(MutantStatus.NoCoverage);
        mutantA.ResultStatus.ShouldBe(MutantStatus.Pending);
        mutantA.CoveringTests.GetIdentifiers().ShouldBe(["TestA"]);
    }

    [TestMethod]
    public void FsharpModuleValue_Mutant_DetectedAsStatic()
    {
        using var project = new MutatedFsharpProject("module Lib\nlet answer = true\nlet f () = true\n");
        var mutants = project.Leaf.Mutants.OrderBy(m => m.Id).ToList();
        var (value, function) = (mutants[0], mutants[1]);

        value.IsStaticValue.ShouldBeTrue("module-level let is a static initialiser");
        function.IsStaticValue.ShouldBeFalse("function bodies run per call");

        // forcing the module initialiser while capturing reports the mutant in the static list
        var loaded = project.Load();
        loaded.Set("CaptureCoverage", true);
        loaded.Call("Lib", "get_answer");
        var data = loaded.CoverageData();
        data[0].ShouldContain(value.Id);
        data[1].ShouldBe([value.Id]);
    }
}
