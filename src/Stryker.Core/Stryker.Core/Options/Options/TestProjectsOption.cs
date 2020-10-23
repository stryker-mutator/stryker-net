using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class TestProjectsOption : BaseStrykerOption<IEnumerable<string>>
    {
        public TestProjectsOption(IEnumerable<string> paths)
        {
            Value = paths?
                .Where(path => !path.IsNullOrEmptyInput())
                .Select(path => FilePathUtils.NormalizePathSeparators(Path.GetFullPath(path)))
                ?? DefaultValue;
        }

        public override StrykerOption Type => StrykerOption.TestProjects;

        public override string HelpText => "Specify which test projects should run on the project under test.";

        public override IEnumerable<string> DefaultValue => Enumerable.Empty<string>();
    }
}
