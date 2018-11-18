using Serilog.Events;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
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
        public string BasePath { get; }
        public string Reporter { get; }
        public LogOptions LogOptions { get; set; }

        /// <summary>
        /// The user can pass a filter to match the project under test from multiple project references
        /// </summary>
        public string ProjectUnderTestNameFilter { get; }

        public int AdditionalTimeoutMS { get; }

        public int MaxConcurrentTestrunners { get; }

        public ThresholdOptions ThresholdOptions { get; set; }

        public List<string> FilesToExclude { get; set; }

        public StrykerOptions(string basePath, string reporter, string projectUnderTestNameFilter, int additionalTimeoutMS, string logLevel, bool logToFile, 
        int maxConcurrentTestRunners, int thresholdHigh, int thresholdLow, int thresholdBreak, string filesToExclude)
        {
            BasePath = basePath;
            Reporter = ValidateReporter(reporter);
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            AdditionalTimeoutMS = additionalTimeoutMS;
            LogOptions = new LogOptions(ValidateLogLevel(logLevel), logToFile);
            MaxConcurrentTestrunners = ValidateMaxConcurrentTestrunners(maxConcurrentTestRunners);
            ThresholdOptions = ValidateThresholds(thresholdHigh, thresholdLow, thresholdBreak);
            FilesToExclude = ValidateFilesToExclude(filesToExclude);
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
                    return LogEventLevel.Warning;
                case "info":
                case "":
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

        private ThresholdOptions ValidateThresholds(int thresholdHigh, int thresholdLow, int thresholdBreak) 
        {
            List<int> thresholdList = new List<int> {thresholdHigh, thresholdLow, thresholdBreak};
            if(thresholdList.Any(x => x > 100 || x < 0)) {
                throw new ValidationException("The thresholds must be between 0 and 100");
            }

            // ThresholdLow and ThresholdHigh can be the same value
            if (thresholdBreak >= thresholdLow || thresholdLow > thresholdHigh) 
            {
                throw new ValidationException("The values of your thresholds are incorrect. Change `--threshold-break` to the lowest value and `--threshold-high` to the highest.");
            }

            return new ThresholdOptions(thresholdHigh, thresholdLow, thresholdBreak);
        }

        private List<string> ValidateFilesToExclude(string filesToExclude)
        {
            var excludedFiles = new List<string>();
            try
            {
                var jsonExcludedFiles = JsonConvert.DeserializeObject<List<string>>(filesToExclude);

                foreach (var excludedFile in jsonExcludedFiles)
                {
                    var platformFilePath = FilePathUtils.ConvertToPlatformSupportedFilePath(excludedFile);
                    excludedFiles.Add(platformFilePath);
                }
            }
            catch
            {
                throw new ValidationException("Invalid JSON value provided for --files-to-exclude. The correct format, for example, should be: ['./ExampleClass.cs','./ExampleDirectory/ExampleClass2.cs','C:\\ExampleDirectory\\ExampleClass.cs'].");
            }

            return excludedFiles;
        }
    }
}
