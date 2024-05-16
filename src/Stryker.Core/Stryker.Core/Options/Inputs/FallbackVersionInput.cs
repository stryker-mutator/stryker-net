namespace Stryker.Core.Options.Inputs;

public class FallbackVersionInput : Input<string>
{
    protected override string Description => @"Commitish used as a fallback when no report could be found based on Git information for the baseline feature.
Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing.
When you don't specify a fallback version the since target will be used as fallback version.
Example: If the current branch is based on the main branch, set 'main' as the fallback version";

    public override string Default => new SinceTargetInput().Default;

    public string Validate(bool withBaseline, string projectVersion, string sinceTarget)
    {
        if (withBaseline)
        {
            if(SuppliedInput is null)
            {
                return sinceTarget;
            }

            return SuppliedInput;
        }

        return Default;
    }
}
