using System.Linq;
using System.Text.Json;
using Stryker.Core.Options;

namespace Stryker.CLI;

public static class FileConfigGenerator
{
    public static string GenerateConfigAsync(IStrykerInputs inputs)
    {
        var config = CreateConfig(inputs);
        return SerializeConfig(config);
    }

    private static FileBasedInputOuter CreateConfig(IStrykerInputs inputs)
    {
        return new FileBasedInputOuter
        {
            Input = new FileBasedInput
            {
                Baseline = new Baseline
                {
                    Enabled = inputs.WithBaselineInput.SuppliedInput ?? inputs.WithBaselineInput.Default,
                    Provider = inputs.BaselineProviderInput.SuppliedInput ?? inputs.BaselineProviderInput.Default,
                    FallbackVersion = inputs.FallbackVersionInput.SuppliedInput ?? inputs.FallbackVersionInput.Default,
                    AzureFileShareUrl = inputs.AzureFileStorageUrlInput.SuppliedInput ?? inputs.AzureFileStorageUrlInput.Default,
                },
                Since = new Since
                {
                    Enabled = inputs.SinceInput.SuppliedInput ?? inputs.SinceInput.Default,
                    IgnoreChangesIn = inputs.DiffIgnoreChangesInput.SuppliedInput?.ToArray() ?? inputs.DiffIgnoreChangesInput.Default.ToArray(),
                    Target = inputs.SinceTargetInput.SuppliedInput ?? inputs.SinceTargetInput.Default,
                },
                ProjectInfo = new ProjectInfo
                {
                    Name = inputs.ProjectNameInput.SuppliedInput ?? inputs.ProjectNameInput.Default,
                    Module = inputs.ModuleNameInput.SuppliedInput ?? inputs.ModuleNameInput.Default,
                    Version = inputs.ProjectVersionInput.SuppliedInput ?? inputs.ProjectVersionInput.Default,
                },
                Thresholds = new ThresholdsConfig
                {
                    Break = inputs.ThresholdBreakInput.SuppliedInput ?? inputs.ThresholdBreakInput.Default,
                    High = inputs.ThresholdHighInput.SuppliedInput ?? inputs.ThresholdHighInput.Default,
                    Low = inputs.ThresholdLowInput.SuppliedInput ?? inputs.ThresholdLowInput.Default,
                },
                CoverageAnalysis = inputs.CoverageAnalysisInput.SuppliedInput ?? inputs.CoverageAnalysisInput.Default,
                DisableBail = inputs.DisableBailInput.SuppliedInput ?? inputs.DisableBailInput.Default,
                DisableMixMutants = inputs.DisableMixMutantsInput.SuppliedInput ?? inputs.DisableMixMutantsInput.Default,
                AdditionalTimeout = inputs.AdditionalTimeoutInput.SuppliedInput ?? inputs.AdditionalTimeoutInput.Default,
                Mutate = inputs.MutateInput.SuppliedInput?.ToArray() ?? inputs.MutateInput.Default.ToArray(),
                MutationLevel = inputs.MutationLevelInput.SuppliedInput ?? inputs.MutationLevelInput.Default,
                Reporters = inputs.ReportersInput.SuppliedInput?.ToArray() ?? inputs.ReportersInput.Default.ToArray(),
                Solution = inputs.SolutionInput.SuppliedInput ?? inputs.SolutionInput.Default,
                TargetFramework = inputs.TargetFrameworkInput.SuppliedInput ?? inputs.TargetFrameworkInput.Default,
                Project = inputs.SourceProjectNameInput.SuppliedInput ?? inputs.SourceProjectNameInput.Default,
                Verbosity = inputs.VerbosityInput.SuppliedInput ?? inputs.VerbosityInput.Default,
                LanguageVersion = inputs.LanguageVersionInput.SuppliedInput ?? inputs.LanguageVersionInput.Default,
                TestProjects = inputs.TestProjectsInput.SuppliedInput?.ToArray() ?? inputs.TestProjectsInput.Default.ToArray(),
                TestCaseFilter = inputs.TestCaseFilterInput.SuppliedInput ?? inputs.TestCaseFilterInput.Default,
                DashboardUrl = inputs.DashboardUrlInput.SuppliedInput ?? inputs.DashboardUrlInput.Default,
                IgnoreMutations = inputs.IgnoreMutationsInput.SuppliedInput?.ToArray() ?? inputs.IgnoreMutationsInput.Default.ToArray(),
                IgnoreMethods = inputs.IgnoredMethodsInput.SuppliedInput?.ToArray() ?? inputs.IgnoredMethodsInput.Default.ToArray(),
                ReportFileName = inputs.ReportFileNameInput.SuppliedInput ?? inputs.ReportFileNameInput.Default,
                BreakOnInitialTestFailure = inputs.BreakOnInitialTestFailureInput.SuppliedInput ?? inputs.BreakOnInitialTestFailureInput.Default,
                Concurrency = inputs.ConcurrencyInput.SuppliedInput ?? inputs.ConcurrencyInput.Default
            }
        };
    }

    private static string SerializeConfig(FileBasedInputOuter config)
    {
        var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
        return JsonSerializer.Serialize(config, serializerOptions);
    }
}
