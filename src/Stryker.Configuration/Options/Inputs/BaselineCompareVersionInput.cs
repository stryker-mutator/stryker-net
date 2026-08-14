namespace Stryker.Configuration.Options.Inputs;

public class BaselineCompareVersionInput : Input<string>
{
    public override string Default => string.Empty;
    protected override string Description => "The version of the baseline report to compare the current codebase with. When empty the version of the current run is used, resulting in an incremental run.";

    public string Validate(bool baselineEnabled)
    {
        if (baselineEnabled && !string.IsNullOrWhiteSpace(SuppliedInput))
        {
            return SuppliedInput;
        }

        return Default;
    }
}
