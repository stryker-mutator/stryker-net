using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Baseline;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options.Inputs;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text.RegularExpressions;

namespace Stryker.Core.Options
{
    public class StrykerOptions
    {
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;

        public bool DevMode { get; private set; }

        public string BasePath { get; private set; }
        public string SolutionPath { get; private set; }
        public string OutputPath { get; private set; }

        public LogOptions LogOptions { get; private set; }
        public MutationLevel MutationLevel { get; private set; }
        public Thresholds Thresholds { get; private set; }

        public int AdditionalTimeoutMS { get; private set; }
        public LanguageVersion LanguageVersion { get; private set; }
        public TestRunner TestRunner { get; private set; }

        public int ConcurrentTestrunners { get; private set; }
        public string ProjectUnderTestNameFilter { get; private set; }
        public IEnumerable<string> TestProjects { get; private set; }

        public bool CompareToDashboard { get; private set; }
        public IEnumerable<Reporter> Reporters { get; private set; }

        public BaselineProvider BaselineProvider { get; private set; }
        public string AzureFileStorageUrl { get; private set; }
        public string AzureSAS { get; private set; }

        public string DashboardUrl { get; private set; }
        public string DashboardApiKey { get; private set; }
        public string ProjectName { get; private set; }

        public bool DiffEnabled { get; private set; }
        public string GitDiffTarget { get; private set; }
        public IEnumerable<FilePattern> DiffIgnoreFiles { get; private set; }

        public string FallbackVersion { get; private set; }
        public string ProjectVersion { get; private set; }
        public string ModuleName { get; private set; }

        public IEnumerable<FilePattern> FilePatterns { get; private set; }
        public IEnumerable<Regex> IgnoredMethods { get; private set; }
        public IEnumerable<Mutator> ExcludedMutations { get; private set; }

        public string OptimizationMode { get; private set; }
        public OptimizationModes Optimizations { get; private set; }


        public StrykerOptions(string basePath, ILogger logger = null, IFileSystem fileSystem = null)
        {
            _logger = logger;
            _fileSystem = fileSystem ?? new FileSystem();

            BasePath = new BasePathInput(_fileSystem, basePath).Value;
            OutputPath = new OutputPathInput(_logger, _fileSystem, BasePath).Value;
        }

        /// <summary>
        /// Enable a stryker feature
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns>new StrykerOptions with supplied feature enabled</returns>
        public StrykerOptions With(StrykerInput inputType, bool? enabled)
        {
            return inputType switch
            {
                StrykerInput.DevMode => SetDevMode(enabled),
                StrykerInput.DashboardCompare => SetCompareToDashboard(enabled),
                StrykerInput.LogToFile => SetLogToFile(enabled),
                _ => throw new GeneralStrykerException($"Input {inputType} is invalid for enable feature.")
            };
        }

        /// <summary>
        /// Enables any stryker features and sets their option to chosen the value
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="value"></param>
        /// <returns>new StrykerOptions with supplied feature enabled and it's option set to chosen value</returns>
        public StrykerOptions With(StrykerInput inputType, bool? enabled, string value)
        {
            return inputType switch
            {
                StrykerInput.DiffCompare => SetDiff(enabled, value),
                StrykerInput.DashboardCompare => SetCompareToDashboard(enabled, value),
                _ => throw new GeneralStrykerException($"Input {inputType} is invalid for enable feature with value.")
            };
        }

        /// <summary>
        /// Sets option to chosen value
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="values"></param>
        /// <returns>new StrykerOptions with supplied option set to chosen value</returns>
        public StrykerOptions With(StrykerInput inputType, string value)
        {
            return inputType switch
            {
                StrykerInput.ThresholdBreak => SetThresholdBreak(value),
                StrykerInput.SolutionPath => SetSolutionPath(value),
                StrykerInput.ProjectUnderTestName => SetProjectUnderTestName(value),
                StrykerInput.MutationLevel => SetMutationLevel(value),
                StrykerInput.LogLevel => SetLogLevel(value),
                StrykerInput.DashboardApiKey => SetDashboardApiKey(value),
                StrykerInput.AzureFileStorageSas => SetAzureFileStorageSas(value),
                StrykerInput.ProjectVersion => SetProjectVersion(value),
                StrykerInput.FallbackVersion => SetFallbackVersion(value),
                StrykerInput.Concurrency => SetConcurrency(value),
                _ => throw new GeneralStrykerException($"Input {inputType} is invalid for single value.")
            };
        }

        /// <summary>
        /// Sets option to chosen values
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="values"></param>
        /// <returns>new StrykerOptions with supplied option set to chosen values</returns>
        public StrykerOptions With(StrykerInput inputType, IEnumerable<string> values)
        {
            return inputType switch
            {
                StrykerInput.Mutate => SetMutate(values),
                StrykerInput.Reporters => SetReporters(values),
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
            CompareToDashboard = new CompareToDashboardInput(enabled).Value;
            return this;
        }

        // bool with values
        private StrykerOptions SetCompareToDashboard(bool? enabled, string value)
        {
            CompareToDashboard = new CompareToDashboardInput(enabled).Value;
            GitDiffTarget = new GitDiffTargetInput(value, DiffEnabled).Value;
            return this;
        }

        private StrykerOptions SetDiff(bool? enabled, string value)
        {
            DiffEnabled = new DiffEnabledInput(enabled).Value;
            GitDiffTarget = new GitDiffTargetInput(value, DiffEnabled).Value;
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
            FilePatterns = new MutateInput(values).Value;
            return this;
        }

        private StrykerOptions SetReporters(IEnumerable<string> values)
        {
            Reporters = new ReportersInput(values, CompareToDashboard).Value;
            return this;
        }

        #endregion
    }
}