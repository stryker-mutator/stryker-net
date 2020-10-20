using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Core.UnitTest
{
    internal static class TextWriterExtenstions
    {
        private const string Escape = "\u001b";

        private static readonly Regex Ansi = new Regex("\u001b\\[[\\d;]+m", RegexOptions.ECMAScript);

        /// <summary>
        /// Removes ANSI escape sequences from the <paramref name="stringWriter"/>.
        /// </summary>
        /// <returns>A basic string without ANSI escape sequences.</returns>
        public static string RemoveAnsi(this TextWriter stringWriter)
        {
            return Ansi.Replace(stringWriter.ToString(), string.Empty);
        }

        /// <summary>
        /// Counts the number of green spans in the <paramref name="stringWriter"/>.
        /// </summary>
        /// <returns>The number of green spans.</returns>
        public static int GreenSpanCount(this TextWriter stringWriter)
        {
            return stringWriter.Split().Count(s => s.StartsWith("[32m", StringComparison.Ordinal));
        }

        /// <summary>
        /// Counts the number of red spans in the <paramref name="stringWriter"/>.
        /// </summary>
        /// <returns>The number of red spans.</returns>
        public static int RedSpanCount(this TextWriter stringWriter)
        {
            return stringWriter.Split().Count(s => s.StartsWith("[31m", StringComparison.Ordinal));
        }

        /// <summary>
        /// Counts the number of blue spans in the <paramref name="stringWriter"/>.
        /// </summary>
        /// <returns>The number of blue spans.</returns>
        public static int BlueSpanCount(this TextWriter stringWriter)
        {
            return stringWriter.Split().Count(s => s.StartsWith("[36m", StringComparison.Ordinal));
        }

        /// <summary>
        /// Counts the number of yellow spans in the <paramref name="stringWriter"/>.
        /// </summary>
        /// <returns>The number of yellow spans.</returns>
        public static int YellowSpanCount(this TextWriter stringWriter)
        {
            return stringWriter.Split().Count(s => s.StartsWith("[33m", StringComparison.Ordinal));
        }

        /// <summary>
        /// Counts the number of bright black spans in the <paramref name="stringWriter"/>.
        /// </summary>
        /// <returns>The number of bright black spans.</returns>
        public static int DarkGraySpanCount(this TextWriter stringWriter)
        {
            return stringWriter.Split().Count(s => s.StartsWith("[30;1m", StringComparison.Ordinal));
        }

        /// <summary>
        /// Counts the number of any colored span in the <paramref name="stringWriter"/>.
        /// </summary>
        /// <returns>The number of colored spans.</returns>
        public static int AnyForegroundColorSpanCount(this TextWriter stringWriter)
        {
            return stringWriter.Split().Count(s => s.StartsWith("[3", StringComparison.Ordinal));
        }

        private static string[] Split(this TextWriter stringWriter)
        {
            return stringWriter.ToString().Split(Escape);
        }
    }
}
