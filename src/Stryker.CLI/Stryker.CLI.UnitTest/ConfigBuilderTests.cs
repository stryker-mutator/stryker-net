using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.CLI.UnitTest;

public class ConfigBuilderTests
{
    private readonly Mock<IStrykerInputs> _inputs;
    private readonly CommandLineApplication _app;
    private readonly CommandLineConfigReader _cmdConfigHandler;

    public ConfigBuilderTests()
    {
        _inputs = GetMockInputs();
        _app = GetCommandLineApplication();

        _cmdConfigHandler = new CommandLineConfigReader();
        _cmdConfigHandler.RegisterCommandLineOptions(_app, _inputs.Object);
    }

    [Fact]
    public void InvalidConfigFile_ShouldThrowInputException()
    {
        var args = new[] { "-f", "invalidconfig.json" };

        var reader = new ConfigBuilder();

        var exception = Assert.Throws<InputException>(() => reader.Build(_inputs.Object, args, _app, _cmdConfigHandler));
        exception.Message.ShouldStartWith("Config file not found");
    }

    [Fact]
    public void InvalidDefaultConfigFile_ShouldNotThrowInputExceptionAndNotParseConfigFile()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory($"..{Path.DirectorySeparatorChar}");

        var args = new string[] { };

        var reader = new ConfigBuilder();

        reader.Build(_inputs.Object, args, _app, _cmdConfigHandler);

        VerifyConfigFileDeserialized(Times.Never());

        Directory.SetCurrentDirectory(currentDirectory);
    }

    [Fact]
    public void ValidDefaultConfigFile_ShouldParseConfigFile()
    {
        var args = new string[] { };

        var reader = new ConfigBuilder();

        reader.Build(_inputs.Object, args, _app, _cmdConfigHandler);

        VerifyConfigFileDeserialized(Times.Once());
    }

    private void VerifyConfigFileDeserialized(Times time) => _inputs.VerifyGet(x => x.BaselineProviderInput, time);

    private static CommandLineApplication GetCommandLineApplication()
    {
        var app = new CommandLineApplication
        {
            Name = "Stryker",
            FullName = "Stryker: Stryker mutator for .Net",
            Description = "Stryker mutator for .Net",
            ExtendedHelpText = "Welcome to Stryker for .Net! Run dotnet stryker to kick off a mutation test run",
            HelpTextGenerator = new GroupedHelpTextGenerator()
        };
        return app;
    }

    private static Mock<IStrykerInputs> GetMockInputs()
    {
        var inputs = new Mock<IStrykerInputs>();
        inputs.Setup(x => x.BasePathInput).Returns(new BasePathInput());

        inputs.Setup(x => x.ThresholdBreakInput).Returns(new ThresholdBreakInput());
        inputs.Setup(x => x.ThresholdHighInput).Returns(new ThresholdHighInput());
        inputs.Setup(x => x.ThresholdLowInput).Returns(new ThresholdLowInput());
        inputs.Setup(x => x.LogToFileInput).Returns(new LogToFileInput());
        inputs.Setup(x => x.VerbosityInput).Returns(new VerbosityInput());
        inputs.Setup(x => x.ConcurrencyInput).Returns(new ConcurrencyInput());
        inputs.Setup(x => x.SolutionInput).Returns(new SolutionInput());
        inputs.Setup(x => x.SourceProjectNameInput).Returns(new SourceProjectNameInput());
        inputs.Setup(x => x.TestProjectsInput).Returns(new TestProjectsInput());
        inputs.Setup(x => x.MsBuildPathInput).Returns(new MsBuildPathInput());
        inputs.Setup(x => x.MutateInput).Returns(new MutateInput());
        inputs.Setup(x => x.MutationLevelInput).Returns(new MutationLevelInput());
        inputs.Setup(x => x.SinceInput).Returns(new SinceInput());
        inputs.Setup(x => x.WithBaselineInput).Returns(new WithBaselineInput());
        inputs.Setup(x => x.OpenReportInput).Returns(new OpenReportInput());
        inputs.Setup(x => x.ReportersInput).Returns(new ReportersInput());
        inputs.Setup(x => x.ProjectVersionInput).Returns(new ProjectVersionInput());
        inputs.Setup(x => x.DashboardApiKeyInput).Returns(new DashboardApiKeyInput());
        inputs.Setup(x => x.AzureFileStorageSasInput).Returns(new AzureFileStorageSasInput());
        inputs.Setup(x => x.DevModeInput).Returns(new DevModeInput());

        inputs.Setup(x => x.SinceInput).Returns(new SinceInput());
        inputs.Setup(x => x.BaselineProviderInput).Returns(new BaselineProviderInput());
        inputs.Setup(x => x.DiffIgnoreChangesInput).Returns(new DiffIgnoreChangesInput());
        inputs.Setup(x => x.FallbackVersionInput).Returns(new FallbackVersionInput());
        inputs.Setup(x => x.AzureFileStorageUrlInput).Returns(new AzureFileStorageUrlInput());
        inputs.Setup(x => x.CoverageAnalysisInput).Returns(new CoverageAnalysisInput());
        inputs.Setup(x => x.DisableBailInput).Returns(new DisableBailInput());
        inputs.Setup(x => x.DisableMixMutantsInput).Returns(new DisableMixMutantsInput());
        inputs.Setup(x => x.AdditionalTimeoutInput).Returns(new AdditionalTimeoutInput());
        inputs.Setup(x => x.ProjectNameInput).Returns(new ProjectNameInput());
        inputs.Setup(x => x.ModuleNameInput).Returns(new ModuleNameInput());
        inputs.Setup(x => x.ReportersInput).Returns(new ReportersInput());
        inputs.Setup(x => x.SinceTargetInput).Returns(new SinceTargetInput());
        inputs.Setup(x => x.TargetFrameworkInput).Returns(new TargetFrameworkInput());
        inputs.Setup(x => x.LanguageVersionInput).Returns(new LanguageVersionInput());
        inputs.Setup(x => x.TestCaseFilterInput).Returns(new TestCaseFilterInput());
        inputs.Setup(x => x.DashboardUrlInput).Returns(new DashboardUrlInput());
        inputs.Setup(x => x.IgnoreMutationsInput).Returns(new IgnoreMutationsInput());
        inputs.Setup(x => x.IgnoredMethodsInput).Returns(new IgnoreMethodsInput());
        inputs.Setup(x => x.ReportFileNameInput).Returns(new ReportFileNameInput());
        inputs.Setup(x => x.BreakOnInitialTestFailureInput).Returns(new BreakOnInitialTestFailureInput());
        inputs.Setup(x => x.OutputPathInput).Returns(new OutputPathInput());

        return inputs;
    }
}
