using Stryker.Core.Reporters;
using Stryker.Core.Baseline;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Stryker.Core.Options.Options
{
	public class BaselineProviderOption : BaseStrykerOption<BaselineProvider>
	{
		BaselineProviderOption(IEnumerable<Reporter> reporters, string baselineProviderLocation) : base(baselineProviderLocation)
		{
			var normalizedLocation = baselineProviderLocation?.ToLower() ?? "";

			if (string.IsNullOrEmpty(normalizedLocation) && reporters.Any(x => x == Reporter.Dashboard))
			{
				Value = BaselineProvider.Dashboard;
			}
			else
			{
				Value = normalizedLocation switch
				{
					"disk" => BaselineProvider.Disk,
					"dashboard" => BaselineProvider.Dashboard,
					"azurefilestorage" => BaselineProvider.AzureFileStorage,
					_ => BaselineProvider.Disk,
				};
			}
		}

		public override StrykerOption Type => StrykerOption.BaselineProvider;
		public override string HelpText => "";
		public override BaselineProvider DefaultValue => BaselineProvider.Disk;
		protected override void Validate(params string[] parameters)
		{

		}
	}
}
