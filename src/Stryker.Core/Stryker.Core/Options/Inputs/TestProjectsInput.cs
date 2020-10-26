using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    public class TestProjectsInput : SimpleStrykerInput<IEnumerable<string>>
    {
        static TestProjectsInput()
        {
            DefaultValue = ;
        }

        public override StrykerInput Type => StrykerInput.TestProjects;
        public override IEnumerable<string> DefaultInput { get => base.DefaultInput; protected set => base.DefaultInput = value; }
        public override IEnumerable<string> DefaultValue => Enumerable.Empty<string>();

        protected override string Description => "Specify the test projects.";
        protected override string HelpOptions => throw new System.NotImplementedException();

        public TestProjectsInput(IEnumerable<string> paths)
        {
            if (paths is { })
            {
                Value = paths?.Where(path => !path.IsNullOrEmptyInput()).Select(path => FilePathUtils.NormalizePathSeparators(Path.GetFullPath(path)));
            }
        }
    }
}
