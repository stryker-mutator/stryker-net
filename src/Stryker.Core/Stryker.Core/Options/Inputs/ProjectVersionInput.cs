using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Exceptions;
using Stryker.Core.Reporters;

namespace Stryker.Core.Options.Inputs;

public class ProjectVersionInput : Input<string>
{
    public override string Default => string.Empty;

    protected override string Description => "Project version used in dashboard reporter and baseline feature.";

    public string Validate(IEnumerable<Reporter> reporters, bool withBaseline)
    {
        if (reporters.Contains(Reporter.Dashboard) || withBaseline)
        {
            if (withBaseline && string.IsNullOrWhiteSpace(SuppliedInput))
            {
                throw new InputException("Project version cannot be empty when baseline is enabled");
            }
            return SuppliedInput ?? Default;
        }

        return Default;
    }
}
