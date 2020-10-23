using Stryker.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Options.Options
{
	class SolutionPath : BaseStrykerOption<string>
	{
		public SolutionPath(string basePath, string solutionPath) : base(basePath, solutionPath)
		{

		}

		public override StrykerOption Type => StrykerOption.BasePath;
		public override string Name => "BasePath";
		public override string HelpText => "";
		public override string Value { get => Value; protected set => Value = value; }
		public override string DefaultValue => null;

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
