using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Options.Options
{
	public class ReportersOption : BaseStrykerOption<string[]>
	{
		public override StrykerOption Type => StrykerOption.Reporters;
		public override string HelpText => "";
		public override string[] DefaultValue => null;
		protected override void Validate(params string[] parameters)
		{
			throw new NotImplementedException();
		}
	}
}
