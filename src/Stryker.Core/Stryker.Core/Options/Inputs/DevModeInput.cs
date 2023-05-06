namespace Stryker.Core.Options.Inputs;

public class DevModeInput : Input<bool?>
{
    public override bool? Default => false;

    protected override string Description => @"Stryker automatically removes all mutations from a method if a failed mutation could not be rolled back
    Setting this flag makes stryker not remove the mutations but rather crash on failed rollbacks";

    public bool Validate()
    {
        return SuppliedInput ?? false;
    }
}
