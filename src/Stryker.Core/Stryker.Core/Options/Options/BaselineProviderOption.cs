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
		public BaselineProviderOption(IEnumerable<Reporter> reporters, string baselineProviderLocation)
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
		public override string HelpText => "Allows to choose a storage location.When using the azure file storage, make sure to configure the 'shared access storage' and 'storage url' options.";
		public override BaselineProvider DefaultValue => BaselineProvider.Disk;
	}
}
