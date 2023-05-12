namespace Stryker.Core.Options.Inputs;

public class BreakOnInitialTestFailureInput : Input<bool?>
{
    public override bool? Default => false;

    protected override string Description => "Instruct Stryker to break execution when at least one test failed on initial run.";

    public bool Validate()
    {
        return SuppliedInput == true;
    }
}
