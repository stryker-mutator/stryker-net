using Stryker.Core.Exceptions;
using System.IO;
using System.IO.Abstractions;

namespace Stryker.Core.Options.Options
{
	class SolutionPathOption : BaseStrykerOption<string>
	{

		public SolutionPathOption(string basePath, string solutionPath, IFileSystem fileSystem) : base(basePath, solutionPath)
		{
			if (fileSystem.File.Exists(Value))	//validate file existance and maintain moq
			{
				return;
			}
			throw new StrykerInputException("SolutionPath does not exist");
		}

		public override StrykerOption Type => StrykerOption.BasePath;
		public override string Name => nameof(SolutionPathOption);
		public override string HelpText => "";
		public override string DefaultValue => null;

		protected override void Validate(params string[] parameters)
		{
			if (parameters.Length == 2)
			{
				if (!string.IsNullOrWhiteSpace(parameters[0]) && !string.IsNullOrWhiteSpace(parameters[1]))
				{
					Value = FilePathUtils.NormalizePathSeparators(Path.Combine(parameters[0], parameters[1]));
				}
			}
			throw new StrykerInputException("SolutionPath needs two parameters: basePath, solutionPath");
		}
	}
}
