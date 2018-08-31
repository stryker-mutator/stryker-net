using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;

namespace Stryker.Core.Options
{
    public class StrykerOptions
    {
        public string BasePath { get; private set; }
        public string Reporter { get; private set; }
        public LogOptions LogOptions { get; set; }

        /// <summary>
        /// The user can pass a filter to match the project under test from multiple project references
        /// </summary>
        public string ProjectUnderTestNameFilter { get; private set; }

        public int AdditionalTimeoutMS { get; private set;}

        public StrykerOptions(string basePath, string reporter, string projectUnderTestNameFilter, int additionalTimeoutMS, string logLevel, bool logToFile)
        {
            BasePath = basePath;
            Reporter = ValidateReporter(reporter);
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            AdditionalTimeoutMS = additionalTimeoutMS;
            LogOptions = new LogOptions(logLevel, logToFile);
        }

        private string ValidateReporter(string reporter)
        {
            if (reporter != "Console" && reporter != "RapportOnly")
            {
                throw new ValidationException("The reporter options are [Console, RapportOnly]");
            }
            return reporter;
        }
    }
}
