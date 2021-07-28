using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options.Inputs
{
    public class ProjectNameInput : Input<string>
    {

        protected override string Description => @"The organizational name for your project. Required when dashboard reporter is turned on.
For example: Your project might be called 'consumer-loans' and it might contains sub-modules 'consumer-loans-frontend' and 'consumer-loans-backend'.";

        public override string Default => string.Empty;

        public string Validate(IEnumerable<Reporter> reporters)
        {
            if (reporters is { } && reporters.Contains(Reporter.Dashboard))
            {
                if (string.IsNullOrWhiteSpace(SuppliedInput))
                {
                    throw new InputException("When the stryker dashboard is enabled the project name is required.");
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
