using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Options.Inputs;

public class TestProjectsInput : Input<IEnumerable<string>>
{
    public override IEnumerable<string> Default => Enumerable.Empty<string>();

    protected override string Description => "Specify the test projects.";

    public IEnumerable<string> Validate()
    {
        if (SuppliedInput is not null)
        {
            return SuppliedInput.Where(path => !string.IsNullOrWhiteSpace(path)).Select(path => FilePathUtils.NormalizePathSeparators(Path.GetFullPath(path)));
        }
        return Default;
    }
}
