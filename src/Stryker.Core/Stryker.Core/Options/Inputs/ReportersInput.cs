using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    public class ReportersInput : ComplexStrykerInput<IEnumerable<string>, IEnumerable<Reporter>>
    {
        static ReportersInput()
        {
            DefaultInput = new List<string>() { "Progress", "Html" };
            DefaultValue = new ReportersInput(DefaultInput, CompareToDashboardInput.DefaultInput).Value;
            HelpText = $"Choose the reporters to enable. | {FormatOptions(DefaultInput, (IEnumerable<string>)Enum.GetValues(DefaultValue.First().GetType()))}";
        }

        public override StrykerInput Type => StrykerInput.Reporters;

        public ReportersInput(IEnumerable<string> chosenReporters, bool compareToDashboard)
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
    }
}