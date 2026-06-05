using Stryker.Abstractions.Baseline;
using Stryker.Abstractions.Exceptions;

namespace Stryker.Configuration.Options.Inputs;

public class BaselineOutputInput : Input<string>
{
    protected override string Description => "The directory the disk baseline provider stores and loads the baseline report from. Required when the disk baseline provider is selected.";

    public override string Default => string.Empty;

    public string Validate(BaselineProvider baselineProvider, bool withBaseline)
    {
        if (withBaseline && baselineProvider == BaselineProvider.Disk)
        {
            if (string.IsNullOrWhiteSpace(SuppliedInput))
            {
                throw new InputException("BaselineOutput can't be null or whitespace");
            }

            return SuppliedInput;
        }
        return Default;
    }
}
