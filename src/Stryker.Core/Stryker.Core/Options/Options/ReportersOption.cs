using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class ReportersOption : BaseStrykerOption<IEnumerable<Reporter>>
    {
        public ReportersOption(IEnumerable<string> chosenReporters, bool compareToDashboard)
        {
            var reporters = new List<Reporter>();

            if (chosenReporters is null)
            {
                foreach (var reporter in DefaultValue)
                {
                    reporters.Add(reporter);
                }
            }

            ValidateChosenReporters(chosenReporters, reporters);

            if (compareToDashboard)
            {
                reporters.Add(Reporter.Baseline);
            }

            Value = reporters;
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

        public override StrykerOption Type => StrykerOption.Reporters;
        public override string HelpText => "Choose the reporters to enable.";
        public override IEnumerable<Reporter> DefaultValue => new[] { Reporter.Progress, Reporter.Html };
    }
}
