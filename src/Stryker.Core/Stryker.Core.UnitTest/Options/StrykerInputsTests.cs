using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class StrykerInputsTests
    {
        [Fact]
        public void ShouldHaveCorrectDefaults()
        {
            var strykerInputs = new StrykerInputs
            {
                AdditionalTimeoutMsInput = new AdditionalTimeoutMsInput(),
                AzureFileStorageSasInput = new AzureFileStorageSasInput(),
                AzureFileStorageUrlInput = new AzureFileStorageUrlInput(),
                BaselineProviderInput = new BaselineProviderInput(),
                BasePathInput = new BasePathInput(),
                ConcurrencyInput = new ConcurrencyInput(),
                DashboardApiKeyInput = new DashboardApiKeyInput(),
                DashboardUrlInput = new DashboardUrlInput(),
                DevModeInput = new DevModeInput(),
                DiffIgnoreFilePatternsInput = new DiffIgnoreFilePatternsInput(),
                DisableAbortTestOnFailInput = new DisableAbortTestOnFailInput(),
                DisableSimultaneousTestingInput = new DisableSimultaneousTestingInput(),
                ExcludedMutatorsInput = new ExcludedMutatorsInput(),
                FallbackVersionInput = new FallbackVersionInput(),
                IgnoredMethodsInput = new IgnoredMethodsInput(),
                LanguageVersionInput = new LanguageVersionInput(),
                VerbosityInput = new VerbosityInput(),
                LogToFileInput = new LogToFileInput(),
                ModuleNameInput = new ModuleNameInput(),
                MutateInput = new MutateInput(),
                MutationLevelInput = new MutationLevelInput(),
                OptimizationModeInput = new OptimizationModeInput(),
                OutputPathInput = new OutputPathInput(),
                ProjectNameInput = new ProjectNameInput(),
                ProjectUnderTestNameInput = new ProjectUnderTestNameInput(),
                ProjectVersionInput = new ProjectVersionInput(),
                ReportersInput = new ReportersInput(),
                SinceInput = new SinceInput(),
                SinceBranchInput = new SinceBranchInput(),
                SinceCommitInput = new SinceCommitInput(),
                SolutionPathInput = new SolutionPathInput(),
                TestProjectsInput = new TestProjectsInput(),
                ThresholdBreakInput = new ThresholdBreakInput(),
                ThresholdHighInput = new ThresholdHighInput(),
                ThresholdLowInput = new ThresholdLowInput(),
                WithBaselineInput = new WithBaselineInput()
            };

            var options = strykerInputs.Validate();
            options.Thresholds.High.ShouldBe(60);
            options.Thresholds.Low.ShouldBe(60);
            options.Thresholds.Break.ShouldBe(50);
        }

        [Fact]
        public void ShouldSetValues()
        {
            var strykerInputs = new StrykerInputs
            {
                AdditionalTimeoutMsInput = new AdditionalTimeoutMsInput(),
                AzureFileStorageSasInput = new AzureFileStorageSasInput(),
                AzureFileStorageUrlInput = new AzureFileStorageUrlInput(),
                BaselineProviderInput = new BaselineProviderInput(),
                BasePathInput = new BasePathInput(),
                ConcurrencyInput = new ConcurrencyInput(),
                DashboardApiKeyInput = new DashboardApiKeyInput(),
                DashboardUrlInput = new DashboardUrlInput(),
                DevModeInput = new DevModeInput(),
                DiffIgnoreFilePatternsInput = new DiffIgnoreFilePatternsInput(),
                DisableAbortTestOnFailInput = new DisableAbortTestOnFailInput(),
                DisableSimultaneousTestingInput = new DisableSimultaneousTestingInput(),
                ExcludedMutatorsInput = new ExcludedMutatorsInput(),
                FallbackVersionInput = new FallbackVersionInput(),
                IgnoredMethodsInput = new IgnoredMethodsInput(),
                LanguageVersionInput = new LanguageVersionInput(),
                VerbosityInput = new VerbosityInput(),
                LogToFileInput = new LogToFileInput(),
                ModuleNameInput = new ModuleNameInput(),
                MutateInput = new MutateInput(),
                MutationLevelInput = new MutationLevelInput(),
                OptimizationModeInput = new OptimizationModeInput(),
                OutputPathInput = new OutputPathInput(),
                ProjectNameInput = new ProjectNameInput(),
                ProjectUnderTestNameInput = new ProjectUnderTestNameInput(),
                ProjectVersionInput = new ProjectVersionInput(),
                ReportersInput = new ReportersInput(),
                SinceInput = new SinceInput(),
                SinceBranchInput = new SinceBranchInput(),
                SinceCommitInput = new SinceCommitInput(),
                SolutionPathInput = new SolutionPathInput(),
                TestProjectsInput = new TestProjectsInput(),
                ThresholdBreakInput = new ThresholdBreakInput(),
                ThresholdHighInput = new ThresholdHighInput(),
                ThresholdLowInput = new ThresholdLowInput(),
                WithBaselineInput = new WithBaselineInput()
            };

            strykerInputs.AdditionalTimeoutMsInput.SuppliedInput =
            strykerInputs.AzureFileStorageSasInput.SuppliedInput =
            strykerInputs.AzureFileStorageUrlInput.SuppliedInput =
            strykerInputs.BaselineProviderInput.SuppliedInput =
            strykerInputs.BasePathInput.SuppliedInput =
            strykerInputs.ConcurrencyInput.SuppliedInput =
            strykerInputs.DashboardApiKeyInput.SuppliedInput =
            strykerInputs.DashboardUrlInput.SuppliedInput =
            strykerInputs.DevModeInput.SuppliedInput =
            strykerInputs.DiffIgnoreFilePatternsInput.SuppliedInput =
            strykerInputs.DisableAbortTestOnFailInput.SuppliedInput =
            strykerInputs.DisableSimultaneousTestingInput.SuppliedInput =
            strykerInputs.ExcludedMutatorsInput.SuppliedInput =
            strykerInputs.FallbackVersionInput.SuppliedInput =
            strykerInputs.IgnoredMethodsInput.SuppliedInput =
            strykerInputs.LanguageVersionInput.SuppliedInput =
            strykerInputs.VerbosityInput.SuppliedInput =
            strykerInputs.LogToFileInput.SuppliedInput =
            strykerInputs.ModuleNameInput.SuppliedInput =
            strykerInputs.MutateInput.SuppliedInput =
            strykerInputs.MutationLevelInput.SuppliedInput =
            strykerInputs.OptimizationModeInput.SuppliedInput =
            strykerInputs.OutputPathInput.SuppliedInput =
            strykerInputs.ProjectNameInput.SuppliedInput =
            strykerInputs.ProjectUnderTestNameInput.SuppliedInput =
            strykerInputs.ProjectVersionInput.SuppliedInput =
            strykerInputs.ReportersInput.SuppliedInput =
            strykerInputs.SinceInput.SuppliedInput =
            strykerInputs.SinceBranchInput.SuppliedInput =
            strykerInputs.SinceCommitInput.SuppliedInput =
            strykerInputs.SolutionPathInput.SuppliedInput =
            strykerInputs.TestProjectsInput.SuppliedInput =
            strykerInputs.ThresholdBreakInput.SuppliedInput =
            strykerInputs.ThresholdHighInput.SuppliedInput =
            strykerInputs.ThresholdLowInput.SuppliedInput =
            strykerInputs.WithBaselineInput.SuppliedInput =

            var options = strykerInputs.Validate();
            options.Thresholds.High.ShouldBe(60);
            options.Thresholds.Low.ShouldBe(60);
            options.Thresholds.Break.ShouldBe(50);
        }
    }
}
