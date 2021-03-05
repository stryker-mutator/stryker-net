using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    public class MutateInput : OptionDefinition<IEnumerable<string>, IEnumerable<FilePattern>>
    {
        private readonly string _defaultInput = "**/*";
        public override IEnumerable<string> DefaultInput => new List<string> { _defaultInput };
        public override IEnumerable<FilePattern> DefaultValue => new MutateInput(DefaultInput).Value;

        protected override string Description => @"Allows to specify file that should in- or excluded for the mutations.
    Use glob syntax for wildcards: https://en.wikipedia.org/wiki/Glob_(programming)
    Use '!' at the start of a pattern to exclude all matched files.
    Use '{<start>..<end>}' at the end of a pattern to specify spans of text in files to in- or exclude.
    Example: ['**/*Service.cs','!**/MySpecialService.cs', '**/MyOtherService.cs{1..10}{32..45}']";
        protected override string HelpOptions => $" | default ({_defaultInput})";

        public MutateInput() { }
        public MutateInput(IEnumerable<string> mutate)
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

                Value = filesToInclude;
            }
        }
    }
}
