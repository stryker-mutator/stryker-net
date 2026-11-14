using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Validation;

/// <summary>
/// Faster in-repo regression for stryker-mutator/stryker-net#3832 (MTP kill count at concurrency &gt; 1).
/// Uses <c>MtpParallelAttributionMini</c> (LaTeX-repro-shaped: multi-file library, two test classes).
/// </summary>
public class Stryker3832FastTests
{
    /// <summary>
    /// Calibrated on the Release CLI with the default mutation level: 73 killed and 259 survived of 332 tested
    /// mutants, identical at concurrency 1 and 8. The lower bound sits above the 53 kills seen when mutants in
    /// static code were not given a fresh host. A mutant that breaks the static initializer in
    /// <c>DateFormatting</c> poisons a reused test host; without the host recycling every later mutant is
    /// reported as killed (about 320 at concurrency 1), so the upper bound catches false kills that parity
    /// between concurrency levels alone could miss.
    /// </summary>
    private const int MinimumExpectedKilled = 58;

    private const int MaximumExpectedKilled = 75;

    private const int ParallelConcurrency = 8;

    /// <summary>
    /// A run takes about 40 s at concurrency 1 and 25 s at concurrency 8. A run that needs minutes is a performance
    /// regression (for example per-call file access in the injected mutant control), not a slow machine.
    /// </summary>
    private static readonly TimeSpan StrykerRunTimeout = TimeSpan.FromMinutes(3);

    [Fact]
    [Trait("Category", "Stryker3832Fast")]
    // XUnitMTP + netcore is the CI job category that runs this fixture (integration-tests.ps1 filters on Category and Runtime).
    [Trait("Category", "XUnitMTP")]
    [Trait("Runtime", "netcore")]
    public Task MtpConcurrencyEight_ShouldMatchConcurrencyOneKillCount_OnMiniFixture() =>
        AssertConcurrencyParityAsync("stryker-config.json");

    /// <summary>
    /// Same checks as the default-mode test, in <c>perTestInIsolation</c>: the isolated capture must also give the same
    /// result whatever the concurrency, and a kill count in the calibrated range.
    /// </summary>
    [Fact]
    [Trait("Category", "Stryker3832Fast")]
    [Trait("Category", "XUnitMTP")]
    [Trait("Runtime", "netcore")]
    public Task MtpPerTestInIsolation_ConcurrencyEight_ShouldMatchConcurrencyOneKillCount_OnMiniFixture() =>
        AssertConcurrencyParityAsync("stryker-config.isolated.json");

    /// <summary>
    /// <c>all</c> mode is a covered mode, so the warm-up applies and the kill count must match the other covered
    /// modes (a looser bound here would let a regression that drops the warm-up pass: it drove the count to 161
    /// while parity still held).
    /// </summary>
    [Fact]
    [Trait("Category", "Stryker3832Fast")]
    [Trait("Category", "XUnitMTP")]
    [Trait("Runtime", "netcore")]
    public Task MtpAllCoverageAnalysis_ConcurrencyEight_ShouldMatchConcurrencyOneKillCount_OnMiniFixture() =>
        AssertConcurrencyParityAsync("stryker-config.all.json");

    /// <summary>
    /// <c>off</c> mode has no warm-up and no coverage data, so a poisoned host would make the kill count depend on the
    /// concurrency. Parity is the only assertion (see configuration docs).
    /// </summary>
    [Fact]
    [Trait("Category", "Stryker3832Fast")]
    [Trait("Category", "XUnitMTP")]
    [Trait("Runtime", "netcore")]
    public Task MtpOffCoverageAnalysis_ConcurrencyEight_ShouldMatchConcurrencyOneKillCount_OnMiniFixture() =>
        AssertConcurrencyParityAsync("stryker-config.off.json");

