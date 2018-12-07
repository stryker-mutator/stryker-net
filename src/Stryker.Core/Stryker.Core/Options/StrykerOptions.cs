using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options
{
    public class StrykerOptions
    {
        private const string ErrorMessage = "The value for one of your settings is not correct. Try correcting or removing them.";
        public string BasePath { get; }
        public string[] Reporters { get; }
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

        public StrykerOptions(string basePath,
            string[] reporters,
            string projectUnderTestNameFilter,
            int additionalTimeoutMS,
            string[] excludedMutations,
            string logLevel,
            bool logToFile,
            bool devMode,
            int maxConcurrentTestRunners,
            int thresholdHigh,
            int thresholdLow,
            int thresholdBreak)
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
        }

        private string[] ValidateReporters(string[] reporters)
        {
            string[] validReporters = new[] { "Console", "ReportOnly", "Json" };
            if (reporters.Any(r => !validReporters.Any(vr => vr.ToLowerInvariant() == r.ToLowerInvariant())))
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    $"Some of these reporters are incorrect: ({string.Join(",", reporters)}). The reporter options are [{string.Join(",", validReporters)}]");
            }
            return reporters;
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
    }
}
