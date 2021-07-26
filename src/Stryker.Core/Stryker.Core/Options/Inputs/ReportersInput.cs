using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options.Inputs
{
    public class ReportersInput : InputDefinition<IEnumerable<string>>
    {
        public override IEnumerable<string> Default => new List<string>() { "Progress", "Html" };

        protected override string Description => "Reporters inform about various stages in the mutation testrun.";
        protected override IEnumerable<string> AllowedOptions => EnumToStrings(typeof(Reporter));

        public IEnumerable<Reporter> Validate()
        {
            if (SuppliedInput is { })
            {
                var reporters = new List<Reporter>();

                ValidateChosenReporters(SuppliedInput, reporters);

                return reporters;
            }
            return new List<Reporter> { Reporter.Progress, Reporter.Html };
        }

        private static void ValidateChosenReporters(IEnumerable<string> chosenReporters, List<Reporter> reporters)
        {
            IList<string> invalidReporters = new List<string>();

            foreach (var reporter in chosenReporters)
            {
                if (Enum.TryParse(reporter, true, out Reporter result))
                {
                    reporters.Add(result);
                }
                else
                {
                    invalidReporters.Add(reporter);
                }
            }
            if (invalidReporters.Any())
            {
                throw new InputException($"These reporter values are incorrect: {string.Join(", ", invalidReporters)}.");
            }
        }
    }
}
