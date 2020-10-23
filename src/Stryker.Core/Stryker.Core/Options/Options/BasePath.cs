using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Options
{
	public class BasePath : BaseStrykerOption<string>
	{
		public BasePath(string basePath) : base(basePath)
		{

		}

		public override StrykerOption Type => StrykerOption.BasePath;
		public override string Name => "BasePath";
		public override string HelpText => "";
		public override string Value { get => Value; protected set => Value = value; }
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
