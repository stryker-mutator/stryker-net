using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Baseline;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options
{
    public class StrykerOptions
    {
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public bool DevMode { get; private set; } = new DevModeInput().Value;

        public string BasePath { get; private set; }
        public string OutputPath { get; private set; }
        public string SolutionPath { get; private set; } = new SolutionPathInput().Value;

        public LogOptions LogOptions { get; private set; }
        public MutationLevel MutationLevel { get; private set; } = new MutationLevelInput().Value;
        public Thresholds Thresholds { get; private set; } = new Thresholds(new ThresholdHighInput().Value, new ThresholdLowInput().Value, new ThresholdBreakInput().Value);

        public int AdditionalTimeoutMS { get; private set; } = new AdditionalTimeoutMsInput().Value;
        public LanguageVersion LanguageVersion { get; private set; } = new LanguageVersionInput().Value;

        public int Concurrency { get; private set; } = new ConcurrencyInput().Value;
        public string ProjectUnderTestName { get; private set; } = new ProjectUnderTestNameInput().Value;
        public IEnumerable<string> TestProjects { get; private set; } = new TestProjectsInput().Value;

        public bool DashboardCompareEnabled { get; private set; } = new DashboardCompareInput().Value;
        public IEnumerable<Reporter> Reporters { get; private set; } = new ReportersInput().Value;

        public BaselineProvider BaselineProvider { get; private set; } = new BaselineProviderInput().Value;
        public string AzureFileStorageUrl { get; private set; } = new AzureFileStorageUrlInput().Value;
        public string AzureFileStorageSas { get; private set; } = new AzureFileStorageSasInput().Value;

        public string DashboardUrl { get; private set; } = new DashboardUrlInput().Value;
        public string DashboardApiKey { get; private set; } = new DashboardApiKeyInput().Value;
        public string ProjectName { get; private set; } = new ProjectNameInput().Value;

        public bool DiffCompareEnabled { get; private set; } = new DiffCompareInput().Value;
        public string GitDiffTarget { get; private set; } = new GitDiffTargetInput().Value;
        public IEnumerable<FilePattern> DiffIgnoreFilePatterns { get; private set; } = new DiffIgnoreFilePatternsInput().Value;

        public string FallbackVersion { get; private set; } = new FallbackVersionInput().Value;
        public string ProjectVersion { get; private set; } = new ProjectVersionInput().Value;
        public string ModuleName { get; private set; } = new ModuleNameInput().Value;

        public IEnumerable<FilePattern> Mutate { get; private set; } = new MutateInput().Value;
        public IEnumerable<Regex> IgnoredMethods { get; private set; } = new IgnoredMethodsInput().Value;
        public IEnumerable<Mutator> ExcludedMutators { get; private set; } = new ExcludedMutatorsInput().Value;

        public OptimizationModes OptimizationMode { get; private set; } =
            new OptimizationModeInput().Value
            | new DisableAbortTestOnFailInput().Value
            | new DisableSimultaneousTestingInput().Value;


        public StrykerOptions(string basePath, ILogger logger = null, IFileSystem fileSystem = null)
        {
            _logger = logger;
            _fileSystem = fileSystem ?? new FileSystem();

            BasePath = new BasePathInput(_fileSystem, basePath).Value;
            OutputPath = new OutputPathInput(_logger, _fileSystem, BasePath).Value;

            LogOptions = new LogOptions(new LogLevelInput().Value, new LogToFileInput().Value, OutputPath);
        }

        /// <summary>
        /// Enable a stryker feature
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns>new StrykerOptions with supplied feature enabled</returns>
        public StrykerOptions With(StrykerOption inputType, bool? enabled)
        {
            return inputType switch
            {
                StrykerOption.DevMode => SetDevMode(enabled),
                StrykerOption.DashboardCompare => SetCompareToDashboard(enabled),
                StrykerOption.LogToFile => SetLogToFile(enabled),
                _ => throw new GeneralStrykerException($"Input {inputType} is invalid for enable feature.")
            };
        }

        /// <summary>
        /// Enables any stryker features and sets their option to chosen the value
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="value"></param>
        /// <returns>new StrykerOptions with supplied feature enabled and it's option set to chosen value</returns>
        public StrykerOptions With(StrykerOption inputType, bool? enabled, string value)
        {
            return inputType switch
            {
                StrykerOption.DiffCompare => SetDiff(enabled, value),
                StrykerOption.DashboardCompare => SetCompareToDashboard(enabled, value),
                _ => throw new GeneralStrykerException($"Input {inputType} is invalid for enable feature with value.")
            };
        }

        /// <summary>
        /// Sets option to chosen value
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="values"></param>
        /// <returns>new StrykerOptions with supplied option set to chosen value</returns>
        public StrykerOptions With(StrykerOption inputType, string value)
        {
            return inputType switch
            {
                StrykerOption.ThresholdBreak => SetThresholdBreak(value),
                StrykerOption.SolutionPath => SetSolutionPath(value),
                StrykerOption.ProjectUnderTestName => SetProjectUnderTestName(value),
                StrykerOption.MutationLevel => SetMutationLevel(value),
                StrykerOption.LogLevel => SetLogLevel(value),
                StrykerOption.DashboardApiKey => SetDashboardApiKey(value),
                StrykerOption.AzureFileStorageSas => SetAzureFileStorageSas(value),
                StrykerOption.ProjectVersion => SetProjectVersion(value),
                StrykerOption.FallbackVersion => SetFallbackVersion(value),
                StrykerOption.Concurrency => SetConcurrency(value),
                _ => throw new GeneralStrykerException($"Input {inputType} is invalid for single value.")
            };
        }

        /// <summary>
        /// Sets option to chosen values
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="values"></param>
        /// <returns>new StrykerOptions with supplied option set to chosen values</returns>
        public StrykerOptions With(StrykerOption inputType, IEnumerable<string> values)
        {
            return inputType switch
            {
                StrykerOption.Mutate => SetMutate(values),
                StrykerOption.Reporters => SetReporters(values),
                _ => throw new GeneralStrykerException($"Input {inputType} is invalid for multi values.")
            };
        }

        #region Fluent setters

        // bool
        private StrykerOptions SetDevMode(bool? devMode)
        {
            DevMode = new DevModeInput(devMode).Value;
            return this;
        }

        private StrykerOptions SetLogToFile(bool? enabled)
        {
            LogOptions = new LogOptions(
                        LogOptions.LogLevel,
                        new LogToFileInput(enabled, OutputPath).Value,
                        OutputPath);
            return this;
        }

        private StrykerOptions SetCompareToDashboard(bool? enabled)
        {
            DashboardCompareEnabled = new DashboardCompareInput(enabled).Value;
            return this;
        }

        // bool with values
        private StrykerOptions SetCompareToDashboard(bool? enabled, string value)
        {
            DashboardCompareEnabled = new DashboardCompareInput(enabled).Value;
            GitDiffTarget = new GitDiffTargetInput(value, DiffCompareEnabled).Value;
            return this;
        }

        private StrykerOptions SetDiff(bool? enabled, string value)
        {
            DiffCompareEnabled = new DiffCompareInput(enabled).Value;
            GitDiffTarget = new GitDiffTargetInput(value, DiffCompareEnabled).Value;
            return this;
        }

        // single value
        private StrykerOptions SetThresholdBreak(string value)
        {
            Thresholds = new Thresholds(Thresholds.High, Thresholds.Low, new ThresholdBreakInput(value, Thresholds.Low).Value);
            return this;
        }

        private StrykerOptions SetConcurrency(string value)
        {
            Concurrency = new ConcurrencyInput(_logger, value).Value;
            return this;
        }

        private StrykerOptions SetFallbackVersion(string value)
        {
            return this;
        }

        private StrykerOptions SetProjectVersion(string value)
        {
            return this;
        }

        private StrykerOptions SetAzureFileStorageSas(string value)
        {
            return this;
        }

        private StrykerOptions SetDashboardApiKey(string value)
        {
            return this;
        }

        private StrykerOptions SetLogLevel(string value)
        {
            return this;
        }

        private StrykerOptions SetMutationLevel(string value)
        {
            return this;
        }

        private StrykerOptions SetProjectUnderTestName(string value)
        {
            return this;
        }

        private StrykerOptions SetSolutionPath(string value)
        {
            return this;
        }

        // multi values
        private StrykerOptions SetMutate(IEnumerable<string> values)
        {
            Mutate = new MutateInput(values).Value;
            return this;
        }

        private StrykerOptions SetReporters(IEnumerable<string> values)
        {
            Reporters = new ReportersInput(values).Value;
            return this;
        }

        #endregion
    }
}
