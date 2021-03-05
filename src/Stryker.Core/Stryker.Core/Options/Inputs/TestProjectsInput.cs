using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    public class TestProjectsInput : InputDefinition<IEnumerable<string>>
    {
        public override IEnumerable<string> Default => Enumerable.Empty<string>();

        protected override string Description => "Specify the test projects.";

        public TestProjectsInput() { }
        public TestProjectsInput(IEnumerable<string> paths)
        {
            if (paths is { })
            {
                Value = paths?.Where(path => !path.IsNullOrEmptyInput()).Select(path => FilePathUtils.NormalizePathSeparators(Path.GetFullPath(path)));
            }
        }
    }
}
