using System;
using DotNet.Globbing;

namespace Stryker.Core
{
    public class ExcludableString
    {
        public ExcludableString(string s)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            IsExcluded = s.StartsWith('!');

            var pattern = IsExcluded ? s[1..] : s;
            var normalized = FilePathUtils.NormalizePathSeparators(pattern);
            Glob = Glob.Parse(normalized);
        }

        public bool IsExcluded { get; }

        public Glob Glob { get; }

        public static ExcludableString Parse(string s) => new(s);
    }
}
