using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options.Inputs
{
    public class ProjectVersionInput : Input<string>
    {
        public override string Default => string.Empty;

        protected override string Description => "Project version used in dashboard reporter and baseline feature.";

        public string Validate(string fallbackVersion, IEnumerable<Reporter> reporters, bool? dashboardCompareEnabled)
        {
            if (reporters.Contains(Reporter.Dashboard))
            {
                if (string.IsNullOrWhiteSpace(SuppliedInput))
                {
                    throw new InputException("When the stryker dashboard is enabled the project version is required. Please provide a project version.");
                }

                if (dashboardCompareEnabled.IsNotNullAndTrue() && fallbackVersion == SuppliedInput)
                {
                    throw new InputException("Project version cannot be the same as the fallback version. Please provide a different version for one of them.");
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
