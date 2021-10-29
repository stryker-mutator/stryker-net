using System;
using System.Collections.Generic;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public enum ReportType
    {
        None,
        HTMLReport
    }

    public class OpenReporterInput : Input<string>
    {
        public override string Default => "None";

        protected override string Description => "The dashboard to open automatically";

        protected override IEnumerable<string> AllowedOptions => Enum.GetNames(typeof(ReportType));

        public ReportType Validate()
        {
            if (SuppliedInput is { })
            {
                if (Enum.TryParse(SuppliedInput, true, out ReportType result))
                {
                    return result;
                }
                else
                {
                    throw new InputException($"The given report ({SuppliedInput}) is invalid. Valid options are: [{string.Join(", ", ((IEnumerable<ReportType>)Enum.GetValues(typeof(ReportType))))}]");
                }
            }
            return ReportType.None;
        }
    }
}
