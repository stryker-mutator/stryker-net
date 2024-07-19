using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Moq;
using Shouldly;
using Stryker.CLI.CommandLineConfig;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.CLI.UnitTest
{
    [TestClass]
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

        [TestMethod]
        public void InvalidConfigFile_ShouldThrowInputException()
        {
            var args = new[] { "-f", "invalidconfig.json" };

            var reader = new ConfigBuilder();

            var exception = Should.Throw<InputException>(() => reader.Build(_inputs.Object, args, _app, _cmdConfigHandler));
            exception.Message.ShouldStartWith("Config file not found");
        }

        [TestMethod]
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

        [TestMethod]
        public void ValidDefaultConfigFile_ShouldParseConfigFile()
        {
            var args = new string[] { };

            var reader = new ConfigBuilder();

            reader.Build(_inputs.Object, args, _app, _cmdConfigHandler);

            VerifyConfigFileDeserialized(Times.Once());
        }

        [TestMethod]
        public void ValidUserConfigFileWithDefault_ShouldParseUserConfig()
        {
            string[] args = ["-f", $"ConfigFiles{Path.DirectorySeparatorChar}UserConfigWithDefault{Path.DirectorySeparatorChar}custom_config.json"];

            var reader = new ConfigBuilder();

            reader.Build(_inputs.Object, args, _app, _cmdConfigHandler);

            VerifyConfigFileDeserialized(Times.Once());
            _inputs.Object.ModuleNameInput.Validate().ShouldBe("custom");
        }

        [TestMethod]
        public void ValidDefaultYmlFile_ShouldParse()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            // Set directory to test folder containing single config yml
            Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "ConfigFiles", "SingleDefaultYml"));

            string[] args = [];

            var reader = new ConfigBuilder();
            reader.Build(_inputs.Object, args, _app, _cmdConfigHandler);

            VerifyConfigFileDeserialized(Times.Once());
            _inputs.Object.ModuleNameInput.Validate().ShouldBe("hello_from_yml");

            // Reset current directory to original folder
            Directory.SetCurrentDirectory(currentDirectory);
        }

        [TestMethod]
        public void ValidDefaultYamlFile_ShouldParse()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            // Set directory to test folder containing single config yml
            Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "ConfigFiles", "SingleDefaultYaml"));

            string[] args = [];

            var reader = new ConfigBuilder();
            reader.Build(_inputs.Object, args, _app, _cmdConfigHandler);

            VerifyConfigFileDeserialized(Times.Once());
            _inputs.Object.ModuleNameInput.Validate().ShouldBe("hello_from_yaml");

            // Reset current directory to original folder
            Directory.SetCurrentDirectory(currentDirectory);
        }

        [TestMethod]
        public void MultipleDefaultConfigsWithJson_ShouldParseJsonConfig()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            // Set directory to test folder containing multiple default files
            Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "ConfigFiles", "MultipleDefaultWithJson"));

            string[] args = [];

            var reader = new ConfigBuilder();
            reader.Build(_inputs.Object, args, _app, _cmdConfigHandler);

            VerifyConfigFileDeserialized(Times.Once());
            _inputs.Object.ModuleNameInput.Validate().ShouldBe("hello_from_json");

            // Reset current directory to original folder
            Directory.SetCurrentDirectory(currentDirectory);
        }

        [TestMethod]
        public void TwoDefaultConfigsWithYml_ShouldParseYmlConfig()
        {
            var currentDirectory = Directory.GetCurrentDirectory();

            // Set directory to test folder containing two default files
            Directory.SetCurrentDirectory(Path.Combine(currentDirectory, "ConfigFiles", "TwoWithYml"));

            string[] args = [];

            var reader = new ConfigBuilder();
            reader.Build(_inputs.Object, args, _app, _cmdConfigHandler);

            VerifyConfigFileDeserialized(Times.Once());
            _inputs.Object.ModuleNameInput.Validate().ShouldBe("hello_from_yml");

            // Reset current directory to original folder
            Directory.SetCurrentDirectory(currentDirectory);
        }

        private void VerifyConfigFileDeserialized(Times time) => _inputs.VerifyGet(x => x.CoverageAnalysisInput, time);

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
            inputs.Setup(x => x.ConfigurationInput).Returns(new ConfigurationInput());
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
}
