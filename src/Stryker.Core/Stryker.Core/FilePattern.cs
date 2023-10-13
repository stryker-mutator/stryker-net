using DotNet.Globbing;
using Microsoft.CodeAnalysis.Text;
using Stryker.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core
{
    /// <summary>
    /// Contains information about which files and which parts of a file should be in- or excluded.
    /// </summary>
    public sealed class FilePattern : IEquatable<FilePattern>
    {
        private static readonly TextSpan _textSpanMaxValue = new TextSpan(0, int.MaxValue);

        public FilePattern(Glob glob, bool isExclude, IReadOnlyCollection<TextSpan> textSpans)
        {
            Glob = glob;
            IsExclude = isExclude;
            TextSpans = textSpans;
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
        public IReadOnlyCollection<TextSpan> TextSpans { get; }

        /// <summary>
        /// Parses a given file pattern string.
        /// Format: (!)&lt;glob&gt;({&lt;spanStart&gt;..&lt;spanEnd&gt;})*
        /// </summary>
        /// <param name="pattern">The pattern to parse.</param>
        /// <returns>The <see cref="FilePattern"/></returns>
        public static FilePattern Parse(string pattern)
        {
            var s = new ExclusionPattern(pattern);
            IReadOnlyCollection<TextSpan> textSpans;

            if (!s.MutantSpans.Any())
            {
                // If there are no spans specified, we add one that will cover the whole file.
                textSpans = new[] { _textSpanMaxValue };
            }
            else
            {
                textSpans = s.MutantSpans
                    .Select(x => TextSpan.FromBounds(x.Start, x.End))
                    .Reduce()
                    .ToList();
            }

            return new FilePattern(s.Glob, s.IsExcluded, textSpans);
        }

        /// <summary>
        /// Checks whether a given file path and span matches the current file pattern.
        /// </summary>
        /// <param name="filePath">The full file path.</param>
        /// <param name="textSpan">The span of the text to check.</param>
        /// <returns>True if the file and span matches the pattern.</returns>
        public bool IsMatch(string filePath, TextSpan textSpan)
        {
            // Check if the file path is matched.
            if (!Glob.IsMatch(FilePathUtils.NormalizePathSeparators(filePath)))
            {
                return false;
            }

            // Check if any span fully contains the specified span
            if (TextSpans.Any(span => span.Contains(textSpan)))
            {
                return true;
            }

            return false;
        }

        public bool Equals(FilePattern other) => Glob.ToString() == other.Glob.ToString() && IsExclude == other.IsExclude && TextSpans.SequenceEqual(other.TextSpans);

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
                hashCode = (hashCode * 397) ^ (TextSpans != null ? UncheckedSum(TextSpans.Select(t => t.GetHashCode())) : 0);
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
