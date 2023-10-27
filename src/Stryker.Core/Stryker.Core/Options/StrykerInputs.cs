using System.IO.Abstractions;
using Stryker.Core.Mutators;
using Stryker.Core.Options.Inputs;

namespace Stryker.Core.Options
{
    public interface IStrykerInputs
    {
        AdditionalTimeoutInput AdditionalTimeoutInput { get; init; }
        AzureFileStorageSasInput AzureFileStorageSasInput { get; init; }
        AzureFileStorageUrlInput AzureFileStorageUrlInput { get; init; }
        BaselineProviderInput BaselineProviderInput { get; init; }
        BasePathInput BasePathInput { get; init; }
        ConcurrencyInput ConcurrencyInput { get; init; }
        CoverageAnalysisInput CoverageAnalysisInput { get; init; }
        DashboardApiKeyInput DashboardApiKeyInput { get; init; }
        DashboardUrlInput DashboardUrlInput { get; init; }
        DevModeInput DevModeInput { get; init; }
        DiffIgnoreChangesInput DiffIgnoreChangesInput { get; init; }
        DisableBailInput DisableBailInput { get; set; }
        DisableMixMutantsInput DisableMixMutantsInput { get; set; }
        IgnoreMutationsInput IgnoreMutationsInput { get; init; }
        FallbackVersionInput FallbackVersionInput { get; init; }
        IgnoreMethodsInput IgnoredMethodsInput { get; init; }
        LanguageVersionInput LanguageVersionInput { get; init; }
        LogToFileInput LogToFileInput { get; init; }
        ModuleNameInput ModuleNameInput { get; init; }
        MutateInput MutateInput { get; init; }
        MutationLevelInput MutationLevelInput { get; init; }
        MsBuildPathInput MsBuildPathInput { get; init; }
        OutputPathInput OutputPathInput { get; init; }
        ReportFileNameInput ReportFileNameInput { get; init; }
        ProjectNameInput ProjectNameInput { get; init; }
        SourceProjectNameInput SourceProjectNameInput { get; init; }
        ProjectVersionInput ProjectVersionInput { get; init; }
        ReportersInput ReportersInput { get; init; }
        SinceInput SinceInput { get; init; }
        SinceTargetInput SinceTargetInput { get; init; }
        SolutionInput SolutionInput { get; init; }
        TargetFrameworkInput TargetFrameworkInput { get; init; }
        TestProjectsInput TestProjectsInput { get; init; }
        TestCaseFilterInput TestCaseFilterInput { get; init; }
        ThresholdBreakInput ThresholdBreakInput { get; init; }
        ThresholdHighInput ThresholdHighInput { get; init; }
        ThresholdLowInput ThresholdLowInput { get; init; }
        VerbosityInput VerbosityInput { get; init; }
        WithBaselineInput WithBaselineInput { get; init; }
        OpenReportInput OpenReportInput { get; init; }
        OpenReportEnabledInput OpenReportEnabledInput { get; init; }
        BreakOnInitialTestFailureInput BreakOnInitialTestFailureInput { get; init; }

        StrykerOptions ValidateAll();
    }

    public class StrykerInputs : IStrykerInputs
    {
        private StrykerOptions _strykerOptionsCache;
        private readonly IFileSystem _fileSystem;

