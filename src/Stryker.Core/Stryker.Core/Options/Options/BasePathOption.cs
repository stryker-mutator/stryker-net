using Stryker.Core.Exceptions;
using System.IO.Abstractions;

namespace Stryker.Core.Options.Options
{
	public class BasePathOption : BaseStrykerOption<string>
	{
		public BasePathOption(string basePath, IFileSystem fileSystem)
		{
            if (!string.IsNullOrWhiteSpace(basePath))
			{
				Value = basePath;
			}
            else
            {
				throw new StrykerInputException("BasePath cannot be null");
            }

			if (fileSystem.File.Exists(Value))  //validate file existance and maintain moq
			{
				return;
			}
			else
			{
				throw new StrykerInputException("SolutionPath does not exist");
			}
		}

		public override StrykerOption Type => StrykerOption.BasePath;
		public override string HelpText => "";
		public override string DefaultValue => "";
	}
}
