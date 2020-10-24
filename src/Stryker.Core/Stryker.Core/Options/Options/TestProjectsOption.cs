using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Options.Options
{
    public class TestProjectsOption : BaseStrykerOption<IEnumerable<string>>
    {
        static TestProjectsOption()
        {
            HelpText = "Specify which test projects should run on the project under test.";
            DefaultValue = Enumerable.Empty<string>();
        }

        public override StrykerOption Type => StrykerOption.TestProjects;

        public TestProjectsOption(IEnumerable<string> paths)
        {
            Value = paths?
                .Where(path => !path.IsNullOrEmptyInput())
                .Select(path => FilePathUtils.NormalizePathSeparators(Path.GetFullPath(path)))
                ?? DefaultValue;
        }
    }
}
