using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class ReportersOption : BaseStrykerOption<IEnumerable<Reporter>>
    {
        public ReportersOption(bool compareToDashboard, string[] reporters)
        {
            var list = new List<Reporter>();
            if (reporters == null)
            {
                foreach (var reporter in new[] { Reporter.Progress, Reporter.Html })
                {
                    list.Add(reporter);
                }
                Value = list;
                return;
            }
            if (compareToDashboard)
            {
                var reportersList = reporters.ToList();
                reportersList.Add("Baseline");
                reporters = reportersList.ToArray();
            }

            IList<string> invalidReporters = new List<string>();
            foreach (var reporter in reporters)
            {
                if (Enum.TryParse(reporter, true, out Reporter result))
                {
                    list.Add(result);
                }
                else
                {
                    invalidReporters.Add(reporter);
                }
            }
            if (invalidReporters.Any())
            {
                throw new StrykerInputException(
                    ErrorMessage,
                    $"These reporter values are incorrect: {string.Join(", ", invalidReporters)}. Valid reporter options are [{string.Join(", ", (IEnumerable<Reporter>)Enum.GetValues(typeof(Reporter)))}]");
            }
            // If we end up here then the user probably disabled all reporters. Return empty IEnumerable.
            Value = list;
        }
        public override StrykerOption Type => StrykerOption.Reporters;
        public override string HelpText => "Sets the reporter";
        public override IEnumerable<Reporter> DefaultValue => null;
    }
}
