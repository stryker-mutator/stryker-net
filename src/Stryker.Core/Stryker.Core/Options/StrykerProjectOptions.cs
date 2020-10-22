using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Baseline;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stryker.Core.Options
{
    public class StrykerProjectOptions : IStrykerOptions
    {
        public string BasePath { get; }
        public string SolutionPath { get; }
        public string OutputPath { get; }
        public IEnumerable<Reporter> Reporters { get; }
        public LogOptions LogOptions { get; }
        public bool DevMode { get; }
        public string ProjectUnderTestNameFilter { get; }
        public string ProjectUnderTest { get; }
        public bool DiffEnabled { get; }
        public string GitSource { get; }
        public int AdditionalTimeoutMS { get; }
        public IEnumerable<Mutator> ExcludedMutations { get; }
        public IEnumerable<Regex> IgnoredMethods { get; }
        public int ConcurrentTestrunners { get; }
        public Threshold Thresholds { get; }
        public TestRunner TestRunner { get; set; }
        public IEnumerable<FilePattern> FilePatterns { get; }
        public LanguageVersion LanguageVersion { get; }
        public OptimizationFlags Optimizations { get; }
        public string OptimizationMode { get; set; }
        public IEnumerable<string> TestProjects { get; set; }
        public DiffOptions DiffOptions { get; }
        public string AzureSAS { get; }
        public string AzureFileStorageUrl { get; set; }
        public BaselineProvider BaselineProvider { get; }
        public MutationLevel MutationLevel { get; }

        public StrykerProjectOptions(
            string basePath = null,
            string outputPath = null,
            IEnumerable<Reporter> reporters = null,
            string projectUnderTestNameFilter = null,
            string projectUnderTest = null,
            int additionalTimeoutMS = 5000,
            IEnumerable<Mutator> excludedMutations = null,
            IEnumerable<Regex> ignoredMethods = null,
            LogOptions logOptions = null,
            OptimizationFlags optimizations = OptimizationFlags.NoOptimization,
            Threshold thresholds = null,
            bool devMode = false,
            string optimizationMode = null,
            int concurrentTestRunners = 4,
            IEnumerable<FilePattern> filePatterns = null,
            TestRunner testRunner = TestRunner.VsTest,
            string solutionPath = null,
            LanguageVersion languageVersion = LanguageVersion.Latest,
            string gitSource = null,
            DiffOptions diffOptions = null,
            IEnumerable<string> testProjects = null,
            string azureSAS = null,
            string azureFileStorageUrl = null,
            BaselineProvider baselineProvider = BaselineProvider.Disk,
            MutationLevel mutationLevel = MutationLevel.Standard)
        {
            IgnoredMethods = ignoredMethods;
            BasePath = basePath;
            OutputPath = outputPath;
            Reporters = reporters;
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            ProjectUnderTest = projectUnderTest;
            AdditionalTimeoutMS = additionalTimeoutMS;
            ExcludedMutations = excludedMutations;
            LogOptions = logOptions;
            DevMode = devMode;
            ConcurrentTestrunners = concurrentTestRunners;
            Thresholds = thresholds;
            FilePatterns = filePatterns;
            TestRunner = testRunner;
            SolutionPath = solutionPath;
            LanguageVersion = languageVersion;
            OptimizationMode = optimizationMode;
            Optimizations = optimizations;
            GitSource = gitSource;
            TestProjects = testProjects;
            DiffOptions = diffOptions ?? new DiffOptions();
            AzureSAS = azureSAS;
            AzureFileStorageUrl = azureFileStorageUrl;
            BaselineProvider = baselineProvider;
            MutationLevel = mutationLevel;
        }
    }
}
