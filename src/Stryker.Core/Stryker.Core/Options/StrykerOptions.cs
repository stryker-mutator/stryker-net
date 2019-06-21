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
        public int AdditionalTimeoutMS { get; }
        public IEnumerable<Mutator> ExcludedMutations { get; }
        public int ConcurrentTestrunners { get; }

        public Threshold Thresholds { get; }
        public TestRunner TestRunner { get; set; }
        public IEnumerable<string> FilesToExclude { get; }

        private const string ErrorMessage = "The value for one of your settings is not correct. Try correcting or removing them.";
        private readonly IFileSystem _fileSystem;

        public StrykerOptions(
            IFileSystem fileSystem = null,
            string basePath = "",
            string[] reporters = null,
            string projectUnderTestNameFilter = "",
            int additionalTimeoutMS = 5000,
            string[] excludedMutations = null,
            string logLevel = "info",
            bool logToFile = false,
            bool devMode = false,
            int maxConcurrentTestRunners = int.MaxValue,
            int thresholdHigh = 80,
            int thresholdLow = 60,
            int thresholdBreak = 0,
            string[] filesToExclude = null,
            string testRunner = "dotnettest",
            string solutionPath = null)
        {
            _fileSystem = fileSystem ?? new FileSystem();

            var outputPath = ValidateOutputPath(basePath);
            BasePath = basePath;
            OutputPath = outputPath;
            Reporters = ValidateReporters(reporters);
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            AdditionalTimeoutMS = additionalTimeoutMS;
            ExcludedMutations = ValidateExludedMutations(excludedMutations);
            LogOptions = new LogOptions(ValidateLogLevel(logLevel), logToFile, outputPath);
            DevMode = devMode;
            ConcurrentTestrunners = ValidateConcurrentTestrunners(maxConcurrentTestRunners);
            Thresholds = ValidateThresholds(thresholdHigh, thresholdLow, thresholdBreak);
            FilesToExclude = ValidateFilesToExclude(filesToExclude);
            TestRunner = ValidateTestRunner(testRunner);
            SolutionPath = ValidateSolutionPath(basePath, solutionPath);
        }

        private string ValidateOutputPath(string basePath)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                return "";
            }

            var outputPath = Path.Combine(basePath, "StrykerOutput", DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss"));
            _fileSystem.Directory.CreateDirectory(FilePathUtils.ConvertPathSeparators(outputPath));

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

        private IEnumerable<Mutator> ValidateExludedMutations(IEnumerable<string> excludedMutations)
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
                if (typeDescriptions.FirstOrDefault(
                    x => x.Value.ToString().ToLower().Contains(excludedMutation.ToLower()))
                    .Key is var foundMutator)
                {
                    yield return foundMutator;
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

        private IEnumerable<string> ValidateFilesToExclude(string[] filesToExclude)
        {
            foreach (var excludedFile in filesToExclude ?? Enumerable.Empty<string>())
            {
                // The logger is not yet available here. The paths will be validated in the InputFileResolver
                var platformFilePath = FilePathUtils.ConvertPathSeparators(excludedFile);
                yield return platformFilePath;
            }
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

            solutionPath = FilePathUtils.ConvertPathSeparators(Path.Combine(basePath, solutionPath));

            return solutionPath;
        }
    }
}
