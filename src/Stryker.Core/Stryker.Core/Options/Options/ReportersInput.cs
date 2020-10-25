using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class ReportersInput : ComplexStrykerInput<IEnumerable<Reporter>, IEnumerable<string>>
    {
        static ReportersInput()
        {
            HelpText = $@"Sets the reporter | { FormatOptionsString<string, Reporter>(DefaultInput, (IEnumerable<Reporter>)Enum.GetValues(DefaultValue.First().GetType()), new List<Reporter> { Reporter.ConsoleProgressBar, Reporter.ConsoleProgressDots, Reporter.ConsoleReport }) }]
    This argument takes a json array as a value. Example: ['{ Reporter.Progress }', '{ Reporter.Html }']";
            DefaultInput = new List<string>() { "Reporter.Progress", "Reporter.Html" };
            DefaultValue = new ReportersInput(DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.Reporters;

        public ReportersInput(IEnumerable<string> chosenReporters)
        {
            var reporters = new List<Reporter>();
            if (chosenReporters != null)
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

                Value = reporters;
            }
        }

        public IEnumerable<Reporter> ReportersList(bool compareToDashboard)
        {
            var list = new List<Reporter>(Value);
            if (compareToDashboard) { list.Add(Reporter.Baseline); }
            return list;
        }
    }
}
