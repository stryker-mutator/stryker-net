using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
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
        IgnoreMutationsInput ExcludedMutationsInput { get; init; }
        FallbackVersionInput FallbackVersionInput { get; init; }
        IgnoreMethodsInput IgnoredMethodsInput { get; init; }
        LanguageVersionInput LanguageVersionInput { get; init; }
        LogToFileInput LogToFileInput { get; init; }
        ModuleNameInput ModuleNameInput { get; init; }
        MutateInput MutateInput { get; init; }
        MutationLevelInput MutationLevelInput { get; init; }
        OutputPathInput OutputPathInput { get; init; }
        ProjectNameInput ProjectNameInput { get; init; }
        ProjectUnderTestNameInput ProjectUnderTestNameInput { get; init; }
        ProjectVersionInput ProjectVersionInput { get; init; }
        ReportersInput ReportersInput { get; init; }
        SinceInput SinceInput { get; init; }
        SinceTargetInput SinceTargetInput { get; init; }
        SolutionInput SolutionInput { get; init; }
        TestProjectsInput TestProjectsInput { get; init; }
        ThresholdBreakInput ThresholdBreakInput { get; init; }
        ThresholdHighInput ThresholdHighInput { get; init; }
        ThresholdLowInput ThresholdLowInput { get; init; }
        VerbosityInput VerbosityInput { get; init; }
        WithBaselineInput WithBaselineInput { get; init; }

        StrykerOptions ValidateAll();
    }

    public class StrykerInputs : IStrykerInputs
    {
        private StrykerOptions _strykerOptionsCache;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public StrykerInputs(ILogger logger = null, IFileSystem fileSystem = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = logger ?? ApplicationLogging.LoggerFactory.CreateLogger<StrykerInputs>();
        }

        public DevModeInput DevModeInput { get; init; }
        public BasePathInput BasePathInput { get; init; }
        public OutputPathInput OutputPathInput { get; init; }
        public SolutionInput SolutionInput { get; init; }
        public VerbosityInput VerbosityInput { get; init; }
        public LogToFileInput LogToFileInput { get; init; }
        public MutationLevelInput MutationLevelInput { get; init; }
        public ThresholdBreakInput ThresholdBreakInput { get; init; }
        public ThresholdHighInput ThresholdHighInput { get; init; }
        public ThresholdLowInput ThresholdLowInput { get; init; }
        public AdditionalTimeoutInput AdditionalTimeoutInput { get; init; }
        public LanguageVersionInput LanguageVersionInput { get; init; }
        public ConcurrencyInput ConcurrencyInput { get; init; }
        public ProjectUnderTestNameInput ProjectUnderTestNameInput { get; init; }
        public TestProjectsInput TestProjectsInput { get; init; }
        public WithBaselineInput WithBaselineInput { get; init; }
        public ReportersInput ReportersInput { get; init; }
        public BaselineProviderInput BaselineProviderInput { get; init; }
        public AzureFileStorageUrlInput AzureFileStorageUrlInput { get; init; }
        public AzureFileStorageSasInput AzureFileStorageSasInput { get; init; }
        public DashboardUrlInput DashboardUrlInput { get; init; }
        public DashboardApiKeyInput DashboardApiKeyInput { get; init; }
        public ProjectNameInput ProjectNameInput { get; init; }
        public SinceInput SinceInput { get; init; }
        public SinceTargetInput SinceTargetInput { get; init; }
        public DiffIgnoreChangesInput DiffIgnoreChangesInput { get; init; }
        public FallbackVersionInput FallbackVersionInput { get; init; }
        public ProjectVersionInput ProjectVersionInput { get; init; }
        public ModuleNameInput ModuleNameInput { get; init; }
        public MutateInput MutateInput { get; init; }
        public IgnoreMethodsInput IgnoredMethodsInput { get; init; }
        public IgnoreMutationsInput ExcludedMutationsInput { get; init; }
        public CoverageAnalysisInput CoverageAnalysisInput { get; init; }
        public DisableBailInput DisableBailInput { get; set; }
        public DisableMixMutantsInput DisableMixMutantsInput { get; set; }

        public StrykerOptions ValidateAll()
        {
            var basePath = BasePathInput.Validate(_fileSystem);
            var outputPath = OutputPathInput.Validate(_logger, _fileSystem, basePath);
            var reporters = ReportersInput.Validate();
            var baselineProvider = BaselineProviderInput.Validate(reporters);
            var sinceEnabled = SinceInput.Validate(WithBaselineInput.SuppliedInput);

            _strykerOptionsCache ??= new StrykerOptions()
            {
                BasePath = basePath,
                OutputPath = outputPath,
                Concurrency = ConcurrencyInput.Validate(_logger),
                MutationLevel = MutationLevelInput.Validate(),
                DevMode = DevModeInput.Validate(),
                SolutionPath = SolutionInput.Validate(_fileSystem),
                LogOptions = new LogOptions
                {
                    LogLevel = VerbosityInput.Validate(),
                    LogToFile = LogToFileInput.Validate(outputPath)
                },
                Thresholds = new Thresholds
                {
                    High = ThresholdHighInput.Validate(ThresholdLowInput.SuppliedInput),
                    Low = ThresholdLowInput.Validate(ThresholdBreakInput.SuppliedInput, ThresholdHighInput.SuppliedInput),
                    Break = ThresholdBreakInput.Validate(ThresholdLowInput.SuppliedInput),
                },
                Reporters = reporters,
                ProjectUnderTestName = ProjectUnderTestNameInput.Validate(),
                AdditionalTimeout = AdditionalTimeoutInput.Validate(),
                ExcludedMutations = ExcludedMutationsInput.Validate(),
                IgnoredMethods = IgnoredMethodsInput.Validate(),
                Mutate = MutateInput.Validate(),
                LanguageVersion = LanguageVersionInput.Validate(),
                OptimizationMode = CoverageAnalysisInput.Validate() | DisableBailInput.Validate() | DisableMixMutantsInput.Validate(),
                TestProjects = TestProjectsInput.Validate(),
                DashboardUrl = DashboardUrlInput.Validate(),
                DashboardApiKey = DashboardApiKeyInput.Validate(WithBaselineInput.SuppliedInput),
                ProjectName = ProjectNameInput.Validate(reporters),
                ModuleName = ModuleNameInput.Validate(),
                ProjectVersion = ProjectVersionInput.Validate(FallbackVersionInput.SuppliedInput, reporters, WithBaselineInput.SuppliedInput),
                DiffIgnoreChanges = DiffIgnoreChangesInput.Validate(),
                AzureFileStorageSas = AzureFileStorageSasInput.Validate(baselineProvider),
                AzureFileStorageUrl = AzureFileStorageUrlInput.Validate(baselineProvider),
                WithBaseline = WithBaselineInput.Validate(),
                BaselineProvider = baselineProvider,
                FallbackVersion = FallbackVersionInput.Validate(ProjectVersionInput.SuppliedInput, WithBaselineInput.SuppliedInput),
                Since = sinceEnabled,
                SinceTarget = SinceTargetInput.Validate(sinceEnabled)
            };
            return _strykerOptionsCache;
        }
    }
}
