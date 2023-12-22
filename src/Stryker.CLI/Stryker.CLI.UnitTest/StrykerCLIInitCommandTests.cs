using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text.Json;
using Moq;
using Shouldly;
using Spectre.Console.Testing;
using Stryker.CLI.Clients;
using Stryker.CLI.Logging;
using Stryker.Core;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.CLI.UnitTest;

public class StrykerCLIInitCommandTests
{
    private readonly StrykerCli _target;
    private readonly Mock<IStrykerRunner> _strykerRunnerMock = new(MockBehavior.Strict);
    private readonly Mock<IStrykerNugetFeedClient> _nugetClientMock = new(MockBehavior.Strict);
    private readonly Mock<ILoggingInitializer> _loggingInitializerMock = new();
    private readonly IFileSystem _fileSystemMock = new MockFileSystem();
    private readonly TestConsole _consoleMock = new TestConsole().EmitAnsiSequences();

    public StrykerCLIInitCommandTests()
    {
        _target = new StrykerCli(_strykerRunnerMock.Object, null, _loggingInitializerMock.Object, _nugetClientMock.Object, _consoleMock, _fileSystemMock);
    }

    [Fact]
    public void Init()
    {
        _target.Run(new[] { "init" });

        _strykerRunnerMock.VerifyAll();

        _fileSystemMock.File.Exists("stryker-config.json").ShouldBeTrue();
        var configFile = _fileSystemMock.File.ReadAllText("stryker-config.json");
        var config = JsonSerializer.Deserialize<FileBasedInputOuter>(configFile).Input;

        config.AdditionalTimeout.ShouldBe(new AdditionalTimeoutInput().Default);
        config.Verbosity.ShouldBe(new VerbosityInput().Default);
        config.Project.ShouldBe(new ProjectNameInput().Default);
        config.Reporters.ShouldBe(new ReportersInput().Default);
        config.Concurrency.ShouldBe(new ConcurrencyInput().Default);
        config.Thresholds.Break.ShouldBe(new ThresholdBreakInput().Default);
        config.Thresholds.Low.ShouldBe(new ThresholdLowInput().Default);
        config.Thresholds.High.ShouldBe(new ThresholdHighInput().Default);
        config.Mutate.ShouldBe(new MutateInput().Default);
        config.MutationLevel.ShouldBe(new MutationLevelInput().Default);
        config.CoverageAnalysis.ShouldBe(new CoverageAnalysisInput().Default);
        config.DisableBail.ShouldBe(new DisableBailInput().Default);
        config.IgnoreMutations.ShouldBe(new IgnoreMutationsInput().Default);
        config.IgnoreMethods.ShouldBe(new IgnoreMethodsInput().Default);
        config.TestCaseFilter.ShouldBe(new TestCaseFilterInput().Default);
        config.TestProjects.ShouldBe(new TestProjectsInput().Default);
        config.DashboardUrl.ShouldBe(new DashboardUrlInput().Default);
        config.BreakOnInitialTestFailure.ShouldBe(new BreakOnInitialTestFailureInput().Default);
    }

    [Theory]
    [InlineData("init", "--config-file", "test.json")]
    [InlineData("init", "-f", "test.json")]
    public void InitCustomPath(params string[] args)
    {
        _target.Run(args);

        _strykerRunnerMock.VerifyAll();

        _fileSystemMock.File.Exists(args.Last()).ShouldBeTrue();
    }

    [Fact]
    public void InitOverwrite()
    {
        // make sure the file exists before calling init
        _fileSystemMock.File.WriteAllText("stryker-config.json", "test");
        // deny overwrite
        _consoleMock.Input.PushKey(ConsoleKey.Enter);

        _target.Run(new[] { "init" });

        _strykerRunnerMock.VerifyAll();

        _fileSystemMock.File.Exists("stryker-config.json").ShouldBeTrue();
        var configFile = _fileSystemMock.File.ReadAllText("stryker-config.json");
        configFile.ShouldBe("test");
    }

    [Fact]
    public void InitOverwriteConfirm()
    {
        // make sure the file exists before calling init
        _fileSystemMock.File.WriteAllText("stryker-config.json", "test");
        // confirm overwrite
        _consoleMock.Input.PushKey(ConsoleKey.Y);
        _consoleMock.Input.PushKey(ConsoleKey.Enter);

        _target.Run(new[] { "init" });

        _strykerRunnerMock.VerifyAll();

        _fileSystemMock.File.Exists("stryker-config.json").ShouldBeTrue();
        var configFile = _fileSystemMock.File.ReadAllText("stryker-config.json");
        var config = JsonSerializer.Deserialize<FileBasedInputOuter>(configFile).Input;
        config.ShouldNotBeNull();
    }

    [Fact]
    public void InitOverride()
    {
        _target.Run(new[] { "init",
            "--verbosity", "debug",
            "--project", "testProject",
            "--reporter", "dots",
            "--concurrency", "1",
            "--break-at", "10",
            "--threshold-low", "20",
            "--threshold-high", "30",
            "--mutate", "test*.cs",
            "--mutation-level", "advanced",
            "--disable-bail",
            "--test-project", "testProject",
            "--break-on-initial-test-failure"
        });

        _strykerRunnerMock.VerifyAll();

        _fileSystemMock.File.Exists("stryker-config.json").ShouldBeTrue();
        var configFile = _fileSystemMock.File.ReadAllText("stryker-config.json");
        var config = JsonSerializer.Deserialize<FileBasedInputOuter>(configFile).Input;

        config.Verbosity.ShouldBe("debug");
        config.Project.ShouldBe("testProject");
        config.Reporters.ShouldHaveSingleItem().ShouldBe("dots");
        config.Concurrency.ShouldBe(1);
        config.Thresholds.Break.ShouldBe(10);
        config.Thresholds.Low.ShouldBe(20);
        config.Thresholds.High.ShouldBe(30);
        config.Mutate.ShouldHaveSingleItem().ShouldBe("test*.cs");
        config.MutationLevel.ShouldBe("advanced");
        config.DisableBail.ShouldBe(true);
        config.TestProjects.ShouldHaveSingleItem().ShouldBe("testProject");
        config.BreakOnInitialTestFailure.ShouldBe(true);
    }
}