        public StrykerInputs(IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public DevModeInput DevModeInput { get; init; } = new();
        public BasePathInput BasePathInput { get; init; } = new();
        public OutputPathInput OutputPathInput { get; init; } = new();
        public ReportFileNameInput ReportFileNameInput { get; init; } = new();
        public SolutionInput SolutionInput { get; init; } = new();
        public TargetFrameworkInput TargetFrameworkInput { get; init; } = new();
        public VerbosityInput VerbosityInput { get; init; } = new();
        public LogToFileInput LogToFileInput { get; init; } = new();
        public MutationLevelInput MutationLevelInput { get; init; } = new();
        public ThresholdBreakInput ThresholdBreakInput { get; init; } = new();
        public ThresholdHighInput ThresholdHighInput { get; init; } = new();
        public ThresholdLowInput ThresholdLowInput { get; init; } = new();
        public AdditionalTimeoutInput AdditionalTimeoutInput { get; init; } = new();
        public LanguageVersionInput LanguageVersionInput { get; init; } = new();
        public ConcurrencyInput ConcurrencyInput { get; init; } = new();
        public SourceProjectNameInput SourceProjectNameInput { get; init; } = new();
        public TestProjectsInput TestProjectsInput { get; init; } = new();
        public TestCaseFilterInput TestCaseFilterInput { get; init; } = new();
        public WithBaselineInput WithBaselineInput { get; init; } = new();
        public ReportersInput ReportersInput { get; init; } = new();
        public BaselineProviderInput BaselineProviderInput { get; init; } = new();
        public AzureFileStorageUrlInput AzureFileStorageUrlInput { get; init; } = new();
        public AzureFileStorageSasInput AzureFileStorageSasInput { get; init; } = new();
        public DashboardUrlInput DashboardUrlInput { get; init; } = new();
        public DashboardApiKeyInput DashboardApiKeyInput { get; init; } = new();
        public ProjectNameInput ProjectNameInput { get; init; } = new();
        public SinceInput SinceInput { get; init; } = new();
        public SinceTargetInput SinceTargetInput { get; init; } = new();
        public DiffIgnoreChangesInput DiffIgnoreChangesInput { get; init; } = new();
        public FallbackVersionInput FallbackVersionInput { get; init; } = new();
        public ProjectVersionInput ProjectVersionInput { get; init; } = new();
        public ModuleNameInput ModuleNameInput { get; init; } = new();
        public MutateInput MutateInput { get; init; } = new();
        public IgnoreMethodsInput IgnoredMethodsInput { get; init; } = new();
        public IgnoreMutationsInput IgnoreMutationsInput { get; init; } = new();
        public CoverageAnalysisInput CoverageAnalysisInput { get; init; } = new();
        public DisableBailInput DisableBailInput { get; set; } = new();
        public DisableMixMutantsInput DisableMixMutantsInput { get; set; } = new();
        public MsBuildPathInput MsBuildPathInput { get; init; } = new();
        public OpenReportInput OpenReportInput { get; init; } = new();
        public OpenReportEnabledInput OpenReportEnabledInput { get; init; } = new();
        public BreakOnInitialTestFailureInput BreakOnInitialTestFailureInput { get; init; } = new();

        public StrykerOptions ValidateAll()
        {
            var basePath = BasePathInput.Validate(_fileSystem);
            var outputPath = OutputPathInput.Validate(_fileSystem);
            var reportFileNameInput = ReportFileNameInput.Validate();
            var withBaseline = WithBaselineInput.Validate();
            var reporters = ReportersInput.Validate(withBaseline);
            var baselineProvider = BaselineProviderInput.Validate(reporters, withBaseline);
            var sinceEnabled = SinceInput.Validate(WithBaselineInput.SuppliedInput);
            var sinceTarget = SinceTargetInput.Validate(sinceEnabled);
            var projectVersion = ProjectVersionInput.Validate(reporters, withBaseline);

            _strykerOptionsCache ??= new StrykerOptions()
            {
                ProjectPath = basePath,
                OutputPath = outputPath,
                ReportFileName = reportFileNameInput,
                Concurrency = ConcurrencyInput.Validate(),
                MutationLevel = MutationLevelInput.Validate(),
                DevMode = DevModeInput.Validate(),
                MsBuildPath = MsBuildPathInput.Validate(_fileSystem),
                SolutionPath = SolutionInput.Validate(basePath, _fileSystem),
                TargetFramework = TargetFrameworkInput.Validate(),
                Thresholds = new Thresholds
                {
                    High = ThresholdHighInput.Validate(ThresholdLowInput.SuppliedInput),
                    Low = ThresholdLowInput.Validate(ThresholdBreakInput.SuppliedInput, ThresholdHighInput.SuppliedInput),
                    Break = ThresholdBreakInput.Validate(ThresholdLowInput.SuppliedInput),
                },
                Reporters = reporters,
                LogOptions = new LogOptions
                {
                    LogLevel = VerbosityInput.Validate(),
                    LogToFile = LogToFileInput.Validate(outputPath)
                },
                SourceProjectName = SourceProjectNameInput.Validate(),
                AdditionalTimeout = AdditionalTimeoutInput.Validate(),
                ExcludedMutations = IgnoreMutationsInput.Validate<Mutator>(),
                ExcludedLinqExpressions = IgnoreMutationsInput.ValidateLinqExpressions(),
                IgnoredMethods = IgnoredMethodsInput.Validate(),
                Mutate = MutateInput.Validate(),
                LanguageVersion = LanguageVersionInput.Validate(),
                OptimizationMode = CoverageAnalysisInput.Validate() | DisableBailInput.Validate() | DisableMixMutantsInput.Validate(),
                TestProjects = TestProjectsInput.Validate(),
                TestCaseFilter = TestCaseFilterInput.Validate(),
                DashboardUrl = DashboardUrlInput.Validate(),
                DashboardApiKey = DashboardApiKeyInput.Validate(withBaseline, baselineProvider, reporters),
                ProjectName = ProjectNameInput.Validate(),
                ModuleName = ModuleNameInput.Validate(),
                ProjectVersion = ProjectVersionInput.Validate(reporters, withBaseline),
                DiffIgnoreChanges = DiffIgnoreChangesInput.Validate(),
                AzureFileStorageSas = AzureFileStorageSasInput.Validate(baselineProvider, withBaseline),
                AzureFileStorageUrl = AzureFileStorageUrlInput.Validate(baselineProvider, withBaseline),
                WithBaseline = withBaseline,
                BaselineProvider = baselineProvider,
                FallbackVersion = FallbackVersionInput.Validate(withBaseline, projectVersion, sinceTarget),
                Since = sinceEnabled,
                SinceTarget = sinceTarget,
                ReportTypeToOpen = OpenReportInput.Validate(OpenReportEnabledInput.Validate()),
                BreakOnInitialTestFailure = BreakOnInitialTestFailureInput.Validate(),
            };
            return _strykerOptionsCache;
        }
    }
}
