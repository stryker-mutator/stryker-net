namespace Stryker.Configuration.Options.Inputs;

public class FallbackVersionInput : Input<string>
{
    protected override string Description => @"Version used as a bootstrap when no baseline report could be found for the compare version.
Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.
Example: If the current branch is based on the main branch, set 'main' as the fallback version";

    public override string Default => "main";

    public string Validate(bool withBaseline)
    {
        if (withBaseline && SuppliedInput is not null)
        {
            return SuppliedInput;
        }

        return Default;
    }
}
