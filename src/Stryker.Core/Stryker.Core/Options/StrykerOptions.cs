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

        public string AdditionalTimeoutMS { get; }

        public StrykerOptions(string basePath,
            string reporter, 
            string projectUnderTestNameFilter,
            string additionalTimeoutMS = "2000",
            LogOptions logOptions = null)
        {
            BasePath = basePath;
            Reporter = reporter;
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            AdditionalTimeoutMS = additionalTimeoutMS;
            LogOptions = logOptions ?? new LogOptions(null, false);
        }

    }
}
