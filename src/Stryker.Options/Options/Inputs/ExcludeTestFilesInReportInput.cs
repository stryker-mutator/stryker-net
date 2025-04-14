namespace Stryker.Abstractions.Options.Inputs;

public class ExcludeTestFilesInReportInput : Input<bool?>
{
    public override bool? Default => false;
    protected override string Description => "Exclude test files in the report. This may reduce the size of the report significantly.";
    public bool Validate() => SuppliedInput ?? false;
}
