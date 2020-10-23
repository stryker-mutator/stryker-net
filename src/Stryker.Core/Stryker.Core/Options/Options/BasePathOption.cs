using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
	public class BasePathOption : BaseStrykerOption<string>
	{
		public BasePathOption(string basePath) : base(basePath)
		{

		}

		public override StrykerOption Type => StrykerOption.BasePath;
		public override string HelpText => "";
		public override string DefaultValue => "";

		protected override void Validate(params string[] parameters)
		{
			foreach (var param in parameters)
			{
				Value = param;
				return;
			}
			throw new StrykerInputException("BasePath cannot be null");
		}
	}
}
