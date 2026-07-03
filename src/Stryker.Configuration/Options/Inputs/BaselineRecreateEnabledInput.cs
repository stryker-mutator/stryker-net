namespace Stryker.Configuration.Options.Inputs;

public class BaselineRecreateEnabledInput : Input<bool?>
{
    public override bool? Default => false;

    protected override string Description => "When enabled a new baseline will be created by doing a full run and storing the mutation results.";

    public BaselineRecreateEnabledInput() { }

    public bool Validate()
    {
        return SuppliedInput ?? Default.Value;
    }
}