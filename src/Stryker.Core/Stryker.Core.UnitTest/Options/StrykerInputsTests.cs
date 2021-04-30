using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Stryker.Core.Options;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class StrykerInputsTests
    {
        private StrykerInputs _target = new StrykerInputs()
        {

            AdditionalTimeoutMsInput = new AdditionalTimeoutMsInput(),
            AzureFileStorageSasInput = new AzureFileStorageSasInput(),
            AzureFileStorageUrlInput = new AzureFileStorageUrlInput(),
            BaselineProviderInput = new BaselineProviderInput(),
            BasePathInput = new BasePathInput() { SuppliedInput = Directory.GetCurrentDirectory() },
            ConcurrencyInput = new ConcurrencyInput(),
            DashboardApiKeyInput = new DashboardApiKeyInput(),
            DashboardUrlInput = new DashboardUrlInput(),
            DevModeInput = new DevModeInput(),
            DiffIgnoreFilePatternsInput = new DiffIgnoreFilePatternsInput(),
            DisableBailInput = new DisableBailInput(),
            DisableMixMutantsInput = new DisableMixMutantsInput(),
            ExcludedMutationsInput = new IgnoreMutationsInput(),
            FallbackVersionInput = new FallbackVersionInput(),
            IgnoredMethodsInput = new IgnoreMethodsInput(),
            LanguageVersionInput = new LanguageVersionInput(),
            VerbosityInput = new VerbosityInput(),
            LogToFileInput = new LogToFileInput(),
            ModuleNameInput = new ModuleNameInput(),
            MutateInput = new MutateInput(),
            MutationLevelInput = new MutationLevelInput(),
            CoverageAnalysisInput = new CoverageAnalysisInput(),
            OutputPathInput = new OutputPathInput(),
            ProjectNameInput = new ProjectNameInput(),
            ProjectUnderTestNameInput = new ProjectUnderTestNameInput(),
            ProjectVersionInput = new ProjectVersionInput(),
            ReportersInput = new ReportersInput(),
            SinceInput = new SinceInput(),
            SinceTargetInput = new SinceTargetInput(),
            SolutionPathInput = new SolutionPathInput(),
            TestProjectsInput = new TestProjectsInput(),
            ThresholdBreakInput = new ThresholdBreakInput(),
            ThresholdHighInput = new ThresholdHighInput(),
            ThresholdLowInput = new ThresholdLowInput(),
            WithBaselineInput = new WithBaselineInput()
        };

        [Fact]
        public void ShouldSetOptimisationFlagsPerTestInIsolation()
        {
            _target.CoverageAnalysisInput.SuppliedInput = "perTestInIsolation";

            var result = _target.ValidateAll();
            
            result.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest).ShouldBeTrue();
            result.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest).ShouldBeTrue();
        }

        [Fact]
        public void ShouldSetOptimisationFlagsDisableMixMutants()
        {
            _target.DisableMixMutantsInput.SuppliedInput = true;

            var result = _target.ValidateAll();
            
            result.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest).ShouldBeTrue();
            result.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest).ShouldBeTrue();
        }

        [Fact]
        public void ShouldSetOptimisationFlagsDisableBail()
        {
            _target.DisableBailInput.SuppliedInput = true;

            var result = _target.ValidateAll();
            
            result.OptimizationMode.HasFlag(OptimizationModes.CoverageBasedTest).ShouldBeTrue();
            result.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest).ShouldBeTrue();
        }

        [Fact]
        public void ShouldSetOptimisationFlagsAll()
        {
            _target.CoverageAnalysisInput.SuppliedInput = "all";

            var result = _target.ValidateAll();

            result.OptimizationMode.HasFlag(OptimizationModes.SkipUncoveredMutants).ShouldBeTrue();
        }

        [Fact]
        public void ShouldSetOptimisationFlagsOff()
        {
            _target.CoverageAnalysisInput.SuppliedInput = "off";

            var result = _target.ValidateAll();

            result.OptimizationMode.HasFlag(OptimizationModes.NoOptimization).ShouldBeTrue();
        }

        [Fact]
        public void ShouldSetOptimisationFlagsDefault()
        {
            _target.CoverageAnalysisInput.SuppliedInput = null;

            var result = _target.ValidateAll();

            result.OptimizationMode.HasFlag(OptimizationModes.CaptureCoveragePerTest).ShouldBeTrue();
        }
    }
}
