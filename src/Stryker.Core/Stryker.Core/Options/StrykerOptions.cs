using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Baseline;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options.Options;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Core.Options
{
    public class StrykerOptions
    {
        public string BasePath { get; }
        public string SolutionPath { get; }
        public string OutputPath { get; }
        public BaselineProvider BaselineProvider { get; }

        public IEnumerable<Reporter> Reporters { get; }
        public LogOptions LogOptions { get; }
        public bool DevMode { get; }

        public string ProjectUnderTestNameFilter { get; }
        public bool DiffEnabled { get; }
        public bool CompareToDashboard { get; }
        public string GitDiffTarget { get; }
        public int AdditionalTimeoutMS { get; }
        public IEnumerable<Mutator> ExcludedMutations { get; }
        public IEnumerable<Regex> IgnoredMethods { get; }
        public int ConcurrentTestrunners { get; }
        public Thresholds Thresholds { get; }
        public TestRunner TestRunner { get; set; }
        public IEnumerable<FilePattern> FilePatterns { get; }
        public LanguageVersion LanguageVersion { get; }
        public OptimizationFlags Optimizations { get; }
        public string OptimizationMode { get; }
        public IEnumerable<string> TestProjects { get; }

        public string DashboardUrl { get; } = "https://dashboard.stryker-mutator.io";
        public string DashboardApiKey { get; }
        public string ProjectName { get; }
        public string ModuleName { get; }
        public string ProjectVersion { get; }
        public MutationLevel MutationLevel { get; }

        public IEnumerable<FilePattern> DiffIgnoreFiles { get; }

        public string AzureSAS { get; }
        public string AzureFileStorageUrl { get; }
        public string FallbackVersion { get; }

        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;

        public StrykerOptions(
            ILogger logger = null,
            IFileSystem fileSystem = null,
            string basePath = "",
            IEnumerable<string> reporters = null,
            string projectUnderTestNameFilter = "",
            int additionalTimeoutMS = 5000,
            IEnumerable<string> excludedMutations = null,
            IEnumerable<string> ignoredMethods = null,
            string logLevel = "info",
            bool logToFile = false,
            bool devMode = false,
            string coverageAnalysis = "perTest",
            bool abortTestOnFail = true,
            bool disableSimultaneousTesting = false,
            int? maxConcurrentTestRunners = null,
            int thresholdHigh = 80,
            int thresholdLow = 60,
            int thresholdBreak = 0,
            IEnumerable<string> mutate = null,
            string testRunner = "vstest",
            string solutionPath = null,
            string languageVersion = "latest",
            bool diff = false,
            bool compareToDashboard = false,
            string gitDiffTarget = "master",
            string dashboardApiKey = null,
            string dashboardUrl = "https://dashboard.stryker-mutator.io",
            string projectName = null,
            string moduleName = null,
            string projectVersion = null,
            string fallbackVersion = null,
            string baselineStorageLocation = null,
            string azureSAS = null,
            string azureFileStorageUrl = null,
            IEnumerable<string> testProjects = null,
            string mutationLevel = null,
            IEnumerable<string> diffIgnoreFiles = null)
        {
            _logger = logger;
            _fileSystem = fileSystem ?? new FileSystem();

            DevMode = new DevModeOption(devMode).Value;

            BasePath = new BasePathOption(_fileSystem, basePath).Value;
            SolutionPath = new SolutionPathOption(_fileSystem, solutionPath).Value;
            OutputPath = new OutputPathOption(_logger, _fileSystem, BasePath).Value;
            LogOptions = new LogOptionsOption(logLevel, logToFile, OutputPath).Value;

            MutationLevel = new MutationLevelOption(mutationLevel).Value;
            Thresholds = new ThresholdsOption(thresholdHigh, thresholdLow, thresholdBreak).Value;
            AdditionalTimeoutMS = new AdditionalTimeoutMsOption(additionalTimeoutMS).Value;
            LanguageVersion = new LanguageVersionOption(languageVersion).Value;
            TestRunner = new TestRunnerOption(testRunner).Value;
            ConcurrentTestrunners = new ConcurrentTestrunnersOption(_logger, maxConcurrentTestRunners).Value;

            ProjectUnderTestNameFilter = new ProjectUnderTestNameFilterOption(projectUnderTestNameFilter).Value;
            TestProjects = new TestProjectsOption(testProjects).Value;

            CompareToDashboard = new CompareToDashboardOption(compareToDashboard).Value;
            Reporters = new ReportersOption(reporters, CompareToDashboard).Value;
            BaselineProvider = new BaselineProviderOption(baselineStorageLocation, Reporters.Contains(Reporter.Dashboard)).Value;
            AzureFileStorageUrl = new AzureFileStorageUrlOption(azureFileStorageUrl, BaselineProvider).Value;
            AzureSAS = new AzureFileStorageSasOption(azureSAS, BaselineProvider).Value;

            var dashboardEnabled = CompareToDashboard || Reporters.Contains(Reporter.Dashboard);

            DashboardUrl = new DashboardUrlOption(dashboardUrl).Value;
            DashboardApiKey = new DashboardApiKeyOption(dashboardApiKey, dashboardEnabled).Value;
            ProjectName = new ProjectNameOption(projectName, dashboardEnabled).Value;

            DiffEnabled = new DiffEnabledOption(diff).Value;
            GitDiffTarget = new GitDiffTargetOption(gitDiffTarget, DiffEnabled).Value;
            DiffIgnoreFiles = new DiffIgnoreFilePatternsOption(diffIgnoreFiles).Value;

            FallbackVersion = new FallbackVersionOption(fallbackVersion, gitDiffTarget).Value;
            ProjectVersion = new ProjectVersionOption(projectVersion, FallbackVersion, dashboardEnabled, CompareToDashboard).Value;
            ModuleName = new ModuleNameOption(moduleName).Value;

            FilePatterns = new MutateOption(mutate).Value;
            IgnoredMethods = new IgnoredMethodsOption(ignoredMethods).Value;
            ExcludedMutations = new ExcludedMutatorsOption(excludedMutations).Value;

            OptimizationMode = new OptimizationModeOption(coverageAnalysis).Value;
            Optimizations = new OptimizationsOption(OptimizationMode, abortTestOnFail, disableSimultaneousTesting).Value;
        }
    }
}
