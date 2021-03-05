using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options.Inputs
{
    public class ReportersInput : InputDefinition<IEnumerable<string>, IEnumerable<Reporter>>
    {
        public override IEnumerable<string> DefaultInput => new List<string>() { "Progress", "Html" };
        public override IEnumerable<Reporter> Default => new ReportersInput(DefaultInput).Value;

        protected override string Description => "Reporters inform about various stages in the mutation testrun.";
        protected override string HelpOptions => FormatEnumHelpOptions(DefaultInput, typeof(Reporter));

        public ReportersInput() { }
        public ReportersInput(IEnumerable<string> chosenReporters)
        {
            if (chosenReporters is { })
            {
                var reporters = new List<Reporter>();

                ValidateChosenReporters(chosenReporters, reporters);

                Value = reporters;
            }
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
                throw new StrykerInputException($"These reporter values are incorrect: {string.Join(", ", invalidReporters)}.");
            }
        }
    }
}
