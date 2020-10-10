using System.Collections.Generic;

namespace Stryker.Core.DiffProviders
{
    public class DiffResult
    {
        public ICollection<string> TestFilesChanged { get; set; }
        public ICollection<string> SourceFilesChanged { get; set; }
    }
}