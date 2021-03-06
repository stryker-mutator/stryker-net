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
        public MutationLevelInput MutationLevel { get; init; }
        public ThresholdBreakInput ThresholdBreak { get; init; }
        public ThresholdHighInput ThresholdHigh { get; init; }
        public ThresholdLowInput ThresholdLow { get; init; }

        public AdditionalTimeoutMsInput AdditionalTimeoutMS { get; init; }
        public LanguageVersionInput LanguageVersion { get; init; }

        public ConcurrencyInput Concurrency { get; set; }
        public ProjectUnderTestNameInput ProjectUnderTestName { get; init; }
        public TestProjectsInput TestProjects { get; init; }

        public WithBaselineInput WithBaseline { get; init; }
        public ReportersInput ReportersInput { get; set; }

        public BaselineProviderInput BaselineProvider { get; init; }
        public AzureFileStorageUrlInput AzureFileStorageUrl { get; init; }
        public AzureFileStorageSasInput AzureFileStorageSas { get; init; }

        public DashboardUrlInput DashboardUrl { get; init; }
        public DashboardApiKeyInput DashboardApiKey { get; init; }
        public ProjectNameInput ProjectName { get; init; }

        public SinceInput SinceInput { get; set; }
        public SinceTargetInput SinceTargetInput { get; set; }
        public DiffIgnoreFilePatternsInput DiffIgnoreFilePatterns { get; init; }

        public FallbackVersionInput FallbackVersion { get; init; }
        public ProjectVersionInput ProjectVersion { get; init; }
        public ModuleNameInput ModuleName { get; init; }

        public MutateInput Mutate { get; init; }
        public IgnoredMethodsInput IgnoredMethods { get; init; }
        public ExcludedMutatorsInput ExcludedMutators { get; init; }

        public OptimizationModeInput OptimizationMode { get; init; }

        public ConcurrencyInput ConcurrencyInput { private get; init; }

        public StrykerInputs(ILogger logger = null)
        {
            _logger = logger;
        }

        public StrykerOptions Validate()
        {
            return _strykerOptionsCache ?? new StrykerOptions()
            {
                Concurrency = ConcurrencyInput.Validate(_logger)
            };
        }
    }
}
