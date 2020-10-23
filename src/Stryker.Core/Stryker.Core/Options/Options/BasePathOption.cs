using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
	public class BasePathOption : BaseStrykerOption<string>
	{
		public BasePathOption(string basePath)
		{
            if (!string.IsNullOrWhiteSpace(basePath))
			{
				Value = basePath;
				return;
			}
			throw new StrykerInputException("BasePath cannot be null");
		}

		public override StrykerOption Type => StrykerOption.BasePath;
		public override string HelpText => "";
		public override string DefaultValue => "";
	}
}
