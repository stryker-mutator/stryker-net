using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Stryker.Core.Options.Inputs;

namespace Stryker.Core.Options
{
    public class StrykerInputs
    {
        private readonly StrykerOptions _strykerOptionsCache;
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public DevModeInput DevMode { get; set; }

        public BasePathInput BasePath { get; init; }
        public OutputPathInput OutputPath { get; init; }
        public SolutionPathInput SolutionPath { get; init; }
        public LogLevelInput LogLevel { get; init; }
        public LogToFileInput LogToFile { get; init; }
        public MutationLevelInput MutationLevelInput { get; init; }
        public ThresholdBreakInput ThresholdBreakInput { get; init; }
        public ThresholdHighInput ThresholdHighInput { get; init; }
        public ThresholdLowInput ThresholdLowInput { get; init; }
        public AdditionalTimeoutMsInput AdditionalTimeoutMSInput { get; init; }
        public LanguageVersionInput LanguageVersionInput { get; init; }
        public ConcurrencyInput ConcurrencyInput { get; set; }
        public ProjectUnderTestNameInput ProjectUnderTestNameInput { get; init; }
        public TestProjectsInput TestProjectsInput { get; init; }
        public WithBaselineInput WithBaselineInput { get; init; }
        public ReportersInput ReportersInput { get; set; }
        public BaselineProviderInput BaselineProviderInput { get; init; }
        public AzureFileStorageUrlInput AzureFileStorageUrlInput { get; init; }
        public AzureFileStorageSasInput AzureFileStorageSasInput { get; init; }
        public DashboardUrlInput DashboardUrlInput { get; init; }
        public DashboardApiKeyInput DashboardApiKeyInput { get; init; }
        public ProjectNameInput ProjectNameInput { get; init; }
        public SinceInput SinceInput { get; set; }
        public SinceTargetInput SinceTargetInput { get; set; }
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
                MutationLevel = MutationLevelInput.Validate()
            };
        }
    }
}
