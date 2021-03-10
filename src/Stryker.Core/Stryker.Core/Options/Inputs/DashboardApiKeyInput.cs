using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class DashboardApiKeyInput : InputDefinition<string>
    {
        protected override string Description => "Api key for dashboard reporter.";

        public override string Default => null;

        public string Validate(bool? dashboardEnabled)
        {
            if (dashboardEnabled.IsNotNullAndTrue())
            {
                if (SuppliedInput.IsNullOrEmptyInput())
                {
                    throw new StrykerInputException("When the stryker dashboard is enabled an api key required.");
                }

                return SuppliedInput;
            }
            return Default;
        }
    }
}
