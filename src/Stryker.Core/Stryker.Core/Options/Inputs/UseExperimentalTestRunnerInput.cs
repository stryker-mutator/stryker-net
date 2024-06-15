namespace Stryker.Core.Options.Inputs;
public class UseExperimentalTestRunnerInput : Input<bool?>
{
    public override bool? Default => false;

    protected override string Description => "If Stryker should use the experimental MSTest runner";

    public bool Validate() => SuppliedInput ?? false;

}
