using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Options.Inputs;

namespace Stryker.Core.Options
{
    public class StrykerInputs
    {
        private readonly StrykerOptions _strykerOptionsCache;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public DevModeInput DevModeInput { get; init; }
        public BasePathInput BasePathInput { get; init; }
        public ProjectInput ProjectInput { get; init; }
        public OutputPathInput OutputPathInput { get; init; }
        public SolutionPathInput SolutionPathInput { get; init; }
        public VerbosityInput VerbosityInput { get; init; }
        public LogToFileInput LogToFileInput { get; init; }
        public MutationLevelInput MutationLevelInput { get; init; }
        public ThresholdBreakInput ThresholdBreakInput { get; init; }
        public ThresholdHighInput ThresholdHighInput { get; init; }
        public ThresholdLowInput ThresholdLowInput { get; init; }
        public AdditionalTimeoutMsInput AdditionalTimeoutMSInput { get; init; }
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
        public SinceBranchInput SinceBranchInput { get; init; }
        public SinceCommitInput SinceCommitInput { get; init; }
        public DiffIgnoreFilePatternsInput DiffIgnoreFilePatternsInput { get; init; }
        public FallbackVersionInput FallbackVersionInput { get; init; }
        public ProjectVersionInput ProjectVersionInput { get; init; }
        public ModuleNameInput ModuleNameInput { get; init; }
        public MutateInput MutateInput { get; init; }
        public IgnoredMethodsInput IgnoredMethodsInput { get; init; }
        public ExcludedMutatorsInput ExcludedMutatorsInput { get; init; }
        public OptimizationModeInput OptimizationModeInput { get; init; }

        public StrykerInputs(ILogger logger = null)
        {
            _logger = logger;
        }

        public StrykerOptions Validate()
        {
            return _strykerOptionsCache ?? new StrykerOptions()
            {
                Concurrency = ConcurrencyInput.Validate(_logger),
                MutationLevel = MutationLevelInput.Validate(),
                DevMode = DevModeInput.Validate(),
                SolutionPath = SolutionPathInput.Validate(_fileSystem),
                LogOptions = new LogOptions
                {
                    LogLevel = VerbosityInput.Validate(),
                    LogToFile = LogToFileInput.Validate(OutputPathInput.SuppliedInput),
                    OutputPath = OutputPathInput.Validate(_logger, _fileSystem, BasePathInput.SuppliedInput)
                },
                Thresholds = new Thresholds
                {
                    High = ThresholdHighInput.Validate(ThresholdLowInput.SuppliedInput),
                    Low = ThresholdLowInput.Validate(ThresholdBreakInput.SuppliedInput, ThresholdHighInput.SuppliedInput)
                    Break = ThresholdBreakInput.Validate(ThresholdLowInput.SuppliedInput),
                }
            };
        }
    }
}
