using System.Collections.Generic;
using System.Text;

namespace MtpParallelAttributionMini;

/// <summary>
///     Assembles the complete LaTeX document from the metadata and test rows: title page,
///     document history, contents, introduction and one page per test. Pure (returns the
///     generated source) so it can be unit-tested without touching the file system.
/// </summary>
public static class MiniReport
{
	public static string Build(MiniMetadata metadata, IReadOnlyList<TestRow> rows, string logoFileName)
	{
		StringBuilder sb = new();

		Preamble.Write(sb, metadata, logoFileName);
		sb.AppendLine(@"\begin{document}");
		sb.AppendLine();

		TitlePageBuilder.Write(sb, metadata);
		sb.AppendLine(@"\newpage");

		if (metadata.DocumentHistory.Count > 0)
		{
			MiniTableBuilder.Write(sb, metadata);
			sb.AppendLine(@"\newpage");
		}

		sb.AppendLine(@"\tableofcontents");
		sb.AppendLine(@"\newpage");

		if (!string.IsNullOrWhiteSpace(metadata.Introduction))
		{
			IntroductionBuilder.Write(sb, metadata);
			sb.AppendLine(@"\newpage");
		}

		for (int i = 0; i < rows.Count; i++)
		{
			MiniPageBuilder.Write(sb, rows[i], i + 1);
			if (i < rows.Count - 1)
				sb.AppendLine(@"\newpage");
		}

		sb.AppendLine(@"\end{document}");
		return sb.ToString();
	}
}
