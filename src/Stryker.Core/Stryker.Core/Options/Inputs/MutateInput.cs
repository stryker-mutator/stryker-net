using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Inputs;

public class MutateInput : Input<IEnumerable<string>>
{
    private readonly string _defaultInput = "**/*";
    public override IEnumerable<string> Default => new List<string> { _defaultInput };

    protected override string Description => @"Allows to specify file that should in- or excluded for the mutations.
    Use glob syntax for wildcards: https://en.wikipedia.org/wiki/Glob_(programming)
    Use '!' at the start of a pattern to exclude all matched files.
    Use '{<start>..<end>}' at the end of a pattern to specify spans of text in files to in- or exclude.
    Example: ['**/*Service.cs','!**/MySpecialService.cs', '**/MyOtherService.cs{1..10}{32..45}']";

    public IEnumerable<FilePattern> Validate()
    {
        if (SuppliedInput is { } && SuppliedInput.Any())
        {
            var filesToInclude = new List<FilePattern>();

            foreach (var pattern in SuppliedInput)
            {
                filesToInclude.Add(FilePattern.Parse(FilePathUtils.NormalizePathSeparators(pattern)));
            }

            if (filesToInclude.All(f => f.IsExclude))
            {
                // If there are only exclude patterns, we add a pattern that matches every file.
                filesToInclude.Add(FilePattern.Parse(_defaultInput));
            }

            return filesToInclude;
        }
        return Default.Select(x => FilePattern.Parse(_defaultInput)).ToList();
    }
}
