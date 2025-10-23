namespace Stryker.Abstractions.Options.Inputs;

public class DiagModeInput : Input<bool?>
{
    public override bool? Default => false;

    protected override string Description => @"Stryker enters diagnostic mode. Useful when encountering issues.
Setting this flag makes Stryker increase the debug level and log more information to help troubleshooting.";

    public bool Validate() => SuppliedInput ?? false;
}
