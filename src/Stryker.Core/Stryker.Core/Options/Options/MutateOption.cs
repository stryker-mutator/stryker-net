using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class MutateOption : BaseStrykerOption<IEnumerable<FilePattern>>
    {
        public MutateOption(IEnumerable<string> mutate)
        {
            if (mutate is { } && mutate.Any())
            {
                var filesToInclude = new List<FilePattern>();

                foreach (var pattern in mutate)
                {
                    filesToInclude.Add(FilePattern.Parse(FilePathUtils.NormalizePathSeparators(pattern)));
                }

                if (filesToInclude.All(f => f.IsExclude))
                {
                    // If there are only exclude patterns, we add a pattern that matches every file.
                    filesToInclude.Add(FilePattern.Parse("**/*"));
                }
            }
        }

        public override StrykerOption Type => StrykerOption.Mutate;

        public override string HelpText => @"Allows to specify file that should in- or excluded from mutating.
    Use glob syntax for wildcards: https://en.wikipedia.org/wiki/Glob_(programming)
    Use '!' at the start of a pattern to exclude all matched files.
    Use '{<start>..<end>}' at the end of a pattern to specify spans of text in files to in- or exclude.
    Example: ['**/*Service.cs','!**/MySpecialService.cs', '**/MyOtherService.cs{1..10}{32..45}']";

        public override IEnumerable<FilePattern> DefaultValue => new[] { FilePattern.Parse("**/*") };
    }
}
