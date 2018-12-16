using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Stryker.Core.Options
{
    public class StrykerOptions
    {
        private const string ErrorMessage = "The value for one of your settings is not correct. Try correcting or removing them.";
        public string BasePath { get; }
        public IEnumerable<Reporter> Reporters { get; }
        public LogOptions LogOptions { get; }
        public bool DevMode { get; }

        /// <summary>
        /// The user can pass a filter to match the project under test from multiple project references
        /// </summary>
        public string ProjectUnderTestNameFilter { get; }

        public int AdditionalTimeoutMS { get; }

        public IEnumerable<MutatorType> ExcludedMutations { get; }

        public int MaxConcurrentTestrunners { get; }

        public ThresholdOptions ThresholdOptions { get; }

        public IEnumerable<string> FilesToExclude { get; }
        public StrykerOptions(string basePath = "",
            string[] reporters = null,
            string projectUnderTestNameFilter = "",
            int additionalTimeoutMS = 30000,
            string[] excludedMutations = null,
            string logLevel = "info",
            bool logToFile = false,
            bool devMode = false,
            int maxConcurrentTestRunners = int.MaxValue,
            int thresholdHigh = 80,
            int thresholdLow = 60,
            int thresholdBreak = 0,
            string[] filesToExclude = null)
        {
            BasePath = basePath;
            Reporters = ValidateReporters(reporters);
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            AdditionalTimeoutMS = additionalTimeoutMS;
            ExcludedMutations = ValidateExludedMutations(excludedMutations);
            LogOptions = new LogOptions(ValidateLogLevel(logLevel), logToFile);
            DevMode = devMode;
            MaxConcurrentTestrunners = ValidateMaxConcurrentTestrunners(maxConcurrentTestRunners);
            ThresholdOptions = ValidateThresholds(thresholdHigh, thresholdLow, thresholdBreak);
            FilesToExclude = ValidateFilesToExclude(filesToExclude);
        }

        private IEnumerable<Reporter> ValidateReporters(string[] reporters)
        {
            if (reporters == null)
            {
                foreach (var reporter in new[] { Reporter.ConsoleProgressBar, Reporter.ConsoleReport })
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

        private IEnumerable<MutatorType> ValidateExludedMutations(IEnumerable<string> excludedMutations)
        {
            if (excludedMutations == null)
            {
                yield break;
            }

            // Get all mutatorTypes and their descriptions
            Dictionary<MutatorType, string> typeDescriptions = Enum.GetValues(typeof(MutatorType))
                .Cast<MutatorType>()
                .ToDictionary(x => x, x => x.GetDescription());

            foreach (string excludedMutation in excludedMutations)
            {
                // Find any mutatorType that matches the name passed by the user
                if (typeDescriptions.Single(x => x.Value.ToString().ToLower()
                    .Contains(excludedMutation.ToLower())).Key
                    is var foundMutator)
                {
                    yield return foundMutator;
                }
                else
                {
                    throw new StrykerInputException($"Invalid excluded mutation '{excludedMutation}'", $"The excluded mutations options are [{String.Join(", ", typeDescriptions.Select(x => x.Key))}]");
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
                        $"Incorrect log level {(levelText)}. The log level options are [Error, Warning, Info, Debug, Trace]");
            }
        }

        private int ValidateMaxConcurrentTestrunners(int maxConcurrentTestRunners)
        {
            if (maxConcurrentTestRunners < 1)
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    "Amount of maximum concurrent testrunners must be greater than zero.");
            }
            return maxConcurrentTestRunners;
        }

        private ThresholdOptions ValidateThresholds(int thresholdHigh, int thresholdLow, int thresholdBreak)
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

            return new ThresholdOptions(thresholdHigh, thresholdLow, thresholdBreak);
        }

        private IEnumerable<string> ValidateFilesToExclude(string[] filesToExclude)
        {
            foreach (var excludedFile in filesToExclude ?? Enumerable.Empty<string>())
            {
                // The logger is not yet available here. The paths will be validated in the InputFileResolver
                var platformFilePath = FilePathUtils.ConvertToPlatformSupportedFilePath(excludedFile);
                yield return platformFilePath;
            }
        }
    }
}
