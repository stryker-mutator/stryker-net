using System.Collections.Generic;
using LibGit2Sharp;

namespace Stryker.Core.DiffProviders
{
    public class ChangedFile
    {
        public string Path { get; set; }
        public IEnumerable<Line> AddedLines { get; set; }
        public IEnumerable<Line> DeletedLines { get; set; }
    }
}
