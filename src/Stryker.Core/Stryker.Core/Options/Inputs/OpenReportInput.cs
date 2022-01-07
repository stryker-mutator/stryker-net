using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public enum ReportType
    {
        Html,
        Dashboard
    }

    public class OpenReportInput : Input<string>
    {
        public override string Default => "Html";

        protected override string Description => "If enabled stryker will attempt to open the Html report in your browser when the results are calculated.";

        protected override IEnumerable<string> AllowedOptions => Enum.GetNames(typeof(ReportType));

        public ReportType? Validate(bool enabled)
        {
            if (enabled)
            {
                if (SuppliedInput is null)
                {
                    return ReportType.Html;
                }
                else
                {
                    if (Enum.TryParse(SuppliedInput, true, out ReportType result))
                    {
                        return result;
                    }
                    else
                    {
                        throw new InputException($"The given report type ({SuppliedInput}) is invalid. Valid options are: [{string.Join(", ", (IEnumerable<ReportType>)Enum.GetValues(typeof(ReportType)))}]");
                    }
                }
            }

            return null;
        }
    }
}
