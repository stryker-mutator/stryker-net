namespace Stryker.Configuration.Options.Inputs;

public class AutoIgnoreTimeoutsInput : Input<bool?>
{
    public override bool? Default => false;

    protected override string Description => "Mark mutants that resulted in a timeout as ignored using a Stryker comment in the source code, so they are skipped in future runs.";

    public bool Validate() => SuppliedInput == true;
}
