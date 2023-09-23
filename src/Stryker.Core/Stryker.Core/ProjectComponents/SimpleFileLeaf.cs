using DotNet.Globbing;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.ProjectComponents
{
    public class SimpleFileLeaf: ProjectComponent, IReadOnlyFileLeaf
    {
        public SimpleFileLeaf(IEnumerable<ExcludableString> strings)
        {
            Patterns = strings.Select(Parse);
        }

        public IEnumerable<FilePattern> Patterns { get; set; }

        public string SourceCode { get; set; }

        public override IEnumerable<Mutant> Mutants { get; set; }

        public override void Display()
        {
            DisplayFile(this);
        }

        public override IEnumerable<IFileLeaf> GetAllFiles() => Enumerable.Empty<IFileLeaf>();

        public bool IsComponentExcluded() => false;

        public bool IsMatch(MutantSpan span) => true;

        private FilePattern Parse(ExcludableString s)
        {
            var glob = Glob.Parse(FilePathUtils.NormalizePathSeparators(s.Pattern));
            var spans = new[] { new MutantSpan(0, int.MaxValue) };
            return new FilePattern(glob, s.IsExclude, spans);
        }
    }
}
