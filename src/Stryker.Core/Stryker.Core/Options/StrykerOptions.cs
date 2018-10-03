using Serilog.Events;
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

        public int ThresholdBreak { get; }

        public int ThresholdLow { get; }

        public int ThresholdHigh { get; }

        public StrykerOptions(string basePath, string reporter, string projectUnderTestNameFilter, int additionalTimeoutMS, string logLevel, bool logToFile, 
        int maxConcurrentTestRunners, int thresholdBreak, int thresholdLow, int thresholdHigh)
        {
            BasePath = basePath;
            Reporter = ValidateReporter(reporter);
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            AdditionalTimeoutMS = additionalTimeoutMS;
            LogOptions = new LogOptions(ValidateLogLevel(logLevel), logToFile);
            MaxConcurrentTestrunners = ValidateMaxConcurrentTestrunners(maxConcurrentTestRunners);
            int[] ThresholdList = ValidateThresholds(new int[3]{thresholdBreak, thresholdLow, thresholdHigh});
            ThresholdBreak = ThresholdList[0];
            ThresholdLow = ThresholdList[1];
            ThresholdHigh = ThresholdList[2];
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

        private int[] ValidateThresholds(int[] thresholdList) 
        {
            foreach(int threshold in thresholdList) 
            {
               if(threshold > 100) 
               {
                   throw new ValidationException("The threshold can never be > 100");
               } else if (threshold < 0)
               {
                   throw new ValidationException("The threshold can not be < 0");
               }
            }

            if (thresholdList[0] >= thresholdList[1] || 
                thresholdList[1] >= thresholdList[2] || 
                thresholdList[0] >= thresholdList[2]) 
            {
                throw new ValidationException("Check if the --threshold-break is the lowest value and --threshold-low value is lower than --threshold-high");
            }

            return thresholdList;
        }
    }
}
