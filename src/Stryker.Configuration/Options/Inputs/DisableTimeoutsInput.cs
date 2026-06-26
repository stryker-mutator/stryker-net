using Stryker.Abstractions.Options;

namespace Stryker.Configuration.Options.Inputs;

public class DisableTimeoutsInput : Input<bool?>
{
    public override bool? Default => false;

    protected override string Description => "After a mutation test run, adds Stryker disable comments to source files for mutants that caused timeouts.";

    public bool Validate() => SuppliedInput == true;
}
