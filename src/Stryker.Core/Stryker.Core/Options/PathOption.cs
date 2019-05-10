using DotNet.Globbing;

namespace Stryker.Core.Options
{
    public class PathOption
    {
        public Glob Matcher { get; set; }
        public bool Exclude { get; set; }
    }
}