using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.HelpText;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Stryker.CLI;

[ExcludeFromCodeCoverage] // Not worth the effort to test
internal class GroupedHelpTextGenerator : DefaultHelpTextGenerator
{
    protected override void GenerateOptions(CommandLineApplication application, TextWriter output, IReadOnlyList<CommandOption> visibleOptions, int firstColumnWidth)
    {
        if (visibleOptions.Any())
        {
            output.WriteLine();
            output.WriteLine("Options:");
            var outputFormat = $"  {{0, -{firstColumnWidth}}}{{1}}";

            var visibleCategorizedOptions = application.Options.OfType<StrykerInputOption>().Intersect(visibleOptions);

            foreach (var group in visibleCategorizedOptions.Cast<StrykerInputOption>().GroupBy(c => c.Category).OrderBy(g => g.Key))
            {
                if (group.Key != InputCategory.Generic)
                {
                    output.WriteLine();
                    output.WriteLine($"{group.Key} options:");
                }

                foreach (var opt in group)
                {
                    var description = opt.Description;

                    var wrappedDescription = IndentWriter?.Write(description);
                    var message = string.Format(outputFormat, Format(opt), wrappedDescription);

                    output.Write(message);
                    output.WriteLine();
                    output.WriteLine();
                }
            }
        }
    }
}
