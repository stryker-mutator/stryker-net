using Stryker.Core.Logging;

namespace Stryker.Core.Options
{
    public class ValidatedStrykerOptions
    {
        public string BasePath { get; }
        public string Reporter { get; }
        public LogOptions LogOptions { get; set; }

        /// <summary>
        /// The user can pass a filter to match the project under test from multiple project references
        /// </summary>
        public string ProjectUnderTestNameFilter { get; }

        public int AdditionalTimeoutMS { get; }

        public ValidatedStrykerOptions(string basePath,
            string reporter, 
            string projectUnderTestNameFilter,
            int additionalTimeoutMS,
            LogOptions logOptions)
        {
            BasePath = basePath;
            Reporter = reporter;
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            AdditionalTimeoutMS = additionalTimeoutMS;
            LogOptions = logOptions;
        }

    }
}
