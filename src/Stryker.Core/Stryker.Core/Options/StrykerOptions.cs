﻿using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;

namespace Stryker.Core.Options
{
    public class StrykerOptions
    {
        public string BasePath { get; }
        public string Reporter { get; }
        public LogOptions LogOptions { get; set; }

        /// <summary>
        /// The user can pass a filter to match the project under test from multiple project references
        /// </summary>
        public string ProjectUnderTestNameFilter { get; }

        public int AdditionalTimeoutMS { get; }

        public int MaxConcurrentTestrunners { get; }

        public StrykerOptions(string basePath, string reporter, string projectUnderTestNameFilter, int additionalTimeoutMS, string logLevel, bool logToFile,
            int maxConcurrentTestRunners)
        {
            BasePath = basePath;
            Reporter = ValidateReporter(reporter);
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            AdditionalTimeoutMS = additionalTimeoutMS;
            LogOptions = new LogOptions(ValidateLogLevel(logLevel), logToFile);
            MaxConcurrentTestrunners = ValidateMaxConcurrentTestrunners(maxConcurrentTestRunners);
        }

        private string ValidateReporter(string reporter)
        {
            if (reporter != "Console" && reporter != "ReportOnly")
            {
                throw new ValidationException("The reporter options are [Console, ReportOnly]");
            }
            return reporter;
        }

        private LogEventLevel ValidateLogLevel(string levelText)
        {
            switch (levelText?.ToLower() ?? "")
            {
                case "error":
                    return LogEventLevel.Error;
                case "warning":
                case "":
                    return LogEventLevel.Warning;
                case "info":
                    return LogEventLevel.Information;
                case "debug":
                    return LogEventLevel.Debug;
                case "trace":
                    return LogEventLevel.Verbose;
                default:
                    throw new ValidationException("The log level options are [Error, Warning, Info, Debug, Trace]");
            }
        }

        private int ValidateMaxConcurrentTestrunners(int maxConcurrentTestRunners)
        {
            if(maxConcurrentTestRunners < 1)
            {
                throw new ValidationException("Amount of maximum concurrent testrunners must be greater than zero.");
            }
            return maxConcurrentTestRunners;
        }
    }
}
