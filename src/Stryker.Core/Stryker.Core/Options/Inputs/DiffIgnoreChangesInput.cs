using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Inputs;

public class DiffIgnoreChangesInput : Input<IEnumerable<string>>
{
    public override IEnumerable<string> Default => Enumerable.Empty<string>();

    protected override string Description => @"Allows to specify an array of C# files which should be ignored if present in the diff.
Any non-excluded files will trigger all mutants to be tested because we cannot determine what mutants are affected by these files. 
This feature is only recommended when you are sure these files will not affect results, or when you are prepared to sacrifice accuracy for performance.

Use glob syntax for wildcards: https://en.wikipedia.org/wiki/Glob_(programming)
Example: ['**/*Assets.json','**/favicon.ico']";

    public IEnumerable<FilePattern> Validate()
    {
        if (SuppliedInput is { })
        {
            var diffIgnoreFilePatterns = new List<FilePattern>();
            foreach (var pattern in SuppliedInput)
            {
                diffIgnoreFilePatterns.Add(FilePattern.Parse(FilePathUtils.NormalizePathSeparators(pattern), spansEnabled: false));
            }

            return diffIgnoreFilePatterns;
        }
        return Enumerable.Empty<FilePattern>();
    }
}
