using DotNet.Globbing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core
{
    public record MutantSpan(int Start, int End);

    /// <summary>
    /// Contains information about which files and which parts of a file should be in- or excluded.
    /// </summary>
    public sealed class FilePattern : IEquatable<FilePattern>
    {
        public FilePattern(Glob glob, bool isExclude, IReadOnlyCollection<MutantSpan> mutantSpans)
        {
            Glob = glob;
            IsExclude = isExclude;
            MutantSpans = mutantSpans;
        }

        /// <summary>
        /// Gets the <see cref="Glob"/> matching the file path.
        /// </summary>
        public Glob Glob { get; }

        /// <summary>
        /// Gets whether the file and text spans should be in- or excluded.
        /// </summary>
        public bool IsExclude { get; }

        /// <summary>
        /// Gets the the text spans of the file this pattern matches.
        /// </summary>
        public IReadOnlyCollection<MutantSpan> MutantSpans { get; }

        public bool Equals(FilePattern other) => Glob.ToString() == other.Glob.ToString() && IsExclude == other.IsExclude && MutantSpans.SequenceEqual(other.MutantSpans);

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((FilePattern)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Glob != null ? Glob.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ IsExclude.GetHashCode();
                hashCode = (hashCode * 397) ^ (MutantSpans != null ? UncheckedSum(MutantSpans.Select(t => t.GetHashCode())) : 0);
                return hashCode;
            }

            static int UncheckedSum(IEnumerable<int> a)
            {
                // regular sum is always checked, even when used in unchecked statement
                return a.Aggregate((sum, i) => unchecked(sum + i));
            }
        }
    }
}
