using System;
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
    /// Calibrated on the Release CLI with the default mutation level: 67 killed and 256 survived of 323 tested mutants.
    /// The lower bound sits above the 53 kills seen when mutants in static code were not given a fresh host.
    /// A mutant that breaks the static initializer in <c>DateFormatting</c> poisons a reused test host; without the
    /// host recycling every later mutant is reported as killed (about 320 at concurrency 1), so the upper bound
    /// catches false kills that parity between concurrency levels alone could miss.
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
    /// <c>all</c> mode has no warm-up, so a poisoned host would make the kill count depend on the concurrency. Parity is
    /// asserted, not equality with <c>perTest</c>: mutants reached only through a static initializer are not killed in this mode.
    /// </summary>
    [Fact]
    [Trait("Category", "Stryker3832Fast")]
    [Trait("Category", "XUnitMTP")]
    [Trait("Runtime", "netcore")]
    public Task MtpAllCoverageAnalysis_ConcurrencyEight_ShouldMatchConcurrencyOneKillCount_OnMiniFixture() =>
        AssertConcurrencyParityAsync("stryker-config.all.json");

    /// <summary>
    /// <c>off</c> mode has no warm-up, so a poisoned host would make the kill count depend on the concurrency. Parity is
    /// asserted, not equality with <c>perTest</c>: mutants reached only through a static initializer are not killed in this mode.
    /// </summary>
    [Fact]
    [Trait("Category", "Stryker3832Fast")]
    [Trait("Category", "XUnitMTP")]
    [Trait("Runtime", "netcore")]
    public Task MtpOffCoverageAnalysis_ConcurrencyEight_ShouldMatchConcurrencyOneKillCount_OnMiniFixture() =>
        AssertConcurrencyParityAsync("stryker-config.off.json");

    /// <summary>
    /// Asserts concurrency 1 and 8 yield the same kill count for <paramref name="configFileName"/>.
    /// For <c>stryker-config.all.json</c> and <c>stryker-config.off.json</c> that is the only parity required;
    /// do not compare those kill counts to <c>perTest</c> (MTP static-initializer limitation; see configuration docs).
    /// </summary>
    private static async Task AssertConcurrencyParityAsync(string configFileName)
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
        atOne.Killed.ShouldBeLessThan(MaximumExpectedKilled);
        atParallel.CoveredVerdicts.ShouldBe(atOne.CoveredVerdicts);
        atParallel.Killed.ShouldBe(atOne.Killed);
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
