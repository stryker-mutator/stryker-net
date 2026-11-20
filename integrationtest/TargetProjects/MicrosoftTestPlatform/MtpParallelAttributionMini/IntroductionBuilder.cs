using System;
using System.Text;

namespace MtpParallelAttributionMini;

/// <summary>
///     Renders the introduction section and registers it in the contents. Paragraphs in the
///     source text are separated by a blank line.
/// </summary>
internal static class IntroductionBuilder
{
	public static void Write(StringBuilder sb, MiniMetadata metadata)
	{
		sb.AppendLine(@"\phantomsection");
		sb.AppendLine(@"\addcontentsline{toc}{section}{Introduction}");
		sb.AppendLine(@"\section*{Introduction}");

		string[] paragraphs = metadata.Introduction.Replace("\r\n", "\n", StringComparison.Ordinal).Split("\n\n");
		foreach (string paragraph in paragraphs)
		{
			string trimmed = paragraph.Trim();
			if (trimmed.Length == 0)
				continue;
			sb.AppendLine(MiniEscape.Escape(trimmed));
			sb.AppendLine();
		}
	}
}
