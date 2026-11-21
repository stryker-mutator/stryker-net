namespace Stryker.Configuration.Options.Inputs;

public class IgnoreMethodsEffectsInput : Input<bool?>
{
    public override bool? Default => false;

    protected override string Description => "Also ignore mutations whose only effect is on whether, or with what arguments, an ignored method is called.";

    public bool Validate() => SuppliedInput == true;
}
