namespace Stryker.Configuration.Options.Inputs;

public class BaselineOutputInput : Input<string>
{
    protected override string Description => "Changes the directory the disk baseline provider stores and loads the baseline report from. A relative path is resolved against the project path, an absolute path is used as-is. Unlike the timestamped output path, this location is stable so baselines persist across runs.";

    public override string Default => "StrykerOutput";

    public string Validate()
    {
        if (string.IsNullOrWhiteSpace(SuppliedInput))
        {
            return Default;
        }
        return SuppliedInput;
    }
}
