using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Options;

namespace Stryker.Configuration.Options.Inputs;

public class ProjectVersionInput : Input<string>
{
    public override string Default => string.Empty;

    protected override string Description => "Project version used in dashboard reporter and baseline feature.";

    public string Validate(IEnumerable<Reporter> reporters, bool withBaseline)
    {
        if (reporters.Contains(Reporter.Dashboard) || reporters.Contains(Reporter.RealTimeDashboard) || withBaseline)
        {
            return SuppliedInput ?? Default;
        }

        return Default;
    }
}
