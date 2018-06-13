using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using System.Collections.Generic;

namespace Stryker.Core.Options
{
    public class ValidatedStrykerOptions
    {
        public string BasePath { get; }
        public string Reporter { get; }
        public LogOptions LogOptions { get; set; }

        /// <summary>
        /// This list of mutators will be used while mutating
        /// </summary>
        public ICollection<IMutator> Mutators { get; }

        /// <summary>
        /// The user can pass a filter to match the project under test from multiple project references
        /// </summary>
        public string ProjectUnderTestNameFilter { get; }

        public int AdditionalTimeoutMS { get; }

        public ValidatedStrykerOptions(string basePath,
            string reporter, 
            string projectUnderTestNameFilter,
            int additionalTimeoutMS,
            LogOptions logOptions,
            ICollection<IMutator> mutators)
        {
            BasePath = basePath;
            Reporter = reporter;
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            AdditionalTimeoutMS = additionalTimeoutMS;
            Mutators = mutators;
            LogOptions = logOptions;
        }

    }
}
