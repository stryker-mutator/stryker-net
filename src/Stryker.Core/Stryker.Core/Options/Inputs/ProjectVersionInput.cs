using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options.Inputs
{
    public class ProjectVersionInput : InputDefinition<string>
    {
        public override string Default => string.Empty;

        protected override string Description => "Project version used in reporters.";

        public string Validate(string fallbackVersion, IEnumerable<Reporter> reporters, bool? dashboardCompareEnabled)
        {
            if (reporters.Contains(Reporter.Dashboard))
            {
                if (SuppliedInput.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("When the stryker dashboard is enabled the project version is required. Please provide a project version.");
                }

                if (dashboardCompareEnabled.IsNotNullAndTrue() && fallbackVersion == SuppliedInput)
                {
                    throw new StrykerInputException("Project version cannot be the same as the fallback version. Please provide a different version for one of them.");
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