    /// <summary>
    /// Asserts concurrency 1 and 8 yield the same kill count for <paramref name="configFileName"/>, within the
    /// calibrated bounds (the same for every mode: the covered modes are all protected, and <c>off</c> measures
    /// the same count on this fixture).
    /// </summary>
    private static async Task AssertConcurrencyParityAsync(string configFileName, int maximumExpectedKilled = MaximumExpectedKilled)
    {
        var miniRoot = StrykerMtpConcurrencyKillCountTestSupport.FindDirectoryUnderRepository(
            "integrationtest",
            "TargetProjects",
            "MicrosoftTestPlatform",
            "MtpParallelAttributionMini");
        miniRoot.ShouldNotBeNullOrWhiteSpace();

        var testProject = Path.Combine(miniRoot, "MtpParallelAttributionMini.Tests");
        var config = Path.Combine(testProject, configFileName);
        Directory.Exists(testProject).ShouldBeTrue();
        File.Exists(config).ShouldBeTrue();

        var cliDll = typeof(Stryker.CLI.Program).Assembly.Location;
        File.Exists(cliDll).ShouldBeTrue();

        await StrykerMtpConcurrencyKillCountTestSupport.EnsureProjectBuiltAsync(
            miniRoot,
            Path.Combine("MtpParallelAttributionMini.Tests", "MtpParallelAttributionMini.Tests.csproj"),
            "MtpParallelAttributionMini.Tests.dll");

        var solutionPath = Path.Combine(miniRoot, "MtpParallelAttributionMini.slnx");
        File.Exists(solutionPath).ShouldBeTrue();

        var atOne = await StrykerMtpConcurrencyKillCountTestSupport.RunStrykerAndGetCountsAsync(
            cliDll,
            testProject,
            config,
            concurrency: 1,
            StrykerRunTimeout,
            solutionPath);

        var atParallel = await StrykerMtpConcurrencyKillCountTestSupport.RunStrykerAndGetCountsAsync(
            cliDll,
            testProject,
            config,
            concurrency: ParallelConcurrency,
            StrykerRunTimeout,
            solutionPath);

        // A timed-out mutant is reported as killed, so slow test sessions would otherwise pass unnoticed.
        atOne.Timeout.ShouldBe(0);
        atParallel.Timeout.ShouldBe(0);
        atOne.Killed.ShouldBeGreaterThan(MinimumExpectedKilled);
        atOne.Killed.ShouldBeLessThan(maximumExpectedKilled);
        atParallel.CoveredVerdicts.ShouldBe(atOne.CoveredVerdicts);
        atParallel.Killed.ShouldBe(atOne.Killed);
    }

    private static MutationStatusCounts Sum(System.Collections.Generic.IEnumerable<MutationStatusCounts> counts)
    {
        var killed = 0;
        var survived = 0;
        var noCoverage = 0;
        var timeout = 0;
        foreach (var count in counts)
        {
            killed += count.Killed;
            survived += count.Survived;
            noCoverage += count.NoCoverage;
            timeout += count.Timeout;
        }

        return new MutationStatusCounts(killed, survived, noCoverage, timeout);
    }

    /// <summary>
    /// Mutants reached only through a static initializer (<c>StaticOnlyHelper</c>) are not covered lexically, so the
    /// isolated capture has to report them as static for them to run on a fresh host. Without that they survive in
    /// isolated mode and are killed in the default mode.
    /// </summary>
    [Fact]
    [Trait("Category", "Stryker3832Fast")]
    [Trait("Category", "XUnitMTP")]
    [Trait("Runtime", "netcore")]
    public async Task MtpPerTestInIsolation_ShouldMatchPerTestKillCount_OnMiniFixture()
    {
        var miniRoot = StrykerMtpConcurrencyKillCountTestSupport.FindDirectoryUnderRepository(
            "integrationtest",
            "TargetProjects",
            "MicrosoftTestPlatform",
            "MtpParallelAttributionMini");
        miniRoot.ShouldNotBeNullOrWhiteSpace();

        var testProject = Path.Combine(miniRoot, "MtpParallelAttributionMini.Tests");
        var perTestConfig = Path.Combine(testProject, "stryker-config.json");
        var isolatedConfig = Path.Combine(testProject, "stryker-config.isolated.json");
        File.Exists(perTestConfig).ShouldBeTrue();
        File.Exists(isolatedConfig).ShouldBeTrue();

        var cliDll = typeof(Stryker.CLI.Program).Assembly.Location;
        File.Exists(cliDll).ShouldBeTrue();

        await StrykerMtpConcurrencyKillCountTestSupport.EnsureProjectBuiltAsync(
            miniRoot,
            Path.Combine("MtpParallelAttributionMini.Tests", "MtpParallelAttributionMini.Tests.csproj"),
            "MtpParallelAttributionMini.Tests.dll");

        var solutionPath = Path.Combine(miniRoot, "MtpParallelAttributionMini.slnx");
        File.Exists(solutionPath).ShouldBeTrue();

        var perTest = await StrykerMtpConcurrencyKillCountTestSupport.RunStrykerAndGetCountsAsync(
            cliDll, testProject, perTestConfig, ParallelConcurrency, StrykerRunTimeout, solutionPath);
        var isolated = await StrykerMtpConcurrencyKillCountTestSupport.RunStrykerAndGetCountsAsync(
            cliDll, testProject, isolatedConfig, ParallelConcurrency, StrykerRunTimeout, solutionPath);

        isolated.Timeout.ShouldBe(0);
        isolated.Killed.ShouldBeGreaterThan(MinimumExpectedKilled);
        isolated.CoveredVerdicts.ShouldBe(perTest.CoveredVerdicts);
        isolated.Killed.ShouldBe(perTest.Killed);
    }
}
