using Microsoft.CodeAnalysis.CSharp;
using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;
using System.IO;
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
        public IEnumerable<Reporter> Reporters { get; }
        public LogOptions LogOptions { get; }
        public bool DevMode { get; }
        /// <summary>
        /// The user can pass a filter to match the project under test from multiple project references
        /// </summary>
        public string ProjectUnderTestNameFilter { get; }
        public string TestProjectNameFilter { get; }
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

        private const string ErrorMessage = "The value for one of your settings is not correct. Try correcting or removing them.";
        private readonly IFileSystem _fileSystem;

        public StrykerOptions(
            IFileSystem fileSystem = null,
            string basePath = "",
            string[] reporters = null,
            string projectUnderTestNameFilter = "",
            string testProjectNameFilter = "*.csproj",
            int additionalTimeoutMS = 5000,
            string[] excludedMutations = null,
            string[] ignoredMethods = null,
            string logLevel = "info",
            bool logToFile = false,
            bool devMode = false,
            string coverageAnalysis = "perTest",
            bool abortTestOnFail = true,
            bool allowSimultaneousTesting = false,
            int maxConcurrentTestRunners = int.MaxValue,
            int thresholdHigh = 80,
            int thresholdLow = 60,
            int thresholdBreak = 0,
            string[] filesToExclude = null,
            string[] mutate = null,
            string testRunner = "vstest",
            string solutionPath = null,
            string languageVersion = "latest",
            bool diff = false,
            string gitSource = "master")
        {
            _fileSystem = fileSystem ?? new FileSystem();

            var outputPath = ValidateOutputPath(basePath);
            IgnoredMethods = ValidateIgnoredMethods(ignoredMethods ?? Array.Empty<string>());
            BasePath = basePath;
            OutputPath = outputPath;
            Reporters = ValidateReporters(reporters);
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            TestProjectNameFilter = ValidateTestProjectFilter(basePath, testProjectNameFilter);
            AdditionalTimeoutMS = additionalTimeoutMS;
            ExcludedMutations = ValidateExcludedMutations(excludedMutations);
            LogOptions = new LogOptions(ValidateLogLevel(logLevel), logToFile, outputPath);
            DevMode = devMode;
            ConcurrentTestrunners = ValidateConcurrentTestrunners(maxConcurrentTestRunners);
            Optimizations = ValidateMode(coverageAnalysis) | (abortTestOnFail ? OptimizationFlags.AbortTestOnKill : 0) | (allowSimultaneousTesting ? OptimizationFlags.RunMultipleMutants : 0);
            Thresholds = ValidateThresholds(thresholdHigh, thresholdLow, thresholdBreak);
            FilePatterns = ValidateFilePatterns(mutate, filesToExclude);
            TestRunner = ValidateTestRunner(testRunner);
            SolutionPath = ValidateSolutionPath(basePath, solutionPath);
            LanguageVersion = ValidateLanguageVersion(languageVersion);
            OptimizationMode = coverageAnalysis;
            DiffEnabled = diff;
            GitSource = ValidateGitSource(gitSource);
        }

        private string ValidateGitSource(string gitSource)
        {
            if (string.IsNullOrEmpty(gitSource))
            {
                throw new StrykerInputException("GitSource may not be empty, please provide a valid git branch name");
            }
            return gitSource;
        }

        private static IEnumerable<Regex> ValidateIgnoredMethods(IEnumerable<string> methodPatterns)
        {
            foreach (var methodPattern in methodPatterns.Where(x => !string.IsNullOrEmpty(x)))
            {
                yield return new Regex("^" + Regex.Escape(methodPattern).Replace("\\*", ".*") + "$", RegexOptions.IgnoreCase);
            }
        }

        private OptimizationFlags ValidateMode(string mode)
        {
            switch (mode)
            {
                case "perTestInIsolation":
                    return OptimizationFlags.CoverageBasedTest | OptimizationFlags.CaptureCoveragePerTest;
                case "perTest":
                    return OptimizationFlags.CoverageBasedTest;
                case "all":
                    return OptimizationFlags.SkipUncoveredMutants;
                case "off":
                case "":
                    return OptimizationFlags.NoOptimization;
                default:
                    throw new StrykerInputException(
                        ErrorMessage,
                        $"Incorrect coverageAnalysis option {mode}. The options are [off, all, perTest or perTestInIsolation].");
            }
        }

        private string ValidateOutputPath(string basePath)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                return "";
            }

            var outputPath = Path.Combine(basePath, "StrykerOutput", DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss"));
            _fileSystem.Directory.CreateDirectory(FilePathUtils.NormalizePathSeparators(outputPath));

            return outputPath;
        }

        private IEnumerable<Reporter> ValidateReporters(string[] reporters)
        {
            if (reporters == null)
            {
                foreach (var reporter in new[] { Reporter.Progress, Reporter.ClearText })
                {
                    yield return reporter;
                }
                yield break;
            }

            IList<string> invalidReporters = new List<string>();
            foreach (var reporter in reporters)
            {
                if (Enum.TryParse(reporter, true, out Reporter result))
                {
                    yield return result;
                }
                else
                {
                    invalidReporters.Add(reporter);
                }
            }
            if (invalidReporters.Any())
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    $"These reporter values are incorrect: {string.Join(",", invalidReporters)}. Valid reporter options are [{string.Join(",", (Reporter[])Enum.GetValues(typeof(Reporter)))}]");
            }
            // If we end up here then the user probably disabled all reporters. Return empty IEnumerable.
            yield break;
        }

        private IEnumerable<Mutator> ValidateExcludedMutations(IEnumerable<string> excludedMutations)
        {
            if (excludedMutations == null)
            {
                yield break;
            }

            // Get all mutatorTypes and their descriptions
            Dictionary<Mutator, string> typeDescriptions = Enum.GetValues(typeof(Mutator))
                .Cast<Mutator>()
                .ToDictionary(x => x, x => x.GetDescription());

            foreach (string excludedMutation in excludedMutations)
            {
                // Find any mutatorType that matches the name passed by the user
                var mutatorDescriptor = typeDescriptions.FirstOrDefault(
                    x => x.Value.ToString().ToLower().Contains(excludedMutation.ToLower()));
                if (mutatorDescriptor.Value != null)
                {
                    yield return mutatorDescriptor.Key;
                }
                else
                {
                    throw new StrykerInputException(ErrorMessage, $"Invalid excluded mutation '{excludedMutation}' " + $"The excluded mutations options are [{string.Join(", ", typeDescriptions.Select(x => x.Key))}]");
                }
            }
        }

        private LogEventLevel ValidateLogLevel(string levelText)
        {
            switch (levelText?.ToLower() ?? "")
            {
                case "error":
                    return LogEventLevel.Error;
                case "warning":
                    return LogEventLevel.Warning;
                case "info":
                case "":
                    return LogEventLevel.Information;
                case "debug":
                    return LogEventLevel.Debug;
                case "trace":
                    return LogEventLevel.Verbose;
                default:
                    throw new StrykerInputException(
                        ErrorMessage,
                        $"Incorrect log level {levelText}. The log level options are [Error, Warning, Info, Debug, Trace]");
            }
        }

        private int ValidateConcurrentTestrunners(int maxConcurrentTestRunners)
        {
            if (maxConcurrentTestRunners < 1)
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    "Amount of maximum concurrent testrunners must be greater than zero.");
            }

            var logicalProcessorCount = Environment.ProcessorCount;
            var usableProcessorCount = Math.Max(logicalProcessorCount / 2, 1);

            if (maxConcurrentTestRunners <= logicalProcessorCount)
            {
                usableProcessorCount = maxConcurrentTestRunners;
            }

            return usableProcessorCount;
        }

        private Threshold ValidateThresholds(int thresholdHigh, int thresholdLow, int thresholdBreak)
        {
            List<int> thresholdList = new List<int> { thresholdHigh, thresholdLow, thresholdBreak };
            if (thresholdList.Any(x => x > 100 || x < 0))
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    "The thresholds must be between 0 and 100");
            }

            // ThresholdLow and ThresholdHigh can be the same value
            if (thresholdBreak >= thresholdLow || thresholdLow > thresholdHigh)
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    "The values of your thresholds are incorrect. Change `--threshold-break` to the lowest value and `--threshold-high` to the highest.");
            }

            return new Threshold(thresholdHigh, thresholdLow, thresholdBreak);
        }

        private IEnumerable<FilePattern> ValidateFilePatterns(string[] filePatterns, string[] filesToExclude)
        {
            var filesToInclude = new List<FilePattern>();

            filePatterns = filePatterns ?? Array.Empty<string>();
            filesToExclude = filesToExclude ?? Array.Empty<string>();

            if (!filePatterns.Any())
            {
                // If there are no patterns provided use a pattern that matches every file.
                filesToInclude.Add(FilePattern.Parse("**/*"));
            }

            foreach (var fileToExclude in filesToExclude)
            {
                // To support the legacy filesToExclude argument we add an exclude rule for each file exclude.
                // The files-to-exclude allowed to specify folders and relative paths.
                var path = fileToExclude;

                if (!Path.HasExtension(path))
                {
                    // Folder exclude
                    path = Path.Combine(path, "*.*");
                }

                // Remove relative path anchors
                if (path.StartsWith(".\\") || path.StartsWith("./"))
                {
                    path = path.Remove(0, 2);
                }

                filesToInclude.Add(FilePattern.Parse("!" + path));
            }

            foreach (var includePattern in filePatterns)
            {
                filesToInclude.Add(FilePattern.Parse(includePattern));
            }

            if (filesToInclude.All(f => f.IsExclude))
            {
                // If there are only exclude patterns, we add a pattern that matches every file.
                filesToInclude.Add(FilePattern.Parse("**/*"));
            }

            return filesToInclude;
        }

        private TestRunner ValidateTestRunner(string testRunner)
        {
            if (Enum.TryParse(testRunner, true, out TestRunner result))
            {
                return result;
            }
            else
            {
                throw new StrykerInputException(ErrorMessage, $"The given test runner ({testRunner}) is invalid. Valid options are: [{string.Join(",", Enum.GetValues(typeof(TestRunner)))}]");
            }
        }

        private string ValidateSolutionPath(string basePath, string solutionPath)
        {
            if (string.IsNullOrWhiteSpace(basePath) || string.IsNullOrWhiteSpace(solutionPath))
            {
                return null;
            }

            solutionPath = FilePathUtils.NormalizePathSeparators(Path.Combine(basePath, solutionPath));

            return solutionPath;
        }

        private string ValidateTestProjectFilter(string basePath, string userSuppliedFilter)
        {
            string filter;
            if (userSuppliedFilter.Contains(".."))
            {
                filter = FilePathUtils.NormalizePathSeparators(Path.GetFullPath(Path.Combine(basePath, userSuppliedFilter)));
            }
            else
            {
                filter = FilePathUtils.NormalizePathSeparators(userSuppliedFilter);
            }

            if (userSuppliedFilter.Contains("..") && !filter.StartsWith(basePath))
            {
                throw new StrykerInputException(ErrorMessage,
                    $"The test project filter {userSuppliedFilter} is invalid. Test project file according to filter should exist at {filter} but this is not a child of {FilePathUtils.NormalizePathSeparators(basePath)} so this is not allowed.");
            }
            return filter;
        }

        private LanguageVersion ValidateLanguageVersion(string languageVersion)
        {
            if (Enum.TryParse(languageVersion, true, out LanguageVersion result) && result != LanguageVersion.CSharp1)
            {
                return result;
            }
            else
            {
                throw new StrykerInputException(ErrorMessage,
                    $"The given c# language version ({languageVersion}) is invalid. Valid options are: [{string.Join(",", ((IEnumerable<LanguageVersion>)Enum.GetValues(typeof(LanguageVersion))).Where(l => l != LanguageVersion.CSharp1))}]");
            }
        }
    }
}
