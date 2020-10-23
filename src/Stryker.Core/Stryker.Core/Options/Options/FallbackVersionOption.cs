using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
    public class FallbackVersionOption : BaseStrykerOption<string>
    {
        public FallbackVersionOption(string fallbackVersion) : base(fallbackVersion)
        {
        }

        public override StrykerOption Type => StrykerOption.FallbackVersion;

        public override string Name => nameof(FallbackVersionOption);

        public override string HelpText => "Report version used when no report could be found for the project version. Can be semver, git commit hash, branch name or anything else to indicate what version of your software you're testing. When you don't specify a fallback version, the GitTarget option will be used as fallback version. Example: If the current branch is based on the master branch, set 'master' as the fallback version";

        public override string DefaultValue => null;

        protected override void Validate(params string[] parameters)
        {
            foreach (var param in parameters)
            {
                if (param == "")
                {
                    throw new StrykerInputException("{0} cannot be empty. Either fill the option or leave it out.", Name);
                }
                Value = param;
                return;
            }
        }
    }
}
