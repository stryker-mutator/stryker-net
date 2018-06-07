using Serilog.Events;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using System.Collections.Generic;

namespace Stryker.Core.Options
{
    public class StrykerOptions
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

        public StrykerOptions(string basePath,
            string reporter, 
            string projectUnderTestNameFilter,
            LogOptions logOptions = null,
            ICollection<IMutator> mutators = null)
        {
            BasePath = basePath;
            Reporter = reporter;
            ProjectUnderTestNameFilter = projectUnderTestNameFilter;
            Mutators = mutators;
            LogOptions = logOptions ?? new LogOptions(null, false);
        }

    }
}
