using System.Collections.Generic;
using System.Text;

namespace MtpParallelAttributionMini;

/// <summary>
///     Escapes characters that have special meaning in LaTeX so that arbitrary CSV and
///     metadata text can be placed safely into the generated document.
/// </summary>
public static class MiniEscape
{
	private static readonly Dictionary<char, string> Replacements = new()
	{
		['\\'] = @"\textbackslash{}",
		['&'] = @"\&",
		['%'] = @"\%",
		['$'] = @"\$",
		['#'] = @"\#",
		['_'] = @"\_",
		['{'] = @"\{",
		['}'] = @"\}",
		['~'] = @"\textasciitilde{}",
		['^'] = @"\textasciicircum{}"
	};

	public static string Escape(string value)
	{
		StringBuilder sb = new(value.Length);
		foreach (char c in value)
		{
			if (Replacements.TryGetValue(c, out string? replacement))
				sb.Append(replacement);
			else
				sb.Append(c);
		}

		return sb.ToString();
	}
}
