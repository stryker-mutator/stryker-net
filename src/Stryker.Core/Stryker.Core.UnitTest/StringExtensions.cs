using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stryker.Core.UnitTest;

internal static class StringExtensions
{
    private const string Escape = "\u001b";

    private static readonly Regex Ansi = new(Escape + "\\[[\\d;]+m", RegexOptions.ECMAScript);

    /// <summary>
    /// Removes ANSI escape sequences from the <paramref name="value"/>.
    /// </summary>
    /// <returns>A basic string without ANSI escape sequences.</returns>
    public static string RemoveAnsi(this string value)
    {
        return Ansi.Replace(value.ToString(), string.Empty);
    }

    /// <summary>
    /// Counts the number of green spans in the <paramref name="value"/>.
    /// </summary>
    /// <returns>The number of green spans.</returns>
    public static int GreenSpanCount(this string value)
    {
        return value.Split(Escape).Count(s => s.StartsWith("[32m", StringComparison.Ordinal) || s.StartsWith("[38;5;2m", StringComparison.Ordinal));
    }

    /// <summary>
    /// Counts the number of red spans in the <paramref name="value"/>.
    /// </summary>
    /// <returns>The number of red spans.</returns>
    public static int RedSpanCount(this string value)
    {
        return value.Split(Escape).Count(s => s.StartsWith("[31m", StringComparison.Ordinal) || s.StartsWith("[38;5;9m", StringComparison.Ordinal));
    }

    /// <summary>
    /// Counts the number of blue spans in the <paramref name="value"/>.
    /// </summary>
    /// <returns>The number of blue spans.</returns>
    public static int BlueSpanCount(this string value)
    {
        return value.Split(Escape).Count(s => s.StartsWith("[36m", StringComparison.Ordinal) || s.StartsWith("[38;5;14m", StringComparison.Ordinal));
    }

    /// <summary>
    /// Counts the number of yellow spans in the <paramref name="value"/>.
    /// </summary>
    /// <returns>The number of yellow spans.</returns>
    public static int YellowSpanCount(this string value)
    {
        return value.Split(Escape).Count(s => s.StartsWith("[33m", StringComparison.Ordinal) || s.StartsWith("[38;5;11m", StringComparison.Ordinal));
    }

    /// <summary>
    /// Counts the number of bright black spans in the <paramref name="value"/>.
    /// </summary>
    /// <returns>The number of bright black spans.</returns>
    public static int DarkGraySpanCount(this string value)
    {
        return value.Split(Escape).Count(s => s.StartsWith("[30;1m", StringComparison.Ordinal) || s.StartsWith("[38;5;8m", StringComparison.Ordinal));
    }

    /// <summary>
    /// Counts the number of any colored span in the <paramref name="value"/>.
    /// </summary>
    /// <returns>The number of colored spans.</returns>
    public static int AnyForegroundColorSpanCount(this string value)
    {
        return value.Split(Escape).Count(s => s.StartsWith("[3", StringComparison.Ordinal));
    }
}
