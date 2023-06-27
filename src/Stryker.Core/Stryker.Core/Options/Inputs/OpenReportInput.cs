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

        protected override string Description => "When this option is passed, generated reports should open in the browser automatically once Stryker starts testing mutants, and will update the report till Stryker is done. Both html and dashboard reports can be opened automatically.";

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
