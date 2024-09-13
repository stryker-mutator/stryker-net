using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stryker.Utilities;

namespace Stryker.Abstractions.Options.Inputs
{
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
}
