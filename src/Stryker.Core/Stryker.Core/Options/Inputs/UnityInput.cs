namespace Stryker.Core.Options.Inputs;

public class UnityInput : Input<bool>
{
    public override bool Default => false;

    protected override string Description => "Is Unity project?";

    public bool Validate() => SuppliedInput;
}
