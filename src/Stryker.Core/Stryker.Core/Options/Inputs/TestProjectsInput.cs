using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    public class TestProjectsInput : SimpleStrykerInput<IEnumerable<string>>
    {
        static TestProjectsInput()
        {
            HelpText = $"Specify what test projects should run on the project under test.";
            DefaultValue = Enumerable.Empty<string>();
        }

        public override StrykerInput Type => StrykerInput.TestProjects;

        public TestProjectsInput(IEnumerable<string> paths)
        {
            Value = paths?
                .Where(path => !path.IsNullOrEmptyInput())
                .Select(path => FilePathUtils.NormalizePathSeparators(Path.GetFullPath(path)))
                ?? DefaultValue;
        }
    }
}
